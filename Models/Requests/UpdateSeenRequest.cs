using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class UpdateSeenRequest : BaseAuthenticatedRequest
    {
        [Required(ErrorMessage = "Last seen message id is required")]
        public long LastSeenId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Target user chat id is required")]
        public string TargetId { get; set; }
    }
}
