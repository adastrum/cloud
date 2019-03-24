using Cloud.CommandStack.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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
            [Queue("commandqueue")]IAsyncCollector<CreateOrderCommand> outputQueue)
        {
            var body = await req.ReadAsStringAsync();

            var model = JsonConvert.DeserializeObject<CreateOrder>(body);

            //todo: validation?

            var command = new CreateOrderCommand(model.Description, model.Amount.Value);

            await outputQueue.AddAsync(command);

            return new OkResult();
        }
    }
}
