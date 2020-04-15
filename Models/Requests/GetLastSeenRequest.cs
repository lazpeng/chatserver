using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class GetLastSeenRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Target user id is required")]
        public string TargetId { get; set; }
    }
}
