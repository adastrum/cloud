using Cloud.Messaging;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cloud.WebJobs
{
    public class Functions
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public Functions(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public void ProcessQueueMessage([QueueTrigger("commandqueue")] string message, ILogger logger)
        {
            logger.LogInformation(message);

            var command = JsonConvert.DeserializeObject<ICommand>(
                message,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

            _commandDispatcher.Publish(command);
        }
    }
}
