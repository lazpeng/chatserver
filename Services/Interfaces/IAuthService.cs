using ChatServer.Models.Requests;
using ChatServer.Models.Responses;
using System.Threading.Tasks;

namespace ChatServer.Services.Interfaces
{
    public interface IAuthService
    {
        Task Authorize(BaseAuthenticatedRequest Request);
        Task<LoginResponse> PerformLogin(string UserName, string Password, bool AppearOffline);
    }
}
