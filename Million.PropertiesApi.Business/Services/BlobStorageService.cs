using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Million.PropertiesApi.Business.Interfaces;

namespace Million.PropertiesApi.Business.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("AzureBlobStorage");
            var containerName = config["BlobContainerName"];
            var client = new BlobServiceClient(connectionString);
            _container = client.GetBlobContainerClient(containerName);
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, CancellationToken ct = default)
        {
            var urls = new List<string>();

            foreach (var file in files)
            {
                var blobName = $"{Guid.NewGuid()}_{file.FileName}";
                var blob = _container.GetBlobClient(blobName);

                using var stream = file.OpenReadStream();
                await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType }, cancellationToken: ct);

                urls.Add(blob.Uri.ToString());
            }

            return urls;
        }
        public async Task<string?> UploadFileAsync(IFormFile? file, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                return null;

            var blobName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = _container.GetBlobClient(blobName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = file.ContentType },
                cancellationToken: ct
            );

            return blobClient.Uri.ToString();
        }

    }
}
