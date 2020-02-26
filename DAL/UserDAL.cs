using System;
using System.Linq;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;
using Dapper;
using ChatServer.Models;
using ChatServer.Exceptions;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ChatServer.DAL
{
    public abstract class UserDAL : BaseDAL
    {
        private static void EnsureDatabase()
        {
            EnsureSchema();

            var usersTable = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                                WHERE TABLE_SCHEMA = 'chat' AND TABLE_NAME = 'USERS') 
                        CREATE TABLE chat.USERS (
                            Id CHAR(36) PRIMARY KEY,
                            UserName VARCHAR(128) NOT NULL,
                            FullName VARCHAR(256) NOT NULL,
                            Email VARCHAR(256),
                            Bio TEXT,
                            AccountCreated DATETIME NOT NULL,
                            LastLogin DATETIME NOT NULL ,
                            LastSeen DATETIME NOT NULL ,
                            DateOfBirth DATE NOT NULL,
                            ProfilePicUrl TEXT,
                            FindInSearch BIT NOT NULL DEFAULT 1,
                            OpenChat BIT NOT NULL DEFAULT 1,
                            PasswordHash CHAR(44) NOT NULL,
                            PasswordSalt CHAR(44) NOT NULL,
                            CONSTRAINT USER_UNIQUE UNIQUE (UserName),
                            CONSTRAINT EMAIL_UNIQUE UNIQUE (Email)
                        )";
            
            var friendsTable = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                                WHERE TABLE_SCHEMA = 'chat' AND TABLE_NAME = 'FRIENDS') 
                            CREATE TABLE chat.FRIENDS (
                                A CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
                                B CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
                                SentDate DATETIME NOT NULL,
                                SettleDate DATETIME,
                                Accepted BIT NOT NULL DEFAULT 0,
                                CONSTRAINT FRIENDS_PK PRIMARY KEY (A, B)
                            )";

            
            var blockTable = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                                WHERE TABLE_SCHEMA = 'chat' AND TABLE_NAME = 'BLACKLIST') 
                            CREATE TABLE chat.BLACKLIST (
                                UserId CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
                                BlockedId CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
                                BlockDate DATETIME NOT NULL,
                                CONSTRAINT BLACKLIST_PK PRIMARY KEY (UserId, BlockedId)
                            )";
            
            var tables = new string[] { usersTable, friendsTable, blockTable };

            using(var cmd = GetCommand())
            {
                foreach(var table in tables)
                {
                    cmd.CommandText = table;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static UserModel Find(string username)
        {
            EnsureDatabase();

            using var conn = GetConnection();
            
            var result = conn.Query<UserModel>("SELECT * FROM chat.USERS WHERE UserName=@UserName AND FindInSearch=1",
                                                new { UserName = username });
            return result.Count() == 0 ? null : result.First();
        }

        public static UserModel Get(string id)
        {
            EnsureDatabase();

            using var conn = GetConnection();
            
            var result = conn.Query<UserModel>("SELECT * FROM chat.USERS WHERE ID=@Id", new { Id = id });
            return result.Count() == 0 ? null : result.First();
        }

        private static string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[32];
            rng.GetBytes(buffer);

            return Convert.ToBase64String(buffer);
        }

        public static string CalculateHash(string password, string salt)
        {
            using var algo = new SHA256Managed();

            var finalBytes = Encoding.UTF8.GetBytes(password.Concat(salt).ToString());

            return Convert.ToBase64String(algo.ComputeHash(finalBytes));
        }

        public static UserModel Register(UserModel user)
        {
            using var conn = GetConnection();

            user.Id = Guid.NewGuid().ToString();

            var salt = GenerateSalt();
            var hash = CalculateHash(user.Password, salt);

            try
            {
                // TODO: Find way to also pass hash and salt values in this insert

                conn.Execute("INSERT INTO chat.USERS(Id, UserName, FullName, Email, Bio, AccountCreated," +
                    " LastLogin, LastSeen, DateOfBirth, ProfilePicUrl, FindInSearch, OpenChat, PasswordHash, PasswordSalt) VALUES (" +
                    " @Id, @Username, @FullName, @Email, @Bio, SYSDATETIME(), SYSDATETIME(), SYSDATETIME(), @DateOfBirth," +
                    " @ProfilePicUrl, @FindInSearch, @OpenChat, '', '')", user);

                conn.Execute("UPDATE chat.USERS SET PasswordHash=@Hash, PasswordSalt=@Salt WHERE Id=@Id", new { user.Id, Hash = hash, Salt = salt });

                return Get(user.Id);
            } catch (SqlException ex)
            {
                var message = ex.Message;

                var constraintMessage = new Dictionary<string, string>()
                {
                    ["USER_UNIQUE"] = "Username already taken",
                    ["EMAIL_UNIQUE"] = "Email used by another account",
                };

                foreach(var constraint in constraintMessage.Keys)
                {
                    if(message.Contains(constraint))
                    {
                        message = constraintMessage[constraint];
                        break;
                    }
                }

                throw new ChatDataException(message);
            }
        }

        public static void SendFriendRequest(string SourceId, string TargetId)
        {
            IDbCommand cmd;

            // If we're already friends, do nothing
            using (cmd = GetCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM chat.FRIENDS WHERE (A = @Id AND B = @Target) OR (A = @Target AND B = @Id)";
                cmd.Parameters.Add(GetParameter("@Id", SourceId));
                cmd.Parameters.Add(GetParameter("@Target", TargetId));

                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            // If we already sent a friend request to target user, do nothing
            using (cmd = GetCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM chat.FRIENDS WHERE A = @Id AND B = @Target AND SettleDate IS NULL";
                cmd.Parameters.Add(GetParameter("@Id", SourceId));
                cmd.Parameters.Add(GetParameter("@Target", TargetId));

                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            // If they have already sent a friend request to us, we accept it
            using (cmd = GetCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM chat.FRIENDS WHERE A=@Target AND B=@Id AND SettleDate IS NULL";
                cmd.Parameters.Add(GetParameter("@Id", SourceId));
                cmd.Parameters.Add(GetParameter("@Target", TargetId));

                if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                {
                    cmd.CommandText = "UPDATE chat.FRIENDS SET SettleDate = SYSDATETIME() AND Accepted = 1 WHERE A = @Target AND B = @Id";
                    cmd.ExecuteNonQuery();

                    return;
                }
            }

            // None of the above, so send the friend request
            using (cmd = GetCommand())
            {
                cmd.CommandText = "INSERT INTO chat.FRIENDS(A, B, SentDate) VALUES (@Id, @Target, SYSTDATETIME())";
                cmd.Parameters.Add(GetParameter("@Id", SourceId));
                cmd.Parameters.Add(GetParameter("@Target", TargetId));

                cmd.ExecuteNonQuery();
            }
        }

        public static void AnswerFriendRequest(string SourceId, string TargetId, bool Accepted)
        {
            var query = "UPDATE chat.FRIENDS SET SettleDate = SYSDATETIME() AND Accepted = @Accepted WHERE A = @Id AND B = @Target";
            GetConnection().Execute(query, new { Accepted = Accepted ? 1 : 0, A = SourceId, B = TargetId });
        }

        public static List<UserModel> FriendList(string UserId)
        {
            using var conn = GetConnection();

            var query = @"SELECT u.* FROM chat.USERS WHERE (Id IN (SELECT A FROM chat.FRIENDS WHERE B=@Id)
                            OR Id IN (SELECT B FROM chat.FRIENDS WHERE A=@Id)) AND ACCEPTED = 1";

            return conn.Query<UserModel>(query, new { Id = UserId }).ToList();
        }

        public static void BlockUser(string SourceId, string BlockedId)
        {
            EnsureDatabase();

            using var cmd = GetCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM chat.BLACKLIST WHERE UserId=@UserId AND Blocked=@BlockedId";
            cmd.Parameters.Add(GetParameter("@UserId", SourceId));
            cmd.Parameters.Add(GetParameter("@BlockedId", BlockedId));

            if(Convert.ToInt32(cmd.ExecuteScalar()) == 0)
            {
                cmd.CommandText = "INSERT INTO chat.BLACKLIST (UserId, Blocked, BlockDate) VALUES (@UserId, @BlockedId, SYSDATETIME())";
                cmd.ExecuteNonQuery();
            }
        }

        public static void UnblockUser(string SourceId, string BlockedId)
        {
            EnsureDatabase();

            using var conn = GetConnection();

            conn.Execute("DELETE FROM chat.BLACKLIST WHERE UserId=@SourceId AND Blocked=@BlockedId", new { SourceId, BlockedId });
        }

        public static List<string> BlockList(string UserId)
        {
            EnsureDatabase();

            var query = "SELECT Blocked FROM chat.BLACKLIST WHERE UserId=@UserId";
            using var cmd = GetCommand(query);
            cmd.Parameters.Add(GetParameter("@UserId", UserId));

            var result = new List<string>();
            var reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                result.Add(reader.GetString(0));
            }

            return result;
        }

        public static bool AreUsersFriends(string IdA, string IdB)
        {
            EnsureDatabase();

            var query = "SELECT COUNT(*) FROM chat.USERS WHERE (A = @A AND B=@B) OR (A = @B AND B = @A)";

            using var cmd = GetCommand(query);
            cmd.Parameters.Add(GetParameter("@A", IdA));
            cmd.Parameters.Add(GetParameter("@B", IdB));

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static void RemoveFriend(string SourceId, string TargetId)
        {
            EnsureDatabase();

            using var conn = GetConnection();

            var query = "DELETE FROM chat.FRIENDS WHERE (A=@SourceId AND B=@TargetId) OR (A = @TargetId AND B=@SourceId)";
            conn.Execute(query, new { SourceId, TargetId });
        }
    }
}