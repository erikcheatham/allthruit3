using AllThruit3.Shared.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Azure;
using System.Text;

namespace AllThruit3.Data.Services;

/// <summary>
/// Provides methods to interact with Azure Blob Storage for storing images and movie data.
/// </summary>
public class BlobStorageService(IAzureClientFactory<BlobServiceClient> clientFactory) : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient = clientFactory.CreateClient("Default");
    private readonly string _containerName = "movies"; // Dedicated container

    /// <summary>
    /// Uploads an image to blob storage.
    /// </summary>
    /// <param name="imageStream">The image stream.</param>
    /// <param name="contentType">The content type of the image.</param>
    /// <returns>The unique identifier of the uploaded blob.</returns>
    public async Task<Guid> UploadImageAsync(Stream imageStream, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobId = Guid.NewGuid();
        var blobClient = containerClient.GetBlobClient($"{blobId}.jpg"); // Or detect extension

        await blobClient.UploadAsync(imageStream, new BlobHttpHeaders { ContentType = contentType });
        return blobId;
    }

    /// <summary>
    /// Gets the URI of a blob by its identifier.
    /// </summary>
    /// <param name="blobId">The blob identifier.</param>
    /// <returns>The URI of the blob.</returns>
    public async Task<string> GetBlobUriAsync(Guid blobId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient($"{blobId}.jpg");
        return blobClient.Uri.ToString();
    }

    /// <summary>
    /// Uploads movie data as JSON to blob storage.
    /// </summary>
    /// <param name="tmdbMovieId">The TMDB movie ID.</param>
    /// <param name="jsonData">The JSON data to upload.</param>
    public async Task UploadMovieDataAsync(string tmdbMovieId, string jsonData)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient($"{tmdbMovieId}.json");
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
        await blobClient.UploadAsync(stream, overwrite: true);
    }

    /// <summary>
    /// Retrieves movie data as JSON from blob storage.
    /// </summary>
    /// <param name="tmdbMovieId">The TMDB movie ID.</param>
    /// <returns>The JSON data if found; otherwise, null.</returns>
    public async Task<string?> GetMovieDataAsync(string tmdbMovieId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient($"{tmdbMovieId}.json");

        if (!await blobClient.ExistsAsync()) return null;

        var response = await blobClient.DownloadContentAsync();
        return response.Value.Content.ToString();
    }
}