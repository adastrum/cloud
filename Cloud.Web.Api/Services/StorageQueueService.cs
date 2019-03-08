using Cloud.CommandStack.Commands;
using Cloud.Messaging;
using Cloud.Web.Api.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.Web.Api.Services
{
    public class StorageQueueService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly CloudQueue _queue;
        private Timer _timer;

        public StorageQueueService(
            ILogger<StorageQueueService> logger,
            ICommandDispatcher commandDispatcher,
            CloudStorageAccount cloudStorageAccount
        )
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
            var cloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            _queue = cloudQueueClient.GetQueueReference("commandqueue");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(_ => DeQueueMessage(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private void DeQueueMessage()
        {
            _queue.CreateIfNotExistsAsync().Wait();
            var message = _queue.GetMessageAsync().Result;
            var messageAsString = message?.AsString;

            if (string.IsNullOrWhiteSpace(messageAsString))
            {
                return;
            }

            _queue.DeleteMessageAsync(message).Wait();

            _logger.LogInformation($"de-queued message {messageAsString}");

            var createOrder = JsonConvert.DeserializeObject<CreateOrder>(messageAsString);
            var command = new CreateOrderCommand(createOrder.Description, createOrder.Amount.Value);

            _commandDispatcher.Publish(command);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
