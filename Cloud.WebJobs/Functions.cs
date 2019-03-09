using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Cloud.WebJobs
{
    public class Functions
    {
        public static void ProcessQueueMessage([QueueTrigger("commandqueue")] string message, ILogger logger)
        {
            logger.LogInformation(message);
        }
    }
}
