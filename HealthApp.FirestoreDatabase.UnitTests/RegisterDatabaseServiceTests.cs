using Firebase.Auth;
using Firebase.Database;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace HealthApp.FirestoreDatabase.UnitTests
{
    public sealed class RegisterDatabaseServiceTests(ITestOutputHelper logger, Container container) : BaseTest(logger, container)
    {
        [Fact]
        public void RegisterDatabaseServices_Should_Register_FirestoreService()
        {
            var firestoreService = Container.ServiceProvider.GetService<FirebaseAuthClient>();

            Assert.NotNull(firestoreService);
        }

        [Fact]
        public async Task RegisterDatabaseServices_Should_Allow_To_Login_To_Test_User()
        {
            var testUserEmail = "test@test.com";
            var testUserPassword = "test_test";
            var firestoreService = Container.ServiceProvider.GetService<FirebaseAuthClient>();

            Assert.NotNull(firestoreService);

            var userCredentials = await firestoreService.SignInWithEmailAndPasswordAsync(testUserEmail, testUserPassword);

            Assert.Equal(testUserEmail, userCredentials.User.Info.Email);
        }

        [Fact]
        public void RegisterDatabaseServices_Should_Register_FirestoreRealDetabase()
        {
            var firebaseClient = Container.ServiceProvider.GetService<FirebaseClient>();

            Assert.NotNull(firebaseClient);
        }
    }
}
