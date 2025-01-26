using Microsoft.Extensions.DependencyInjection;

namespace HealthApp.FatSecretAPI.UnitTests;
public class Container
{
    public ServiceProvider Provider { get; }
    public Container()
    {
        ServiceCollection services = new();

        services.AddFatSecretAPI();

        Provider = services.BuildServiceProvider();
    }
}
