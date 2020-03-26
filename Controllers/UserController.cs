using System;
using ChatServer.Models;
using ChatServer.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatServer.Controllers.Interfaces;
using ChatServer.Domain.Interfaces;
using ChatServer.Models.Requests;
using System.Threading.Tasks;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase, IUserController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserDomain _userDomain;
        private readonly IAuthDomain _authDomain;

        public UserController(ILogger<UserController> logger, IUserDomain userDomain, IAuthDomain authDomain)
        {
            _logger = logger;
            _authDomain = authDomain;
            _userDomain = userDomain;
        }

        ///<summary>
        /// Gets an user based on its username.
        ///<returns>UserModel if found, null if not found</returns>
        ///</summary>
        [HttpGet("search/{username}")]
        public async Task<IActionResult> Search(string username)
        {
            try
            {
                return Ok(await _userDomain.Search(username));
            }
            catch (Exception e)
            {
                if (e is ChatDataException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        ///<summary>
        /// Gets an user based on its id.
        ///<returns>UserModel if found, null if not found</returns>
        ///</summary>
        [HttpPost("{id}")]
        public async Task<IActionResult> Get(string id, [FromBody] GetUserRequest request)
        {
            try
            {
                if(!await _authDomain.IsTokenValid(request.SourceId, request.Token))
                {
                    return Unauthorized();
                }

                var user = await _userDomain.Get(request.SourceId, id);
                if (user != null)
                {
                    return Ok(user);
                }
                else return NotFound();
            }
            catch (Exception e)
            {
                if (e is ChatDataException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> Edit(string Id, [FromBody] EditUserRequest User)
        {
            try
            {
                if(!await _authDomain.IsTokenValid(Id, User.Token))
                {
                    return Unauthorized();
                }

                await _userDomain.Edit(Id, User);

                return Ok(await _userDomain.Get(Id, Id));
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            try
            {
                var result = await _userDomain.Register(user);
                return Created(result.Id, result);
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPut("{SourceId}/friend/add/{TargetId}")]
        public async Task<IActionResult> AddFriend(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if(!await _authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                await _userDomain.SendFriendRequest(SourceId, TargetId);
                return Ok();
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPut("{SourceId}/friend/reject/{TargetId}")]
        public async Task<IActionResult> RejectFriendRequest(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if (!await _authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                await _userDomain.AnswerFriendRequest(SourceId, TargetId, false);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPut("{SourceId}/friend/remove/{TargetId}")]
        public async Task<IActionResult> RemoveFriend(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if (!await _authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                await _userDomain.RemoveFriend(SourceId, TargetId);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPut("{SourceId}/block/add/{TargetId}")]
        public async Task<IActionResult> BlockUser(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if (!await _authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                await _userDomain.BlockUser(SourceId, TargetId);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPut("{SourceId}/block/remove/{TargetId}")]
        public async Task<IActionResult> UnblockUser(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if (!await _authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                await _userDomain.RemoveBlock(SourceId, TargetId);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpGet("{SourceId}/friendlist")]
        public async Task<IActionResult> FriendList(string SourceId, [FromBody] string Token)
        {
            try
            {
                if (!await _authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                return Ok(_userDomain.FriendList(SourceId));
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpGet("{SourceId}/blocklist")]
        public async Task<IActionResult> BlockList(string SourceId, [FromBody] string Token)
        {
            try
            {
                if (!await _authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                return Ok(_userDomain.BlockList(SourceId));
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpDelete("{FromId}")]
        public async Task<IActionResult> DeleteAccount(string FromId, [FromBody] string Token)
        {
            try
            {
                if (!await _authDomain.IsTokenValid(FromId, Token))
                {
                    return Unauthorized();
                }

                await _userDomain.DeleteAccount(FromId);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPost("checkUpdate")]
        public async Task<IActionResult> CheckUsersUpdate([FromBody] CheckUserUpdateRequest Request)
        {
            try
            {
                if (!await _authDomain.IsTokenValid(Request.SourceId, Request.Token))
                {
                    return Unauthorized();
                }

                return Ok(await _userDomain.CheckUsersUpdate(Request));
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else
                {
                    _logger.LogError($"Exception occurred. {e.Message}\n{e.StackTrace}");
                    return StatusCode(500, e.Message);
                }
            }
        }
    }
}