var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AllThruit3>("allthruit3");

builder.AddProject<Projects.AllThruit3_Web>("allthruit3-web");

builder.Build().Run();
