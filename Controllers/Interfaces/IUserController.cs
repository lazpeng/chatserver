using ChatServer.Models;
using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IUserController
    {
        Task<IActionResult> Search(string Username);
        Task<IActionResult> Get(string Id, [FromBody] GetUserRequest request);
        Task<IActionResult> Edit(string Id, [FromBody] EditUserRequest Model);
        Task<IActionResult> Register([FromBody] UserModel user);
        Task<IActionResult> AddFriend(string SourceId, string TargetId, [FromBody] string SessionToken);
        Task<IActionResult> RejectFriendRequest(string SourceId, string TargetId, [FromBody] string SessionToken);
        Task<IActionResult> RemoveFriend(string SourceId, string TargetId, [FromBody] string SessionToken);
        Task<IActionResult> BlockUser(string SourceId, string TargetId, [FromBody] string SessionToken);
        Task<IActionResult> UnblockUser(string SourceId, string TargetId, [FromBody] string SessionToken);
        Task<IActionResult> FriendList(string SourceId, [FromBody] string SessionToken);
        Task<IActionResult> BlockList(string SourceId, [FromBody] string SessionToken);
        Task<IActionResult> DeleteAccount(string SourceId, [FromBody] string SessionToken);
        Task<IActionResult> CheckUsersUpdate([FromBody] CheckUserUpdateRequest Request);
    }
}
