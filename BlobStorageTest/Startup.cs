using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using BlobStorageTest.Services.BlobStorage;
using BlobStorageTest.Services.QueueStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlobStorageTest
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
            services.AddControllers();

            // Blob storage client
            services.AddScoped(_ => {
                return new BlobServiceClient(Configuration.GetConnectionString("BlobConnectionString"));
            });

            // Blob service
            services.AddScoped<IBlobStorageService, BlobStorageService>();


            services.AddScoped(_ => {
                return new QueueServiceClient(Configuration.GetConnectionString("BlobConnectionString"));
            });

            services.AddScoped<IQueueStorageService, QueueStorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
