using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class LoginRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required username")]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required password")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
        public bool AppearOffline { get; set; } = false;
    }
}