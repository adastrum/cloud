using Cloud.CommandStack.CommandHandlers;
using Cloud.CommandStack.Commands;
using Cloud.Infrastructure;
using Cloud.Messaging;
using Cloud.Web.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

namespace Cloud.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<OrderDbContext>(options => options.UseInMemoryDatabase("InMemoryDatabase"));
            services.AddHostedService<StorageQueueService>();
            services.AddTransient<ICommandDispatcher, InMemoryCommandDispatcher>();
            services.AddTransient<ICommandHandler<CreateOrderCommand>, CreateOrderCommandHandler>();

            var connectionString = Configuration.GetValue<string>("CloudStorageAccountConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            services.AddSingleton(storageAccount);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
