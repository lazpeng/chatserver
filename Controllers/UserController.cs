using ChatServer.Models;
using Microsoft.AspNetCore.Mvc;
using ChatServer.Controllers.Interfaces;
using ChatServer.Services.Interfaces;
using ChatServer.Models.Requests;
using System.Threading.Tasks;
using ChatServer.Exceptions;

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
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, [FromHeader] string Authorization)
        {
            var SourceId = _authService.Authorize(Authorization);

            var user = await _userService.Get(SourceId, id);
            if (user != null)
            {
                return Ok(user);
            }
            else return NotFound();
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Edit(string Id, [FromBody] EditUserRequest Request, [FromHeader] string Authorization)
        {
            var User = _authService.Authorize(Authorization);
            if(Id != User)
            {
                throw new ChatAuthException("Editing another user");
            }

            await _userService.Edit(Id, Request.User);

            return Ok(await _userService.Get(Id, Id));
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            var result = await _userService.Register(user);
            return Created(result.Id, result);
        }

        [HttpDelete("{SourceId}")]
        public async Task<IActionResult> DeleteAccount(string SourceId, [FromHeader] string Authorization)
        {
            var User = _authService.Authorize(Authorization);

            if(User != SourceId)
            {
                throw new ChatAuthException("Deleting another user");
            }

            await _userService.DeleteAccount(SourceId);
            return Ok();
        }

        [HttpPost("checkUpdate")]
        public async Task<IActionResult> CheckUsersUpdate([FromBody] CheckUserUpdateRequest Request, [FromHeader] string Authorization)
        {
            _authService.Authorize(Authorization);

            return Ok(await _userService.CheckUsersUpdate(Request));
        }
    }
}