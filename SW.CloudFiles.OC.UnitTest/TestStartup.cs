using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SW.CloudFiles.Extensions;

namespace SW.CloudFiles.OC.UnitTest
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddOracleCloudFiles();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}