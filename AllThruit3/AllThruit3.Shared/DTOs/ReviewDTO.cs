namespace AllThruit3.Shared.DTOs;

public class ReviewDTO
{
    public Guid Id { get; set; }
    public string MovieTitle { get; set; } = string.Empty;  // From Media.Title
    public string? MoviePosterUrl { get; set; }  // TMDB or Blob URI
    public string Content { get; set; } = string.Empty;  // Review.Text
    public int Rating { get; set; }  // Review.Rating
    public string Vibe { get; set; } = string.Empty;  // Review.Vibe
    // Add more if needed, e.g., CreatedBy, CreatedOn
}