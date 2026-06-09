using System;
using System.Collections.Generic;

namespace pollbackend.Dtos
{
    public class CreatePollDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
    }

    public class PollOptionDto
    {
        public long Id { get; set; }
        public string OptionText { get; set; } = string.Empty;
    }

    public class PollDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PollOptionDto> Options { get; set; } = new List<PollOptionDto>();
    }
}
