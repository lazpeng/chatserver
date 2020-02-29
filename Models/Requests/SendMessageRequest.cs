using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class SendMessageRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required Source")]
        public string SourceId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required target")]
        public string TargetId { get; set; }
        public DateTime DateSent { get; set; } = DateTime.Now;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Session token required")]
        public string Token { get; set; }
        [Required(AllowEmptyStrings = true, ErrorMessage = "Message content is required")]
        public string Content { get; set; }
        public long? ReplyTo { get; set; }
    }
}
