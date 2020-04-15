using System;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class EditMessageRequest
    {
        [Required]
        public long MessageId { get; set; }
        [Required]
        public string NewContent { get; set; }
        [Required]
        public DateTime DateEdited { get; set; }
    }
}
