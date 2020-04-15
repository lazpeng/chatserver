using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IChatController
    {
        Task<IActionResult> SendMessage([FromBody] SendMessageRequest Request, [FromHeader] string Authorization);
        Task<IActionResult> CheckNewMessages([FromBody] CheckNewRequest Request, [FromHeader] string Authorization);
        Task<IActionResult> UpdateSeen([FromBody] UpdateSeenRequest Request, [FromHeader] string Authorization);
    }
}
