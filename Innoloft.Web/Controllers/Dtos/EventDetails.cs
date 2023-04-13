using Innoloft.Domain.Entities;

namespace Innoloft.Web.Controllers.Dtos;

public class EventDetails
{
    public Event Event { get; set; }
    public List<ParticipantsDetails> Participants { get; set; }
}