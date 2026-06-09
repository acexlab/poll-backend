using System.Collections.Generic;

namespace pollbackend.Models
{
    public class PollOption
    {
        public long Id { get; set; }
        public long PollId { get; set; }
        public string OptionText { get; set; } = string.Empty;

        // Navigation properties
        public Poll Poll { get; set; } = null!;
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
