using ChatServer.Controllers.Interfaces;
using ChatServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerController : ControllerBase, IServerController
    {
        [HttpGet]
        public IActionResult ServerInfo()
        {
            return Ok(new ServerInfoModel());
        }
    }
}
