namespace SharedKernel.Exceptions;

public class AccountNotFoundException : Exception
{
    public AccountNotFoundException(Guid accountId) : base($"Account with the Id {accountId} not Found ") { }
}