using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.DAL.Interfaces
{
    public interface IUserDAL
    {
        List<UserModel> Find(string Username);
        UserModel Get(string UserId);
        UserModel Register(UserModel User);
        void SendFriendRequest(string SourceId, string TargetId);
        void AnswerFriendRequest(string SourceId, string TargetId, bool Accepted);
        List<FriendModel> FriendList(string UserId);
        void BlockUser(string SourceId, string TargetId);
        void UnblockUser(string SourceId, string TargetId);
        List<string> BlockList(string UserId);
        bool AreUsersFriends(string IdA, string IdB);
        void RemoveFriend(string SourceId, string TargetId);
        void DeleteAccount(string Id);
    }
}
