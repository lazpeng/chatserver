using ChatServer.Controllers.Interfaces;
using ChatServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlockController : ControllerBase, IBlockController
    {
        private readonly IAuthService _authService;
        private readonly IBlockService _blockService;

        public BlockController(IAuthService authService, IBlockService blockService)
        {
            _authService = authService;
            _blockService = blockService;
        }

        [HttpPost("{Blocked}")]
        public async Task<IActionResult> Block(string Blocked, [FromHeader] string Authorization)
        {
            var SourceId = _authService.Authorize(Authorization);

            await _blockService.Block(SourceId, Blocked);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetBlockedList([FromHeader] string Authorization)
        {
            var SourceId = _authService.Authorize(Authorization);

            return Ok(await _blockService.GetBlockedList(SourceId));
        }

        [HttpDelete("{Blocked}")]
        public async Task<IActionResult> RemoveBlock(string Blocked, [FromHeader] string Authorization)
        {
            var SourceId = _authService.Authorize(Authorization);

            await _blockService.RemoveBlock(SourceId, Blocked);
            return Ok();
        }
    }
}
