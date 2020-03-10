
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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



                var task = Task.Run(async () => await AmazonS3Util.DoesS3BucketExistV2Async(client, cloudFilesOptions.BucketName));

                if (!AmazonS3Util.DoesS3BucketExistV2Async(client, cloudFilesOptions.BucketName).WaitAndUnwrapException())
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = cloudFilesOptions.BucketName,
                        CannedACL = S3CannedACL.Private
                    };
                    _ =  client.PutBucketAsync(putBucketRequest).WaitAndUnwrapException();

                }

                 var config = client.GetLifecycleConfigurationAsync(new GetLifecycleConfigurationRequest
                {
                    BucketName = cloudFilesOptions.BucketName
                }).WaitAndUnwrapException().Configuration;

                if (config.Rules.Count == 0)
                {

                    client.PutLifecycleConfigurationAsync(new PutLifecycleConfigurationRequest
                    {
                        BucketName= cloudFilesOptions.BucketName,
                        Configuration = new LifecycleConfiguration
                        {
                            Rules = new List<LifecycleRule>
                            {
                                new LifecycleRule
                                {
                                    Id = "temp1",
                                    Expiration = new LifecycleRuleExpiration { Days=1},
                                    Prefix = "temp1/",
                                },
                                new LifecycleRule
                                {
                                    Id = "temp7",
                                    Expiration = new LifecycleRuleExpiration { Days = 7 },
                                    Prefix = "temp7/",
                                },
                                new LifecycleRule
                                {
                                    Id = "temp30",
                                    Expiration = new LifecycleRuleExpiration { Days = 30 },
                                    Prefix = "temp30/",
                                }

                            }
                        }
                    }).WaitAndUnwrapException();

                };


            }

            serviceCollection.AddSingleton(cloudFilesOptions);
            serviceCollection.AddTransient<CloudFilesService>();
            //serviceCollection.AddHttpClient();

            return serviceCollection;
        }



    }
}
