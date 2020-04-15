using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IFriendController
    {
        Task<IActionResult> GetFriendList([FromHeader] string Authorization);
        Task<IActionResult> GetFriendRequests([FromHeader] string Authorization);
        Task<IActionResult> SendFriendRequest(string TargetUser, [FromHeader] string Authorization);
        Task<IActionResult> DeleteFriendRequest(long Id, [FromHeader] string Authorization);
        Task<IActionResult> AnswerFriendRequest(long Id, [FromBody] AnswerFriendRequest Request, [FromHeader] string Authorization);
        Task<IActionResult> RemoveFriend(string TargetId, [FromHeader] string Authorization);
    }
}
