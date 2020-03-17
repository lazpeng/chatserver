using ChatServer.Models;
using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IUserController
    {
        IActionResult Search(string Username);
        IActionResult Get(string Id, [FromBody] GetUserRequest request);
        IActionResult Register([FromBody] UserModel user);
        IActionResult AddFriend(string SourceId, string TargetId, [FromBody] string SessionToken);
        IActionResult RejectFriendRequest(string SourceId, string TargetId, [FromBody] string SessionToken);
        IActionResult RemoveFriend(string SourceId, string TargetId, [FromBody] string SessionToken);
        IActionResult BlockUser(string SourceId, string TargetId, [FromBody] string SessionToken);
        IActionResult UnblockUser(string SourceId, string TargetId, [FromBody] string SessionToken);
        IActionResult FriendList(string SourceId, [FromBody] string SessionToken);
        IActionResult BlockList(string SourceId, [FromBody] string SessionToken);
        IActionResult DeleteAccount(string SourceId, [FromBody] string SessionToken);
    }
}
