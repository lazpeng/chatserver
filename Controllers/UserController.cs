using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Models;
using ChatServer.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatServer.Controllers.Interfaces;
using ChatServer.Domain.Interfaces;

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
        public IActionResult Search(string username)
        {
            try
            {
                var user = _userDomain.Search(username);
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
                else return StatusCode(500, e.Message);
            }
        }

        ///<summary>
        /// Gets an user based on its id.
        ///<returns>UserModel if found, null if not found</returns>
        ///</summary>
        [HttpGet("{fromId}/get/{id}")]
        public IActionResult Get(string fromId, string id, [FromBody] string token)
        {
            try
            {
                if(!_authDomain.IsTokenValid(fromId, token))
                {
                    return Unauthorized();
                }

                var user = _userDomain.Get(fromId, id);
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
                else return StatusCode(500, e.Message);
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            try
            {
                return Ok(_userDomain.Register(user));
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{SourceId}/friend/add/{TargetId}")]
        public IActionResult AddFriend(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if(!_authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                _userDomain.SendFriendRequest(SourceId, TargetId);
                return Ok();
            } catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{SourceId}/friend/reject/{TargetId}")]
        public IActionResult RejectFriendRequest(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if (!_authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                _userDomain.AnswerFriendRequest(SourceId, TargetId, false);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{SourceId}/friend/remove/{TargetId}")]
        public IActionResult RemoveFriend(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if (!_authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                _userDomain.RemoveFriend(SourceId, TargetId);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{SourceId}/block/add/{TargetId}")]
        public IActionResult BlockUser(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if (!_authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                _userDomain.BlockUser(SourceId, TargetId);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }

        [HttpPut("{SourceId}/block/remove/{TargetId}")]
        public IActionResult UnblockUser(string SourceId, string TargetId, [FromBody] string Token)
        {
            try
            {
                if (!_authDomain.IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                _userDomain.RemoveBlock(SourceId, TargetId);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{SourceId}/friendlist")]
        public IActionResult FriendList(string SourceId, [FromBody] string Token)
        {
            try
            {
                if (!_authDomain.IsTokenValid(SourceId, Token))
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
                else return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{SourceId}/blocklist")]
        public IActionResult BlockList(string SourceId, [FromBody] string Token)
        {
            try
            {
                if (!_authDomain.IsTokenValid(SourceId, Token))
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
                else return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("delete/{FromId}")]
        public IActionResult DeleteAccount(string FromId, [FromBody] string Token)
        {
            try
            {
                if (!_authDomain.IsTokenValid(FromId, Token))
                {
                    return Unauthorized();
                }

                _userDomain.DeleteAccount(FromId);
                return Ok();
            }
            catch (Exception e)
            {
                if (e is ChatBaseException)
                {
                    return BadRequest(e.Message);
                }
                else return StatusCode(500, e.Message);
            }
        }
    }
}