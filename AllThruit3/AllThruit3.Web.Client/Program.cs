using AllThruit3.Shared.Extensions;
using AllThruit3.Shared.Services;
using AllThruit3.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Config (add AppSettings for TMDB/API, as in MAUI)
builder.Configuration
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        { "AppSettings:TMDBUrl", "https://api.themoviedb.org/3/" },
        { "AppSettings:TMDBBearerToken", "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJjZWJiMWRmMDk3MjY1MDljMTdjZDBjNjIxZDU0MDkwYSIsIm5iZiI6MTc2MDg5NTMwMS4wMzksInN1YiI6IjY4ZjUyMTQ1NDI5NmNmMjRiNmY5OWY2MSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.5j0B4CHgvIFrgeLJLV8DddIrQD5l7ObDbo3u5hxqnlM" },
        { "AppSettings:ApiBaseUrl", builder.HostEnvironment.BaseAddress }  // Use dynamic host base for hosted WASM
    });

// Log to console (browser dev tools)
Console.WriteLine("TMDBBearerToken from config: " + builder.Configuration["AppSettings:TMDBBearerToken"] ?? "MISSING");

// Add device-specific services used by the AllThruit.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

// Add shared services with client flag (uses HTTP proxies)
builder.Services.AddSharedServices(builder.Configuration, isClient: true);

// Add HttpClient for API calls (base to your server)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
