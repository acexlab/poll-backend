using System;

namespace pollbackend.Models
{
    public class Vote
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long PollId { get; set; }
        public long PollOptionId { get; set; }
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public Poll Poll { get; set; } = null!;
        public PollOption PollOption { get; set; } = null!;
    }
}
