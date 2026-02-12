using AllThruit3.Shared.Features.Media;
using AllThruit3.Shared.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mapster;

namespace AllThruit3.Shared.Services;

public interface ITMDBClient
{
    Task<MediaSearchResponse> SearchAsync(string searchTerm, string mediaType = "movie", CancellationToken ct = default);
}

public class TMDBClient : ITMDBClient
{
    private readonly HttpClient _client;

    public TMDBClient(HttpClient client) => _client = client;

    public async Task<MediaSearchResponse> SearchAsync(string searchTerm, string mediaType = "movie", CancellationToken ct = default)
    {
        if (_client.BaseAddress == null || string.IsNullOrEmpty(_client.BaseAddress.ToString()))
        {
            return new MediaSearchResponse { Message = "TMDB base URL is missing in configuration." };
        }

        var endpoint = $"search/{mediaType}?query={Uri.EscapeDataString(searchTerm)}";
        var response = await _client.GetAsync(endpoint, ct);

        if (!response.IsSuccessStatusCode)
        {
            return new MediaSearchResponse { Message = $"TMDB API error: {response.StatusCode}" };
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var tmdbResponse = JsonSerializer.Deserialize<MediaSearchResponse>(json, options);

        if (tmdbResponse?.Results == null || !tmdbResponse.Results.Any())
        {
            return new MediaSearchResponse { Message = "No results found." };
        }

        var results = tmdbResponse.Results.Adapt<List<MediaResult>>();
        return new MediaSearchResponse { Results = results };
    }
}