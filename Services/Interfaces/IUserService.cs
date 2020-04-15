using System;
using ChatServer.Models;
using ChatServer.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserModel> Register(UserModel user);
        Task<UserModel> Get(string FromId, string Id);
        Task<List<UserModel>> Search(string Username, bool Partial = true);
        Task DeleteAccount(string UserId);
        Task UpdateLastLogin(string UserId);
        Task UpdateLastSeen(string UserId);
        Task Edit(string TargetId, UserModel Request);
        Task UpdateUserPassword(string UserId, string Password);
        Task<List<string>> CheckUsersUpdate(CheckUserUpdateRequest Request);
        Task<Tuple<string, string>> GetPasswordHashAndSalt(string UserId);
    }
}
