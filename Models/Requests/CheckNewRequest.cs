using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class CheckNewRequest : BaseAuthenticatedRequest
    {
        [Required(ErrorMessage = "Last received message is required")]
        public long LastReceivedId { get; set; }
        [Required]
        public long LastDeletedId { get; set; }
        [Required]
        public long LastEditedId { get; set; }
        [Required]
        public bool AppearOffline { get; set; }
    }
}
