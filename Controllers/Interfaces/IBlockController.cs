using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ChatServer.Models.Requests;

namespace ChatServer.Controllers.Interfaces
{
    public interface IBlockController
    {
        Task<IActionResult> Block(string Blocked, [FromBody] BaseAuthenticatedRequest Request);
        Task<IActionResult> RemoveBlock(string Blocked, [FromBody] BaseAuthenticatedRequest Request);
        Task<IActionResult> GetBlockedList(string SourceId, string Token);
    }
}
