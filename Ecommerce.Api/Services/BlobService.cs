using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Ecommerce.Api.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobClient;

    public BlobService(BlobServiceClient blobClient) 
    {
        _blobClient = blobClient;
    }
    
    public async Task<string> GetBlob(string blobName, string containerName)
    {
        var blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);
        return blobClient.Uri.AbsoluteUri;
    }

    public async Task<bool> DeleteBlob(string blobName, string containerName)
    {
       var blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
       var blobClient = blobContainerClient.GetBlobClient(blobName);
       return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<string> UploadBlob(string blobName, string containerName, IFormFile file)
    {
        var blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);
        var httpHeaders = new BlobHttpHeaders()
        {
            ContentType = file.ContentType
        };
        // finally we have to upload
        var result = await blobClient.UploadAsync(file.OpenReadStream(),httpHeaders);
        if (result is not null)
        {
            return await GetBlob(blobName, containerName);
        }

        return "";

    }
}