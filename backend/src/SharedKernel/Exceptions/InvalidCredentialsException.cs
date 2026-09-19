namespace SharedKernel.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
    : base($"The email or password you entered is incorrect.")
    {
    }
}