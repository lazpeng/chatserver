using ChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Domain.Interfaces
{
    public interface IUserDomain
    {
        void SendFriendRequest(string SourceId, string TargetId);

        void RemoveFriend(string SourceId, string TargetId);

        void BlockUser(string SourceId, string TargetId);

        void RemoveBlock(string SourceId, string TargetId);

        bool CanSendMessage(string FromUser, string ToUser);

        List<FriendModel> FriendList(string UserId);

        List<string> BlockList(string UserId);

        void AnswerFriendRequest(string UserId, string SourceId, bool Answer);

        UserModel Register(UserModel user);

        UserModel Get(string FromId, string Id);

        List<UserModel> Search(string Username);

        void DeleteAccount(string UserId);
    }
}
