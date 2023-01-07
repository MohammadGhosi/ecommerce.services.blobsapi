using ecommerce.services.blobsapi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ecommerce.services.blobsapi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ecommerce.services.blobsapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController : ControllerBase
    {
        private readonly IAzureBlobService _azureBlobService;
        public BlobsController(IAzureBlobService azureBlobService)
        {
            _azureBlobService = azureBlobService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFileCollection files)
        {
			try
			{
				if (files.Count == 0)
				{
					return BadRequest("Could not upload empty files");
				}

				await _azureBlobService.UploadAsync(files);
			}
			catch (Exception ex)
			{
				
			}
			return Ok("File Uploaded Successfully");
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var blobs = await _azureBlobService.ListAsync();
            return Ok(blobs);
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetAsync(string fileName)
        {
            if (fileName == null || fileName == "")
                return BadRequest("File name is not provided");

            FileDetails downloadedFile = await _azureBlobService.DownloadFile(fileName);
            return File(downloadedFile.FileStream, downloadedFile.ContentType, downloadedFile.FileName);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(string fileName)
        {
            await _azureBlobService.DeleteFileAsync(fileName);
            return Ok("File Deleted");
        }
    }
}
