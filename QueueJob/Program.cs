using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.IO;
using System.Threading.Tasks;

namespace QueueJob
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string SENDGRID_API_KEY = "<YOUR_SENDGRIP_API_KEY>";
            const string EMAIL = "<YOUR_SENGRID_SENDER_EMAIL>";
            const string BLOB_CONN_STRING = "<YOUR_BLOB_CONNECTION_STRING>";
            const string CONTAINER_NAME = "<YOUR_CONTAINER_NAME>";
            const string QUEUE_NAME = "<YOUR_QUEUE_NAME>";

            const string NAME_TO_DISPLAY = "QueueJob";
            const string TO_SPLIT = "@@@";
            const string SUBJECT = "Notification";
            const string PLAINTEXT_CONTENT = "Message sent by scheduled program";

            QueueClient queueClient = new QueueClient(BLOB_CONN_STRING, QUEUE_NAME);
            QueueMessage[] queueMessages = await queueClient.ReceiveMessagesAsync();

            if (queueMessages.Length != 0)
            {
                var sendGridClient = new SendGridClient(SENDGRID_API_KEY);
                var blobClient = new BlobServiceClient(BLOB_CONN_STRING);

                foreach (var message in queueMessages)
                {
                    var messageParts = message.Body.ToString().Split(TO_SPLIT);

                    var blobContainer = blobClient.GetBlobContainerClient(CONTAINER_NAME);
                    var blob = blobContainer.GetBlobClient(messageParts[0]);
                    var downloadContent = await blob.DownloadAsync();

                    var from = new EmailAddress(EMAIL, NAME_TO_DISPLAY);
                    var to = new EmailAddress(messageParts[1], messageParts[2]);
                    var email = MailHelper.CreateSingleEmail(from, to, SUBJECT, PLAINTEXT_CONTENT, null);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        await downloadContent.Value.Content.CopyToAsync(ms);
                        email.AddAttachment(messageParts[0], Convert.ToBase64String(ms.ToArray()));
                    }

                    var response = await sendGridClient.SendEmailAsync(email);

                    if (response.StatusCode.ToString().ToLower() == "unauthorized")
                    {
                        Console.WriteLine("Error while sending the message");
                    }
                    else
                    {
                        queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
                    }
                }
            }

            //Console.WriteLine("Job finished");
        }
    }
}
