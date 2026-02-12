using AllThruit3.Shared.Common.Decorator;
using AllThruit3.Shared.Common.Handlers;
using AllThruit3.Shared.Configuration;
using AllThruit3.Shared.DTOs;
using AllThruit3.Shared.Features.Media;  // Adjust if moving to Web
using AllThruit3.Shared.Models;
using AllThruit3.Shared.Repositories;
using AllThruit3.Shared.Services;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Scrutor;
using System.Reflection;

namespace AllThruit3.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isClient = false)
    {
        // Bind and validate AppSettings
        var appSettings = new AppSettings();
        configuration.GetSection("AppSettings").Bind(appSettings);
        if (string.IsNullOrWhiteSpace(appSettings.TMDBUrl))
            throw new InvalidOperationException("TMDBUrl is missing or empty in configuration.");
        if (string.IsNullOrWhiteSpace(appSettings.TMDBBearerToken))
            throw new InvalidOperationException("TMDBBearerToken is missing or empty in configuration.");
        services.AddSingleton(appSettings);

        // TMDB HttpClient with Polly retry policy
        services.AddHttpClient<TMDBClient>((sp, client) =>
        {
            var settings = sp.GetRequiredService<AppSettings>();
            client.BaseAddress = new Uri(settings.TMDBUrl);
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.TMDBBearerToken);
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromSeconds(10);
        })
        .AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

        TypeAdapterConfig.GlobalSettings
            .ForType<SearchMediaQueryHandler.TMDBMediaItem, MediaResult>()
            .Map(dest => dest.Title, src => src.Title ?? src.Name ?? string.Empty)
            .Map(dest => dest.ReleaseDate, src => src.ReleaseDate ?? src.FirstAirDate);

        // Common services (validators available everywhere)
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        if (isClient)
        {
            // Client-only (MAUI/Blazor Hybrid / WASM)
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            services.AddHttpClient<IReviewService, ReviewService>(client =>
            {
                var baseUrl = configuration.GetValue<string>("AppSettings:ApiBaseUrl")
                    ?? "https://localhost:7199/";
                client.BaseAddress = new Uri(baseUrl);
            });
        }
        else
        {
            // Server-only registrations
            var assembliesToScan = new[]
            {
                typeof(ServiceCollectionExtensions).Assembly, // Shared project
                Assembly.GetExecutingAssembly() // Entry point / Web project
                // Add more assemblies later if needed, e.g. typeof(SomeHandlerInApplication).Assembly
            };

            // Register all query/command handlers
            services.Scan(scan => scan
                .FromAssemblies(assembliesToScan)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );

            // Validators (already added above – this is optional duplicate if you want server-only)
            services.AddValidatorsFromAssembly(
                typeof(ServiceCollectionExtensions).Assembly,
                includeInternalTypes: true);

            services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
            // Decorators (validation + logging pipeline)
            //services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
            //services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));
            //services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
            //services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

            // Optional generic API client
            services.AddHttpClient("ApiClient", (sp, client) =>
            {
                var settings = sp.GetRequiredService<AppSettings>();
                client.BaseAddress = new Uri(settings.ApiBaseUrl ?? "https://localhost:7199/");
            });
        }

        return services;
    }
}