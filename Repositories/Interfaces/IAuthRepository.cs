using System;
using System.Collections.Generic;

namespace ChatServer.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        List<string> GetValidTokensForUser(string Id);
        void SaveNewToken(string UserId, string Token, TimeSpan Validity);
        Tuple<string, string> GetPasswordHashAndSalt(string UserId);
        void SavePasswordHash(string UserId, string NewHash, string Salt);
    }
}
