using AllThruit3.Shared.Common;
using AllThruit3.Shared.Common.Handlers;
using AllThruit3.Shared.DTOs;
using AllThruit3.Shared.Features.Reviews;
using Carter;
using Microsoft.AspNetCore.Mvc;

namespace AllThruit3.Web.Endpoints;

public class ReviewSearchEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/reviews/by-vibe", async (
        [FromQuery] string vibe,
        IQueryHandler<GetReviewsByVibeQuery, Result<List<ReviewDTO>>> handler,
        CancellationToken ct) =>
            {
                var query = new GetReviewsByVibeQuery(vibe);
                var result = await handler.Handle(query, ct);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value);
            })
            .WithTags("Reviews")
            .WithSummary("Get reviews by vibe")
            .Produces<List<ReviewDTO>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}