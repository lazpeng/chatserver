using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.DAL;
using ChatServer.Exceptions;
using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [HttpPut("send")]
        public IActionResult SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var targetUser = UserDAL.Get(request.TargetId);

                var openChatNotValid = !targetUser.OpenChat && !UserDAL.AreUsersFriends(request.SourceId, request.TargetId);

                if(!AuthDAL.IsTokenValid(request.SourceId, request.Token) || openChatNotValid)
                {
                    return Unauthorized();
                }

                ChatDAL.SendMessage(request);
                return Ok();
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }

        [HttpGet("check")]
        public IActionResult CheckNewMessages([FromBody] CheckNewRequest request)
        {
            try
            {
                if(!AuthDAL.IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                return Ok(ChatDAL.CheckNewMessages(request));
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }

        [HttpPut("seen")]
        public IActionResult UpdateSeen([FromBody] UpdateSeenRequest request)
        {
            try
            {
                if (!AuthDAL.IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                ChatDAL.UpdateSeen(request);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }
    }
}
