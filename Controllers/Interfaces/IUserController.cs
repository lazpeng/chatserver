using ChatServer.Models;
using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IUserController
    {
        Task<IActionResult> Search(string Username);
        Task<IActionResult> Get(string Id, [FromBody] BaseAuthenticatedRequest request);
        Task<IActionResult> Edit(string Id, [FromBody] EditUserRequest Model);
        Task<IActionResult> Register([FromBody] UserModel user);
        Task<IActionResult> DeleteAccount(string SourceId, [FromBody] string SessionToken);
        Task<IActionResult> CheckUsersUpdate([FromBody] CheckUserUpdateRequest Request);
    }
}
