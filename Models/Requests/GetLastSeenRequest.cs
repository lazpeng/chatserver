using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer.Models.Requests
{
    public class GetLastSeenRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Source user id is required")]
        public string SourceId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Target user id is required")]
        public string TargetId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token is required")]
        public string Token { get; set; }
    }
}
