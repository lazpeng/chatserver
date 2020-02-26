using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.DAL;
using ChatServer.Models;
using ChatServer.Models.Responses;
using ChatServer.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChatServer.DAL.SqlServer;

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        ///<summary>
        /// Gets an user based on its username.
        ///<returns>UserModel if found, null if not found</returns>
        ///</summary>
        [HttpGet("find/{username}")]
        public IActionResult Find(string username)
        {
            try
            {
                var user = new UserDAL().Find(username);
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
                if(!new AuthDAL().IsTokenValid(token, fromId))
                {
                    return Unauthorized();
                }

                var user = new UserDAL().Get(id);
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
                return Ok(new UserDAL().Register(user));
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
                if(!new AuthDAL().IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                new UserDAL().SendFriendRequest(SourceId, TargetId);
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
                if (!new AuthDAL().IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                new UserDAL().AnswerFriendRequest(SourceId, TargetId, false);
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
                if (!new AuthDAL().IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                new UserDAL().RemoveFriend(SourceId, TargetId);
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
                if (!new AuthDAL().IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                new UserDAL().BlockUser(SourceId, TargetId);
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
                if (!new AuthDAL().IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                new UserDAL().UnblockUser(SourceId, TargetId);
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
                if (!new AuthDAL().IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                return Ok(new UserDAL().FriendList(SourceId));
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
                if (!new AuthDAL().IsTokenValid(SourceId, Token))
                {
                    return Unauthorized();
                }

                return Ok(new UserDAL().BlockList(SourceId));
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