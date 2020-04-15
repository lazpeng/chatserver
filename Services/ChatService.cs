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
        private readonly IUserService _userService;
        private readonly IFriendService _friendService;

        public ChatService(IChatRepository chatRepository, IUserService userDomain, IFriendService friendService)
        {
            _chatRepository = chatRepository;
            _userService = userDomain;
            _friendService = friendService;
        }

        public async Task<MessageModel> SendMessage(SendMessageRequest request, string SourceId)
        {
            var targetUser = await _userService.Get(SourceId, request.TargetId);

            if(targetUser != null && 
                (targetUser.OpenChat || await _friendService.AreUsersFriends(SourceId, request.TargetId)))
            {
                return await _chatRepository.SendMessage(request, SourceId);
            }

            throw new ChatPermissionException("Cannot send message to user");
        }

        public async Task EditMessage(EditMessageRequest request, string SourceId)
        {
            if (await _chatRepository.GetSentUserId(request.MessageId) != SourceId)
            {
                throw new ChatPermissionException("Not sent by this user");
            }
            await _chatRepository.EditMessage(request);
        }

        public async Task DeleteMessage(DeleteMessageRequest request, string SourceId)
        {
            if(await _chatRepository.GetSentUserId(request.MessageId) != SourceId)
            {
                throw new ChatPermissionException("Not sent by this user");
            }

            if(! await _chatRepository.IsMessageDeleted(request.MessageId))
            {
                await _chatRepository.DeleteMessage(request);
            }
        }

        public async Task UpdateSeen(UpdateSeenRequest request, string SourceId)
        {
            await _chatRepository.UpdateSeenMessage(request, SourceId);
        }

        public async Task<CheckNewResponse> CheckNewMessages(CheckNewRequest request, string SourceId)
        {
            return new CheckNewResponse
            {
                NewMessages = await _chatRepository.GetSentMessages(SourceId, request.LastReceivedId),
                EditedMessages = await _chatRepository.GetEditedMessages(SourceId, request.LastEditedId),
                DeletedMessages = await _chatRepository.GetDeletedMessages(SourceId, request.LastDeletedId)
            };
        }
    }
}
