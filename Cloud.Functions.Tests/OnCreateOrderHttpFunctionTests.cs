using Cloud.CommandStack.Commands;
using Cloud.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cloud.Functions.Tests
{
    public class OnCreateOrderHttpFunctionTests
    {
        [Fact]
        public async Task Should_queue_create_order_command()
        {
            var model = new CreateOrder
            {
                Description = "test",
                Amount = 42m
            };

            var json = JsonConvert.SerializeObject(model);

            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new MemoryStream(Encoding.UTF8.GetBytes(json))
            };

            var logger = NullLoggerFactory.Instance.CreateLogger("null");
            var queueMock = new Mock<IAsyncCollector<CreateOrderCommand>>();

            var response = (OkResult)await OnCreateOrderHttpFunction.Run(request, logger, queueMock.Object);

            queueMock.Verify(_ =>
                _.AddAsync(It.Is<CreateOrderCommand>(x => x.Description == model.Description && x.Amount == model.Amount),
                    default(CancellationToken)));
        }
    }
}
