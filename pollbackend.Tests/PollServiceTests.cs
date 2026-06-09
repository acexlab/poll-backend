using AutoMapper;
using Moq;
using pollbackend.Application.DTOs;
using pollbackend.Application.Interfaces;
using pollbackend.Application.Services;
using pollbackend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace pollbackend.Tests
{
    public class PollServiceTests
    {
        private readonly Mock<IPollRepository> _pollRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PollService _pollService;

        public PollServiceTests()
        {
            _pollRepoMock = new Mock<IPollRepository>();
            _mapperMock = new Mock<IMapper>();
            _pollService = new PollService(_pollRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreatePoll_Should_Create_And_Return_Dto()
        {
            // Arrange
            var createPollDto = new CreatePollDto
            {
                Title = "Test Poll",
                Description = "Test description",
                Options = new List<string> { "Opt A", "Opt B" }
            };

            var poll = new Poll { Id = 1, Title = createPollDto.Title, Description = createPollDto.Description };
            var pollDto = new PollDto { Id = 1, Title = createPollDto.Title, Description = createPollDto.Description };

            _pollRepoMock.Setup(r => r.AddAsync(It.IsAny<Poll>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<PollDto>(It.IsAny<Poll>())).Returns(pollDto);

            // Act
            var result = await _pollService.CreatePollAsync(createPollDto, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Poll", result.Title);
            _pollRepoMock.Verify(r => r.AddAsync(It.IsAny<Poll>()), Times.Once);
        }

        [Fact]
        public async Task EnablePoll_Should_Set_IsEnabled_To_True()
        {
            // Arrange
            var poll = new Poll { Id = 1, Title = "Test Poll", IsEnabled = false };
            _pollRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(poll);
            _pollRepoMock.Setup(r => r.UpdateAsync(poll)).Returns(Task.CompletedTask);

            // Act
            var result = await _pollService.EnablePollAsync(1);

            // Assert
            Assert.True(result);
            Assert.True(poll.IsEnabled);
            _pollRepoMock.Verify(r => r.UpdateAsync(poll), Times.Once);
        }
    }
}
