using ChatServer.Models;
using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IUserController
    {
        Task<IActionResult> Search([FromQuery] string Username);
        Task<IActionResult> Get(string Id, [FromHeader] string Authorization);
        Task<IActionResult> Edit(string Id, [FromBody] EditUserRequest Model, [FromHeader] string Authorization);
        Task<IActionResult> Register([FromBody] UserModel user);
        Task<IActionResult> DeleteAccount(string SourceId, [FromHeader] string Authorization);
        Task<IActionResult> CheckUsersUpdate([FromBody] CheckUserUpdateRequest Request, [FromHeader] string Authorization);
    }
}
