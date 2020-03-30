using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatServer.Models.Requests
{
    public class CheckUserUpdateModel
    {
        public string UserId { get; set; }
        public string LastDataHash { get; set; }
    }

    public class CheckUserUpdateRequest : BaseAuthenticatedRequest
    {
        public List<CheckUserUpdateModel> KnownUsers { get; set; }
    }
}
