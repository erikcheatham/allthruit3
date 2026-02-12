using AllThruit3.Shared.DTOs;
using AllThruit3.Shared.Repositories;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllThruit3.Data.Repositories;

/// <summary>
/// Repository for handling media-related operations such as uploading images and movie data to Azure Blob Storage.
/// </summary>
public class MediaRepository : IMediaRepository
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "movies";

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaRepository"/> class.
    /// </summary>
    /// <param name="blobServiceClient">The Azure BlobServiceClient used for blob storage operations.</param>
    public MediaRepository(BlobServiceClient blobServiceClient) => _blobServiceClient = blobServiceClient;

    public async Task<MediaDTO> UploadImageAsync(Stream imageStream, string contentType, CancellationToken ct = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await container.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobId = Guid.NewGuid();
        var blobClient = container.GetBlobClient($"{blobId}.jpg");
        await blobClient.UploadAsync(
            imageStream,
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            },
            ct
        );
        return new MediaDTO { Id = blobId, Uri = blobClient.Uri.ToString(), ContentType = contentType };
    }

    public async Task<string> GetBlobUriAsync(Guid blobId, CancellationToken ct = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = container.GetBlobClient($"{blobId}.jpg");
        return blobClient.Uri.ToString(); // Or check exists
    }

    public async Task UploadMovieDataAsync(string tmdbMovieId, string jsonData, CancellationToken ct = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await container.CreateIfNotExistsAsync(cancellationToken: ct);

        var blobClient = container.GetBlobClient($"{tmdbMovieId}.json");
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
        await blobClient.UploadAsync(stream, overwrite: true, ct);
    }

    public async Task<string?> GetMovieDataAsync(string tmdbMovieId, CancellationToken ct = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = container.GetBlobClient($"{tmdbMovieId}.json");

        if (!await blobClient.ExistsAsync(ct)) return null;

        var response = await blobClient.DownloadContentAsync(ct);
        return response.Value.Content.ToString();
    }
}