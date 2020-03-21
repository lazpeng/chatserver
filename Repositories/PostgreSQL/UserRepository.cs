using ChatServer.Domain;
using ChatServer.Exceptions;
using ChatServer.Models;
using ChatServer.Repositories.Interfaces;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Repositories.PostgreSQL
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(string ConnectionString) : base(ConnectionString) { }

        public async Task<List<UserModel>> Find(string username, bool includePartial = true)
        {
            using var conn = await GetConnection();

            var fuzzyUsername = $"%{username}%";

            var query = "SELECT * FROM chat.USERS WHERE UserName=@UserName";

            if(includePartial)
            {
                query += " OR (UserName like @Fuzzy AND FindInSearch=TRUE)";
            }

            return (await conn.QueryAsync<UserModel>(query, new { UserName = username, Fuzzy = fuzzyUsername })).ToList();
        }

        public async Task<UserModel> Get(string Id)
        {
            using var conn = await GetConnection();

            return await conn.QuerySingleAsync<UserModel>("SELECT * FROM chat.USERS WHERE ID=@Id", new { Id });
        }

        public async Task UpdateLastLogin(string UserId)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("UPDATE chat.USERS SET LastLogin = CURRENT_TIMESTAMP WHERE Id=@UserId", new { UserId });
        }

        public async Task UpdateLastSeen(string UserId)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("UPDATE chat.USERS SET LastSeen = CURRENT_TIMESTAMP WHERE Id=@UserId", new { UserId });
        }

        public async Task<UserModel> Register(UserModel user)
        {
            using var conn = await GetConnection();

            user.Id = Guid.NewGuid().ToString();

            try
            {
                await conn.ExecuteAsync("INSERT INTO chat.USERS(Id, UserName, FullName, Email, Bio, AccountCreated," +
                    " LastLogin, LastSeen, DateOfBirth, FindInSearch, OpenChat, PasswordHash, PasswordSalt) VALUES (" +
                    " @Id, @Username, @FullName, @Email, @Bio, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @DateOfBirth," +
                    " @FindInSearch, @OpenChat, '', '')", user);

                var authRepository = new AuthRepository(ConnectionString);
                await new UserDomain(this, authRepository).UpdateUserPassword(user.Id, user.Password);

                return await Get(user.Id);
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

        public async Task SendFriendRequest(string SourceId, string TargetId)
        {
            using var conn = await GetConnection();
            var query = "INSERT INTO chat.FRIENDS(A, B, SentDate) VALUES (@Id, @Target, CURRENT_TIMESTAMP)";
            await conn.ExecuteAsync(query, new { Id = SourceId, Target = TargetId });
        }

        public async Task AnswerFriendRequest(string SourceId, string TargetId, bool Accepted)
        {
            using var conn = await GetConnection();
            var query = "UPDATE chat.FRIENDS SET SettleDate = CURRENT_TIMESTAMP, Accepted = @Accepted WHERE A = @Id AND B = @Target";
            await conn.ExecuteAsync(query, new { Accepted = Accepted ? 1 : 0, A = SourceId, B = TargetId });
        }

        public async Task<List<FriendModel>> FriendList(string UserId)
        {
            using var conn = await GetConnection();

            var query = @"SELECT A as SourceId, B as TargetId, SettleDate as FriendsSince, SentDate as RequestSent 
                          FROM chat.FRIENDS WHERE (A = @Id OR B = @Id) AND (SettleDate IS NULL OR Accepted = TRUE)";

            return (await conn.QueryAsync<FriendModel>(query, new { Id = UserId })).ToList();
        }

        public async Task<bool> IsUserBlocked(string SourceId, string BlockedId)
        {
            using var conn = await GetConnection();
            return await conn.QuerySingleAsync<long>("SELECT COUNT(*) FROM chat.BLOCKLIST WHERE UserId=@UserId AND Blocked=@BlockedId") > 0;
        }

        public async Task BlockUser(string SourceId, string BlockedId)
        {
            using var conn = await GetConnection();
            var query = "INSERT INTO chat.BLOCKLIST (UserId, Blocked, BlockDate) VALUES (@UserId, @BlockedId, CURRENT_TIMESTAMP)";
            await conn.ExecuteAsync(query, new { UserId = SourceId, BlockedId });
        }

        public async Task UnblockUser(string SourceId, string BlockedId)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("DELETE FROM chat.BLOCKLIST WHERE UserId=@SourceId AND Blocked=@BlockedId", new { SourceId, BlockedId });
        }

        public async Task<List<string>> BlockList(string UserId)
        {
            using var conn = await GetConnection();
            var query = "SELECT Blocked FROM chat.BLOCKLIST WHERE UserId=@UserId";
            return (await conn.QueryAsync<string>(query, new { UserId })).ToList();
        }

        public async Task<bool> AreUsersFriends(string A, string B)
        {
            using var conn = await GetConnection();
            var query = "SELECT COUNT(*) FROM chat.FRIENDS WHERE (A = @A AND B=@B) OR (A = @B AND B = @A)";

            return await conn.QuerySingleAsync<long>(query, new { A, B }) > 0;
        }

        public async Task RemoveFriend(string SourceId, string TargetId)
        {
            using var conn = await GetConnection();

            var query = "DELETE FROM chat.FRIENDS WHERE (A=@SourceId AND B=@TargetId) OR (A = @TargetId AND B=@SourceId)";
            await conn.ExecuteAsync(query, new { SourceId, TargetId });
        }

        public async Task DeleteAccount(string Id)
        {
            using var conn = await GetConnection();

            var query = "DELETE FROM chat.USERS WHERE Id=@Id";
            await conn.ExecuteAsync(query, new { Id });
        }

        public async Task<bool> HasFriendRequestSentToTarget(string SourceId, string TargetId)
        {
            using var conn = await GetConnection();
            var query = "SELECT COUNT(*) FROM chat.FRIENDS WHERE A = @Id AND B = @Target AND SettleDate IS NULL";
            return await conn.QuerySingleAsync<long>(query, new { Id = SourceId, Target = TargetId }) > 0;
        }

        public async Task Edit(UserModel User)
        {
            using var conn = await GetConnection();

            var query = "UPDATE chat.USERS SET" +
                        " UserName = @Username, FullName = @FullName, Email = @Email, Bio = @Bio, DateOfBirth = @DateOfBirth" +
                        " FindInSearch = @FindInSearch, OpenChat = @OpenChat" +
                        " WHERE Id = @Id";

            await conn.ExecuteAsync(query, User);
        }
    }
}
