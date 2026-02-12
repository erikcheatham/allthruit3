var builder = DistributedApplication.CreateBuilder(args);

//var blobs = builder.AddAzureStorage("storage").AddBlobs("movieblobs");

var web = builder.AddProject<Projects.AllThruit3_Web>("allthruit3-web")
    .WithExternalHttpEndpoints();  // Exposes API/WASM ports externally

builder.AddProject<Projects.AllThruit3>("allthruit3");  // MAUI if needed

builder.Build().Run();