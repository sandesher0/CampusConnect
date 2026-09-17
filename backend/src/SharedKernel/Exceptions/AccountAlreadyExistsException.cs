namespace SharedKernel.Exceptions;

public class AccountAlreadyExistsException : Exception
{
    public AccountAlreadyExistsException(string email)
    : base($"An email with provided Email: {email} Already Exists")
    {

    }
}