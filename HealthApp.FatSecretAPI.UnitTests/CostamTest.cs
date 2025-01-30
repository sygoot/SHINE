using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace HealthApp.FatSecretAPI.UnitTests;
public class CostamTest : BaseTest
{
    public CostamTest(Container container, ITestOutputHelper logger) : base(container, logger)
    {
    }
    [Fact]
    public void Test1()
    {
        Logger.WriteLine("Test1");
        var costam2 = Container.Provider.GetRequiredService<FatSecretAPIClass>();
        costam2.GetFood("chicken");
        Assert.True(true);
    }
}
