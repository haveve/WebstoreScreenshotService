using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Shared.Core.Services;

public class BlobConfigurations
{
    public string ConnectionString { get; set; } = null!;

    public string ContainerName { get; set; } = null!;
}

public class BlobStorageService
{
    private readonly BlobContainerClient _container;

    public BlobStorageService(BlobConfigurations blobConfigurations)
    {
        var client = new BlobServiceClient(blobConfigurations.ConnectionString);
        _container = client.GetBlobContainerClient(blobConfigurations.ContainerName);
        _container.CreateIfNotExists(PublicAccessType.None);
    }

    public async Task<string> UploadAsync(
        byte[] fileStream,
        string fileName,
        string contentType)
    {
        using var memoryStream = new MemoryStream(fileStream);
        var blob = _container.GetBlobClient(fileName);
        await blob.UploadAsync(memoryStream, new BlobHttpHeaders
        {
            ContentType = contentType
        });

        return fileName;
    }

    public async Task DeleteAsync(string fileName)
    {
        var blob = _container.GetBlobClient(fileName);
        await blob.DeleteIfExistsAsync();
    }

    public string GetReadSasUrl(string fileName, TimeSpan lifetime)
    {
        var blob = _container.GetBlobClient(fileName);

        var sas = blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(lifetime));

        return sas.ToString();
    }
}

