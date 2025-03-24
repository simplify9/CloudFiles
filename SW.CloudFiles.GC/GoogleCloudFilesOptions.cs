using Google.Cloud.Storage.V1;
using SW.PrimitiveTypes;

namespace SW.CloudFiles.GC;

public class GoogleCloudFilesOptions: CloudFilesOptions
{
    public string ProjectId { get; set; }
    public string PrivateKeyId { get; set; }
    public string PrivateKey { get; set; }
    public string ClientEmail { get; set; }
    public string ClientId { get; set; }
    public string AuthUri { get; set; } = "https://accounts.google.com/o/oauth2/auth";
    public string TokenUri { get; set; } = "https://oauth2.googleapis.com/token";
    public string AuthProviderX509CertUrl { get; set; } ="https://www.googleapis.com/oauth2/v1/certs";
    public string ClientX509CertUrl { get; set; }
    public string UniverseDomain { get; set; } = "googleapis.com";
}