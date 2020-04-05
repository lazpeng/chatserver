using ChatServer.Controllers.Interfaces;
using ChatServer.Models.Requests;
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
        public async Task<IActionResult> Block(string Blocked, [FromBody] BaseAuthenticatedRequest Request)
        {
            await _authService.Authorize(Request);

            await _blockService.Block(Request.SourceId, Blocked);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetBlockedList([FromQuery] string SourceId, [FromQuery] string Token)
        {
            await _authService.Authorize(new BaseAuthenticatedRequest { SourceId = SourceId, Token = Token });

            return Ok(await _blockService.GetBlockedList(SourceId));
        }

        [HttpDelete("{Blocked}")]
        public async Task<IActionResult> RemoveBlock(string Blocked, [FromBody] BaseAuthenticatedRequest Request)
        {
            await _authService.Authorize(Request);

            await _blockService.RemoveBlock(Request.SourceId, Blocked);
            return Ok();
        }
    }
}
