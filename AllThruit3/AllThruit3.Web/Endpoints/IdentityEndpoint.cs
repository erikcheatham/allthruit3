using AllThruit3.Shared.Features.Identity;  // Commands from Shared
using Carter;
using Microsoft.AspNetCore.Authorization;  // For [AllowAnonymous]
using Microsoft.AspNetCore.Http;  // For HttpContext
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;  // For [FromBody]
using AllThruit3.Data.Entities;  // ApplicationUser from Data

namespace AllThruit3.Web.Endpoints;

public class IdentityEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/account/register", async (
            [FromBody] RegisterCommand command,  // Explicit [FromBody] fixes inference
            UserManager<ApplicationUser> userManager,
            CancellationToken ct) =>
        {
            var user = new ApplicationUser { UserName = command.Email, Email = command.Email };
            var result = await userManager.CreateAsync(user, command.Password);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors.Select(e => e.Description));
            return Results.Ok("Registration successful");
        })
        .WithTags("Identity")
        .WithSummary("Register a new user")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .AllowAnonymous();

        app.MapPost("/api/v1/account/login", async (
            [FromBody] LoginCommand command,  // Explicit [FromBody]
            SignInManager<ApplicationUser> signInManager,
            CancellationToken ct) =>
        {
            var result = await signInManager.PasswordSignInAsync(command.Email, command.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
                return Results.BadRequest("Invalid login attempt");
            return Results.Ok("Login successful");
        })
        .WithTags("Identity")
        .WithSummary("Login a user")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .AllowAnonymous();

        app.MapPost("/api/v1/account/logout", async (SignInManager<ApplicationUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Ok("Logout successful");
        })
        .WithTags("Identity")
        .WithSummary("Logout the current user")
        .Produces(StatusCodes.Status200OK)
        .RequireAuthorization();

        app.MapGet("/api/v1/account/current-user", [AllowAnonymous] (HttpContext context) =>
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                var claims = context.User.Claims.Select(c => new { Type = c.Type, Value = c.Value });
                return Results.Ok(new
                {
                    IsAuthenticated = true,
                    UserName = context.User.Identity.Name,
                    Claims = claims
                });
            }
            return Results.Ok(new { IsAuthenticated = false });
        })
        .WithTags("Identity")
        .WithSummary("Get current user info")
        .Produces(StatusCodes.Status200OK)
        .AllowAnonymous();
    }
}