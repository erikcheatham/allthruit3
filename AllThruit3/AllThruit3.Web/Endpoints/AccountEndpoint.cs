using Carter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace AllThruit3.Web.Modules;

public class AccountModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/account/current-user", [AllowAnonymous] (HttpContext context) =>
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
        .AllowAnonymous();  // Ensures public access
    }
}