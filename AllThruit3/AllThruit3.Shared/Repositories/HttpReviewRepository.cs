using AllThruit3.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AllThruit3.Shared.Repositories;

public class HttpReviewRepository : IReviewRepository
{
    private readonly HttpClient _httpClient;

    public HttpReviewRepository(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<List<ReviewDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<ReviewDTO>>("api/reviews", cancellationToken) ?? new List<ReviewDTO>();
    }

    public async Task<ReviewDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<ReviewDTO>($"api/reviews/{id}", cancellationToken);
    }

    public async Task<List<ReviewDTO>> GetByVibeAsync(string vibe, CancellationToken cancellationToken = default)
    {
        // Use query string parameter "vibe"
        var uri = $"api/reviews/by-vibe?vibe={Uri.EscapeDataString(vibe)}";

        var reviews = await _httpClient.GetFromJsonAsync<List<ReviewDTO>>(uri, cancellationToken);

        return reviews ?? new List<ReviewDTO>();
    }

    public async Task<ReviewDTO> CreateAsync(ReviewDTO dto, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/reviews", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReviewDTO>(cancellationToken) ?? throw new Exception("Failed to deserialize created review");
    }

    public async Task<ReviewDTO> UpdateAsync(ReviewDTO dto, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/reviews/{dto.Id}", dto, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReviewDTO>(cancellationToken) ?? throw new Exception("Failed to deserialize updated review");
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/reviews/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}