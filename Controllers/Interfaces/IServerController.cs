using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers.Interfaces
{
    public interface IServerController
    {
        IActionResult ServerInfo();
    }
}
