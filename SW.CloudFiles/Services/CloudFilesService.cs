using Amazon.Runtime.SharedInterfaces;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SW.CloudFiles
{
    public class CloudFilesService : IDisposable
    {
        private readonly CloudFilesOptions cloudFilesOptions;
        //private readonly IHttpClientFactory httpClientFactory;
        private readonly AmazonS3Client client;

        public CloudFilesService(CloudFilesOptions cloudFilesOptions)
        {
            this.cloudFilesOptions = cloudFilesOptions;
            client = cloudFilesOptions.CreateClient(); // new AmazonS3Client(cloudFilesOptions.AccessKeyId, cloudFilesOptions.SecretAccessKey, clientConfig);

        }

        async public Task WriteAcync(string key, Stream inputStream)
        {
            var request = new PutObjectRequest
            {
                
                CannedACL = S3CannedACL.PublicRead,
                BucketName = cloudFilesOptions.BucketName,
                Key = key,
                //ContentBody = "sample text",
                InputStream = inputStream,
                //ContentType = "image/png",
                //StorageClass = S3StorageClass.Glacier

            };

            var response = await client.PutObjectAsync(request);
            
        }

        public Task<WriteWrapper> OpenWriteAsync(string key)
        {

            var request = new GetPreSignedUrlRequest
            {
                BucketName = cloudFilesOptions.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddHours(1),
                Verb = HttpVerb.PUT,
            };

            var url = client.GetPreSignedURL(request);

            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            httpWebRequest.Method = "PUT";

            return Task.FromResult(new WriteWrapper(httpWebRequest));
        }

        async public Task<ReadWrapper> OpenReadAcync(string key)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = cloudFilesOptions.BucketName,
                Key = key,
            };

            var response = await client.GetObjectAsync(request);

            return new ReadWrapper(response);
            //using (StreamReader reader = new StreamReader(responseStream))
            //{
            //    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
            //    string contentType = response.Headers["Content-Type"];
            //    Console.WriteLine("Object metadata, Title: {0}", title);
            //    Console.WriteLine("Content type: {0}", contentType);

            //    responseBody = reader.ReadToEnd(); // Now you process the response body.
            //}
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
