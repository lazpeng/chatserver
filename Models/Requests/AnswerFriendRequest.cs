namespace ChatServer.Models.Requests
{
    public class AnswerFriendRequest : BaseAuthenticatedRequest
    {
        public bool Accepted { get; set; }
    }
}
