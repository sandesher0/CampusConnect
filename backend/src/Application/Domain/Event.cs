using SharedKernel.Constants;

namespace Application.Domain;

public class Event
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Location { get; set; }
    public required Guid CommunityId { get; set; }
    public Guid CreatedBy { get; set; }
    public required DateTimeOffset EventDate { get; set; }
    public required DateTimeOffset EventEndDate { get; set; }
    public required EventVisibility Visibility { get; set; }
    public required EventCategory Category { get; set; }
}