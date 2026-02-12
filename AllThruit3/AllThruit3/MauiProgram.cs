using AllThruit3.Services;
using AllThruit3.Shared.Extensions;
using AllThruit3.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AllThruit3;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Config (already sets AppSettings:TMDBUrl, TMDBBearerToken, ApiBaseUrl)
        builder.Configuration
            .AddEnvironmentVariables()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
            { "AppSettings:TMDBUrl", "https://api.themoviedb.org/3/" },
            { "AppSettings:TMDBBearerToken", "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJjZWJiMWRmMDk3MjY1MDljMTdjZDBjNjIxZDU0MDkwYSIsIm5iZiI6MTc2MDg5NTMwMS4wMzksInN1YiI6IjY4ZjUyMTQ1NDI5NmNmMjRiNmY5OWY2MSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.5j0B4CHgvIFrgeLJLV8DddIrQD5l7ObDbo3u5hxqnlM" },
            { "AppSettings:ApiBaseUrl", "https://localhost:7199/" }  // Adjust for prod
            });

        // Device-specific (e.g., for Bootstrap/Razor UI adaptation)
        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        // Add shared services (now includes IReviewService)
        builder.Services.AddSharedServices(builder.Configuration, isClient: true);

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
