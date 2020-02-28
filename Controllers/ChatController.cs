using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Controllers.Interfaces;
using ChatServer.DAL;
using ChatServer.DAL.SqlServer;
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

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [HttpPut("send")]
        public IActionResult SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var userDAL = new UserDAL();
                var targetUser = userDAL.Get(request.TargetId);

                var openChatNotValid = !targetUser.OpenChat && !userDAL.AreUsersFriends(request.SourceId, request.TargetId);

                if(!new AuthDAL().IsTokenValid(request.SourceId, request.Token) || openChatNotValid)
                {
                    return Unauthorized();
                }

                return Ok(new ChatDAL().SendMessage(request));
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
                if(!new AuthDAL().IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                return Ok(new ChatDAL().CheckNewMessages(request));
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
                if (!new AuthDAL().IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                new ChatDAL().UpdateSeen(request);
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
