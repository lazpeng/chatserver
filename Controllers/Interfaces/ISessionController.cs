using ChatServer.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface ISessionController
    {
        Task<IActionResult> Login([FromBody] LoginRequest Request);
        IActionResult Check([FromHeader] string Authorization);
    }
}
