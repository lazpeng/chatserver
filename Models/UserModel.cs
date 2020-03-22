using System;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Must provide username")]
        [MaxLength(128, ErrorMessage = "Username must have at most 128 characters")]
        public string Username { get; set; }
        [Required]
        [MaxLength(128, ErrorMessage = "Full name must have at most 256 characters")]
        public string FullName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(256, ErrorMessage = "Email address must have at most 256 characters")]
        public string Email { get; set; }
        public string Bio { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? AccountCreated { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool FindInSearch { get; set; } = true;
        public bool OpenChat { get; set; } = true;
        [MinLength(6, ErrorMessage = "Password must have at least 6 characters")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string ProfilePicUrl { get; set; }
        public string DataHash { get; set; }
    }
}