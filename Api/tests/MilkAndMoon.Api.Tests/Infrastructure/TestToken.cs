namespace MilkAndMoon.Api.Tests.Infrastructure;

public static class TestToken
{
    public static CancellationToken TestCancellationToken => TestContext.Current.CancellationToken;
}
