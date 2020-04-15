using ChatServer.Models.Responses;
using System.Threading.Tasks;

namespace ChatServer.Services.Interfaces
{
    public interface IAuthService
    {
        string Authorize(string Request);
        Task<LoginResponse> PerformLogin(string UserName, string Password, bool AppearOffline);
    }
}
