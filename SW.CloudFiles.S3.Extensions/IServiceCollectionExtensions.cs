
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SW.CloudFiles.S3;

namespace SW.CloudFiles.Extensions
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
        public static IServiceCollection AddCloudFiles(this IServiceCollection serviceCollection, Action<CloudFilesOptions> configure = null)
        {
            var cloudFilesOptions = new CloudFilesOptions();
            if (configure != null) configure.Invoke(cloudFilesOptions);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetRequiredService<IConfiguration>().GetSection(CloudFilesOptions.ConfigurationSection).Bind(cloudFilesOptions);

            using (var client = cloudFilesOptions.CreateClient())
            {

                if (!AmazonS3Util.DoesS3BucketExistV2Async(client, cloudFilesOptions.BucketName).WaitAndUnwrapException())
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = cloudFilesOptions.BucketName,
                        CannedACL = S3CannedACL.Private
                    };
                    client.PutBucketAsync(putBucketRequest).WaitAndUnwrapException();

                }

                 var config = client.GetLifecycleConfigurationAsync(new GetLifecycleConfigurationRequest
                {
                    BucketName = cloudFilesOptions.BucketName
                }).WaitAndUnwrapException().Configuration;


                var newRules = new List<LifecycleRule> { };

                if (config.Rules?.FirstOrDefault(r => r.Id == "temp1") == null || config.Rules?.FirstOrDefault(r => r.Id == "temp1")?.Status != LifecycleRuleStatus.Enabled) newRules.Add(new LifecycleRule
                {
                    Id = "temp1",
                    Expiration = new LifecycleRuleExpiration { Days = 1 },
                    Filter = new LifecycleFilter()
                    {
                        LifecycleFilterPredicate = new LifecyclePrefixPredicate()
                        {
                            Prefix = "temp1/"
                        }
                    },
                    Status = LifecycleRuleStatus.Enabled

                });

                if (config.Rules?.FirstOrDefault(r => r.Id  == "temp7") == null || config.Rules?.FirstOrDefault(r => r.Id == "temp7")?.Status != LifecycleRuleStatus.Enabled) newRules.Add(new LifecycleRule
                {
                    Id = "temp7",
                    Expiration = new LifecycleRuleExpiration { Days = 7 },
                    Filter = new LifecycleFilter()
                    {
                        LifecycleFilterPredicate = new LifecyclePrefixPredicate()
                        {
                            Prefix = "temp7/"
                        }
                    },
                    Status = LifecycleRuleStatus.Enabled
                });

                if (config.Rules?.FirstOrDefault(r => r.Id == "temp30") == null || config.Rules?.FirstOrDefault(r => r.Id == "temp30")?.Status != LifecycleRuleStatus.Enabled) newRules.Add(new LifecycleRule
                {
                    Id = "temp30",
                    Expiration = new LifecycleRuleExpiration { Days = 30 },
                    Filter = new LifecycleFilter
                    {
                        LifecycleFilterPredicate = new LifecyclePrefixPredicate
                        {
                            Prefix = "temp30/"
                        }
                    },
                    Status = LifecycleRuleStatus.Enabled
                });

                if (config.Rules?.FirstOrDefault(r => r.Id == "temp365") == null || config.Rules?.FirstOrDefault(r => r.Id == "temp365")?.Status != LifecycleRuleStatus.Enabled) newRules.Add(new LifecycleRule
                {
                    Id = "temp365",
                    Expiration = new LifecycleRuleExpiration { Days = 365 },
                    Filter = new LifecycleFilter()
                    {
                        LifecycleFilterPredicate = new LifecyclePrefixPredicate()
                        {
                            Prefix = "temp365/"
                        }
                    },
                    Status = LifecycleRuleStatus.Enabled

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

            return serviceCollection;
        }



    }
}
