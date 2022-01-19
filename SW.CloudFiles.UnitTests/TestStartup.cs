using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SW.CloudFiles.Extensions;

namespace SW.CloudFiles.UnitTests
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCloudFiles(config =>
            //{
            //    config.AccessKeyId = "R3LNFRKWMAC4OCCRICS5";
            //    config.SecretAccessKey = "YPyyTdxs+lZMQEtYIDRK9lkIzjJrCKXinE3OfKEfc7k";
            //    config.ServiceUrl = "https://fra1.digitaloceanspaces.com";
            //    config.BucketName = "sf9";
            //});
            services.AddS3CloudFiles();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseRouting();
            //app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
        }
    }
}
