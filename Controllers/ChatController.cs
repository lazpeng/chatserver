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

        [HttpPut("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            await _authService.Authorize(request);

            return Ok(_chatService.SendMessage(request));
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckNewMessages([FromBody] CheckNewRequest request)
        {
            await _authService.Authorize(request);

            return Ok(_chatService.CheckNewMessages(request));
        }

        [HttpPost("seen")]
        public async Task<IActionResult> UpdateSeen([FromBody] UpdateSeenRequest request)
        {
            await _authService.Authorize(request);

            await _chatService.UpdateSeen(request);
            return Ok();
        }
    }
}
