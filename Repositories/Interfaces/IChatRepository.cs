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
        CheckNewResponse CheckNewMessages(CheckNewRequest Request);
        void UpdateSeenMessage(UpdateSeenRequest Request);
        LastSeenResponse GetLastSeenMessage(GetLastSeenRequest request);
    }
}
