using ChatServer.Domain;
using ChatServer.Exceptions;
using ChatServer.Models;
using ChatServer.Repositories.Interfaces;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ChatServer.Repositories.PostgreSQL
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(string ConnectionString) : base(ConnectionString) { }

        public List<UserModel> Find(string username, bool includePartial = true)
        {
            using var conn = GetConnection();

            var fuzzyUsername = $"%{username}%";

            var query = "SELECT * FROM chat.USERS WHERE UserName=@UserName";

            if(includePartial)
            {
                query += " OR (UserName like @Fuzzy AND FindInSearch=TRUE)";
            }

            return conn.Query<UserModel>(query, new { UserName = username, Fuzzy = fuzzyUsername }).ToList();
        }

        public UserModel Get(string id)
        {
            using var conn = GetConnection();

            var result = conn.Query<UserModel>("SELECT * FROM chat.USERS WHERE ID=@Id", new { Id = id });
            return result.Count() == 0 ? null : result.First();
        }

        public void UpdateLastLogin(string UserId)
        {
            using var conn = GetConnection();

            conn.Execute("UPDATE chat.USERS SET LastLogin = CURRENT_TIMESTAMP WHERE Id=@UserId", new { UserId });
        }

        public void UpdateLastSeen(string UserId)
        {
            using var conn = GetConnection();

            conn.Execute("UPDATE chat.USERS SET LastSeen = CURRENT_TIMESTAMP WHERE Id=@UserId", new { UserId });
        }

        public UserModel Register(UserModel user)
        {
            using var conn = GetConnection();

            user.Id = Guid.NewGuid().ToString();

            try
            {
                conn.Execute("INSERT INTO chat.USERS(Id, UserName, FullName, Email, Bio, AccountCreated," +
                    " LastLogin, LastSeen, DateOfBirth, FindInSearch, OpenChat, PasswordHash, PasswordSalt) VALUES (" +
                    " @Id, @Username, @FullName, @Email, @Bio, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @DateOfBirth," +
                    " @FindInSearch, @OpenChat, '', '')", user);

                new AuthDomain(new AuthRepository(ConnectionString), this).UpdateUserPassword(user.Id, user.Password);

                return Get(user.Id);
            }
            catch (PostgresException ex)
            {
                var message = ex.Message;

                var constraintMessage = new Dictionary<string, string>()
                {
                    ["USER_UNIQUE"] = "Username already taken",
                    ["EMAIL_UNIQUE"] = "Email used by another account",
                };

                if(constraintMessage.ContainsKey(ex.ConstraintName))
                {
                    message = constraintMessage[ex.ConstraintName];
                }

                throw new ChatDataException(message);
            }
        }

        public void SendFriendRequest(string SourceId, string TargetId)
        {
            var query = "INSERT INTO chat.FRIENDS(A, B, SentDate) VALUES (@Id, @Target, CURRENT_TIMESTAMP)";
            using var cmd = GetCommand(query);

            cmd.Parameters.Add(GetParameter("@Id", SourceId));
            cmd.Parameters.Add(GetParameter("@Target", TargetId));

            cmd.ExecuteNonQuery();
        }

        public void AnswerFriendRequest(string SourceId, string TargetId, bool Accepted)
        {
            var query = "UPDATE chat.FRIENDS SET SettleDate = CURRENT_TIMESTAMP, Accepted = @Accepted WHERE A = @Id AND B = @Target";
            GetConnection().Execute(query, new { Accepted = Accepted ? 1 : 0, A = SourceId, B = TargetId });
        }

        public List<FriendModel> FriendList(string UserId)
        {
            using var conn = GetConnection();

            var query = @"SELECT A as SourceId, B as TargetId, SettleDate as FriendsSince, SentDate as RequestSent 
                          FROM chat.FRIENDS WHERE (A = @Id OR B = @Id) AND (SettleDate IS NULL OR Accepted = TRUE)";

            return conn.Query<FriendModel>(query, new { Id = UserId }).ToList();
        }

        public void BlockUser(string SourceId, string BlockedId)
        {
            using var cmd = GetCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM chat.BLOCKLIST WHERE UserId=@UserId AND Blocked=@BlockedId";
            cmd.Parameters.Add(GetParameter("@UserId", SourceId));
            cmd.Parameters.Add(GetParameter("@BlockedId", BlockedId));

            if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
            {
                cmd.CommandText = "INSERT INTO chat.BLOCKLIST (UserId, Blocked, BlockDate) VALUES (@UserId, @BlockedId, CURRENT_TIMESTAMP)";
                cmd.ExecuteNonQuery();
            }
        }

        public void UnblockUser(string SourceId, string BlockedId)
        {
            using var conn = GetConnection();

            conn.Execute("DELETE FROM chat.BLOCKLIST WHERE UserId=@SourceId AND Blocked=@BlockedId", new { SourceId, BlockedId });
        }

        public List<string> BlockList(string UserId)
        {
            var query = "SELECT Blocked FROM chat.BLOCKLIST WHERE UserId=@UserId";
            using var cmd = GetCommand(query);
            cmd.Parameters.Add(GetParameter("@UserId", UserId));

            var result = new List<string>();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
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

        public bool HasFriendRequestSentToTarget(string SourceId, string TargetId)
        {
            var query = "SELECT COUNT(*) FROM chat.FRIENDS WHERE A = @Id AND B = @Target AND SettleDate IS NULL";
            using var cmd = GetCommand(query);

            cmd.Parameters.Add(GetParameter("@Id", SourceId));
            cmd.Parameters.Add(GetParameter("@Target", TargetId));

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public bool HasFriendRequestFromTarget(string SourceId, string TargetId)
        {
            var query = "SELECT COUNT(*) FROM chat.FRIENDS WHERE A=@Target AND B=@Id AND SettleDate IS NULL";
            using var cmd = GetCommand(query);

            cmd.Parameters.Add(GetParameter("@Id", SourceId));
            cmd.Parameters.Add(GetParameter("@Target", TargetId));

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }
}
