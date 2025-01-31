namespace Models;
public sealed record User(string FirstName, string LastName, string Username, string Password, string Email, Gender Gender, double Height, double Weight, DateTime DateOfBirth);
