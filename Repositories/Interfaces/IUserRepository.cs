using ChatServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserModel>> Find(string Username, bool IncludePartialMatches = true);
        Task<UserModel> Get(string UserId);
        Task<UserModel> Register(UserModel User);
        Task DeleteAccount(string Id);
        Task UpdateLastLogin(string UserId);
        Task UpdateLastSeen(string UserId);
        Task Edit(UserModel User);
        Task<bool> IsUserUpToDate(string UserId, string LastKnownDataHash);
    }
}
