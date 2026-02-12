using System;
using System.Collections.Generic;
using System.Text;

namespace AllThruit3.Shared.Models;

public class ReviewResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Vibe { get; set; } = string.Empty;
    public Guid? MediaId { get; set; } // FK for one direct Media
    public MediaResult? Media { get; set; } // One-way nav to direct Media
    public List<MediaResult> LinkedMedia { get; set; } = []; // One-way many-to-many to linked Media
}
