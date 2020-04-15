using System.Threading.Tasks;
using ChatServer.Controllers.Interfaces;
using ChatServer.Services.Interfaces;
using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase, IChatController
    {
        private readonly IChatService _chatService;
        private readonly IAuthService _authService;

        public ChatController(IChatService chatDomain, IAuthService authDomain)
        {
            _chatService = chatDomain;
            _authService = authDomain;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request, [FromHeader] string Authorization)
        {
            var UserId = _authService.Authorize(Authorization);

            return Ok(await _chatService.SendMessage(request, UserId));
        }

        [HttpGet]
        public async Task<IActionResult> CheckNewMessages([FromBody] CheckNewRequest request, [FromHeader] string Authorization)
        {
            var userId = _authService.Authorize(Authorization);

            return Ok(await _chatService.CheckNewMessages(request, userId));
        }

        [HttpPut("seen")]
        public async Task<IActionResult> UpdateSeen([FromBody] UpdateSeenRequest request, [FromHeader] string Authorization)
        {
            var userId = _authService.Authorize(Authorization);

            await _chatService.UpdateSeen(request, userId);
            return Ok();
        }
    }
}
