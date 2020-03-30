using ChatServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace ChatServer.Repositories.PostgreSQL
{
    public class AuthRepository : BaseRepository, IAuthRepository
    {
        public AuthRepository(IConnectionStringProvider provider) : base(provider) { }

        public async Task<Tuple<string, string>> GetPasswordHashAndSalt(string UserId)
        {
            using var conn = await GetConnection();
            var result = await conn.QuerySingleAsync("SELECT PasswordHash, PasswordSalt FROM chat.USERS WHERE Id=@UserId", new { UserId });
            return new Tuple<string, string>(result["PasswordHash"], result["PasswordSalt"]);
        }

        public async Task<List<string>> GetValidTokensForUser(string Id)
        {
            using var conn = await GetConnection();
            return (await conn.QueryAsync<string>("SELECT Token FROM chat.SESSIONS WHERE UserId = @Id AND ExpirationDate > CURRENT_TIMESTAMP", new { Id })).ToList();
        }

        public async Task SaveLogin(string UserId)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("UPDATE chat.USERS SET LastLogin = CURRENT_TIMESTAMP WHERE Id=@UserId", new { UserId });
        }

        public async Task SaveLastSeen(string UserId)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("UPDATE chat.USERS SET LastSeen = CURRENT_TIMESTAMP WHERE Id=@UserId", new { UserId });
        }

        public async Task SaveNewToken(string UserId, string Token, TimeSpan Validity)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("INSERT INTO chat.SESSIONS (UserId, Token, CreatedOn, ExpirationDate) VALUES" +
                " (@UserId, @Token, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + interval '1' HOUR * @ExpirationHours)",
                new { UserId, Token, ExpirationHours = Validity.TotalHours });
        }

        public async Task SavePasswordHash(string UserId, string NewHash, string Salt)
        {
            using var conn = await GetConnection();

            await conn.ExecuteAsync("UPDATE chat.USERS SET PasswordHash=@NewHash, PasswordSalt=@Salt WHERE Id=@UserId", new { UserId, NewHash, Salt });
        }

        public async Task DeleteExpiredSessions()
        {
            using var conn = await GetConnection();
            var query = "DELETE FROM chat.SESSIONS WHERE ExpirationDate < CURRENT_TIMESTAMP";

            await conn.ExecuteAsync(query);
        }
    }
}
