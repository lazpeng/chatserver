using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers.Interfaces
{
    public interface IBlockController
    {
        Task<IActionResult> Block(string Blocked, [FromHeader] string Authorization);
        Task<IActionResult> RemoveBlock(string Blocked, [FromHeader] string Authorization);
        Task<IActionResult> GetBlockedList([FromHeader] string Authorization);
    }
}
