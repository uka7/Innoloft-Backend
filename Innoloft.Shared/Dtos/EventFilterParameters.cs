namespace Innoloft.Shared.Dtos;

public class EventFilterParameters : PaginationParameters
{
    public int? IdFilter { get; set; }
    public string? TitleFilter { get; set; }
    public DateTime? StartDateFilter { get; set; }
    public DateTime? EndDateFilter { get; set; }
    public string? LocationFilter { get; set; }
    public string? DescriptionFilter { get; set; }
    public int? CreatorUserIdFilter { get; set; }
    public bool OnlyEventsCreatedByCurrentUserFilter { get; set; }
    public bool OnlyEventsCurrentUserIsParticipantInFilter { get; set; }
}