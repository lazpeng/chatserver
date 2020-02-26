using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class UpdateSeenRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Source user id is required")]
        public string SourceId { get; set; }
        [Required(ErrorMessage = "Last seen message id is required")]
        public ulong LastReadId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Target user chat id is required")]
        public string TargetId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Token is required")]
        public string Token { get; set; }
    }
}
