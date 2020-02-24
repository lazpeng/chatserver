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

namespace ChatServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private ILogger<UserController> _logger;

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
            try {
                var user = UserDAL.Find(username);
                if(user != null)
                {
                    return Ok(user);
                } else return NotFound();
            } catch (Exception e) {
                if(e is DataException)
                {
                    return BadRequest(e.Message);
                } else return StatusCode(500, e.Message);
            }
        }

        ///<summary>
        /// Gets an user based on its id.
        ///<returns>UserModel if found, null if not found</returns>
        ///</summary>
        [HttpGet("get/{id}")]
        public IActionResult Get(string id)
        {
            try {
                var user = UserDAL.Get(id);
                if(user != null)
                {
                    return Ok(user);
                } else return NotFound();
            } catch (Exception e) {
                if(e is DataException)
                {
                    return BadRequest(e.Message);
                } else return StatusCode(500, e.Message);
            }
        }
    }
}