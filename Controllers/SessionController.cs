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
    public class SessionController : ControllerBase
    {
        private readonly ILogger<SessionController> _logger;

        public SessionController(ILogger<SessionController> logger)
        {
            _logger = logger;
        }

        [HttpPut("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                return Ok(AuthDAL.Login(loginRequest.Username, loginRequest.Password));
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
