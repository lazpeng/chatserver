using ChatServer.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.DAL.Interfaces
{
    public interface IAuthDAL
    {
        bool IsTokenValid(string UserId, string Token);
        LoginResponse Login(string Username, string Password);
    }
}
