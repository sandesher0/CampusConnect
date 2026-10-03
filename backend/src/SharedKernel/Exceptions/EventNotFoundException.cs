namespace SharedKernel.Exceptions;

public class EventNotFoundException : Exception
{
    public EventNotFoundException()
        : base("Event not found.") { }

    public EventNotFoundException(Guid eventId)
        : base($"Event with ID {eventId} was not found.") { }
}