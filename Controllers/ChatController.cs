using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Controllers.Interfaces;
using ChatServer.Domain;
using ChatServer.Exceptions;
using ChatServer.Models.Requests;
using ChatServer.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase, IChatController
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;

        public ChatController(ILogger<ChatController> logger, IChatRepository chatRepository, IUserRepository userRepository, IAuthRepository authRepository)
        {
            _logger = logger;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _authRepository = authRepository;
        }

        [HttpPut("send")]
        public IActionResult SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                // TODO: Chat domain
                var targetUser = _userRepository.Get(request.TargetId);

                var openChatNotValid = !targetUser.OpenChat && !_userRepository.AreUsersFriends(request.SourceId, request.TargetId);

                if(!new AuthDomain(_authRepository, _userRepository).IsTokenValid(request.SourceId, request.Token) || openChatNotValid)
                {
                    return Unauthorized();
                }

                return Ok(_chatRepository.SendMessage(request));
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
                if(!new AuthDomain(_authRepository, _userRepository).IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                return Ok(_chatRepository.CheckNewMessages(request));
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }

        [HttpPost("seen")]
        public IActionResult UpdateSeen([FromBody] UpdateSeenRequest request)
        {
            try
            {
                if (!new AuthDomain(_authRepository, _userRepository).IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                _chatRepository.UpdateSeenMessage(request);
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
