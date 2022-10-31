namespace NashPhaseOne.API.BlobHelper
{
    public interface IBlobService
    {
        public Task UploadFileBlobAsync(IFormFile file, string fileName);

        public Task DeleteBlobAsync(string blobname);
    }
}
