using ChatServer.Domain.Interfaces;
using ChatServer.Exceptions;
using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using ChatServer.Repositories.Interfaces;

namespace ChatServer.Domain
{
    public class ChatDomain : IChatDomain
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public ChatDomain(IChatRepository chatRepository, IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public MessageModel SendMessage(SendMessageRequest request)
        {
            var userDomain = new UserDomain(_userRepository);

            if(!userDomain.CanSendMessage(request.SourceId, request.TargetId))
            {
                throw new ChatPermissionException("Cannot send message to user");
            }

            return _chatRepository.SendMessage(request);
        }

        public void EditMessage(EditMessageRequest request)
        {
            if (_chatRepository.GetSentUserId(request.MessageId) != request.UserId)
            {
                throw new ChatPermissionException("Not sent by this user");
            }
            _chatRepository.EditMessage(request);
        }

        public void DeleteMessage(DeleteMessageRequest request)
        {
            if(_chatRepository.GetSentUserId(request.MessageId) != request.UserId)
            {
                throw new ChatPermissionException("Not sent by this user");
            }

            if(!_chatRepository.IsMessageDeleted(request.MessageId))
            {
                _chatRepository.DeleteMessage(request);
            }
        }

        public void UpdateSeen(UpdateSeenRequest request)
        {
            _chatRepository.UpdateSeenMessage(request);
        }

        public CheckNewResponse CheckNewMessages(CheckNewRequest request)
        {
            return new CheckNewResponse
            {
                NewMessages = _chatRepository.GetSentMessages(request.SourceId, request.LastReceivedId),
                EditedMessages = _chatRepository.GetEditedMessages(request.SourceId, request.LastEditedId),
                DeletedMessages = _chatRepository.GetDeletedMessages(request.SourceId, request.LastDeletedId)
            };
        }
    }
}
