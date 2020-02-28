using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IServerController
    {
        IActionResult ServerInfo();
    }
}
