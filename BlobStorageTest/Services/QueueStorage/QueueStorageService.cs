using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BlobStorageTest.Services.QueueStorage
{
    public class QueueStorageService : IQueueStorageService
    {
        private readonly QueueServiceClient _queueServiceClient;
        private readonly string _queueName;

        public QueueStorageService(QueueServiceClient queueServiceClient, IConfiguration configuration)
        {
            _queueServiceClient = queueServiceClient;
            _queueName = configuration.GetValue<string>("BlobQueueName");

            _ = InitializeBlobQueueAsync();
        }

        private async Task InitializeBlobQueueAsync()
        {
            var queueClient = _queueServiceClient.GetQueueClient(_queueName);
            await queueClient.CreateIfNotExistsAsync();
        }

        public async Task SendMessageToQueue(string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(_queueName);
            await queueClient.SendMessageAsync(message);
        }
    }
}
