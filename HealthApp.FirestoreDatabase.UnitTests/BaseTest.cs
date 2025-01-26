using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Xunit.Abstractions;

namespace HealthApp.FirestoreDatabase.UnitTests
{
    public abstract class BaseTest : IClassFixture<Container>
    {
        protected ITestOutputHelper Logger { get; init; }
        protected Container Container { get; init; }
        public IFixture Fixture { get; init; }

        protected BaseTest(ITestOutputHelper logger, Container container)
        {
            Logger = logger;
            Container = container;
            Fixture = new Fixture().Customize(new AutoNSubstituteCustomization() { ConfigureMembers = true });
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(throwingRecursionBehavior => Fixture.Behaviors.Remove(throwingRecursionBehavior));
            Fixture.Behaviors.Add(new NullRecursionBehavior());
        }
    }
}
