﻿using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using System.Threading.Tasks;

namespace ChatServer.Domain.Interfaces
{
    public interface IChatDomain
    {
        Task<MessageModel> SendMessage(SendMessageRequest request);

        Task EditMessage(EditMessageRequest request);

        Task DeleteMessage(DeleteMessageRequest request);

        Task UpdateSeen(UpdateSeenRequest request);

        Task<CheckNewResponse> CheckNewMessages(CheckNewRequest request);
    }
}
