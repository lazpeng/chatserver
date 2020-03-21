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
    public class SessionController : ControllerBase, ISessionController
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IAuthDomain _authDomain;

        public SessionController(ILogger<SessionController> logger, IAuthDomain authDomain)
        {
            _logger = logger;
            _authDomain = authDomain;
        }

        public async Task<IActionResult> Check([FromBody] CheckRequest Request)
        {
            try
            {
                if (!await _authDomain.IsTokenValid(Request.UserId, Request.Token))
                {
                    return Unauthorized();
                }
                else return Ok();
            } catch (Exception e)
            {
                _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                return Ok(await _authDomain.PerformLogin(loginRequest.Username, loginRequest.Password, loginRequest.AppearOffline));
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
    }
}
