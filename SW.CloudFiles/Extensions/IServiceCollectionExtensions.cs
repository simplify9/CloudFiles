
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SW.CloudFiles
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Method to add all the needed services &
        /// it initializes a space if it doesnot exist &
        /// it makes sure of the lifecycle prefix rules to exist
        /// 
        /// Life cycle rules are:
        /// 1. temp1/ expires after a day
        /// 2. temp7/ expires after a week
        /// 3. temp30/ expires after a month
        /// 4. tem365/ expires after a year
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
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


                var newRules = new List<LifecycleRule> { };

                if (config.Rules.FirstOrDefault(r => r.Prefix == "temp1/") == null) newRules.Add(new LifecycleRule
                {
                    Id = "temp1",
                    Expiration = new LifecycleRuleExpiration { Days = 1 },
                    Prefix = "temp1/",
                });

                if (config.Rules.FirstOrDefault(r => r.Prefix == "temp7/") == null) newRules.Add(new LifecycleRule
                {
                    Id = "temp7",
                    Expiration = new LifecycleRuleExpiration { Days = 7 },
                    Prefix = "temp7/",
                });

                if (config.Rules.FirstOrDefault(r => r.Prefix == "temp30/") == null) newRules.Add(new LifecycleRule
                {
                    Id = "temp30",
                    Expiration = new LifecycleRuleExpiration { Days = 30 },
                    Prefix = "temp30/",
                });

                if (newRules.Count > 0)
                {

                    client.PutLifecycleConfigurationAsync(new PutLifecycleConfigurationRequest
                    {
                        BucketName= cloudFilesOptions.BucketName,
                        Configuration = new LifecycleConfiguration { Rules = newRules }
                    }).WaitAndUnwrapException();

                };


            }

            
            serviceCollection.AddSingleton(cloudFilesOptions);
            serviceCollection.AddTransient<ICloudFilesService, CloudFilesService>();
            //serviceCollection.AddTransient<CloudFilesService>();
            //serviceCollection.AddHttpClient();

            return serviceCollection;
        }



    }
}
