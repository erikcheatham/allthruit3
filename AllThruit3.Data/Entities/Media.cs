using System.Collections.Generic;

namespace AllThruit3.Data.Entities;

public class Media
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? Overview { get; set; }
    public string? ReleaseDate { get; set; }
    public string? FirstAirDate { get; set; }
    public double Popularity { get; set; }
    public string? PosterPath { get; set; } // TMDB path
    public string? BackdropPath { get; set; }
    public List<int>? GenreIds { get; set; }
    public bool Adult { get; set; }
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}