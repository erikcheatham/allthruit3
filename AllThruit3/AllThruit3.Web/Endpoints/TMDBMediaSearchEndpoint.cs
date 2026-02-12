using AllThruit3.Shared.Common;
using AllThruit3.Shared.Common.Handlers;
using AllThruit3.Shared.Features.Media;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AllThruit3.Web.Endpoints;

public class TMDBMediaSearchEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/media/search", async (
            [FromQuery] string query,
            [FromQuery] string? type,
            IQueryHandler<SearchMediaQuery, MediaSearchResponse> handler,
            CancellationToken ct) =>
        {
            var searchQuery = new SearchMediaQuery(query, type ?? "movie");
            var result = await handler.Handle(searchQuery, ct);

            if (result.IsFailure)
                return Results.BadRequest(result.Error);

            return Results.Ok(result.Value);
        })
        .WithTags("TMDB")
        .WithSummary("Search for movies or TV shows via TMDB")
        .Produces<MediaSearchResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}