using System.Collections.Generic;

namespace ChatServer.Models.Requests
{
    public class CheckUserUpdateModel
    {
        public string UserId { get; set; }
        public string LastDataHash { get; set; }
    }

    public class CheckUserUpdateRequest
    {
        public List<CheckUserUpdateModel> KnownUsers { get; set; }
    }
}
