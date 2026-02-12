namespace AllThruit3.Shared.Models;

public class MediaResult
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Overview { get; set; }
    public string? ReleaseDate { get; set; }
    public double Popularity { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public List<int>? GenreIds { get; set; }
    public bool Adult { get; set; }
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
}