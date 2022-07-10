using BlobStorageTest.Services.BlobStorage;
using BlobStorageTest.Services.QueueStorage;
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
        private readonly IBlobStorageService _blobStorageService;
        private readonly IQueueStorageService _queueStorageService;

        public BlobTestController(IBlobStorageService blobStorageService, IQueueStorageService queueStorageService)
        {
            _blobStorageService = blobStorageService;
            _queueStorageService = queueStorageService;
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
            if (file == null || file.ContentType != "application/pdf")
                return BadRequest("Only PDFs are allowed");

            await _blobStorageService.UploadAsync(file.OpenReadStream(), file.FileName, file.ContentType);
            var emailToSend = "alexis.ortiz.2096@gmail.com";
            var name = "Alexis Ortiz";
            await _queueStorageService.SendMessageToQueue(file.FileName + "@@@" + emailToSend + "@@@" + name);

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
