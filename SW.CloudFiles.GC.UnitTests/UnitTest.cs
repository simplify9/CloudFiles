using SW.PrimitiveTypes;

namespace SW.CloudFiles.GC.UnitTests;

[TestClass]
public class UnitTest
{
    [TestMethod]
    public async Task TestBasic()
    {
        var options = new GoogleCloudFilesOptions
        {

        };
        var client = options.BuildGoogleCloudStorageClient();
        Assert.IsNotNull(client);
        var cloudFilesService = new CloudFilesService(options);
        Assert.IsNotNull(cloudFilesService);
        var key = $"test{Guid.NewGuid():N}.txt";
        await cloudFilesService.WriteTextAsync("test", new WriteFileSettings
        {
            Key = key,
            ContentType = "text/plain"
        });
    }
}