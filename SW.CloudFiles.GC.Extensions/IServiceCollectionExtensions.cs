using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Google.Api.Gax;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SW.CloudFiles.GC;
using SW.PrimitiveTypes;

namespace SW.CloudFiles.Extensions;

internal class GoogleJsonCredentialsModel
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

public static class ServiceCollectionExtensions
{
    public static StorageClient BuildGoogleCloudStorageClient(this GoogleCloudFilesOptions options)
    {
        var model = new GoogleJsonCredentialsModel()
        {
            type = "service_account",
            project_id = options.ProjectId,
            private_key_id = options.PrivateKeyId,
            private_key = options.PrivateKey,
            client_email = options.ClientEmail,
            client_id = options.ClientId,
            auth_uri = options.AuthUri,
            token_uri = options.TokenUri,
            auth_provider_x509_cert_url = options.AuthProviderX509CertUrl,
            client_x509_cert_url = options.ClientX509CertUrl,
            universe_domain = options.UniverseDomain
        };
        
        var json= JsonSerializer.Serialize(model);
        var builder = new StorageClientBuilder
        {
            JsonCredentials = json
        };
        return builder.Build();
    }
    public static IServiceCollection AddOracleCloudFiles(this IServiceCollection serviceCollection,
        Action<GoogleCloudFilesOptions> configure = null)
    {
        var cloudFilesOptions = new GoogleCloudFilesOptions();
        if (configure != null) configure.Invoke(cloudFilesOptions);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        serviceProvider.GetRequiredService<IConfiguration>().GetSection(CloudFilesOptions.ConfigurationSection)
            .Bind(cloudFilesOptions);

        var client = cloudFilesOptions.BuildGoogleCloudStorageClient();
        serviceCollection.AddScoped<ICloudFilesService, CloudFilesService>();
        serviceCollection.AddSingleton(cloudFilesOptions);
        serviceCollection.AddSingleton((CloudFilesOptions)cloudFilesOptions);
        serviceCollection.AddSingleton(client);
        return serviceCollection;
    }
}


