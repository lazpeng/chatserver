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
        Task<bool> HasFriendRequestSentToTarget(string SourceId, string TargetId);
        Task SendFriendRequest(string SourceId, string TargetId);
        Task AnswerFriendRequest(string SourceId, string TargetId, bool Accepted);
        Task<List<FriendModel>> FriendList(string UserId);
        Task BlockUser(string SourceId, string TargetId);
        Task UnblockUser(string SourceId, string TargetId);
        Task<List<string>> BlockList(string UserId);
        Task<bool> AreUsersFriends(string IdA, string IdB);
        Task RemoveFriend(string SourceId, string TargetId);
        Task DeleteAccount(string Id);
        Task UpdateLastLogin(string UserId);
        Task UpdateLastSeen(string UserId);
        Task<bool> IsUserBlocked(string SourceId, string BlockedId);
        Task Edit(UserModel User);
        Task<bool> IsUserUpToDate(string UserId, string LastKnownDataHash);
    }
}
