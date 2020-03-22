using ChatServer.Domain.Interfaces;
using ChatServer.Models;
using ChatServer.Repositories.Interfaces;
using ChatServer.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using ChatServer.Helpers;
using ChatServer.Models.Requests;

namespace ChatServer.Domain
{
    public class UserDomain : IUserDomain
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;

        public UserDomain(IUserRepository userRepository, IAuthRepository authRepository)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
        }

        public async Task SendFriendRequest(string SourceId, string TargetId)
        {
            if(await _userRepository.IsUserBlocked(TargetId, SourceId))
            {
                throw new ChatBaseException("Cannot send friend request");
            } else if(await _userRepository.HasFriendRequestSentToTarget(TargetId, SourceId))
            {
                await _userRepository.AnswerFriendRequest(TargetId, SourceId, true);
            } else if(! await _userRepository.AreUsersFriends(SourceId, TargetId) && ! await _userRepository.HasFriendRequestSentToTarget(SourceId, TargetId))
            {
                await _userRepository.SendFriendRequest(SourceId, TargetId);
            }
        }

        public async Task RemoveFriend(string SourceId, string TargetId)
        {
            await _userRepository.RemoveFriend(SourceId, TargetId);
        }

        public async Task BlockUser(string SourceId, string TargetId)
        {
            if(! await _userRepository.IsUserBlocked(SourceId, TargetId))
            {
                await _userRepository.BlockUser(SourceId, TargetId);
            }
        }

        public async Task RemoveBlock(string SourceId, string TargetId)
        {
            await _userRepository.UnblockUser(SourceId, TargetId);
        }

        public async Task<bool> CanSendMessage(string FromUser, string ToUser)
        {
            var target = await _userRepository.Get(ToUser);

            if(target != null)
            {
                return target.OpenChat || await _userRepository.AreUsersFriends(FromUser, ToUser);
            }

            return false;
        }

        public async Task<List<FriendModel>> FriendList(string UserId)
        {
            return await _userRepository.FriendList(UserId);
        }

        public async Task<List<string>> BlockList(string UserId)
        {
            return await _userRepository.BlockList(UserId);
        }

        public async Task AnswerFriendRequest(string UserId, string SourceId, bool Answer)
        {
            await _userRepository.AnswerFriendRequest(UserId, SourceId, Answer);
        }

        public async Task<UserModel> Register(UserModel user)
        {
            user.DataHash = GenerateSalt();
            return await _userRepository.Register(user);
        }

        public async Task<UserModel> Get(string FromId, string Id)
        {
            var user = await _userRepository.Get(Id);

            if(!await _userRepository.AreUsersFriends(Id, FromId))
            {
                user.DateOfBirth = DateTime.MinValue;
                user.FullName = user.Email = null;
                user.LastLogin = user.LastSeen = user.AccountCreated = null;
            }

            return user;
        }

        public async Task<List<UserModel>> Search(string Username, bool Partial = true)
        {
            return await _userRepository.Find(Username, Partial);
        }

        public async Task DeleteAccount(string UserId)
        {
            await _userRepository.DeleteAccount(UserId);
        }

        public async Task UpdateLastLogin(string UserId)
        {
            await _userRepository.UpdateLastLogin(UserId);
        }

        public async Task UpdateLastSeen(string UserId)
        {
            await _userRepository.UpdateLastSeen(UserId);
        }

        private string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[32];
            rng.GetBytes(buffer);

            return Convert.ToBase64String(buffer);
        }

        public async Task UpdateUserPassword(string UserId, string Password)
        {
            var newSalt = GenerateSalt();
            var newHash = HashHelper.CalculateHash(Password, newSalt);

            await _authRepository.SavePasswordHash(UserId, newHash, newSalt);
        }

        public async Task Edit(string Id, UserModel User)
        {
            User.Id = Id;
            User.DataHash = GenerateSalt();

            await _userRepository.Edit(User);

            if(! string.IsNullOrEmpty(User.Password))
            {
                if(User.Password.Length < 6)
                {
                    throw new ChatBaseException("Password length must be >= 6 characters");
                }

                await UpdateUserPassword(Id, User.Password);
            }
        }

        public async Task<List<string>> CheckUsersUpdate(CheckUserUpdateRequest Request)
        {
            var outdated = new List<string>();

            foreach(var user in Request.KnownUsers)
            {
                if(! await _userRepository.IsUserUpToDate(user.UserId, user.LastDataHash))
                {
                    outdated.Add(user.UserId);
                }
            }

            return outdated;
        }
    }
}
