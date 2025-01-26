using Xunit.Abstractions;

namespace HealthApp.FatSecretAPI.UnitTests;
public abstract class BaseTest : IClassFixture<Container>
{
    protected Container Container { get; init; }
    protected ITestOutputHelper Logger { get; init; }
    public BaseTest(Container container, ITestOutputHelper logger)
    {
        this.Container = container;
        this.Logger = logger;
    }

}
