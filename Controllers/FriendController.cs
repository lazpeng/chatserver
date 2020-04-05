using ChatServer.Controllers.Interfaces;
using ChatServer.Models.Requests;
using ChatServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendController : ControllerBase, IFriendController
    {
        private readonly IAuthService _authService;
        private readonly IFriendService _friendService;

        public FriendController(IAuthService authService, IFriendService friendService)
        {
            _authService = authService;
            _friendService = friendService;
        }

        [HttpPut("request/{Id}")]
        public async Task<IActionResult> AnswerFriendRequest(long Id, [FromBody] AnswerFriendRequest Request)
        {
            await _authService.Authorize(Request);

            await _friendService.AnswerFriendRequest(Id, Request.Accepted);

            return Ok();
        }

        [HttpDelete("request/{Id}")]
        public async Task<IActionResult> DeleteFriendRequest(long Id, [FromBody] BaseAuthenticatedRequest Request)
        {
            await _authService.Authorize(Request);

            await _friendService.DeleteFriendRequest(Id);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendList([FromQuery] string User, [FromQuery] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = User, Token = Token });

            return Ok(await _friendService.GetFriendList(User));
        }

        [HttpGet("request")]
        public async Task<IActionResult> GetFriendRequests([FromQuery] string User, [FromQuery] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = User, Token = Token });

            return Ok(await _friendService.GetFriendRequests(User));
        }

        [HttpDelete("{TargetId}")]
        public async Task<IActionResult> RemoveFriend(string TargetId, [FromBody] BaseAuthenticatedRequest Request)
        {
            await _authService.Authorize(Request);

            await _friendService.RemoveFriend(TargetId, Request.SourceId);
            return Ok();
        }

        [HttpPost("request/{TargetUser}")]
        public async Task<IActionResult> SendFriendRequest(string TargetUser, [FromBody] BaseAuthenticatedRequest Request)
        {
            await _authService.Authorize(Request);

            var request = await _friendService.SendFriendRequest(Request.SourceId, TargetUser);

            if(request != null)
            {
                return Created($"{request.Id}", request);
            } else
            {
                return Ok();
            }
        }
    }
}
