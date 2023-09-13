using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SW.CloudFiles.OC;
using SW.PrimitiveTypes;

namespace SW.CloudFiles.Extensions
{

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddOracleCloudFiles(this IServiceCollection serviceCollection,
            Action<OracleCloudFilesOptions> configure = null)
        {
            var cloudFilesOptions = new OracleCloudFilesOptions();
            if (configure != null) configure.Invoke(cloudFilesOptions);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetRequiredService<IConfiguration>().GetSection(CloudFilesOptions.ConfigurationSection)
                .Bind(cloudFilesOptions);

            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);


            var pemPath = Path.Combine(directory, $"{Guid.NewGuid().ToString("N")}.pem");


            // Create the file and write the content
            File.WriteAllText(pemPath, cloudFilesOptions.RSAKey);


            var configPAth = Path.Combine(directory, $"{Guid.NewGuid().ToString("N")}.config");
            File.WriteAllText(configPAth, @$"[DEFAULT]
user={cloudFilesOptions.UserId}
fingerprint={cloudFilesOptions.FingerPrint}
tenancy={cloudFilesOptions.TenantId}
region={cloudFilesOptions.Region}
key_file={pemPath}");
            cloudFilesOptions.ConfigPath = configPAth;
            serviceCollection.AddScoped<ICloudFilesService, CloudFilesService>();
            serviceCollection.AddSingleton(cloudFilesOptions);
            serviceCollection.AddSingleton((CloudFilesOptions)cloudFilesOptions);
            return serviceCollection;
        }
    }
}