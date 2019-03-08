using Cloud.CommandStack.Commands;
using Cloud.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.Web.Api.Services
{
    public class StorageQueueService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private Timer _timer;

        public StorageQueueService(
            ILogger<StorageQueueService> logger,
            ICommandDispatcher commandDispatcher
        )
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(_ => DeQueueMessage(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private void DeQueueMessage()
        {
            _logger.LogInformation("de-queued message");

            var command = new CreateOrderCommand("test", 42m);

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
