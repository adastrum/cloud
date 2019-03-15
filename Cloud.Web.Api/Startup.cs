using Cloud.Caching;
using Cloud.Infrastructure;
using Cloud.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Swagger;
using System;

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
            services.AddDbContext<OrderDbContext>(options => options.UseSqlServer(Configuration.GetValue<string>("SqlServerConnectionString")));
            services.AddTransient<ICommandQueueService, CommandQueueService>();

            var cloudStorageAccountConnectionString = Configuration.GetValue<string>("CloudStorageAccountConnectionString");
            var storageAccount = CloudStorageAccount.Parse(cloudStorageAccountConnectionString);

            services.AddSingleton(storageAccount);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var redisConnectionString = Configuration.GetValue<string>("RedisConnectionString");
                return ConnectionMultiplexer.Connect(redisConnectionString);
            });
            var cache = lazyConnection.Value.GetDatabase();
            services.AddSingleton(cache);
            services.AddScoped<ICachingService, RedisCachingService>(provider =>
            {
                var database = provider.GetService<IDatabase>();
                return new RedisCachingService(database, TimeSpan.FromMinutes(1));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
