using System;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class SendMessageRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required target")]
        public string TargetId { get; set; }
        public DateTime DateSent { get; set; } = DateTime.Now;
        [Required(AllowEmptyStrings = true, ErrorMessage = "Message content is required")]
        public string Content { get; set; }
        public long? ReplyTo { get; set; }
    }
}
