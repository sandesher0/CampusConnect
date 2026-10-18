namespace SharedKernel.Exceptions;

public class ReservationNotFoundException : Exception
{
    public ReservationNotFoundException()
        : base("Reservation not found.") { }

    public ReservationNotFoundException(Guid eventId, Guid userId)
        : base($"No active reservation found for event {eventId} and user {userId}.") { }
}
