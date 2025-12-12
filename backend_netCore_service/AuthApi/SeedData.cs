using AuthApi.Models;
using AuthApi.Services;
using MongoDB.Bson;

namespace AuthApi;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userService = serviceProvider.GetRequiredService<IUserService>();
        var todoService = serviceProvider.GetRequiredService<ITodoService>();
        
        Console.WriteLine("Seeding database...");

        // Ensure users exist. 
        // For MongoUserService, this is done in constructor.
        var admin = userService.GetUser("admin@todo.dev");
        var user = userService.GetUser("user@todo.dev");

        if (admin == null || user == null)
        {
            Console.WriteLine("Users not found. Ensure users are seeded first.");
            return;
        }

        await SeedTodosForUserAsync(todoService, admin, "Admin Task");
        await SeedTodosForUserAsync(todoService, user, "User Task");
        
        Console.WriteLine("Seeding completed successfully.");
    }

    private static async Task SeedTodosForUserAsync(ITodoService todoService, ApplicationUser user, string taskPrefix)
    {
        if (string.IsNullOrEmpty(user.Id))
        {
            Console.WriteLine($"User {user.Username} has no ID. Skipping.");
            return;
        }

        // Check if user has todos
        var existing = await todoService.GetTodosAsync(user.Id, 1, 1, "created_at", true, null, null, null, null);
        
        if (existing.Total > 0)
        {
            Console.WriteLine($"User {user.Username} already has todos. Skipping.");
            return;
        }

        Console.WriteLine($"Seeding todos for {user.Username}...");

        if (!ObjectId.TryParse(user.Id, out ObjectId userObjectId))
        {
             Console.WriteLine($"Error: User ID {user.Id} is not a valid ObjectId. Cannot seed todos.");
             return;
        }

        var todos = new[]
        {
            new Todo 
            { 
                Title = $"{taskPrefix} 1: Review Reports", 
                Description = "Review the weekly status reports from the team.", 
                Status = Status.PENDING,
                DueDate = DateTime.UtcNow.AddDays(1),
                Duration = "30m",
                UserLink = new TodoUserLink { Id = userObjectId }
            },
            new Todo 
            { 
                Title = $"{taskPrefix} 2: Update Documentation", 
                Description = "Update the API documentation with the latest changes.", 
                Status = Status.IN_PROGRESS,
                DueDate = DateTime.UtcNow.AddDays(2),
                Duration = "2h",
                UserLink = new TodoUserLink { Id = userObjectId }
            },
             new Todo 
            { 
                Title = $"{taskPrefix} 3: Team Meeting", 
                Description = "Weekly sync with the development team.", 
                Status = Status.COMPLETED,
                DueDate = DateTime.UtcNow.AddDays(-1),
                Duration = "1h",
                UserLink = new TodoUserLink { Id = userObjectId }
            },
            new Todo 
            { 
                Title = $"{taskPrefix} 4: Plan Next Sprint", 
                Description = "Prepare the backlog for the upcoming sprint.", 
                Status = Status.BACKLOG,
                DueDate = DateTime.UtcNow.AddDays(5),
                Duration = "4h",
                UserLink = new TodoUserLink { Id = userObjectId }
            }
        };

        foreach (var todo in todos)
        {
            await todoService.CreateTodoAsync(todo);
        }
        Console.WriteLine($"Seeded {todos.Length} todos for {user.Username}");
    }
}
