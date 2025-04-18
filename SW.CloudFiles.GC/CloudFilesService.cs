using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using SW.PrimitiveTypes;

namespace SW.CloudFiles.GC;
public class Root
{
    public string type { get; set; }
    public string project_id { get; set; }
    public string private_key_id { get; set; }
    public string private_key { get; set; }
    public string client_email { get; set; }
    public string client_id { get; set; }
    public string auth_uri { get; set; }
    public string token_uri { get; set; }
    public string auth_provider_x509_cert_url { get; set; }
    public string client_x509_cert_url { get; set; }
    public string universe_domain { get; set; }
}
public class CloudFilesService(StorageClient storageClient, GoogleCloudFilesOptions options) : ICloudFilesService
{
    
    // Create the StorageClient with optional credentials

    public async Task<RemoteBlob> WriteAsync(Stream inputStream, WriteFileSettings settings)
    {
        
        
        var obj= await storageClient.UploadObjectAsync(options.BucketName, settings.Key, settings.ContentType, inputStream);
        
        return new RemoteBlob
        {
            Location = settings.Public ? GetUrl(settings.Key) : GetSignedUrl(settings.Key, TimeSpan.FromHours(1)),
            MimeType = settings.ContentType,
            Name = settings.Key,
            Size = (int)(obj.Size ?? 0)
        };
    }

    public async Task<RemoteBlob> WriteTextAsync(string text, WriteFileSettings settings)
    {
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        return await WriteAsync(stream, settings);
    }

    public string GetSignedUrl(string key, TimeSpan expiry)
    {
        var signer = UrlSigner.FromServiceAccountPath("path/to/your-service-account.json"); // Update path
        return signer.Sign(options.BucketName, key, expiry);
    }

    public string GetUrl(string key)
    {
        return $"https://storage.googleapis.com/{options.BucketName}/{key}";
    }

    public WriteWrapper OpenWrite(WriteFileSettings settings)
    {
        var url = GetSignedUrl(settings.Key, TimeSpan.FromHours(1)); // Generate a signed URL for writing

        var request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "PUT";
        request.ContentType = settings.ContentType;

        foreach (var metadata in settings.Metadata)
        {
            request.Headers[metadata.Key] = metadata.Value;
        }

        return new WriteWrapper(request, this, settings);
    }

    public async Task<Stream> OpenReadAsync(string key)
    {
        var stream = new MemoryStream();
        await storageClient.DownloadObjectAsync(options.BucketName, key, stream);
        stream.Position = 0;
        return stream;
    }

    public async Task<IEnumerable<CloudFileInfo>> ListAsync(string prefix)
    {
        var fileList = new List<CloudFileInfo>();

        await foreach (var obj in storageClient.ListObjectsAsync(options.BucketName, prefix))
        {
            var size = obj.Size ?? 0;
            fileList.Add(new CloudFileInfo
            {
                Key = obj.Name,
                Size = (long)size,
                Signature = obj.Md5Hash != null ? Convert.ToBase64String(Convert.FromBase64String(obj.Md5Hash)) : string.Empty
            });
        }

        return fileList;
    }


    public async Task<IReadOnlyDictionary<string, string>> GetMetadataAsync(string key)
    {
        var obj = await storageClient.GetObjectAsync(options.BucketName, key);
        return new ReadOnlyDictionary<string,string>(obj.Metadata ?? new Dictionary<string,string>());
        
    }

    public async Task<bool> DeleteAsync(string key)
    {
        try
        {
            await storageClient.DeleteObjectAsync(options.BucketName, key);
            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}