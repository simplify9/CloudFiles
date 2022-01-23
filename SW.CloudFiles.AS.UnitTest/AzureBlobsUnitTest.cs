using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SW.CloudFiles.AS.UnitTest;

namespace SW.CloudFiles.UnitTests
{
    [TestClass]
    public class AzureBobsUnitTest
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
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            await using var stream =
                await cloudFiles.OpenReadAsync("test/test");

            using var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public async Task TestWriteAsyncFillData()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            var bytes = Convert.FromBase64String("aGVsbG8=");
            var stream = new MemoryStream(bytes);
            var rb = await cloudFiles.WriteAsync(stream, new WriteFileSettings
            {
                Key = "test/test",
                ContentType = "plain/text",
                Public = true
            });

            Assert.IsNotNull(rb.Location);
        }

        [TestMethod]
        public async Task TestWriteAsync()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            var bytes = Convert.FromBase64String("aGVsbG8=");
            var stream = new MemoryStream(bytes);
            var rb = await cloudFiles.WriteAsync(stream, new WriteFileSettings
            {
                Key = "test/TestWriteAsync",
                ContentType = "plain/text",
                Public = true
            });

            Assert.IsNotNull(rb.Location);
        }

        [TestMethod]
        public async Task TestWriteTextAsync()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            var rb = await cloudFiles.WriteTextAsync("test content", new WriteFileSettings
            {
                Key = "test/TestWriteTextAsync",
                ContentType = "text/plain",
                Public = true
            });
            Assert.AreEqual(rb.MimeType, "text/plain");
            Assert.IsNotNull(rb.Location);
        }


        [TestMethod]
        public async Task TestDefaultMimeTypeWriteTextAsync()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            var rb = await cloudFiles.WriteTextAsync("test content", new WriteFileSettings
            {
                Key = "test/TestDefaultMimeTypeWriteTextAsync",
                Public = true
            });
            Assert.AreEqual("text/plain", rb.MimeType);
            Assert.IsNotNull(rb.Location);
        }


        [TestMethod]
        public async Task TestListAsync()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            var result = await cloudFiles.ListAsync("test");
        }


        [TestMethod]
        public void TestGetUnsignedUrl()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            var url = cloudFiles.GetUrl("test/test");
            Assert.IsNotNull(url);
        }

        [TestMethod]
        public async Task TestWriteWithMetadataTextAsync()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            var rb = await cloudFiles.WriteTextAsync("test content", new WriteFileSettings
            {
                Key = "test/metadata",
                ContentType = "text/plain",
                Public = true,
                Metadata = new Dictionary<string, string>
                {
                    ["EntryAssembly"] = "Asm"
                }
            });
            Assert.AreEqual(rb.MimeType, "text/plain");
            Assert.IsNotNull(rb.Location);
        }

        [TestMethod]
        public async Task TestGetMetadata()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            var metadata =
                await cloudFiles.GetMetadataAsync("adapters/infolink5.handlers.http");

            Assert.AreNotEqual(metadata.Count, 0);
            Assert.IsNotNull(metadata["EntryAssembly"]);
            Assert.IsNotNull(metadata["Hash"]);
        }
        // [TestMethod]
        // public void TestGetSignedUrl()
        // {
        //     var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();
        //
        //     //using (Stream cloudStream = await cloudFiles.OpenReadAsync("test/sample.txt"))
        //     _ = cloudFiles.GetSignedUrl("test/TestOpenWriteAsync.txt", TimeSpan.FromMinutes(2));
        // }

        // [TestMethod]
        // async public Task TestOpenWriteAsync()
        // {
        //     var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();
        //
        //     //using (Stream cloudStream = await cloudFiles.OpenReadAsync("test/sample.txt"))
        //     using var writeWrapper = cloudFiles.OpenWrite(new WriteFileSettings
        //     {
        //         Key = "test/TestOpenWriteAsync1.txt",
        //         ContentType = "text/plain"
        //     });
        //     await using var textWriter = new StreamWriter(await writeWrapper.GetStreamAsync());
        //     await textWriter.WriteAsync("hello");
        //     await textWriter.FlushAsync();
        //     var rb = await writeWrapper.CompleteRequestAsync();
        //
        //     Assert.IsNotNull(rb.Location);
        // }
    }
}