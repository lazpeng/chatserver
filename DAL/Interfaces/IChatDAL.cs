using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.DAL.Interfaces
{
    public interface IChatDAL
    {
        MessageModel SendMessage(SendMessageRequest Request);
        CheckNewResponse CheckNewMessages(CheckNewRequest Request);
        void UpdateSeen(UpdateSeenRequest Request);
    }
}
