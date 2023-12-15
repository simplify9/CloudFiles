using System;
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
        public string Region { get; set; } 
        public string NamespaceName { get; set; } 
        internal string GetFileUrl(string key) => $"https://objectstorage.{Region}.oraclecloud.com/n/{NamespaceName}/b/{BucketName}/o/{Uri.EscapeDataString(key)}";
    }
}