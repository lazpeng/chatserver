using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Repositories.Interfaces
{
    public interface IChatRepository
    {
        Task<MessageModel> SendMessage(SendMessageRequest Request, string UserId);
        Task<List<MessageModel>> GetSentMessages(string UserId, long OffsetId);
        Task<List<MessageModel>> GetEditedMessages(string UserId, long OffsetId);
        Task<List<long>> GetDeletedMessages(string UserId, long OffsetId);
        Task EditMessage(EditMessageRequest request);
        Task DeleteMessage(DeleteMessageRequest request);
        Task<string> GetSentUserId(long MessageId);
        Task UpdateSeenMessage(UpdateSeenRequest Request, string SourceId);
        Task<LastSeenResponse> GetLastSeenMessage(GetLastSeenRequest request, string UserId);
        Task<bool> IsMessageDeleted(long Id);
    }
}
