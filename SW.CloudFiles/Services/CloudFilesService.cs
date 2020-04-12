using Amazon.Runtime.SharedInterfaces;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SW.CloudFiles
{
    public class CloudFilesService : IDisposable, ICloudFilesService
    {

        private const string metadataPrefix = "x-amz-meta-";
        private readonly CloudFilesOptions cloudFilesOptions;
        private readonly AmazonS3Client client;

        public CloudFilesService(CloudFilesOptions cloudFilesOptions)
        {
            this.cloudFilesOptions = cloudFilesOptions;
            client = cloudFilesOptions.CreateClient();

        }

        async public Task<RemoteBlob> WriteAcync(Stream inputStream, WriteFileSettings settings)
        {

            var contentLength = inputStream.Length;

            var request = new PutObjectRequest
            {
                Key = settings.Key,
                CannedACL = settings.Public ? S3CannedACL.PublicRead : S3CannedACL.Private,
                ContentType = settings.ContentType,
                InputStream = inputStream,
                AutoCloseStream = settings.CloseInputStream,
                BucketName = cloudFilesOptions.BucketName,
               

            };

            foreach (var kvp in settings.Metadata)
                request.Metadata.Add(kvp.Key, kvp.Value);

            await client.PutObjectAsync(request);

            return new RemoteBlob
            {
                Location = settings.Public ? GetUrl(settings.Key) : GetSignedUrl(settings.Key, TimeSpan.FromHours(1)),
                MimeType = settings.ContentType,
                Name = settings.Key,
                Size = Convert.ToInt32(contentLength)
            };
        }

        async public Task<RemoteBlob> WriteTextAcync(string text, WriteFileSettings settings)
        {
            var request = new PutObjectRequest
            {
                Key = settings.Key,
                CannedACL = settings.Public ? S3CannedACL.PublicRead : S3CannedACL.Private,
                ContentBody = text,
                BucketName = cloudFilesOptions.BucketName,

            };

            await client.PutObjectAsync(request);

            return new RemoteBlob
            {
                Location = settings.Public ? GetUrl(settings.Key) : GetSignedUrl(settings.Key, TimeSpan.FromHours(1)),
                MimeType = settings.ContentType,
                Name = settings.Key,
                Size = Convert.ToInt32(text.Length)
            };
        }

        public string GetSignedUrl(string key, TimeSpan expiry)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = cloudFilesOptions.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.Add(expiry),
                Verb = HttpVerb.GET,
            };

            return client.GetPreSignedURL(request);
        }

        public string GetUrl(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = cloudFilesOptions.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddSeconds(1),
                Verb = HttpVerb.GET,
            };

            var signedUrl = client.GetPreSignedURL(request);

            return signedUrl.Substring(0, signedUrl.IndexOf('?'));
        }



        public WriteWrapper OpenWrite(WriteFileSettings settings)
        {

            var request = new GetPreSignedUrlRequest
            {
                BucketName = cloudFilesOptions.BucketName,
                Key = settings.Key,
                ContentType = settings.ContentType,
                Expires = DateTime.UtcNow.AddHours(1),
                Verb = HttpVerb.PUT,
                
            };

            if (settings.Public)
                request.Headers["x-amz-acl"] = "public-read";

            var url = client.GetPreSignedURL(request);

            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            httpWebRequest.ContentType = settings.ContentType;
            if (settings.Public)
                httpWebRequest.Headers.Add("x-amz-acl", "public-read");

            httpWebRequest.Method = "PUT";


            return new WriteWrapper(httpWebRequest, this, settings);
        }

        async public Task<IReadOnlyDictionary<string, string>> GetMetadataAsync(string key)
        {
            GetObjectMetadataRequest request = new GetObjectMetadataRequest
            {
                BucketName = cloudFilesOptions.BucketName,
                Key = key,


            };

            var response = await client.GetObjectMetadataAsync(request);

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Hash",response.ETag.Replace("\"", "") },
                {"ContentType",response.Headers["Content-Type"] },
                {"ContentLength",response.Headers["Content-Length"] },

            };

            foreach (var metadataKey in response.Metadata.Keys)
            {
                result.Add(metadataKey.Replace(metadataPrefix, ""), response.Metadata[metadataKey]);
            }


            return result;

        }

        async public Task<Stream> OpenReadAcync(string key)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = cloudFilesOptions.BucketName,
                Key = key,


            };

            var response = await client.GetObjectAsync(request);
            return response.ResponseStream;
            //using (StreamReader reader = new StreamReader(responseStream))
            //{
            //    string title = response.Metadata["x-amz-meta-title"]; // Assume you have "title" as medata added to the object.
            //    string contentType = response.Headers["Content-Type"];
            //    Console.WriteLine("Object metadata, Title: {0}", title);
            //    Console.WriteLine("Content type: {0}", contentType);

            //    responseBody = reader.ReadToEnd(); // Now you process the response body.
            //}
        }

        public async Task<IEnumerable<CloudFileInfo>> ListAsync(string prefix)
        {

            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = cloudFilesOptions.BucketName,
                Prefix = prefix
            };

            var result = new List<CloudFileInfo>();

            var response = await client.ListObjectsV2Async(request);

            foreach (var entry in response.S3Objects)
            {
                result.Add(new CloudFileInfo
                {
                    Key = entry.Key,
                    Size = entry.Size,
                    Signature = entry.ETag

                });
            }

            return result;

        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
