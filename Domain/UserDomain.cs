using ChatServer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Domain
{
    public class UserDomain
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
    }
}
