using ChatServer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Repositories.Interfaces
{
    public interface IFriendRepository
    {
        Task<bool> AreUsersFriends(string A, string B);
        Task<FriendRequestModel> SendFriendRequest(string SourceId, string TargetId);
        Task DeleteFriendRequest(long Id);
        Task AnswerFriendRequest(long Id, bool Accepted);
        Task<List<FriendModel>> GetFriendList(string User);
        Task<List<FriendRequestModel>> GetFriendRequests(string User);
        Task<bool> HasPendingRequest(string SourceId, string TargetId);
        Task<FriendRequestModel> FindRequest(string SourceId, string TargetId);
        Task<FriendRequestModel> GetRequest(long Id);
        Task RemoveFriend(string A, string B);
    }
}
