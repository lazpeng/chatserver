using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IChatController
    {
        IActionResult SendMessage([FromBody] SendMessageRequest Request);
        IActionResult CheckNewMessages([FromBody] CheckNewRequest Request);
        IActionResult UpdateSeen([FromBody] UpdateSeenRequest Request);
    }
}
