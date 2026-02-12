using AllThruit3.Shared.Common;  // For Result
using AllThruit3.Shared.Features.Identity;  // For RegisterCommand, LoginCommand
using Microsoft.Extensions.Configuration;  // For config in factory
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AllThruit3.Shared.Services;

public interface IIdentityClient
{
    Task<Result> RegisterAsync(RegisterCommand command, CancellationToken ct = default);
    Task<Result<string>> LoginAsync(LoginCommand command, CancellationToken ct = default);
    // Add more as needed, e.g., Task<Result> LogoutAsync(); Task<CurrentUserResponse> GetCurrentUserAsync();
}

public class IdentityClient : IIdentityClient
{
    private readonly HttpClient _client;

    public IdentityClient(HttpClient client) => _client = client;

    public async Task<Result> RegisterAsync(RegisterCommand command, CancellationToken ct = default)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/account/register", command, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            return Result.Failure(new Error("Identity.RegisterError", $"Registration failed: {errorContent}", string.Empty));
        }
        return Result.Success();
    }

    public async Task<Result<string>> LoginAsync(LoginCommand command, CancellationToken ct = default)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/account/login", command, ct);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            return Result.Failure<string>(new Error("Identity.LoginError", $"Login failed: {errorContent}", string.Empty));
        }
        var successMessage = await response.Content.ReadAsStringAsync(ct);  // e.g., "Login successful" or token
        return Result.Success(successMessage);
    }

    // Implement more methods similarly, e.g., for logout or current-user GET
}