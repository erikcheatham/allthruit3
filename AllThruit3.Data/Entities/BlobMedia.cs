using AllThruit3.Data.Enums;

namespace AllThruit3.Data.Entities;

public class BlobMedia
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string BlobName { get; set; } = string.Empty; // e.g., "media/123/poster.jpg"
    public string ContainerName { get; set; } = "allthruit-media";
    public BlobMediaType Type { get; set; }
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}