namespace Application.Tests.TestData;

public class TestTimeProvider : TimeProvider
{
    private DateTimeOffset utcNow;

    public TestTimeProvider(DateTimeOffset initialUtcNow)
    {
        utcNow = initialUtcNow;
    }

    public override DateTimeOffset GetUtcNow() => utcNow;

    public void SetUtcNow(DateTimeOffset newUtcNow) => utcNow = newUtcNow;
}
