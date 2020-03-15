using ChatServer.Domain.Interfaces;
using ChatServer.Models;
using ChatServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Domain
{
    public class UserDomain : IUserDomain
    {
        private readonly IUserRepository userRepository;

        public UserDomain(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public void SendFriendRequest(string SourceId, string TargetId)
        {
            if(userRepository.AreUsersFriends(SourceId, TargetId) || userRepository.HasFriendRequestSentToTarget(SourceId, TargetId))
            {
                return;
            } else if(userRepository.HasFriendRequestFromTarget(SourceId, TargetId))
            {
                userRepository.AnswerFriendRequest(SourceId, TargetId, true);
            } else
            {
                userRepository.SendFriendRequest(SourceId, TargetId);
            }
        }

        public void RemoveFriend(string SourceId, string TargetId)
        {
            userRepository.RemoveFriend(SourceId, TargetId);
        }

        public void BlockUser(string SourceId, string TargetId)
        {
            if(!userRepository.IsUserBlocked(SourceId, TargetId))
            {
                userRepository.BlockUser(SourceId, TargetId);
            }
        }

        public void RemoveBlock(string SourceId, string TargetId)
        {
            userRepository.UnblockUser(SourceId, TargetId);
        }

        public bool CanSendMessage(string FromUser, string ToUser)
        {
            var target = userRepository.Get(ToUser);

            if(target != null)
            {
                return target.OpenChat || userRepository.AreUsersFriends(FromUser, ToUser);
            }

            return false;
        }

        public List<FriendModel> FriendList(string UserId)
        {
            return userRepository.FriendList(UserId);
        }

        public List<string> BlockList(string UserId)
        {
            return userRepository.BlockList(UserId);
        }

        public void AnswerFriendRequest(string UserId, string SourceId, bool Answer)
        {
            userRepository.AnswerFriendRequest(UserId, SourceId, Answer);
        }

        public UserModel Register(UserModel user)
        {
            return userRepository.Register(user);
        }

        public UserModel Get(string FromId, string Id)
        {
            var user = userRepository.Get(Id);

            if(!userRepository.AreUsersFriends(Id, FromId))
            {
                // TODO: Strip user of personal information
            }

            return user;
        }

        public List<UserModel> Search(string Username)
        {
            return userRepository.Find(Username, true);
        }

        public void DeleteAccount(string UserId)
        {
            userRepository.DeleteAccount(UserId);
        }
    }
}
