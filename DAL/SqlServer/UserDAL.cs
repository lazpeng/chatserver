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
using ChatServer.DAL.Interfaces;

namespace ChatServer.DAL.SqlServer
{
    public class UserDAL : BaseDAL, IUserDAL
    {
        public List<UserModel> Find(string username)
        {
            using var conn = GetConnection();
            
            return conn.Query<UserModel>("SELECT * FROM chat.USERS WHERE UserName=@UserName OR (UserName like '%@UserName%' AND FindInSearch=1)",
                                                new { UserName = username }).ToList();
        }

        public UserModel Get(string id)
        {
            using var conn = GetConnection();
            
            var result = conn.Query<UserModel>("SELECT * FROM chat.USERS WHERE ID=@Id", new { Id = id });
            return result.Count() == 0 ? null : result.First();
        }

        private string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[32];
            rng.GetBytes(buffer);

            return Convert.ToBase64String(buffer);
        }

        public string CalculateHash(string password, string salt)
        {
            using var algo = new SHA256Managed();

            var finalBytes = Encoding.UTF8.GetBytes(password.Concat(salt).ToString());

            return Convert.ToBase64String(algo.ComputeHash(finalBytes));
        }

        public UserModel Register(UserModel user)
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

        public void SendFriendRequest(string SourceId, string TargetId)
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

        public void AnswerFriendRequest(string SourceId, string TargetId, bool Accepted)
        {
            var query = "UPDATE chat.FRIENDS SET SettleDate = SYSDATETIME() AND Accepted = @Accepted WHERE A = @Id AND B = @Target";
            GetConnection().Execute(query, new { Accepted = Accepted ? 1 : 0, A = SourceId, B = TargetId });
        }

        public List<UserModel> FriendList(string UserId)
        {
            using var conn = GetConnection();

            var query = @"SELECT * FROM chat.USERS WHERE (Id IN (SELECT A FROM chat.FRIENDS WHERE B=@Id)
                            OR Id IN (SELECT B FROM chat.FRIENDS WHERE A=@Id)) AND ACCEPTED = 1";

            return conn.Query<UserModel>(query, new { Id = UserId }).ToList();
        }

        public void BlockUser(string SourceId, string BlockedId)
        {
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

        public void UnblockUser(string SourceId, string BlockedId)
        {
            using var conn = GetConnection();

            conn.Execute("DELETE FROM chat.BLACKLIST WHERE UserId=@SourceId AND Blocked=@BlockedId", new { SourceId, BlockedId });
        }

        public List<string> BlockList(string UserId)
        {
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

        public bool AreUsersFriends(string IdA, string IdB)
        {
            var query = "SELECT COUNT(*) FROM chat.USERS WHERE (A = @A AND B=@B) OR (A = @B AND B = @A)";

            using var cmd = GetCommand(query);
            cmd.Parameters.Add(GetParameter("@A", IdA));
            cmd.Parameters.Add(GetParameter("@B", IdB));

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public void RemoveFriend(string SourceId, string TargetId)
        {
            using var conn = GetConnection();

            var query = "DELETE FROM chat.FRIENDS WHERE (A=@SourceId AND B=@TargetId) OR (A = @TargetId AND B=@SourceId)";
            conn.Execute(query, new { SourceId, TargetId });
        }

        public void DeleteAccount(string Id)
        {
            using var conn = GetConnection();

            var query = "DELETE FROM chat.USERS WHERE Id=@Id";
            conn.Execute(query, new { Id });
        }
    }
}