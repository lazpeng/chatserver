using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class GetLastSeenRequest : BaseAuthenticatedRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Target user id is required")]
        public string TargetId { get; set; }
    }
}
