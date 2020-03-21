using ChatServer.Models.Responses;
using System.Threading.Tasks;

namespace ChatServer.Domain.Interfaces
{
    public interface IAuthDomain
    {
        Task<bool> IsTokenValid(string UserId, string Token);
        Task<LoginResponse> PerformLogin(string UserName, string Password, bool AppearOffline);
    }
}
