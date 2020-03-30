using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class SendMessageRequest : BaseAuthenticatedRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required target")]
        public string TargetId { get; set; }
        public DateTime DateSent { get; set; } = DateTime.Now;
        [Required(AllowEmptyStrings = true, ErrorMessage = "Message content is required")]
        public string Content { get; set; }
        public long? ReplyTo { get; set; }
    }
}
