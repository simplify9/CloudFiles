using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Oci.Common.Auth;
using Oci.ObjectstorageService;
using Oci.ObjectstorageService.Requests;
using Oci.ObjectstorageService.Transfer;
using SW.PrimitiveTypes;

namespace SW.CloudFiles.OC
{
    public class OracleCloudFilesOptions : CloudFilesOptions
    {
        public string TenantId { get; set; }
            

        public string FingerPrint { get; set; }
        public string UserId { get; set; }

        public string RSAKey { get; set; }
        public string ConfigPath { get; set; }
        public string Region { get; set; } = "me-dubai-1";
        public string NamespaceName { get; set; } = "axdz0mmf49qx";
    }

    public class CloudFilesService : ICloudFilesService, IDisposable
    {
    
        private readonly OracleCloudFilesOptions cloudFilesOptions;
        private readonly UploadManager uploadManager;
        private readonly ObjectStorageClient client;
        private readonly ILogger<CloudFilesService> logger;

        public CloudFilesService(OracleCloudFilesOptions cloudFilesOptions, ILogger<CloudFilesService> logger)
        {
            this.cloudFilesOptions = cloudFilesOptions;
            this.logger = logger;
            var provider = new ConfigFileAuthenticationDetailsProvider(this.cloudFilesOptions.ConfigPath, "DEFAULT");
            client = new ObjectStorageClient(provider);

            uploadManager = new UploadManager(client, new UploadConfiguration());
        }

        public async Task<RemoteBlob> WriteAsync(Stream inputStream, WriteFileSettings settings)
        {
            var mimeType = settings.ContentType ?? "application/octet-stream";
            var metadata = settings.Metadata != null ? new Dictionary<string, string>(settings.Metadata) : null;
            var request = new PutObjectRequest
            {
                NamespaceName = cloudFilesOptions.NamespaceName,
                ContentType = mimeType,
                BucketName = cloudFilesOptions.BucketName,
                ObjectName = settings.Key,
                OpcMeta = metadata,
                PutObjectBody = inputStream,
            };
            var response = await uploadManager.Upload(new UploadManager.UploadRequest(request) { AllowOverwrite = true });
            return new RemoteBlob
            {
                Location = "",
                MimeType = mimeType
            };
        }

        public async Task<RemoteBlob> WriteTextAsync(string text, WriteFileSettings settings)
        {
            var byteArray = Encoding.UTF8.GetBytes(text);
            using var stream = new MemoryStream(byteArray);

            var mimeType = settings.ContentType ?? "text/plain";
            var metadata = settings.Metadata != null ? new Dictionary<string, string>(settings.Metadata) : null;
            var request = new PutObjectRequest
            {
                ContentType = mimeType,
                BucketName = cloudFilesOptions.BucketName,
                ObjectName = settings.Key,
                OpcMeta = metadata,
                PutObjectBody = stream,
            };
            var response = await uploadManager.Upload(new UploadManager.UploadRequest(request) { AllowOverwrite = true });
            return new RemoteBlob
            {
                Location = "",
                MimeType = mimeType
            };
        }

        public string GetSignedUrl(string key, TimeSpan expiry)
        {
            throw new NotImplementedException();
        }

        public string GetUrl(string key)
        {
            throw new NotImplementedException();
        }

        public WriteWrapper OpenWrite(WriteFileSettings settings)
        {
            throw new NotImplementedException();
        }

        public async Task<Stream> OpenReadAsync(string key)
        {
            var getObjectObjectRequest = new GetObjectRequest()
            {
                BucketName = cloudFilesOptions.BucketName,
                NamespaceName = cloudFilesOptions.NamespaceName,
                ObjectName = key,
            };
            var response = await client.GetObject(getObjectObjectRequest);
            return response.InputStream;
        }

        public async Task<IEnumerable<CloudFileInfo>> ListAsync(string prefix)
        {
            var response = await client.ListObjects(new ListObjectsRequest
            {
                BucketName = cloudFilesOptions.BucketName,
                NamespaceName = cloudFilesOptions.NamespaceName,
                Prefix = prefix,
                Fields = "size"
            });
        
            return response.ListObjects.Objects.Select(x => new CloudFileInfo
            {
                Key = x.Name,
                Size = x.Size ?? 0,
            });
        }

        public async Task<IReadOnlyDictionary<string, string>> GetMetadataAsync(string key)
        {
        
            var headObject = await client.HeadObject(new HeadObjectRequest()
            {
                BucketName = cloudFilesOptions.BucketName,
                NamespaceName = cloudFilesOptions.NamespaceName,
                ObjectName = key
            });
        
            var result = new Dictionary<string, string>(headObject.OpcMeta);
            result.TryAdd("ContentType", headObject.ContentType);
            return result;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                await client.DeleteObject(new DeleteObjectRequest
                {
                    BucketName = cloudFilesOptions.BucketName,
                    NamespaceName = cloudFilesOptions.NamespaceName,
                    ObjectName = key
                });
                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to delete file");
                return false;
            }
        
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}