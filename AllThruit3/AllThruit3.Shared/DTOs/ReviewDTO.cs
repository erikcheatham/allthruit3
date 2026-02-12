namespace AllThruit3.Shared.DTOs;

public class ReviewDTO
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty; // string for Identity compatibility
    public string MovieTitle { get; set; } = string.Empty; // From TMDB
    public string TmdbMovieId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // Was Text?
    public int Rating { get; set; }
    public string Vibe { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty; // Resolved from Blob
    public string? MoviePosterUrl { get; set; } // Resolved Blob URI if needed
    public DateTime CreatedOn { get; set; }

    // Media references (GUIDs to Azure Blob)
    public Guid? PosterBlobId { get; set; }
    // Future: Guid? GifBlobId { get; set; }
    // Future: Guid? VideoBlobId { get; set; }
}