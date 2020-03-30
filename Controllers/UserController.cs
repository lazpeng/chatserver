using ChatServer.Models;
using Microsoft.AspNetCore.Mvc;
using ChatServer.Controllers.Interfaces;
using ChatServer.Services.Interfaces;
using ChatServer.Models.Requests;
using System.Threading.Tasks;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase, IUserController
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UserController(IUserService userDomain, IAuthService authDomain)
        {
            _authService = authDomain;
            _userService = userDomain;
        }

        ///<summary>
        /// Gets an user based on its username.
        ///<returns>UserModel if found, null if not found</returns>
        ///</summary>
        [HttpGet("search/{username}")]
        public async Task<IActionResult> Search(string username)
        {
            return Ok(await _userService.Search(username));
        }

        ///<summary>
        /// Gets an user based on its id.
        ///<returns>UserModel if found, null if not found</returns>
        ///</summary>
        [HttpPost("{id}")]
        public async Task<IActionResult> Get(string id, [FromBody] BaseAuthenticatedRequest request)
        {
            await _authService.Authorize(request);

            var user = await _userService.Get(request.SourceId, id);
            if (user != null)
            {
                return Ok(user);
            }
            else return NotFound();
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Edit(string Id, [FromBody] EditUserRequest Request)
        {
            await _authService.Authorize(Request);
            await _userService.Edit(Id, Request.User);

            return Ok(await _userService.Get(Id, Id));
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            var result = await _userService.Register(user);
            return Created(result.Id, result);
        }

        [HttpPut("{SourceId}/friend/add/{TargetId}")]
        public async Task<IActionResult> AddFriend(string SourceId, string TargetId, [FromBody] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = SourceId, Token = Token });
            await _userService.SendFriendRequest(SourceId, TargetId);
            return Ok();
        }

        [HttpPut("{SourceId}/friend/reject/{TargetId}")]
        public async Task<IActionResult> RejectFriendRequest(string SourceId, string TargetId, [FromBody] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = SourceId, Token = Token });

            await _userService.AnswerFriendRequest(SourceId, TargetId, false);
            return Ok();
        }

        [HttpPut("{SourceId}/friend/remove/{TargetId}")]
        public async Task<IActionResult> RemoveFriend(string SourceId, string TargetId, [FromBody] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = SourceId, Token = Token });

            await _userService.RemoveFriend(SourceId, TargetId);
            return Ok();
        }

        [HttpPut("{SourceId}/block/add/{TargetId}")]
        public async Task<IActionResult> BlockUser(string SourceId, string TargetId, [FromBody] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = SourceId, Token = Token });

            await _userService.BlockUser(SourceId, TargetId);
            return Ok();
        }

        [HttpPut("{SourceId}/block/remove/{TargetId}")]
        public async Task<IActionResult> UnblockUser(string SourceId, string TargetId, [FromBody] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = SourceId, Token = Token });

            await _userService.RemoveBlock(SourceId, TargetId);
            return Ok();
        }

        [HttpGet("{SourceId}/friendlist")]
        public async Task<IActionResult> FriendList(string SourceId, [FromBody] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = SourceId, Token = Token });

            return Ok(_userService.FriendList(SourceId));
        }

        [HttpGet("{SourceId}/blocklist")]
        public async Task<IActionResult> BlockList(string SourceId, [FromBody] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = SourceId, Token = Token });

            return Ok(_userService.BlockList(SourceId));
        }

        [HttpDelete("{SourceId}")]
        public async Task<IActionResult> DeleteAccount(string SourceId, [FromBody] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = SourceId, Token = Token });

            await _userService.DeleteAccount(SourceId);
            return Ok();
        }

        [HttpPost("checkUpdate")]
        public async Task<IActionResult> CheckUsersUpdate([FromBody] CheckUserUpdateRequest Request)
        {
            await _authService.Authorize(Request);

            return Ok(await _userService.CheckUsersUpdate(Request));
        }
    }
}