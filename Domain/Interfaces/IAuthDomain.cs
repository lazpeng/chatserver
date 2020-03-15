using ChatServer.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Domain.Interfaces
{
    public interface IAuthDomain
    {
        void UpdateUserPassword(string UserId, string Password);

        bool IsTokenValid(string UserId, string Token);

        LoginResponse PerformLogin(string UserName, string Password);
    }
}
