using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class BaseAuthenticatedRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Source user Id is required")]
        public string SourceId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "User token is required")]
        public string Token { get; set; }
    }
}
