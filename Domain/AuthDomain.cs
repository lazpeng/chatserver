using ChatServer.Models.Responses;
using ChatServer.Repositories.Interfaces;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ChatServer.Domain
{
    public class AuthDomain
    {
        private readonly IAuthRepository authRepository;
        private readonly IUserRepository userRepository;
        private readonly TimeSpan TokenValidity = TimeSpan.FromHours(12);

        public AuthDomain(IAuthRepository authRepository, IUserRepository userRepository)
        {
            this.authRepository = authRepository;
            this.userRepository = userRepository;
        }

        private string GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var buffer = new byte[32];
            rng.GetBytes(buffer);

            return Convert.ToBase64String(buffer);
        }

        private string CalculateHash(string password, string salt)
        {
            using var algo = new SHA256Managed();

            var finalBytes = Encoding.UTF8.GetBytes(password + salt);

            return Convert.ToBase64String(algo.ComputeHash(finalBytes));
        }

        public void UpdateUserPassword(string UserId, string Password)
        {
            var newSalt = GenerateSalt();
            var newHash = CalculateHash(Password, newSalt);

            authRepository.SavePasswordHash(UserId, newHash);
        }

        public bool IsTokenValid(string UserId, string Token)
        {
            return authRepository.GetValidTokensForUser(UserId).Contains(Token);
        }

        public LoginResponse PerformLogin(string UserName, string Password)
        {
            var users = userRepository.Find(UserName, false);
            if(users.Any())
            {
                var user = users.First();

                var hashAndSalt = authRepository.GetPasswordHashAndSalt(user.Id);

                var calculated = CalculateHash(Password, hashAndSalt.Item2);

                if (calculated == hashAndSalt.Item1)
                {
                    var token = Guid.NewGuid().ToString();

                    authRepository.SaveNewToken(user.Id, token, TokenValidity);
                    userRepository.UpdateLastLogin(user.Id);
                    userRepository.UpdateLastSeen(user.Id);

                    return new LoginResponse
                    {
                        ErrorMessage = "",
                        Success = true,
                        Token = token,
                        Id = user.Id
                    };
                }
            }

            return new LoginResponse
            {
                ErrorMessage = "No such username/password combination",
                Success = false,
                Token = null,
                Id = null
            };
        }
    }
}
