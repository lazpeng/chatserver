using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Services.Interfaces
{
    public interface IFriendService
    {
        Task<bool> AreUsersFriends(string A, string B);
        Task<FriendRequestModel> SendFriendRequest(string SourceId, string TargetId);
        Task DeleteFriendRequest(long Id);
        Task AnswerFriendRequest(long Id, bool Accepted);
        Task<List<FriendModel>> GetFriendList(string User);
        Task<List<FriendRequestModel>> GetFriendRequests(string User);
        Task RemoveFriend(string A, string B);
    }
}
