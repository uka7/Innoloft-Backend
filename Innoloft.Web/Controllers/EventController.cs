using System.Security.Claims;
using AutoMapper;
using Innoloft.Domain.Entities;
using Innoloft.Domain.Repositories;
using Innoloft.Domain.Users;
using Innoloft.Shared.Dtos;
using Innoloft.Shared.Enums;
using Innoloft.Web.Controllers.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once IdentifierTypo
namespace Innoloft.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventParticipantRepository _eventParticipantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public EventController(IEventRepository eventRepository,
        IEventParticipantRepository eventParticipantRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _eventRepository = eventRepository;
        _eventParticipantRepository = eventParticipantRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    // Get all events with pagination
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] EventFilterParameters filterParameters)
    {
        var events = await _eventRepository.GetAllAsync(filterParameters, GetUserIdFromToken());
        return Ok(events);
    }

    [HttpGet("{id}/details")]
    public async Task<ActionResult<EventDetails>> GetEventDetails(int id)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(id);
        if (eventEntity == null)
        {
            return NotFound();
        }

        var eventParticipants = await _eventParticipantRepository.GetAllByEventIdAsync(id);

        var details = new List<ParticipantsDetails>();
        foreach (var participant in eventParticipants)
        {
            var user = await _userRepository.GetUserAsync(participant.UserId);
            details.Add(new ParticipantsDetails
            {
                Id = participant.Id,
                Type = participant.Type,
                IsApproved = participant.IsApproved,
                User = user
            });
        }

        var eventDetails = new EventDetails
        {
            Event = eventEntity,
            Participants = details
        };

        return Ok(eventDetails);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(id);
        if (eventEntity == null)
        {
            return NotFound();
        }
        return Ok(eventEntity);
    }
    
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateEventDto eventEntity)
    {
        var newEvent = _mapper.Map<Event>(eventEntity);
        newEvent.CreatorUserId = GetUserIdFromToken();
        newEvent = await _eventRepository.AddAsync(newEvent);
        return CreatedAtAction(nameof(GetById), new {id = newEvent.Id}, newEvent);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateEventDto eventEntity)
    {
        var existingEvent = await _eventRepository.GetByIdAsync(eventEntity.Id);
        if (existingEvent == null)
        {
            return NotFound();
        }
        var creatorUserId = existingEvent.CreatorUserId;
        existingEvent = _mapper.Map<Event>(eventEntity);
        existingEvent.CreatorUserId = creatorUserId;
        await _eventRepository.UpdateAsync(existingEvent);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (await _eventParticipantRepository.DoesEventHaveParticipants(id))
        {
            return BadRequest("Event can't be deleted because it has participants in it");
        }
        await _eventRepository.DeleteAsync(id);
        return NoContent();
    }

    private int GetUserIdFromToken()
    {
        if (Environment.GetEnvironmentVariable("TEST_ENV") == "true") return 1; // to use during testing
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }
}