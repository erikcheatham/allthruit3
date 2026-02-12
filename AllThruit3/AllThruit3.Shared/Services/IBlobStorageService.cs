namespace AllThruit3.Shared.Services;

public interface IBlobStorageService
{
    Task<Guid> UploadImageAsync(Stream imageStream, string contentType);
    Task<string> GetBlobUriAsync(Guid blobId);
    Task UploadMovieDataAsync(string tmdbMovieId, string jsonData);
    Task<string?> GetMovieDataAsync(string tmdbMovieId);
}