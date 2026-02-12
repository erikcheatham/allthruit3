namespace AllThruit3.Shared.DTOs;

public class UserDTO
{
    public string Id { get; set; } = string.Empty; // IdentityUser.Id (string)
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public string? XHandle { get; set; } // Custom, e.g., @W4v3sBy3

    // Optional: List<string> Roles { get; set; } = new(); // If needed for auth checks
}