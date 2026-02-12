using AllThruit3.Data.Contexts;
using AllThruit3.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AllThruit3.Data;

public class AllThruitDbInitializer(IServiceProvider serviceProvider) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

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

        // Seed sample Media if none
        if (!await dbContext.Media.AnyAsync(cancellationToken))
        {
            var sampleMedia1 = new Media { Id = Guid.NewGuid(), Title = "Inception", PosterPath = "/8ZTVqvKDQ8emxNAaBekwX2iJ2R4.jpg", CreatedBy = testUser.Id, CreatedOn = DateTime.UtcNow };  // TMDB sample path
            var sampleMedia2 = new Media { Id = Guid.NewGuid(), Title = "The Matrix", PosterPath = "/f89U3ADr1WkGrDBQQJDwBCejBBE.jpg", CreatedBy = testUser.Id, CreatedOn = DateTime.UtcNow };
            dbContext.Media.AddRange(sampleMedia1, sampleMedia2);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        // Seed reviews with MediaId and vibes
        if (!await dbContext.Reviews.AnyAsync(cancellationToken))
        {
            var media = await dbContext.Media.ToListAsync(cancellationToken);
            dbContext.Reviews.AddRange(
                new Review { Id = Guid.NewGuid(), CreatedBy = testUser.Id, Text = "Angry review!", Rating = 2, CreatedOn = DateTime.UtcNow, Vibe = "Angry", MediaId = media[0].Id },
                new Review { Id = Guid.NewGuid(), CreatedBy = testUser.Id, Text = "Awestruck masterpiece!", Rating = 9, CreatedOn = DateTime.UtcNow, Vibe = "Awestruck", MediaId = media[1].Id },
                new Review { Id = Guid.NewGuid(), CreatedBy = testUser.Id, Text = "Informative breakdown.", Rating = 6, CreatedOn = DateTime.UtcNow, Vibe = "Informative", MediaId = media[0].Id }
            );
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}