using System;
using System.Collections.Generic;

namespace pollbackend.Application.DTOs
{
    public class CastVoteDto
    {
        public long PollId { get; set; }
        public long PollOptionId { get; set; }
    }

    public class VoteResultItemDto
    {
        public long OptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
        public int VoteCount { get; set; }
        public double Percentage { get; set; }
    }

    public class PollResultDto
    {
        public long PollId { get; set; }
        public string PollTitle { get; set; } = string.Empty;
        public int TotalVotes { get; set; }
        public List<VoteResultItemDto> Results { get; set; } = new List<VoteResultItemDto>();
    }

    public class UserVoteDto
    {
        public long PollId { get; set; }
        public string PollTitle { get; set; } = string.Empty;
        public long VotedOptionId { get; set; }
        public string VotedOptionText { get; set; } = string.Empty;
        public DateTime VotedAt { get; set; }
    }
}
