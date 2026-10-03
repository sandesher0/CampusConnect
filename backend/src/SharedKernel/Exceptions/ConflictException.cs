namespace SharedKernel.Exceptions;

public class ConflictException : Exception
{
    public ConflictException()
        : base("The requested operation conflicts with the current state of the resource.") { }

    public ConflictException(string message) : base(message) { }
}