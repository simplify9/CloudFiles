using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SW.CloudFiles.OC.UnitTest
{
    [TestClass]
    public class Tests
    {
        static TestServer server;


        [ClassInitialize]
        public static void ClassInitialize(TestContext tcontext)
        {
            server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseDefaultServiceProvider((context, options) => { options.ValidateScopes = true; })
                .UseEnvironment("Development")
                .UseStartup<TestStartup>());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            server.Dispose();
        }

        [TestMethod]
        public async Task TestOpenReadAsync()
        {
            var z = server.Host.Services.GetService<OracleCloudFilesOptions>();

            var cloudFilesOptions = new OracleCloudFilesOptions
            {
                TenantId = z.TenantId,
                FingerPrint = z.FingerPrint,
                UserId = z.UserId,
                RSAKey = z.RSAKey,
                Region = z.Region,
                BucketName = z.BucketName,
                NamespaceName = z.BucketName,
            };


            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var pemPath = Path.Combine(directory, $"{Guid.NewGuid().ToString("N")}.pem");

            await File.WriteAllTextAsync(pemPath, cloudFilesOptions.RSAKey);


            var configPAth = Path.Combine(directory, $"{Guid.NewGuid().ToString("N")}.config");
            await File.WriteAllTextAsync(configPAth, @$"[DEFAULT]
user={cloudFilesOptions.UserId}
fingerprint={cloudFilesOptions.FingerPrint}
tenancy={cloudFilesOptions.TenantId}
region={cloudFilesOptions.Region}
key_file={pemPath}");
            cloudFilesOptions.ConfigPath = configPAth;
            var cloudFiles = new CloudFilesService(cloudFilesOptions, null);

            var list =
                await cloudFiles.ListAsync("tests");

            Console.WriteLine(string.Join(", ", list));

            foreach (var item in list)
            {
                var metadataAsync =
                    await cloudFiles.GetMetadataAsync(item.Key);
                Assert.IsNotNull(metadataAsync);
                await using var stream =
                    await cloudFiles.OpenReadAsync(item.Key);
                Assert.IsNotNull(stream);
            }
        }
    }
}