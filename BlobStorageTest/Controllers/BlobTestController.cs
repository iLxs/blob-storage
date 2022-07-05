using BlobStorageTest.Services.BlobStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlobStorageTest.Controllers
{
    [ApiController]
    [Route("api/blob")]
    public class BlobTestController : ControllerBase
    {
        private IBlobStorageService _blobStorageService;

        public BlobTestController(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello world");
        }

        /// <summary>
        /// Upload a file to the blob storage
        /// </summary>
        /// <param name="file">The file to upload</param>
        [Route("upload")]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file != null)
            {
                await _blobStorageService.UploadAsync(file.OpenReadStream(), file.FileName, file.ContentType);
            }
            return Ok();
        }

        /// <summary>
        /// Download a file from the blob storage given the filename
        /// </summary>
        /// <param name="fileName">File name with extension. Example: dummy.pdf</param>
        /// <returns>The file</returns>
        [Route("download")]
        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var fileExists = await _blobStorageService.FileExists(fileName);
            if (!fileExists)
                return BadRequest("The file does not exist.");

            var imagBytes = await _blobStorageService.DownloadAsync(fileName);
            return new FileContentResult(imagBytes, "application/pdf")
            {
                FileDownloadName = $"{Guid.NewGuid()}_{fileName}.pdf",
            };
        }
    }
}
