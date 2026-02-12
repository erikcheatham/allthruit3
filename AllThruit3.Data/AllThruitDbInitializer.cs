using AllThruit3.Data.Contexts;
using AllThruit3.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AllThruit3.Data;

public class AllThruitDbInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public AllThruitDbInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        // Get services
        var dbContext = scope.ServiceProvider.GetRequiredService<AllThruitDbContext>(); // Updated
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Apply migrations
        await dbContext.Database.MigrateAsync(cancellationToken);

        // Seed roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Seed test user
        var testUser = new ApplicationUser { UserName = "test@allthruit.com", Email = "test@allthruit.com" };
        var existingUser = await userManager.FindByEmailAsync(testUser.Email);
        if (existingUser == null)
        {
            await userManager.CreateAsync(testUser, "Password123!");
            await userManager.AddToRoleAsync(testUser, "User");
        }
        else
        {
            testUser = existingUser;
        }

        // Seed sample reviews (if none exist; expand with TMDB/Blob if injected)
        if (!await dbContext.Reviews.AnyAsync(cancellationToken))
        {
            dbContext.Reviews.AddRange(
                new Review
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = testUser.Id, // Assumes UserId is Guid; adjust if string
                    Text = "Angry review of a bad movie!",
                    Rating = 2,
                    CreatedOn = DateTime.UtcNow,
                    Vibe = "Angry"
                },
                new Review
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = testUser.Id,
                    Text = "Awestruck by this masterpiece!",
                    Rating = 9,
                    CreatedOn = DateTime.UtcNow,
                    Vibe = "Awestruck"
                },
                new Review
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = testUser.Id,
                    Text = "Informative breakdown of plot and themes.",
                    Rating = 6,
                    CreatedOn = DateTime.UtcNow,
                    Vibe = "Informative"
                }
            );
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}