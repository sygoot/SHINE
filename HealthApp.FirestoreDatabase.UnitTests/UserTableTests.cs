using AutoFixture;
using Firebase.Auth;
using Microsoft.Extensions.DependencyInjection;
using Models.Services.Database;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Xunit.Abstractions;

namespace HealthApp.FirestoreDatabase.UnitTests
{
    public class UserTableTests(ITestOutputHelper logger, Container container) : BaseTest(logger, container)
    {
        [Fact]
        public void Add_Should_Create_New_User_In_Database_When_Correct_New_User_Data_Was_Provided()
        {
            var user = Fixture.Create<Models.User>();

            var databaseService = Container.ServiceProvider.GetRequiredService<IDatabaseService>();

            var savedUser = LoginUserAsync()
                .ToObservable()
                .Do(_ => Logger.WriteLine("User login success"))
                .SelectMany(_ => databaseService.UserTable.Add(user))
                .Do(_ => Logger.WriteLine("User added to database"))
                .SelectMany(databaseService.UserTable.Get())
                .Do(_ => Logger.WriteLine("User retrieved from database"))
                .Wait();

            Assert.NotNull(savedUser);
            Assert.Equal(user, savedUser);
        }
        [Fact]
        public void Delete_Should_Remove_User_From_Database()
        {
            var user = Fixture.Create<Models.User>();

            var databaseService = Container.ServiceProvider.GetRequiredService<IDatabaseService>();

            var savedUser = LoginUserAsync()
                .ToObservable()
                .Do(_ => Logger.WriteLine("User login success"))
                .SelectMany(_ => databaseService.UserTable.Delete())
                .Do(_ => Logger.WriteLine("User was deleted from database"))
                .SelectMany(databaseService.UserTable.Get())
                .Do(_ => Logger.WriteLine("User retrieved from database"))
                .Wait();

            Assert.Null(savedUser);
        }
        private async Task LoginUserAsync()
        {
            var testUserEmail = "test@test.com";
            var testUserPassword = "test_test";
            var firestoreService = Container.ServiceProvider.GetService<FirebaseAuthClient>();

            Assert.NotNull(firestoreService);

            var userCredentials = await firestoreService.SignInWithEmailAndPasswordAsync(testUserEmail, testUserPassword);

            Assert.Equal(testUserEmail, userCredentials.User.Info.Email);
        }
    }
}
