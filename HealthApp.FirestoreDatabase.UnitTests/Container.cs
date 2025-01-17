using Microsoft.Extensions.DependencyInjection;

namespace HealthApp.FirestoreDatabase.UnitTests
{
    public sealed class Container
    {
        public ServiceProvider ServiceProvider { get; private init; }

        public Container()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.RegisterDatabaseServices();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
