using System.IO;
using System.Threading.Tasks;

namespace BlobStorageTest.Services.BlobStorage
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType);
        Task<byte[]> DownloadAsync(string imageName);
        Task<bool> FileExists(string imageName);
    }
}
