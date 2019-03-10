using Cloud.CommandStack.CommandHandlers;
using Cloud.CommandStack.Commands;
using Cloud.Infrastructure;
using Cloud.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace Cloud.WebJobs
{
    public class Program
    {
        private static void Main()
        {
            var builder = new HostBuilder()
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices();
                    b.AddAzureStorage();
                })
                .ConfigureLogging((context, b) =>
                {
                    b.AddConsole();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<OrderDbContext>(options => options.UseSqlServer(hostContext.Configuration.GetValue<string>("SqlServerConnectionString")));
                    services.AddTransient<ICommandDispatcher, InMemoryCommandDispatcher>();
                    services.AddTransient<ICommandHandler<CreateOrderCommand>, CreateOrderCommandHandler>();
                    services.AddTransient<ICommandHandler<PayOrderCommand>, PayOrderCommandHandler>();
                    services.AddTransient<ICommandHandler<CancelOrderCommand>, CancelOrderCommandHandler>();

                    var connectionString = hostContext.Configuration.GetValue<string>("CloudStorageAccountConnectionString");
                    var storageAccount = CloudStorageAccount.Parse(connectionString);

                    services.AddSingleton(storageAccount);
                });

            var host = builder.Build();
            using (host)
            {
                host.Run();
            }
        }
    }
}
