using ChatServer.Services.Interfaces;
using ChatServer.Models.Responses;
using ChatServer.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Exceptions;
using ChatServer.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;

namespace ChatServer.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ISecretProvider _secretProvider;
        private readonly TimeSpan TokenValidity = TimeSpan.FromHours(12);

        public AuthService(IUserService userDomain, ISecretProvider secretProvider)
        {
            _userService = userDomain;
            _secretProvider = secretProvider;
        }

        public string Authorize(string Token)
        {
            var validationParams = GetValidationParameters();

            var handler = new JwtSecurityTokenHandler();

            try
            {
                var result = handler.ValidateToken(Token, validationParams, out SecurityToken securityToken);
                var userId = result.Claims.First(c => c.Type == "UserId");

                return userId != null ? userId.Value : throw new ChatAuthException("Invalid token");
            } catch (Exception e)
            {
                throw new ChatAuthException(e.Message);
            }
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretProvider.GetSecret()))
            };
        }

        private string GenerateToken(AuthToken token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretProvider.GetSecret()));

            var claims = new Dictionary<string, object>
            {
                ["UserId"] = token.UserId
            };

            var securityDescription = new SecurityTokenDescriptor
            {
                Claims = claims,
                Expires = token.Expiration,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(handler.CreateToken(securityDescription));
        }

        public async Task<LoginResponse> PerformLogin(string UserName, string Password, bool AppearOffline)
        {
            var users = await _userService.Search(UserName, false);
            if(users.Any())
            {
                var user = users.First();

                var hashAndSalt = await _userService.GetPasswordHashAndSalt(user.Id);

                var calculated = HashHelper.CalculateHash(Password, hashAndSalt.Item2);

                if (calculated == hashAndSalt.Item1)
                {
                    if(!AppearOffline)
                    {
                        await _userService.UpdateLastLogin(user.Id);
                        await _userService.UpdateLastSeen(user.Id);
                    }

                    var authToken = new AuthToken
                    {
                        UserId = user.Id,
                        Expiration = DateTime.Now.Add(TokenValidity)
                    };

                    return new LoginResponse
                    {
                        ErrorMessage = "",
                        Success = true,
                        Token = GenerateToken(authToken),
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
