namespace Innoloft.Web.Controllers.Dtos;

public class UpdateEventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; }
    public string? Description { get; set; }
    public string? ImagePath { get; set; }
}