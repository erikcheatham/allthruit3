using AllThruit3.Shared.DTOs;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AllThruit3.Shared.Repositories;

public interface IMediaRepository
{
    Task<MediaDTO> UploadImageAsync(Stream imageStream, string contentType, CancellationToken cancellationToken = default);
    Task<string> GetBlobUriAsync(Guid blobId, CancellationToken cancellationToken = default);
    Task UploadMovieDataAsync(string tmdbMovieId, string jsonData, CancellationToken cancellationToken = default);
    Task<string?> GetMovieDataAsync(string tmdbMovieId, CancellationToken cancellationToken = default);
    // Future: Task<MediaDto> UploadGifAsync(...), UploadVideoAsync(...)
}