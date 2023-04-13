using Innoloft.Domain.Users.Models;
using Innoloft.Shared.Enums;

namespace Innoloft.Web.Controllers.Dtos;

public class ParticipantsDetails
{
    public int Id { get; set; }
    public ParticipantType Type { get; set; }
    public bool IsApproved { get; set; } 
    public User User { get; set; }
}