namespace SharedKernel.Exceptions;

public class PlanLimitExceededException : Exception
{
    public string LimitKey { get; }
    public int Limit { get; }

    public PlanLimitExceededException(string limitKey, int limit)
        : base($"You have reached the maximum allowed limit for '{limitKey}' ({limit}).")
    {
        LimitKey = limitKey;
        Limit = limit;
    }
}
