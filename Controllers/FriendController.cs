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
        public async Task<IActionResult> AnswerFriendRequest(long Id, [FromBody] AnswerFriendRequest Request, [FromHeader] string Authorization)
        {
            _authService.Authorize(Authorization);

            await _friendService.AnswerFriendRequest(Id, Request.Accepted);

            return Ok();
        }

        [HttpDelete("request/{Id}")]
        public async Task<IActionResult> DeleteFriendRequest(long Id, [FromHeader] string Authorization)
        {
            _authService.Authorize(Authorization);

            await _friendService.DeleteFriendRequest(Id);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendList([FromHeader] string Authorization)
        {
            var User = _authService.Authorize(Authorization);

            return Ok(await _friendService.GetFriendList(User));
        }

        [HttpGet("request")]
        public async Task<IActionResult> GetFriendRequests([FromHeader] string Authorization)
        {
            var User = _authService.Authorize(Authorization);

            return Ok(await _friendService.GetFriendRequests(User));
        }

        [HttpDelete("{TargetId}")]
        public async Task<IActionResult> RemoveFriend(string TargetId, [FromHeader] string Authorization)
        {
            var SourceId = _authService.Authorize(Authorization);

            await _friendService.RemoveFriend(TargetId, SourceId);
            return Ok();
        }

        [HttpPost("request/{TargetUser}")]
        public async Task<IActionResult> SendFriendRequest(string TargetUser, [FromHeader] string Authorization)
        {
            var SourceId = _authService.Authorize(Authorization);

            var request = await _friendService.SendFriendRequest(SourceId, TargetUser);

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
