using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NashPhaseOne.API.BlobHelper;

namespace NashPhaseOne.API.BlobService
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        const string CONTAINER_NAME = "nashstoreimage";

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public Task DeleteBlobAsync(string blobname)
        {
            throw new NotImplementedException();
        }

        public async Task UploadFileBlobAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(CONTAINER_NAME);
            var blobClient = containerClient.GetBlobClient(file.FileName);

            await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType});
        }
    }
}
