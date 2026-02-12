using System.Collections.Generic;

namespace AllThruit3.Data.Entities;

public class Media
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Title { get; set; } = string.Empty;
    public string? Overview { get; set; } = string.Empty;
    public string? ReleaseDate { get; set; } = string.Empty;
    public double Popularity { get; set; }
    public string? PosterPath { get; set; } = string.Empty;
    public string? BackdropPath { get; set; } = string.Empty;
    public List<int>? GenreIds { get; set; }
    public bool Adult { get; set; }
    public double VoteAverage { get; set; }
    public int VoteCount { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}