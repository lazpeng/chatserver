using ChatServer.Models;
using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using System.Threading.Tasks;

namespace ChatServer.Services.Interfaces
{
    public interface IChatService
    {
        Task<MessageModel> SendMessage(SendMessageRequest request, string SourceId);

        Task EditMessage(EditMessageRequest request, string SourceId);

        Task DeleteMessage(DeleteMessageRequest request, string SourceId);

        Task UpdateSeen(UpdateSeenRequest request, string SourceId);

        Task<CheckNewResponse> CheckNewMessages(CheckNewRequest request, string SourceId);
    }
}
