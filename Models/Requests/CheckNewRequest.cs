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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Target user id is required")]
        public string TargetId { get; set; }
        [Required(ErrorMessage = "Last received message is required")]
        public ulong LastReceivedId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token is required")]
        public string Token { get; set; }
    }
}
