namespace AllThruit3.Shared.DTOs;

public class MediaDTO
{
    public Guid Id { get; set; } // Blob GUID
    public string? Uri { get; set; } // Resolved URL
    public string? ContentType { get; set; } // e.g., "image/jpeg"
    public string? TmdbMovieId { get; set; } // For movie data JSON
    public string? JsonData { get; set; } // For cached movie JSON
}