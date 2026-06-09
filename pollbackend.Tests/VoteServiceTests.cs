using Moq;
using pollbackend.Dtos;
using pollbackend.Data;
using pollbackend.Services;
using pollbackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace pollbackend.Tests
{
    public class VoteServiceTests
    {
        private readonly Mock<IVoteRepository> _voteRepoMock;
        private readonly Mock<IPollRepository> _pollRepoMock;
        private readonly VoteService _voteService;

        public VoteServiceTests()
        {
            _voteRepoMock = new Mock<IVoteRepository>();
            _pollRepoMock = new Mock<IPollRepository>();
            _voteService = new VoteService(_voteRepoMock.Object, _pollRepoMock.Object);
        }

        [Fact]
        public async Task CastVote_Should_Succeed_When_Valid()
        {
            // Arrange
            var poll = new Poll
            {
                Id = 1,
                IsEnabled = true,
                Options = new List<PollOption> { new PollOption { Id = 10, PollId = 1, OptionText = "Opt 1" } }
            };

            _pollRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(poll);
            _voteRepoMock.Setup(r => r.HasUserVotedOnPollAsync(1, 1)).ReturnsAsync(false);
            _voteRepoMock.Setup(r => r.AddAsync(It.IsAny<Vote>())).Returns(Task.CompletedTask);

            var castVoteDto = new CastVoteDto { PollId = 1, PollOptionId = 10 };

            // Act
            var result = await _voteService.CastVoteAsync(castVoteDto, 1);

            // Assert
            Assert.True(result);
            _voteRepoMock.Verify(r => r.AddAsync(It.IsAny<Vote>()), Times.Once);
        }

        [Fact]
        public async Task CastVote_Should_Throw_When_Already_Voted()
        {
            // Arrange
            var poll = new Poll
            {
                Id = 1,
                IsEnabled = true,
                Options = new List<PollOption> { new PollOption { Id = 10, PollId = 1, OptionText = "Opt 1" } }
            };

            _pollRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(poll);
            _voteRepoMock.Setup(r => r.HasUserVotedOnPollAsync(1, 1)).ReturnsAsync(true);

            var castVoteDto = new CastVoteDto { PollId = 1, PollOptionId = 10 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _voteService.CastVoteAsync(castVoteDto, 1));
        }

        [Fact]
        public async Task CastVote_Should_Throw_When_Poll_Disabled()
        {
            // Arrange
            var poll = new Poll
            {
                Id = 1,
                IsEnabled = false,
                Options = new List<PollOption> { new PollOption { Id = 10, PollId = 1, OptionText = "Opt 1" } }
            };

            _pollRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(poll);

            var castVoteDto = new CastVoteDto { PollId = 1, PollOptionId = 10 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _voteService.CastVoteAsync(castVoteDto, 1));
        }
    }
}
