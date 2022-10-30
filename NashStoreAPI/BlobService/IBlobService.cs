namespace NashPhaseOne.API.BlobHelper
{
    public interface IBlobService
    {
        public Task UploadFileBlobAsync(IFormFile file);

        public Task DeleteBlobAsync(string blobname);
    }
}
