
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace SW.CloudFiles
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCloudFiles(this IServiceCollection serviceCollection, Action<CloudFilesOptions> configure)
        {
            var cloudFilesOptions = new CloudFilesOptions();
            configure.Invoke(cloudFilesOptions);

            using (var client = cloudFilesOptions.CreateClient())
            {
                if (!AmazonS3Util.DoesS3BucketExistV2Async(client, cloudFilesOptions.BucketName).Result)
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = cloudFilesOptions.BucketName,
                        //UseClientRegion = true,

                        CannedACL = S3CannedACL.Private
                    };

                    PutBucketResponse putBucketResponse = client.PutBucketAsync(putBucketRequest).Result;
                }

            }

            serviceCollection.AddSingleton(cloudFilesOptions);
            serviceCollection.AddTransient<CloudFilesService>();
            //serviceCollection.AddHttpClient();

            return serviceCollection;
        }



    }
}
