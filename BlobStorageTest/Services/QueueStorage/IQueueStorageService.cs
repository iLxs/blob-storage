using System.Threading.Tasks;

namespace BlobStorageTest.Services.QueueStorage
{
    public interface IQueueStorageService
    {
        Task SendMessageToQueue(string message);
    }
}
