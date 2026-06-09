using System;
using System.Collections.Generic;
using pollbackend.Domain.Enums;

namespace pollbackend.Domain.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Role Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Poll> Polls { get; set; } = new List<Poll>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}
