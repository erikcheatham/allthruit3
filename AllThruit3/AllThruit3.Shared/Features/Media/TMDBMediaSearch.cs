using AllThruit3.Shared.Common;
using AllThruit3.Shared.Common.Handlers;
using AllThruit3.Shared.Models;
using AllThruit3.Shared.Services;
using FluentValidation;
using System.Text.Json.Serialization;

namespace AllThruit3.Shared.Features.Media;

public class TMDBSearchResponse
{
    [JsonPropertyName("page")] public int Page { get; set; }
    [JsonPropertyName("results")] public List<TMDBMediaItem>? Results { get; set; }
    [JsonPropertyName("total_pages")] public int TotalPages { get; set; }
    [JsonPropertyName("total_results")] public int TotalResults { get; set; }
}

public class TMDBMediaItem
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("overview")] public string? Overview { get; set; }
    [JsonPropertyName("release_date")] public string? ReleaseDate { get; set; }
    [JsonPropertyName("first_air_date")] public string? FirstAirDate { get; set; }
    [JsonPropertyName("popularity")] public double Popularity { get; set; }
    [JsonPropertyName("poster_path")] public string? PosterPath { get; set; }
    [JsonPropertyName("backdrop_path")] public string? BackdropPath { get; set; }
    [JsonPropertyName("genre_ids")] public List<int>? GenreIds { get; set; }
    [JsonPropertyName("adult")] public bool Adult { get; set; }
    [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
    [JsonPropertyName("vote_count")] public int VoteCount { get; set; }
}

// Domain response (simplified for app use; add pagination if needed)
public class MediaSearchResponse
{
    public List<MediaResult> Results { get; set; } = new();
    public string? Message { get; set; }
    // Optional: public int Page { get; set; } etc., if adding pagination
}

// Query record
public sealed record SearchMediaQuery(string SearchTerm, string MediaType = "movie") : IQuery;

// Validator
public sealed class SearchMediaQueryValidator : AbstractValidator<SearchMediaQuery>
{
    public SearchMediaQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage("Search query cannot be empty.");
        RuleFor(x => x.MediaType)
            .Must(x => x == "movie" || x == "tv")
            .WithMessage("Media type must be 'movie' or 'tv'.");
    }
}

// Handler (orchestrates service call)
public sealed class SearchMediaQueryHandler : IQueryHandler<SearchMediaQuery, MediaSearchResponse>
{
    private readonly ITMDBClient _tmdbClient;

    public SearchMediaQueryHandler(ITMDBClient tmdbClient)
    {
        _tmdbClient = tmdbClient;
    }

    public async Task<Result<MediaSearchResponse>> Handle(SearchMediaQuery query, CancellationToken cancellationToken = default)
    {
        var response = await _tmdbClient.SearchAsync(query.SearchTerm, query.MediaType, cancellationToken);
        if (!string.IsNullOrEmpty(response.Message))
        {
            return Result.Failure<MediaSearchResponse>(
                new Error("SearchMedia.Error", response.Message, string.Empty));
        }
        return Result.Success(response);
    }
}