using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IFriendController
    {
        Task<IActionResult> GetFriendList([FromQuery] string User, [FromQuery] string Token);
        Task<IActionResult> GetFriendRequests([FromQuery] string User, [FromQuery] string Token);
        Task<IActionResult> SendFriendRequest(string TargetUser, [FromBody] BaseAuthenticatedRequest Request);
        Task<IActionResult> DeleteFriendRequest(long Id, [FromBody] BaseAuthenticatedRequest Request);
        Task<IActionResult> AnswerFriendRequest(long Id, [FromBody] AnswerFriendRequest Request);
        Task<IActionResult> RemoveFriend(string TargetId, [FromBody] BaseAuthenticatedRequest Request);
    }
}
