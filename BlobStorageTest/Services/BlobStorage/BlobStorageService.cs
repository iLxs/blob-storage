using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace BlobStorageTest.Services.BlobStorage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private string _blobContainerName;

        public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            _blobServiceClient = blobServiceClient;
            _blobContainerName = configuration.GetValue<string>("BlobContainerName");

            InitializeBlobContainerAsync();
        }

        private async Task InitializeBlobContainerAsync()
        {
            var container = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            var createResponse = await container.CreateIfNotExistsAsync();
            if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                await container.SetAccessPolicyAsync(PublicAccessType.Blob);
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            var container = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            var blob = container.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
            return blob.Uri.ToString();
        }

        public async Task<byte[]> DownloadAsync(string imageName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_blobContainerName);

            var blobClient = blobContainer.GetBlobClient(imageName);
            var downloadContent = await blobClient.DownloadAsync();
            using (MemoryStream ms = new MemoryStream())
            {
                await downloadContent.Value.Content.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public Task<bool> FileExists(string imageName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
            var blobClient = blobContainer.GetBlobClient(imageName);

            bool result = false;
            if (blobClient.Exists())
                result = true;

            return Task.FromResult(result);
        }
    }
}
