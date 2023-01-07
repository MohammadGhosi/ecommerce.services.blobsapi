using ecommerce.services.blobsapi.Interfaces;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ecommerce.services.blobsapi.Services
{
    public class AzureBlobService : IAzureBlobService
    {
		private readonly IAzureBlobConnectionFactory _azureBlobConnectionFactory;

		public AzureBlobService(IAzureBlobConnectionFactory azureBlobConnectionFactory)
		{
			_azureBlobConnectionFactory = azureBlobConnectionFactory;
		}

		public async Task DeleteAllAsync()
		{
			var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();

			BlobContinuationToken blobContinuationToken = null;
			do
			{
				var response = await blobContainer.ListBlobsSegmentedAsync(blobContinuationToken);
				foreach (IListBlobItem blob in response.Results)
				{
					if (blob.GetType() == typeof(CloudBlockBlob))
						await ((CloudBlockBlob)blob).DeleteIfExistsAsync();
				}
				blobContinuationToken = response.ContinuationToken;
			} while (blobContinuationToken != null);
		}

		public async Task DeleteAsync(string fileUri)
		{
			var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();

			Uri uri = new Uri(fileUri);
			string filename = Path.GetFileName(uri.LocalPath);

			var blob = blobContainer.GetBlockBlobReference(filename);
			await blob.DeleteIfExistsAsync();
		}

		public async Task DeleteFileAsync(string fileName)
		{
			var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();

			var blob = blobContainer.GetBlobReference(fileName);
			await blob.DeleteIfExistsAsync();
		}

		public async Task<IEnumerable<Uri>> ListAsync()
		{
			var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();
			var allBlobs = new List<Uri>();
			BlobContinuationToken blobContinuationToken = null;
			do
			{
				var response = await blobContainer.ListBlobsSegmentedAsync(blobContinuationToken);
				foreach (IListBlobItem blob in response.Results)
				{
					if (blob.GetType() == typeof(CloudBlockBlob))
						allBlobs.Add(blob.Uri);
				}
				blobContinuationToken = response.ContinuationToken;
			} while (blobContinuationToken != null);
			return allBlobs;
		}

		public async Task<FileDetails> DownloadFile(string fileName)
		{
            CloudBlockBlob blockBlob;
			await using (MemoryStream memoryStream = new MemoryStream())
            {
				var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();
				blockBlob = blobContainer.GetBlockBlobReference(fileName);
                await blockBlob.DownloadToStreamAsync(memoryStream);
            }
            Stream blobStream = blockBlob.OpenReadAsync().Result;

			FileDetails fileDetails = new FileDetails();
			fileDetails.FileName = blockBlob.Name;
			fileDetails.ContentType = blockBlob.Properties.ContentType;
			fileDetails.FileStream = blobStream;

            return fileDetails;
        }

		public async Task UploadAsync(IFormFileCollection files)
		{
			var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();

			for (int i = 0; i < files.Count; i++)
			{
				var blob = blobContainer.GetBlockBlobReference(files[i].FileName);
				using (var stream = files[i].OpenReadStream())
				{
					await blob.UploadFromStreamAsync(stream);

				}
			}
		}

		/// <summary> 
		/// string GetRandomBlobName(string filename): Generates a unique random file name to be uploaded  
		/// </summary> 
		private string GetRandomBlobName(string filename)
		{
			string ext = Path.GetExtension(filename);
			return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
		}
	}

	public class FileDetails
	{
		public string FileName { get; set; } = String.Empty;
		public string ContentType { get; set; } = String.Empty;
		public Stream FileStream { get; set; }
    }
}
