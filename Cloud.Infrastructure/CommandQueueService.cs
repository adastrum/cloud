using System;
using System.Threading.Tasks;
using Cloud.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Cloud.Infrastructure
{
    public class CommandQueueService : ICommandQueueService
    {
        private readonly CloudQueue _queue;

        public CommandQueueService(CloudStorageAccount cloudStorageAccount)
        {
            var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            _queue = cloudQueueClient.GetQueueReference("commandqueue");
        }

        public async Task<CommandQueueServiceResponse> TryAddMessageAsync(string message)
        {
            try
            {
                await _queue.CreateIfNotExistsAsync();

                var cloudQueueMessage = new CloudQueueMessage(message);

                await _queue.AddMessageAsync(cloudQueueMessage);

                return CommandQueueServiceResponse.Success(message);
            }
            catch (Exception exception)
            {
                return CommandQueueServiceResponse.Failure(exception.Message);
            }
        }

        public async Task<CommandQueueServiceResponse> TryDeleteMessageAsync()
        {
            try
            {
                await _queue.CreateIfNotExistsAsync();

                var cloudQueueMessage = await _queue.GetMessageAsync();

                await _queue.DeleteMessageAsync(cloudQueueMessage);

                return CommandQueueServiceResponse.Success(cloudQueueMessage.AsString);
            }
            catch (Exception exception)
            {
                return CommandQueueServiceResponse.Failure(exception.Message);
            }
        }
    }
}
