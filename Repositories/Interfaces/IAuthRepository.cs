using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<List<string>> GetValidTokensForUser(string Id);
        Task SaveNewToken(string UserId, string Token, TimeSpan Validity);
        Task<Tuple<string, string>> GetPasswordHashAndSalt(string UserId);
        Task SavePasswordHash(string UserId, string NewHash, string Salt);
        Task DeleteExpiredSessions();
    }
}
