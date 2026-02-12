using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AllThruit3.Shared.Services;
using AllThruit3.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the AllThruit3.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
