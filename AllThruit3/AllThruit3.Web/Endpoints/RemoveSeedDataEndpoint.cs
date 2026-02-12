using AllThruit3.Data.Contexts;
using AllThruit3.Shared.Common;
using AllThruit3.Shared.Common.Handlers;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;  // For [FromServices]
using Microsoft.EntityFrameworkCore;
using static AllThruit3.Web.Endpoints.RemoveSeedData;

namespace AllThruit3.Web.Endpoints;

/// <summary>
/// Carter module for removing seeded review data.
/// </summary>
public class RemoveSeedDataEndpoint : ICarterModule
{
    /// <inheritdoc/>
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/seed", async (
            [FromServices] ICommandHandler<RemoveSeedDataCommand, bool> handler,  // Explicit [FromServices] fixes inference
            CancellationToken ct) =>
        {
            var command = new RemoveSeedDataCommand();
            var result = await handler.Handle(command, ct);
            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }
            return Results.Ok(result.Value); // true if successful
        })
        .WithTags("Admin", "Seed")
        .WithSummary("Remove seeded review data")
        .Produces<bool>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}

/// <summary>
/// Command to remove all seeded review data.
/// </summary>
public static class RemoveSeedData
{
    /// <inheritdoc/>
    public sealed record RemoveSeedDataCommand : ICommand<bool>;

    /// <inheritdoc/>
    public sealed class RemoveSeedDataCommandValidator : AbstractValidator<RemoveSeedDataCommand>
    {
        /// <inheritdoc/>
        public RemoveSeedDataCommandValidator()
        {
            // Add rules if needed (e.g. admin-only check via claims later)
            // For now, no validation required for this admin operation
        }
    }

    internal sealed class RemoveSeedDataCommandHandler : ICommandHandler<RemoveSeedDataCommand, bool>
    {
        private readonly AllThruitDbContext _db;

        public RemoveSeedDataCommandHandler(AllThruitDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Result<bool>> Handle(
            RemoveSeedDataCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                // Option 1: Remove ALL reviews (simple / destructive)
                var allReviews = await _db.Reviews.ToListAsync(cancellationToken);
                _db.Reviews.RemoveRange(allReviews);

                // Option 2: Only remove seeded data (recommended for production)
                // var seededReviews = await _db.Reviews
                // .Where(r => r.IsSeeded == true) // if you have an IsSeeded flag
                // .ToListAsync(cancellationToken);
                // _db.Reviews.RemoveRange(seededReviews);

                var rowsAffected = await _db.SaveChangesAsync(cancellationToken);
                return Result.Success(true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>(
                    new Error(
                        Method: nameof(RemoveSeedDataCommandHandler),
                        Message: $"Failed to remove seed data: {ex.Message}",
                        StackTrace: ex.StackTrace ?? string.Empty
                    )
                );
            }
        }
    }
}