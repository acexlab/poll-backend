using System;
using System.Collections.Generic;

namespace pollbackend.Models
{
    public class Poll
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsEnabled { get; set; } = false;
        public bool ShowResults { get; set; } = false;
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User Creator { get; set; } = null!;
        public ICollection<PollOption> Options { get; set; } = new List<PollOption>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
