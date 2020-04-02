using ChatServer.Services;
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
        public UserRepository(IConnectionStringProvider provider) : base(provider) { }

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
                    " LastLogin, LastSeen, DateOfBirth, FindInSearch, OpenChat, PasswordHash, PasswordSalt, DataHash) VALUES (" +
                    " @Id, @Username, @FullName, @Email, @Bio, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, @DateOfBirth," +
                    " @FindInSearch, @OpenChat, '', '', @DataHash)", user);

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

                throw new Exception(message);
            }
        }

        public async Task DeleteAccount(string Id)
        {
            using var conn = await GetConnection();

            var query = "DELETE FROM chat.USERS WHERE Id=@Id";
            await conn.ExecuteAsync(query, new { Id });
        }

        public async Task Edit(UserModel User)
        {
            using var conn = await GetConnection();

            var query = "UPDATE chat.USERS SET" +
                        " UserName = @Username, FullName = @FullName, Email = @Email, Bio = @Bio, DateOfBirth = @DateOfBirth" +
                        " FindInSearch = @FindInSearch, OpenChat = @OpenChat, DataHash = @DataHash" +
                        " WHERE Id = @Id";

            await conn.ExecuteAsync(query, User);
        }

        public async Task<bool> IsUserUpToDate(string UserId, string LastKnownDataHash)
        {
            using var conn = await GetConnection();

            var query = "SELECT DataHash FROM chat.USERS Where UserId = @UserId";

            var currentHash = await conn.QuerySingleAsync<string>(query, new { UserId });

            return currentHash == LastKnownDataHash;
        }
    }
}
