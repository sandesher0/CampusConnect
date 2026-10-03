namespace Modules.Events.Domain;

public class EventReservation
{
    public Guid Id { get; set; }
    public required Guid EventId { get; set; }
    public required Guid CommunityId { get; set; }
    public required Guid UserId { get; set; }
}