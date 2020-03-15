using ChatServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace ChatServer.Repositories.PostgreSQL
{
    public class AuthRepository : BaseRepository, IAuthRepository
    {
        public AuthRepository(string ConnectionString) : base(ConnectionString) { }

        public Tuple<string, string> GetPasswordHashAndSalt(string UserId)
        {
            using var cmd = GetCommand("SELECT PasswordHash, PasswordSalt FROM chat.USERS WHERE Id=@UserId");
            cmd.Parameters.Add(GetParameter("UserId", UserId));

            var reader = cmd.ExecuteReader();
            if(reader.Read())
            {
                return new Tuple<string, string>(reader.GetString(0), reader.GetString(1));
            }

            return null;
        }

        public List<string> GetValidTokensForUser(string Id)
        {
            using var cmd = GetCommand("SELECT Token FROM chat.SESSIONS WHERE UserId = @Id AND ExpirationDate > CURRENT_TIMESTAMP");
            cmd.Parameters.Add(GetParameter("Id", Id));

            var result = new List<string>();

            var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                result.Add(reader.GetString(0));
            }

            return result;
        }

        public void SaveLogin(string UserId)
        {
            using var conn = GetConnection();

            conn.Execute("UPDATE chat.USERS SET LastLogin = CURRENT_TIMESTAMP WHERE Id=@UserId", new { UserId });
        }

        public void SaveLastSeen(string UserId)
        {
            using var conn = GetConnection();

            conn.Execute("UPDATE chat.USERS SET LastSeen = CURRENT_TIMESTAMP WHERE Id=@UserId", new { UserId });
        }

        public void SaveNewToken(string UserId, string Token, TimeSpan Validity)
        {
            using var conn = GetConnection();

            conn.Execute("INSERT INTO chat.SESSIONS (UserId, Token, CreatedOn, ExpirationDate) VALUES" +
                " (@UserId, @Token, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP + interval '1' HOUR * @ExpirationHours)",
                new { UserId, Token, ExpirationHours = Validity.TotalHours });
        }

        public void SavePasswordHash(string UserId, string NewHash, string Salt)
        {
            using var conn = GetConnection();

            conn.Execute("UPDATE chat.USERS SET PasswordHash=@NewHash, PasswordSalt=@Salt WHERE Id=@UserId", new { UserId, NewHash, Salt });
        }
    }
}
