using AllThruit3.Shared.Common;
using AllThruit3.Shared.Common.Handlers;
using AllThruit3.Shared.Models;
using FluentValidation;
using Mapster;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllThruit3.Shared.Features.Media;

public sealed record SearchMediaQuery(string SearchTerm, string MediaType = "movie") : IQuery;

public class MediaSearchResponse
{
    public List<MediaResult> Results { get; set; } = new();
    public string? Message { get; set; }
}

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

public sealed class SearchMediaQueryHandler : IQueryHandler<SearchMediaQuery, MediaSearchResponse>
{
    private readonly TMDBClient _tmdbClient;

    public SearchMediaQueryHandler(TMDBClient tmdbClient)
    {
        _tmdbClient = tmdbClient;
    }

    public async Task<Result<MediaSearchResponse>> Handle(SearchMediaQuery query, CancellationToken cancellationToken = default)
    {
        var client = _tmdbClient.Client;

        if (client.BaseAddress == null || string.IsNullOrEmpty(client.BaseAddress.ToString()))
        {
            return Result.Failure<MediaSearchResponse>(
                new Error("SearchMedia.ConfigError", "TMDB base URL is missing in configuration.", string.Empty));
        }

        var endpoint = $"search/{query.MediaType}?query={Uri.EscapeDataString(query.SearchTerm)}";

        var response = await client.GetAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<MediaSearchResponse>(
                new Error("SearchMedia.TMDBError", $"TMDB API error: {response.StatusCode}", string.Empty));
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var tmdbResponse = JsonSerializer.Deserialize<TMDBSearchResponse>(json, options);

        if (tmdbResponse?.Results == null || !tmdbResponse.Results.Any())
        {
            return Result.Success(new MediaSearchResponse { Message = "No results found." });
        }

        var results = tmdbResponse.Results.Adapt<List<MediaResult>>();

        return Result.Success(new MediaSearchResponse { Results = results });
    }

    // Same private DTOs as before
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
}