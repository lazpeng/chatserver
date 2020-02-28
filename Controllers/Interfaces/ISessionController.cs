using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface ISessionController
    {
        IActionResult Login([FromBody] LoginRequest Request);
    }
}
