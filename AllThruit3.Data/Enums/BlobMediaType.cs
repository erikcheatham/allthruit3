namespace AllThruit3.Data.Enums;

public enum BlobMediaType
{
    Image,      // e.g., JPEG, PNG
    Video,      // e.g., MP4, AVI
    Audio,      // e.g., MP3, WAV
    Document,   // e.g., PDF, DOCX
    Archive,    // e.g., ZIP, RAR
    Text,       // e.g., TXT, CSV
    Executable, // e.g., EXE (careful with security)
    Other       // Catch-all for custom blobs
}
