using AllThruit3.Data.Contexts;
using AllThruit3.Data.Entities;
using AllThruit3.Data.Repositories;
using AllThruit3.Data.Services;
using AllThruit3.Shared.Repositories;
using AllThruit3.Shared.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AllThruit3.Data.Extensions;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Connection string with validation
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // DbContext with SQL Server and retry
        services.AddDbContext<AllThruitDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
            }));

        // Database developer page filter (for dev DB errors)
        services.AddDatabaseDeveloperPageExceptionFilter();

        // Identity setup
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AllThruitDbContext>()
        .AddSignInManager()
        .AddApiEndpoints()
        .AddDefaultTokenProviders();

        // Email sender (no-op)
        services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        services.AddScoped<IDbConnection>(sp =>
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        });

        // Repo registration unchanged
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();

        services.AddAzureClients(clientBuilder =>
        {
            var blobConn = configuration.GetConnectionString("AzureBlobStorage") ?? "UseDevelopmentStorage=true";
            clientBuilder.AddBlobServiceClient(blobConn).WithName("Default");
        });
        services.AddSingleton<IBlobStorageService, BlobStorageService>();

        // Hosted initializer for seeding/migrations
        services.AddHostedService<AllThruitDbInitializer>();

        return services;
    }
}