using Innoloft.Shared.Enums;

namespace Innoloft.Domain.Entities;

public class EventParticipant
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event? Event { get; set; }
    public int UserId { get; set; }
    public ParticipantType Type { get; set; }
    public bool IsApproved { get; set; }
}