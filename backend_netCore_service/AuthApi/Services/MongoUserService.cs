using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace AuthApi.Services;

public class MongoUserService : IUserService
{
    private readonly IMongoCollection<ApplicationUser> _users;
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    public MongoUserService(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _users = database.GetCollection<ApplicationUser>("users");

        // Seed data if collection is empty
        if (_users.CountDocuments(FilterDefinition<ApplicationUser>.Empty) == 0)
        {
            SeedUsers();
        }
    }

    private void SeedUsers()
    {
        var hasher = new PasswordHasher<ApplicationUser>();

        var users = new[]
        {
            new ApplicationUser
            {
                Id = 1,
                Username = "admin@example.com",
                Role = "Admin",
                PasswordHash = hasher.HashPassword(new ApplicationUser { Username = "admin@example.com", Role = "Admin", Id = 1, PasswordHash = "" }, "Admin123!")
            },
            new ApplicationUser
            {
                Id = 2,
                Username = "user@example.com",
                Role = "User",
                PasswordHash = hasher.HashPassword(new ApplicationUser { Username = "user@example.com", Role = "User", Id = 2, PasswordHash = "" }, "User123!")
            }
        };

        _users.InsertMany(users);
    }

    public ApplicationUser? ValidateCredentials(string username, string password)
    {
        var user = _users.Find(u => u.Username.ToLower() == username.ToLower()).FirstOrDefault();
        if (user is null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success ? user : null;
    }

    public ApplicationUser? GetUser(string username) =>
        _users.Find(u => u.Username.ToLower() == username.ToLower()).FirstOrDefault();
}