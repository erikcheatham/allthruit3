using AllThruit3.Shared.DTOs;
using System.Net.Http.Json;

namespace AllThruit3.Shared.Services;

public interface IReviewService
{
    Task<List<ReviewDTO>> GetByVibeAsync(string vibe);
}

public class ReviewService : IReviewService
{
    private readonly HttpClient _httpClient;

    public ReviewService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<ReviewDTO>> GetByVibeAsync(string vibe)
    {
        return await _httpClient.GetFromJsonAsync<List<ReviewDTO>>($"/api/v1/reviews/by-vibe?vibe={vibe}") ?? new List<ReviewDTO>();
    }
}