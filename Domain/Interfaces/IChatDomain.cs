using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Domain.Interfaces
{
    public interface IChatDomain
    {
        MessageModel SendMessage(SendMessageRequest request);

        void EditMessage(EditMessageRequest request);

        void DeleteMessage(DeleteMessageRequest request);

        void UpdateSeen(UpdateSeenRequest request);

        CheckNewResponse CheckNewMessages(CheckNewRequest request);
    }
}
