using Cloud.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.Web.Api.Services
{
    public class CommandDispatchingService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ICommandQueueService _commandQueueService;
        private Timer _timer;

        public CommandDispatchingService(
            ILogger<CommandDispatchingService> logger,
            ICommandDispatcher commandDispatcher,
            ICommandQueueService commandQueueService
        )
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
            _commandQueueService = commandQueueService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(_ => DeQueueMessage(), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private void DeQueueMessage()
        {
            var result = _commandQueueService.TryDeleteMessageAsync().Result;
            if (!result.IsSuccess || string.IsNullOrWhiteSpace(result.Content))
            {
                return;
            }

            _logger.LogInformation($"de-queued message {result.Content}");

            var command = JsonConvert.DeserializeObject<ICommand>(
                result.Content,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

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
