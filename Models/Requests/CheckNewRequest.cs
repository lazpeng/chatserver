using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class CheckNewRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Source user id is required")]
        public string SourceId { get; set; }
        [Required(ErrorMessage = "Last received message is required")]
        public long LastReceivedId { get; set; }
        [Required]
        public long LastDeletedId { get; set; }
        [Required]
        public long LastEditedId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token is required")]
        public string Token { get; set; }
        [Required]
        public bool AppearOffline { get; set; }
    }
}
