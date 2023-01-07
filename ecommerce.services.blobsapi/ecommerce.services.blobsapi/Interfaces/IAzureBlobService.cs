using ecommerce.services.blobsapi.Services;

namespace ecommerce.services.blobsapi.Interfaces
{
    public interface IAzureBlobService
    {
        Task<IEnumerable<Uri>> ListAsync();
        Task UploadAsync(IFormFileCollection files);
        Task<FileDetails> DownloadFile(string fileName);
        Task DeleteAsync(string fileUri);
        Task DeleteFileAsync(string fileName);
        Task DeleteAllAsync();
    }
}
