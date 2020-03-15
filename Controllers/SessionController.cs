using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Controllers.Interfaces;
using ChatServer.Domain;
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                return Ok(_authDomain.PerformLogin(loginRequest.Username, loginRequest.Password));
            } catch (Exception e)
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
