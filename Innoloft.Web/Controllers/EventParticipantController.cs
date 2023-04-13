using System.Security.Claims;
using AutoMapper;
using Innoloft.Domain.Entities;
using Innoloft.Domain.Repositories;
using Innoloft.Domain.Users;
using Innoloft.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Innoloft.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EventParticipantController : ControllerBase
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventParticipantRepository _eventParticipantRepository;
    private readonly IUserRepository _userRepository;

    public EventParticipantController(IEventRepository eventRepository,
        IEventParticipantRepository eventParticipantRepository,
        IUserRepository userRepository)
    {
        _eventRepository = eventRepository;
        _eventParticipantRepository = eventParticipantRepository;
        _userRepository = userRepository;
    }

    [HttpGet("getSentInvitations")]
    public async Task<IActionResult> GetSentInvitations()
    {
        return Ok(await _eventParticipantRepository.GetSentInvitations(GetUserIdFromToken()));
    }
    
    [HttpGet("getReceivedInvitations")]
    public async Task<IActionResult> GetReceivedInvitations()
    {
        return Ok(await _eventParticipantRepository.GetReceivedInvitations(GetUserIdFromToken()));
    }
    
    [HttpPost("{eventId}/invite/{userId}")]
    public async Task<IActionResult> InviteUser(int eventId, int userId)
    {
        // Check if the user is the event creator
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null || eventEntity.CreatorUserId != GetUserIdFromToken())
        {
            return Unauthorized();
        }
        if (eventEntity.EndDate < DateTime.Now) return BadRequest("Event has ended");
        var user = await _userRepository.GetUserAsync(userId);
        if (user == null)
        {
            return BadRequest("User not found");
        }
        var eventParticipant = await _eventParticipantRepository.GetParticipantByEventIdAsync(eventId, userId);
        if (eventParticipant != null)
        {
            return BadRequest("User is already a participant");
        }
        eventParticipant = new EventParticipant
        {
            EventId = eventId,
            UserId = userId,
            Type = ParticipantType.Invited,
            IsApproved = false
        };

        await _eventParticipantRepository.AddAsync(eventParticipant);
        return Ok(eventParticipant);
    }

    [HttpPost("register/{eventId}")]
    public async Task<IActionResult> RegisterToEvent(int eventId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null)
        {
            return NotFound();
        }

        if (eventEntity.EndDate < DateTime.Now)
        {
            return BadRequest("Event has ended");
        }
        var eventParticipant = await _eventParticipantRepository
            .GetParticipantByEventIdAsync(eventId, GetUserIdFromToken());
        if (eventParticipant != null)
        {
            return BadRequest("User is already registered for this event");
        }
        eventParticipant = new EventParticipant
        {
            EventId = eventId,
            UserId = GetUserIdFromToken(),
            Type = ParticipantType.Registered,
            IsApproved = true
        };

        await _eventParticipantRepository.AddAsync(eventParticipant);
        return Ok(eventParticipant);
    }

    [HttpPut("approve/{eventId}")]
    public async Task<IActionResult> ApproveOrDeclineInvitation(int eventId, bool isApproved)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null)
        {
            return NotFound();
        }

        if (eventEntity.EndDate < DateTime.Now)
        {
            return BadRequest("Event has ended");
        }
        var eventParticipant =
            await _eventParticipantRepository.GetParticipantByEventIdAsync(eventId, GetUserIdFromToken());
        if (eventParticipant == null)
        {
            return BadRequest("Event participant not found");
        }

        if (isApproved && eventParticipant.IsApproved)
        {
            return BadRequest("Already approved");
        }

        if (!isApproved && !eventParticipant.IsApproved)
        {
            return BadRequest("Already declined");
        }
        eventParticipant.IsApproved = isApproved;
        await _eventParticipantRepository.UpdateAsync(eventParticipant);
        return Ok(eventParticipant);
    }
    
    [HttpDelete("unregister/{eventId}")]
    public async Task<IActionResult> CancelRegistrationToEvent(int eventId)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null)
        {
            return NotFound();
        }
        if (eventEntity.EndDate < DateTime.Now) return BadRequest("Event has ended");

        var eventParticipant = await _eventParticipantRepository
            .GetParticipantByEventIdAsync(eventId, GetUserIdFromToken());
        if (eventParticipant == null)
        {
            return BadRequest("User is not registered for this event");
        }
        await _eventParticipantRepository.DeleteAsync(eventParticipant.Id);
        return Ok(eventParticipant);
    }
    
    private int GetUserIdFromToken()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

}