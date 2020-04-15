using System;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class DeleteMessageRequest
    {
        [Required]
        public long MessageId { get; set; }
        [Required]
        public DateTime DateDeleted { get; set; }
    }
}
