using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SW.PrimitiveTypes;

namespace SW.CloudFiles.AS.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAsCloudFiles(this IServiceCollection serviceCollection, Action<CloudFilesOptions> configure = null)
        {
            var cloudFilesOptions = new CloudFilesOptions();
            if (configure != null) configure.Invoke(cloudFilesOptions);

            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetRequiredService<IConfiguration>().GetSection(CloudFilesOptions.ConfigurationSection).Bind(cloudFilesOptions);

            var blobServiceClient = new BlobServiceClient(new Uri(cloudFilesOptions.ServiceUrl),
                new StorageSharedKeyCredential(cloudFilesOptions.AccessKeyId, cloudFilesOptions.SecretAccessKey));

            var containers = blobServiceClient.GetBlobContainers();
            BlobContainerClient blobContainerClient;
            
            if (containers.All(c => c.Name != cloudFilesOptions.BucketName))
                blobContainerClient =
                    blobServiceClient.CreateBlobContainer(cloudFilesOptions.BucketName, PublicAccessType.BlobContainer);
            else
                blobContainerClient = blobServiceClient.GetBlobContainerClient(cloudFilesOptions.BucketName);

            serviceCollection.AddSingleton(blobContainerClient);
            serviceCollection.AddSingleton(cloudFilesOptions);
            serviceCollection.AddTransient<ICloudFilesService, CloudFilesService>();
            return serviceCollection;
        }
    }
}