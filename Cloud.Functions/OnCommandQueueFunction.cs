using Cloud.CommandStack.CommandHandlers;
using Cloud.CommandStack.Commands;
using Cloud.Infrastructure;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Cloud.Functions
{
    public static class OnCommandQueueFunction
    {
        [FunctionName("OnCommandQueueFunction")]
        public static void Run([QueueTrigger("commandqueue")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            var command = JsonConvert.DeserializeObject<CreateOrderCommand>(myQueueItem);

            var orderDbContext = new OrderDbContext();
            var createOrderCommandHandler = new CreateOrderCommandHandler(orderDbContext);

            createOrderCommandHandler.Handle(command);
        }
    }
}
