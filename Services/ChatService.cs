using ChatServer.Services.Interfaces;
using ChatServer.Exceptions;
using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using ChatServer.Repositories.Interfaces;
using System.Threading.Tasks;

namespace ChatServer.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserService _userDomain;

        public ChatService(IChatRepository chatRepository, IUserService userDomain)
        {
            _chatRepository = chatRepository;
            _userDomain = userDomain;
        }

        public async Task<MessageModel> SendMessage(SendMessageRequest request)
        {
            if(!await _userDomain.CanSendMessage(request.SourceId, request.TargetId))
            {
                throw new ChatPermissionException("Cannot send message to user");
            }

            return await _chatRepository.SendMessage(request);
        }

        public async Task EditMessage(EditMessageRequest request)
        {
            if (await _chatRepository.GetSentUserId(request.MessageId) != request.SourceId)
            {
                throw new ChatPermissionException("Not sent by this user");
            }
            await _chatRepository.EditMessage(request);
        }

        public async Task DeleteMessage(DeleteMessageRequest request)
        {
            if(await _chatRepository.GetSentUserId(request.MessageId) != request.SourceId)
            {
                throw new ChatPermissionException("Not sent by this user");
            }

            if(! await _chatRepository.IsMessageDeleted(request.MessageId))
            {
                await _chatRepository.DeleteMessage(request);
            }
        }

        public async Task UpdateSeen(UpdateSeenRequest request)
        {
            await _chatRepository.UpdateSeenMessage(request);
        }

        public async Task<CheckNewResponse> CheckNewMessages(CheckNewRequest request)
        {
            return new CheckNewResponse
            {
                NewMessages = await _chatRepository.GetSentMessages(request.SourceId, request.LastReceivedId),
                EditedMessages = await _chatRepository.GetEditedMessages(request.SourceId, request.LastEditedId),
                DeletedMessages = await _chatRepository.GetDeletedMessages(request.SourceId, request.LastDeletedId)
            };
        }
    }
}
