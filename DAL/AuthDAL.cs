using ChatServer.Models.Responses;
using System;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace ChatServer.DAL
{
    public abstract class AuthDAL : BaseDAL
    {
        public static TimeSpan TokenExpiration = TimeSpan.FromHours(12);

        private static void EnsureDatabase()
        {
            EnsureSchema();

            var query = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                                WHERE TABLE_SCHEMA = 'chat' AND TABLE_NAME = 'SESSIONS') 
                            CREATE TABLE chat.SESSIONS (
                                UserId CHAR(36) NOT NULL,
                                Token CHAR(36) NOT NULL,
                                CreatedOn DATETIME NOT NULL,
                                ExpirationDate DATETIME NOT NULL,
                                CONSTRAINT SESSIONS_PK PRIMARY KEY (UserId, Token),
                                CHECK (ExpirationDate > SYSDATETIME())
                            )";

            using var cmd = GetCommand(query);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Checks if the given token is valid considering the time and originating user id
        /// </summary>
        /// <param name="UserId">Id for the user that made the request</param>
        /// <param name="Token">Token given by the user</param>
        /// <returns>Boolean result to wether or not the user auth is valid</returns>
        public static bool IsTokenValid(string UserId, string Token)
        {
            EnsureDatabase();

            using var cmd = GetCommand();

            cmd.CommandText = "SELECT COUNT(*) FROM chat.SESSIONS WHERE UserId = @Id AND Token = @Token AND ExpirationDate < SYSDATETIME()";
            cmd.Parameters.Add(GetParameter("@Id", UserId));
            cmd.Parameters.Add(GetParameter("@Token", Token));
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private static string GenerateNewToken(string UserId)
        {
            EnsureDatabase();

            var token = Guid.NewGuid().ToString();

            using var conn = GetConnection();

            conn.Execute(@"INSERT INTO chat.SESSIONS(UserId, Token, CreatedOn, ExpirationDate) VALUES
                        (@UserId, @Token, SYSDATETIME(), dateadd(HOUR, @HoursExpiration, SYSDATETIME())",
                new { UserId, Token = token, HoursExpiration = TokenExpiration.TotalHours });

            return token;
        }

        public static LoginResponse Login(string Username, string Password)
        {
            EnsureDatabase();

            using var cmd = GetCommand();

            cmd.CommandText = "SELECT Id, PasswordHash, PasswordSalt FROM chat.USERS WHERE Username=@Username";
            cmd.Parameters.Add(GetParameter("@Username", Username));

            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var hash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                var salt = reader.GetString(reader.GetOrdinal("PasswordSalt"));
                var userId = reader.GetString(reader.GetOrdinal("Id"));

                if (hash == UserDAL.CalculateHash(Password, salt))
                {
                    return new LoginResponse { Success = true, ErrorMessage = "", Token = GenerateNewToken(userId) };
                }
            }

            return new LoginResponse { Success = false, ErrorMessage = "Invalid user/password combination" };
        }
    }
}
