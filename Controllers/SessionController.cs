using System.Threading.Tasks;
using ChatServer.Controllers.Interfaces;
using ChatServer.Services.Interfaces;
using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase, ISessionController
    {
        private readonly IAuthService _authService;

        public SessionController(IAuthService authDomain)
        {
            _authService = authDomain;
        }

        [HttpPost("check")]
        public IActionResult Check([FromHeader] string Authorization)
        {
            _authService.Authorize(Authorization);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            return Ok(await _authService.PerformLogin(loginRequest.Username, loginRequest.Password, loginRequest.AppearOffline));
        }
    }
}
