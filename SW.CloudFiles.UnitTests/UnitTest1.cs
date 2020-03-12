using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

using System.Threading.Tasks;

namespace SW.CloudFiles.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        static TestServer server;


        [ClassInitialize]
        public static void ClassInitialize(TestContext tcontext)
        {
            server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseDefaultServiceProvider((context, options) => { options.ValidateScopes = true; })
                .UseEnvironment("UnitTesting")
                .UseStartup<TestStartup>());

        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            server.Dispose();
        }

        [TestMethod]
        async public Task TestOpenReadAcync()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            using var stream = await cloudFiles.OpenReadAcync("test/TestWriteAcync.txt");
            using var diskFile = File.OpenWrite(@"c:\temp\sample.txt");
            await stream.CopyToAsync(diskFile);
        }

        
        [TestMethod]
        async public Task TestWriteAcync()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            //using (Stream cloudStream = await cloudFiles.OpenReadAcync("test/sample.txt"))
            using var diskFile = File.OpenRead(@"c:\temp\sample.txt");
            await cloudFiles.WriteAcync(diskFile, new WriteFileSettings
            {
                Key = "test/TestWriteAcync.txt",
                ContentType = "plain/text",
                Public = true
            });
        }



        [TestMethod]
        async public Task TestOpenWriteAsync()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            //using (Stream cloudStream = await cloudFiles.OpenReadAcync("test/sample.txt"))
            using var writeWrapper =  cloudFiles.OpenWrite(new WriteFileSettings
            {
                Key = "test/TestOpenWriteAsync1.txt",
                ContentType = "text/plain"
            });
            using var textWriter = new StreamWriter(writeWrapper.Stream);
            textWriter.Write("hello");
            textWriter.Flush();
            await writeWrapper.CompleteRequestAsync();
        }


        [TestMethod]
        public void TestGetSignedUrl()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            //using (Stream cloudStream = await cloudFiles.OpenReadAcync("test/sample.txt"))
            _ = cloudFiles.GetSignedUrl("test/TestOpenWriteAsync.txt", TimeSpan.FromMinutes(2));
        }

        [TestMethod]
        public void TestGetUnsignedUrl()
        {
            var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

            //using (Stream cloudStream = await cloudFiles.OpenReadAcync("test/sample.txt"))
            var url = cloudFiles.GetUrl("test/TestOpenWriteAsync.txt");
        }




    }
}
