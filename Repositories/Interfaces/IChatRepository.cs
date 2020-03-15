using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Repositories.Interfaces
{
    public interface IChatRepository
    {
        MessageModel SendMessage(SendMessageRequest Request);
        List<MessageModel> GetSentMessages(string UserId, long OffsetId);
        List<MessageModel> GetEditedMessages(string UserId, long OffsetId);
        List<long> GetDeletedMessages(string UserId, long OffsetId);
        void EditMessage(EditMessageRequest request);
        void DeleteMessage(DeleteMessageRequest request);
        string GetSentUserId(long MessageId);
        void UpdateSeenMessage(UpdateSeenRequest Request);
        LastSeenResponse GetLastSeenMessage(GetLastSeenRequest request);
        bool IsMessageDeleted(long Id);
    }
}
