using AllThruit3.Data.Extensions;
using AllThruit3.Shared.Extensions;
using AllThruit3.Shared.Services;
using AllThruit3.Web.Components;
using AllThruit3.Web.Components.Account;
using AllThruit3.Web.Extensions;
using AllThruit3.Web.Services;
using Carter;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
// Simplified config: Rely on env vars from launchSettings/host
builder.Configuration
    .AddEnvironmentVariables()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        { "AppSettings:TMDBUrl", "https://api.themoviedb.org/3/" },
        { "AppSettings:TMDBBearerToken", "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJjZWJiMWRmMDk3MjY1MDljMTdjZDBjNjIxZDU0MDkwYSIsIm5iZiI6MTc2MDg5NTMwMS4wMzksInN1YiI6IjY4ZjUyMTQ1NDI5NmNmMjRiNmY5OWY2MSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.5j0B4CHgvIFrgeLJLV8DddIrQD5l7ObDbo3u5hxqnlM" },
        { "AppSettings:ApiBaseUrl", "https://localhost:7199/" }
    });

builder.AddServiceDefaults();
// Add data abstractions (DbContext, Identity, repos, Blob, initializer)
builder.Services.AddDataServices(builder.Configuration);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication().AddIdentityCookies();
builder.Services.AddAuthorization();

// HttpClient for TMDB/internal calls (if not moved to Shared)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["AppSettings:ApiBaseUrl"] ?? "https://localhost:7199/") });

// Add device-specific services used by the AllThruit.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

builder.Services.AddSharedServices(builder.Configuration);

builder.Services.AddCarter();

// Register custom endpoints (scans Web assembly for IEndpoint impls like MediaEndpoint)
builder.Services.AddEndpoints(typeof(Program).Assembly); // Or specify typeof(MediaEndpoint).Assembly if separate

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AllThruit API", Version = "v1", Description = "Movie Review Platform API with TMDB Integration" });
    // Optional: Include XML comments for better docs (enable in project properties > Build > Output XML documentation file)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AllThruit API v1"));
    //app.UseMigrationsEndPoint(); // Now recognized due to the added using directive
    //app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseRouting();  // Add this to enable routing middleware before UseEndpoints

app.UseAntiforgery();

app.UseStaticFiles();

app.UseBlazorFrameworkFiles();

app.MapStaticAssets();

app.MapCarter();

// Map custom endpoints (after Carter for integration)
app.MapEndpoints();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddInteractiveWebAssemblyRenderMode()
        .AddAdditionalAssemblies(
            typeof(AllThruit3.Shared._Imports).Assembly);
});

app.Run();