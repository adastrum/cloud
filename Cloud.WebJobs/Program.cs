using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cloud.WebJobs
{
    public class Program
    {
        private static void Main()
        {
            var builder = new HostBuilder();
            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
            });
            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });
            var host = builder.Build();
            using (host)
            {
                host.Run();
            }
        }
    }
}
