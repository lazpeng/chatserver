using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<UserModel> Find(string Username, bool IncludePartialMatches = true);
        UserModel Get(string UserId);
        UserModel Register(UserModel User);
        bool HasFriendRequestSentToTarget(string SourceId, string TargetId);
        bool HasFriendRequestFromTarget(string SourceId, string TargetId);
        void SendFriendRequest(string SourceId, string TargetId);
        void AnswerFriendRequest(string SourceId, string TargetId, bool Accepted);
        List<FriendModel> FriendList(string UserId);
        void BlockUser(string SourceId, string TargetId);
        void UnblockUser(string SourceId, string TargetId);
        List<string> BlockList(string UserId);
        bool AreUsersFriends(string IdA, string IdB);
        void RemoveFriend(string SourceId, string TargetId);
        void DeleteAccount(string Id);
        void UpdateLastLogin(string UserId);
        void UpdateLastSeen(string UserId);
        bool IsUserBlocked(string SourceId, string BlockedId);
    }
}
