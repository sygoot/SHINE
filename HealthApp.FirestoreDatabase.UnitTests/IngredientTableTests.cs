using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Models.Services.Database;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Xunit.Abstractions;

namespace HealthApp.FirestoreDatabase.UnitTests
{
    public class IngredientTableTests(ITestOutputHelper logger, Container container) : BaseTest(logger, container)
    {
        [Fact]
        public void Add_Should_Create_New_Ingredient_In_Storage()
        {
            var ingredientToAdd = Fixture.Create<Ingredient>();
            var databaseService = Container.ServiceProvider.GetRequiredService<IDatabaseService>();

            var addedIngredientId = LoginUserAsync()
                .ToObservable()
                .SelectMany(_ => databaseService.IngredientTable.Add(ingredientToAdd))
                .Do(ingredientId => Logger.WriteLine($"Created id: [{ingredientId}]"))
                .Wait();

            Assert.NotEqual(0, addedIngredientId);
        }
        [Fact]
        public void Delete_Should_Remove_Ingredient_From_Storage_When_Existing_Ingredient_In_Storage_Was_Provided()
        {
            var ingredientToAdd = Fixture.Create<Ingredient>();
            var databaseService = Container.ServiceProvider.GetRequiredService<IDatabaseService>();

            var ingredientFromStorage = LoginUserAsync()
                .ToObservable()
                .SelectMany(_ => databaseService.IngredientTable.Add(ingredientToAdd))
                .Do(ingredientId => Logger.WriteLine($"Created id: [{ingredientId}]"))
                .SelectMany(ingredientId => databaseService.IngredientTable.Delete(ingredientToAdd with { Id = ingredientId })
                    .Do(_ => Logger.WriteLine("Ingredient removed from database"))
                    .SelectMany(_ => databaseService.IngredientTable.GetById(ingredientId)))
                .Wait();

            Assert.Null(ingredientFromStorage);
        }
        [Fact]
        public void Delete_Should_Remove_Ingredient_From_Storage_When_Exists_Ingredient_Id_Was_Provided()
        {
            var ingredientToAdd = Fixture.Create<Ingredient>();
            var databaseService = Container.ServiceProvider.GetRequiredService<IDatabaseService>();

            var ingredientFromStorage = LoginUserAsync()
                .ToObservable()
                .SelectMany(_ => databaseService.IngredientTable.Add(ingredientToAdd))
                .Do(ingredientId => Logger.WriteLine($"Created id: [{ingredientId}]"))
                .SelectMany(ingredientId => databaseService.IngredientTable.Delete(ingredientId)
                    .Do(_ => Logger.WriteLine("Ingredient removed from database"))
                    .SelectMany(_ => databaseService.IngredientTable.GetById(ingredientId)))
                .Wait();

            Assert.Null(ingredientFromStorage);
        }
        [Fact]
        public void GetAll_Should_Return_All_Ingredient_From_Storage()
        {
            var firstIngredientToAdd = Fixture.Create<Ingredient>();
            var secondIngredientToAdd = Fixture.Create<Ingredient>();
            var databaseService = Container.ServiceProvider.GetRequiredService<IDatabaseService>();

            var firstIngredientSavedInStorageId = LoginUserAsync()
                .ToObservable()
                .SelectMany(_ => databaseService.IngredientTable.Add(firstIngredientToAdd))
                .Do(ingredientId => Logger.WriteLine($"First ingredient id: [{ingredientId}]"))
                .Wait();

            var secondIngredientSavedInStorageId = databaseService.IngredientTable.Add(secondIngredientToAdd)
                .Do(ingredientId => Logger.WriteLine($"Second ingredient : [{ingredientId}]"))
                .Wait();

            var allIngredientsFromStorage = databaseService.IngredientTable.GetAll()
                .Wait();

            Assert.NotNull(allIngredientsFromStorage);
            Assert.NotEmpty(allIngredientsFromStorage);
            Assert.Contains(firstIngredientToAdd with { Id = firstIngredientSavedInStorageId }, allIngredientsFromStorage);
            Assert.Contains(secondIngredientToAdd with { Id = secondIngredientSavedInStorageId }, allIngredientsFromStorage);
        }
        [Fact]
        public void DeleteAll_Should_Remove_All_Ingredient_From_Storage()
        {

            var firstIngredientToAdd = Fixture.Create<Ingredient>();
            var secondIngredientToAdd = Fixture.Create<Ingredient>();
            var databaseService = Container.ServiceProvider.GetRequiredService<IDatabaseService>();

            var firstIngredientSavedInStorageId = LoginUserAsync()
                .ToObservable()
                .SelectMany(_ => databaseService.IngredientTable.Add(firstIngredientToAdd))
                .Do(ingredientId => Logger.WriteLine($"First ingredient id: [{ingredientId}]"))
                .Wait();

            var secondIngredientSavedInStorageId = databaseService.IngredientTable.Add(secondIngredientToAdd)
                .Do(ingredientId => Logger.WriteLine($"Second ingredient : [{ingredientId}]"))
                .Wait();

            _ = databaseService.IngredientTable.DeleteAll()
                .Do(_ => Logger.WriteLine("Delete all ingredients"))
                .Wait();

            var allIngredientsFromStorage = databaseService.IngredientTable.GetAll()
                .Wait();

            Assert.NotNull(allIngredientsFromStorage);
            Assert.Empty(allIngredientsFromStorage);
        }
        [Fact]
        public void GetById_Should_Return_Ingredient_From_Storage_With_GivenId()
        {
            var ingredientToAdd = Fixture.Create<Ingredient>();
            var databaseService = Container.ServiceProvider.GetRequiredService<IDatabaseService>();

            var ingredientSavedInStorage_Id = databaseService.IngredientTable.Add(ingredientToAdd)
                .Do(ingredientId => Logger.WriteLine($"Added ingredient id: [{ingredientId}]"))
                .Do(ingredient => Logger.WriteLine($"Get ingredient from storage: [{ingredient}]"))
                .Wait();

            var ingredientSavedInStorage = databaseService.IngredientTable.GetById(ingredientSavedInStorage_Id)
                .Wait();

            _ = databaseService.IngredientTable.Delete(ingredientSavedInStorage_Id)
                .Do(_ => Logger.WriteLine("Delete ingredient from storage"))
                .Wait();

            Assert.NotNull(ingredientSavedInStorage);
            Assert.Equal(ingredientToAdd with { Id = ingredientSavedInStorage_Id }, ingredientSavedInStorage);
        }
        [Fact]
        public void Update_Should_Update_Ingredient_In_Storage_With_Given_New_Ingredient()
        {
            var ingredientToAdd = Fixture.Create<Ingredient>();
            var databaseService = Container.ServiceProvider.GetRequiredService<IDatabaseService>();

            var ingredientSavedInStorage_Id = databaseService.IngredientTable.Add(ingredientToAdd)
                .Do(ingredientId => Logger.WriteLine($"Added ingredient id: [{ingredientId}]"))
                .Wait();

            var ingredientSavedInStorage = databaseService.IngredientTable.GetById(ingredientSavedInStorage_Id)
                .Do(ingredient => Logger.WriteLine($"Get ingredient from storage: [{ingredient}]"))
                .Wait();

            var ingredientWithChangedFields = Fixture.Build<Ingredient>()
                .With(ingredient => ingredient.Id, ingredientSavedInStorage_Id) // only copy id and random other fields
                .Create();

            Logger.WriteLine($"New random ingredient with the same id as previous: [{ingredientWithChangedFields}]");

            _ = databaseService.IngredientTable.Update(ingredientWithChangedFields).Wait();

            var updatedIngredientFromStorage = databaseService.IngredientTable.GetById(ingredientWithChangedFields.Id!.Value)
                .Do(ingredient => Logger.WriteLine($"Updated ingredient from storage: [{ingredient}]"))
                .Wait();

            Assert.NotNull(updatedIngredientFromStorage);
            Assert.Equal(ingredientSavedInStorage_Id, updatedIngredientFromStorage.Id);
            Assert.NotEqual(ingredientSavedInStorage.Name, updatedIngredientFromStorage.Name);
        }
    }
}
