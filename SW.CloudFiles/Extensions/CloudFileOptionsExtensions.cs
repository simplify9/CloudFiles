using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CloudFiles
{
    static class CloudFileOptionsExtensions
    {
        public static AmazonS3Client CreateClient(this CloudFilesOptions cloudFilesOptions)
        {

            var clientConfig = new AmazonS3Config
            {
                //RegionEndpoint = RegionEndpoint.
                ServiceURL = cloudFilesOptions.ServiceURL,
                //HttpClientFactory = httpClientFactory

            };

            return new AmazonS3Client(cloudFilesOptions.AccessKeyId, cloudFilesOptions.SecretAccessKey, clientConfig);
        }
    }
}
