namespace SharedKernel.Exceptions;


public class UsernameAlreadyTakenException : Exception
{
    public UsernameAlreadyTakenException(string username) : base($"Username: {username} already taken ") { }
}