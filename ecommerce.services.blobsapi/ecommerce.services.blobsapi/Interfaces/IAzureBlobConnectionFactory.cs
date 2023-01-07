using Microsoft.WindowsAzure.Storage.Blob;

namespace ecommerce.services.blobsapi.Interfaces
{
    public interface IAzureBlobConnectionFactory
    {
        Task<CloudBlobContainer> GetBlobContainer();
    }
}
