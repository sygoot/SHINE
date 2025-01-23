using Models.Services.Database.Tables;

namespace Models;
public sealed record User(long? Id, string Name, string Password, string Email, Gender Gender, double Height, double Weight, DateTime DateOfBirth) : IEntity;
