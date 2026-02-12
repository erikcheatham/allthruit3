using System;
using System.Collections.Generic;

namespace AllThruit3.Data.Entities;

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Vibe { get; set; } = string.Empty;
    public int? MediaId { get; set; } // FK for one direct Media
    public Media? Media { get; set; } // One-way nav to direct Media
    public List<Media> LinkedMedia { get; set; } = []; // One-way many-to-many to linked Media
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}