using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace cXMLHandler
{
    public class Startup
    {
        public const string AppS3BucketKey = "AppS3Bucket";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();
            services.AddAWSService<Amazon.SimpleEmail.IAmazonSimpleEmailService>();

            var settings = new LambdaEnvironmentSettings();
            services.AddSingleton<IEnvironmentSettings>(settings);
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add S3 to the ASP.NET Core dependency injection framework.
            var settings = new LocalWindowsEnvironmentSettings(Configuration);
            services.AddSingleton<IEnvironmentSettings>(settings);

            var s3 = new Amazon.S3.AmazonS3Client(settings.AWSAccessKey, settings.AWSSecret, Amazon.RegionEndpoint.USWest2);
            services.AddSingleton<IAmazonS3>(s3);

            var emailService = new AmazonSimpleEmailServiceClient(settings.AWSAccessKey, settings.AWSSecret, Amazon.RegionEndpoint.USWest2); 
            //var emailClient = new Mock<IAmazonSimpleEmailService>();
            //emailClient.Setup(e => e.SendEmailAsync(It.IsAny<SendEmailRequest>(), new System.Threading.CancellationToken()))
            //    .Callback<SendEmailRequest, System.Threading.CancellationToken>((req, token) =>
            //      {
            //          Console.WriteLine("Email Sent:");
            //          Console.WriteLine(req.Message.Body);
            //      })
            //    .Returns(Task.FromResult(new SendEmailResponse()));
            services.AddSingleton<IAmazonSimpleEmailService>(emailService);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            throw new NotImplementedException("Environment name not found; can't configure services.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
