using AllThruit3.Shared.Common;
using AllThruit3.Shared.Common.Handlers;
using AllThruit3.Shared.DTOs;
using AllThruit3.Shared.Repositories;
using FluentValidation;
using Mapster;

namespace AllThruit3.Shared.Features.Reviews;

/// <summary>
/// Query to retrieve reviews filtered by vibe.
/// </summary>
public sealed record GetReviewsByVibeQuery(string Vibe) : IQuery;

/// <summary>
/// Validates the GetReviewsByVibeQuery.
/// </summary>
public sealed class GetReviewsByVibeQueryValidator : AbstractValidator<GetReviewsByVibeQuery>
{
    public GetReviewsByVibeQueryValidator()
    {
        RuleFor(x => x.Vibe)
            .NotEmpty().WithMessage("Vibe cannot be empty")
            .MaximumLength(100).WithMessage("Vibe must not exceed 100 characters");

        // Add more rules as needed, e.g.:
        // .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("Vibe contains invalid characters");
    }
}

/// <summary>
/// Handler for retrieving reviews by vibe.
/// </summary>
public sealed class GetReviewsByVibeQueryHandler : IQueryHandler<GetReviewsByVibeQuery, List<ReviewDTO>>
{
    private readonly IReviewRepository _repository;

    public GetReviewsByVibeQueryHandler(IReviewRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Result<List<ReviewDTO>>> Handle(
        GetReviewsByVibeQuery query,
        CancellationToken cancellationToken = default)
    {
        // Fetch reviews from repository
        var reviews = await _repository.GetByVibeAsync(query.Vibe, cancellationToken);

        // Handle no results gracefully
        if (reviews == null || !reviews.Any())
        {
            return Result.Success(new List<ReviewDTO>());
            // Alternative: return Result.NoContent(new List<ReviewDTO>());
        }

        // Map domain entities to DTOs using Mapster
        var reviewDtos = reviews.Adapt<List<ReviewDTO>>();

        return Result.Success(reviewDtos);
    }
}