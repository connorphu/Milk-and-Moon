namespace MilkAndMoon.Api.Tests.Infrastructure;

[CollectionDefinition(Name)]
public class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "Api";
}
