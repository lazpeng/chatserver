using ChatServer.Models;
using ChatServer.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatServer.Domain.Interfaces
{
    public interface IUserService
    {
        Task SendFriendRequest(string SourceId, string TargetId);
        Task RemoveFriend(string SourceId, string TargetId);
        Task BlockUser(string SourceId, string TargetId);
        Task RemoveBlock(string SourceId, string TargetId);
        Task<bool> CanSendMessage(string FromUser, string ToUser);
        Task<List<FriendModel>> FriendList(string UserId);
        Task<List<string>> BlockList(string UserId);
        Task AnswerFriendRequest(string UserId, string SourceId, bool Answer);
        Task<UserModel> Register(UserModel user);
        Task<UserModel> Get(string FromId, string Id);
        Task<List<UserModel>> Search(string Username, bool Partial = true);
        Task DeleteAccount(string UserId);
        Task UpdateLastLogin(string UserId);
        Task UpdateLastSeen(string UserId);
        Task Edit(string TargetId, UserModel Request);
        Task UpdateUserPassword(string UserId, string Password);
        Task<List<string>> CheckUsersUpdate(CheckUserUpdateRequest Request);
    }
}
