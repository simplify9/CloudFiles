﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SW.PrimitiveTypes;

namespace SW.CloudFiles.AS
{
    public class CloudFilesService : ICloudFilesService
    {
        private readonly BlobContainerClient blobContainerClient;

        public CloudFilesService(BlobContainerClient blobContainerClient)
        {
            this.blobContainerClient = blobContainerClient;
        }

        public async Task<RemoteBlob> WriteAsync(Stream inputStream, WriteFileSettings settings)
        {
            var blobClient = blobContainerClient.GetBlobClient(settings.Key);


            await blobClient.UploadAsync(inputStream, new BlobUploadOptions
            {
                Metadata = settings.Metadata ?? new Dictionary<string, string>(),
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = settings.ContentType ??
                                  "application/octet-stream"
                }
            });

            return new RemoteBlob
            {
                Location = $"{blobContainerClient.Uri}/{settings.Key}",
                Name = settings.Key,
                MimeType = settings.ContentType,
                Size = (int) inputStream.Length
            };
        }

        public async Task<RemoteBlob> WriteTextAsync(string text, WriteFileSettings settings)
        {
            var blobClient = blobContainerClient.GetBlobClient(settings.Key);


            var content = Encoding.UTF8.GetBytes(text);
            await using var ms = new MemoryStream(content);
            var contentType = settings.ContentType ?? "text/plain";
            await blobClient.UploadAsync(ms, new BlobHttpHeaders
            {
                ContentType = contentType
            }, settings.Metadata);

            return new RemoteBlob
            {
                Location = $"{blobContainerClient.Uri}/{settings.Key}",
                Name = settings.Key,
                MimeType = contentType
            };
        }

        public string GetSignedUrl(string key, TimeSpan expiry)
        {
            throw new NotImplementedException();
        }

        public string GetUrl(string key) => $"{blobContainerClient.Uri}/{key}";

        public WriteWrapper OpenWrite(WriteFileSettings settings)
        {
            throw new NotImplementedException();
        }

        public async Task<Stream> OpenReadAsync(string key)
        {
            var blob = blobContainerClient.GetBlobClient(key);

            var result = await blob.DownloadAsync();

            return result.Value.Content;
        }

        public async Task<IEnumerable<CloudFileInfo>> ListAsync(string prefix)
        {
            var blobHierarchyItems =
                blobContainerClient.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, null, $"{prefix}");

            var files = new List<CloudFileInfo>();

            await foreach (var blobHierarchyItem in blobHierarchyItems)
            {
                if (!blobHierarchyItem.IsPrefix)
                    files.Add(new CloudFileInfo
                    {
                        Key = blobHierarchyItem.Blob.Name,
                        Signature = blobHierarchyItem.Blob.Properties.ETag?.ToString(),
                        Size = blobHierarchyItem.Blob.Properties.ContentLength ?? 0
                    });
            }

            return files;
        }

        public async Task<IReadOnlyDictionary<string, string>> GetMetadataAsync(string key)
        {
            var blob = blobContainerClient.GetBlobClient(key);
            var properties = await blob.GetPropertiesAsync();
            return new ReadOnlyDictionary<string, string>(properties.Value.Metadata);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var blob = blobContainerClient.GetBlobClient(key);
            await blob.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
            return true;
        }
    }
}