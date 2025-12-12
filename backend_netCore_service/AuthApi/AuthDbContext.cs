using AuthApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApi;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<ApplicationUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();

        // Seed users
        modelBuilder.Entity<ApplicationUser>().HasData(
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
        );
    }
}