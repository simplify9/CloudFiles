using Amazon.S3;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CloudFiles
{
    public static class CloudFilesOptionsExtensions
    {
        public static AmazonS3Client CreateClient(this CloudFilesOptions cloudFilesOptions)
        {

            var clientConfig = new AmazonS3Config
            {
                //RegionEndpoint = RegionEndpoint.
                ServiceURL = cloudFilesOptions.ServiceUrl,
                //HttpClientFactory = httpClientFactory

            };

            return new AmazonS3Client(cloudFilesOptions.AccessKeyId, cloudFilesOptions.SecretAccessKey, clientConfig);
        }
    }
}
