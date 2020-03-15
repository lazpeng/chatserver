using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models.Requests
{
    public class DeleteMessageRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public long MessageId { get; set; }
        [Required]
        public DateTime DateDeleted { get; set; }
    }
}
