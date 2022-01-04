using Amazon.S3;
using Amazon;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.CloudFiles.S3
{
    public static class CloudFilesOptionsExtensions
    {
        public static AmazonS3Client CreateClient(this CloudFilesOptions cloudFilesOptions)
        {

            var clientConfig = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast1,
                ServiceURL = cloudFilesOptions.ServiceUrl,
                UseHttp = new Uri(cloudFilesOptions.ServiceUrl).Scheme.ToLower() == "http",
                ForcePathStyle = true
                
            };

            return new AmazonS3Client(cloudFilesOptions.AccessKeyId, cloudFilesOptions.SecretAccessKey, clientConfig);
        }
    }
}
