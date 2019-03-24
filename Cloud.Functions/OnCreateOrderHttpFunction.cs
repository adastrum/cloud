using Cloud.CommandStack.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Cloud.Functions
{
    public static class OnCreateOrderHttpFunction
    {
        [FunctionName("OnCreateOrderHttpFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var body = await req.ReadAsStringAsync();

            var model = JsonConvert.DeserializeObject<CreateOrder>(body);

            var command = new CreateOrderCommand(model.Description, model.Amount.Value);

            var message = JsonConvert.SerializeObject(command,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var cloudStorageAccountConnectionString = config["CloudStorageAccountConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(cloudStorageAccountConnectionString);
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();
            var queue = cloudQueueClient.GetQueueReference("commandqueue");

            await queue.CreateIfNotExistsAsync();

            var cloudQueueMessage = new CloudQueueMessage(message);

            await queue.AddMessageAsync(cloudQueueMessage);

            return new OkResult();
        }
    }
}
