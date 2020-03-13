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
    public class SessionController : ControllerBase, ISessionController
    {
        private readonly ILogger<SessionController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;

        public SessionController(ILogger<SessionController> logger, IUserRepository userRepository, IAuthRepository authRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _authRepository = authRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                return Ok(new AuthDomain(_authRepository, _userRepository).PerformLogin(loginRequest.Username, loginRequest.Password));
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
