using Microsoft.EntityFrameworkCore;
using pollbackend.Models;
using System;

namespace pollbackend.Data
{
    public class PollDbContext : DbContext
    {
        public PollDbContext(DbContextOptions<PollDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Poll> Polls { get; set; } = null!;
        public DbSet<PollOption> PollOptions { get; set; } = null!;
        public DbSet<Vote> Votes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Entity Configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Role)
                    .HasConversion<string>()
                    .IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // Poll Entity Configurations
            modelBuilder.Entity<Poll>(entity =>
            {
                entity.ToTable("Polls");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasColumnType("TEXT");
                entity.Property(e => e.IsEnabled).HasDefaultValue(false);
                entity.Property(e => e.ShowResults).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.IsEnabled).HasDatabaseName("idx_poll_enabled");

                entity.HasOne(e => e.Creator)
                    .WithMany(u => u.Polls)
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PollOption Entity Configurations
            modelBuilder.Entity<PollOption>(entity =>
            {
                entity.ToTable("PollOptions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.OptionText).IsRequired().HasMaxLength(255);

                entity.HasOne(e => e.Poll)
                    .WithMany(p => p.Options)
                    .HasForeignKey(e => e.PollId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Vote Entity Configurations
            modelBuilder.Entity<Vote>(entity =>
            {
                entity.ToTable("Votes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.VotedAt).IsRequired();

                // Composite unique constraint: UserId and PollId
                entity.HasIndex(e => new { e.UserId, e.PollId }).IsUnique();

                // Custom Indexes
                entity.HasIndex(e => e.PollId).HasDatabaseName("idx_vote_poll");
                entity.HasIndex(e => e.UserId).HasDatabaseName("idx_vote_user");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Votes)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Poll)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(e => e.PollId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PollOption)
                    .WithMany(po => po.Votes)
                    .HasForeignKey(e => e.PollOptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed initial data
            var adminId = 1L;
            var userId = 2L;

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    Username = "admin",
                    Email = "admin@pollingapp.com",
                    PasswordHash = "$2a$12$Z0H9kU.iJg1tEebmPZ/jQO4q0kS/qI4K1b3qDq5.O3G2Z3P6lO7yG",
                    Role = Role.Admin,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = userId,
                    Username = "user",
                    Email = "user@pollingapp.com",
                    PasswordHash = "$2a$12$hCenLp787qOswX3T.j03WODNq3b4V16W0y9lM.U.Z4aC8a6v6rPte",
                    Role = Role.User,
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
