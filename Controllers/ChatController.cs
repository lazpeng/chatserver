using System;
using System.Threading.Tasks;
using ChatServer.Controllers.Interfaces;
using ChatServer.Domain.Interfaces;
using ChatServer.Exceptions;
using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase, IChatController
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatDomain _chatDomain;
        private readonly IAuthDomain _authDomain;

        public ChatController(ILogger<ChatController> logger, IChatDomain chatDomain, IAuthDomain authDomain)
        {
            _logger = logger;
            _chatDomain = chatDomain;
            _authDomain = authDomain;
        }

        [HttpPut("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                if (! await _authDomain.IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                return Ok(_chatDomain.SendMessage(request));
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                } else if(e is ChatPermissionException)
                {
                    return Unauthorized(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckNewMessages([FromBody] CheckNewRequest request)
        {
            try
            {
                if(! await _authDomain.IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                return Ok(_chatDomain.CheckNewMessages(request));
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPost("seen")]
        public async Task<IActionResult> UpdateSeen([FromBody] UpdateSeenRequest request)
        {
            try
            {
                if (! await _authDomain.IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                await _chatDomain.UpdateSeen(request);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }
    }
}
