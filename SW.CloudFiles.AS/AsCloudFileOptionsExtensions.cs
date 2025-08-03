using System;
using System.Linq;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SW.PrimitiveTypes;

namespace SW.CloudFiles.AS;

public static class AsCloudFileOptionsExtensions
{
    public static BlobContainerClient CreateClient(this CloudFilesOptions cloudFilesOptions)
    {
        var blobServiceClient = new BlobServiceClient(new Uri(cloudFilesOptions.ServiceUrl),
            new StorageSharedKeyCredential(cloudFilesOptions.AccessKeyId, cloudFilesOptions.SecretAccessKey));

        var containers = blobServiceClient.GetBlobContainers();
        BlobContainerClient blobContainerClient;

        if (containers.All(c => c.Name != cloudFilesOptions.BucketName))
            blobContainerClient =
                blobServiceClient.CreateBlobContainer(cloudFilesOptions.BucketName, PublicAccessType.BlobContainer);
        else
            blobContainerClient = blobServiceClient.GetBlobContainerClient(cloudFilesOptions.BucketName);
        
        return blobContainerClient;
    }
}