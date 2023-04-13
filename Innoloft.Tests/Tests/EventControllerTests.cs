using AutoMapper;
using Innoloft.Domain.Entities;
using Innoloft.Domain.Repositories;
using Innoloft.Domain.Users;
using Innoloft.Web.Controllers;
using Innoloft.Web.Controllers.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Innoloft.Tests.Tests;

public class EventControllerTests
{
    public EventControllerTests()
    {
        Environment.SetEnvironmentVariable("TEST_ENV", "true");
    }
    
    [Fact]
    public async Task Add_NewEvent_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var mockEventRepository = new Mock<IEventRepository>();
        var mockEventParticipantRepository = new Mock<IEventParticipantRepository>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockMapper = new Mock<IMapper>();

        var eventEntity = new CreateEventDto
        {
            Title = "Test Event",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddHours(2),
            Location = "Test Location",
            Description = "Test Description"
        };

        var mappedEvent = new Event
        {
            Title = eventEntity.Title,
            StartDate = eventEntity.StartDate,
            EndDate = eventEntity.EndDate,
            Location = eventEntity.Location,
            Description = eventEntity.Description
        };

        mockMapper.Setup(m => m.Map<Event>(eventEntity)).Returns(mappedEvent);
        mockEventRepository.Setup(r => r.AddAsync(mappedEvent)).ReturnsAsync(mappedEvent);

        var controller = new EventController(mockEventRepository.Object,
            mockEventParticipantRepository.Object,
            mockUserRepository.Object,
            mockMapper.Object);

        // Act
        var result = await controller.Add(eventEntity);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedEvent = Assert.IsType<Event>(createdAtActionResult.Value);
        Assert.Equal(mappedEvent.Title, returnedEvent.Title);
        Assert.Equal(mappedEvent.StartDate, returnedEvent.StartDate);
        Assert.Equal(mappedEvent.EndDate, returnedEvent.EndDate);
        Assert.Equal(mappedEvent.Location, returnedEvent.Location);
        Assert.Equal(mappedEvent.Description, returnedEvent.Description);
    }
    
    public void Dispose()
    {
        Environment.SetEnvironmentVariable("TEST_ENV", "false");
    }

}