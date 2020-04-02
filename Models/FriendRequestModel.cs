using System;

namespace ChatServer.Models
{
    public class FriendRequestModel
    {
        public long Id { get; set; }
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public DateTime SentDate { get; set; }
        public bool Accepted { get; set; }
        public DateTime? AnswerDate { get; set; }
        public bool Active { get; set; }
    }
}
