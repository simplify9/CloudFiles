![Banner](https://i.imgur.com/k6cpqfM.png)

| **Package**       | **Version** |
| :----------------:|:----------------------:|
| ``SimplyWorks.HttpExtensions``|![Nuget](https://img.shields.io/nuget/v/SimplyWorks.CloudFiles?style=for-the-badge)|


[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)  ![Contributions welcome](https://img.shields.io/badge/contributions-welcome-orange.svg)
## Introduction 
*CloudFiles* is a minimalist library that abstracts the [Amazon S3](https://aws.amazon.com/s3/) SDK. It has the core needed from a file-uploading library without the hassle of going through mountains of documentation.

*CloudFiles* has extensions to integrate it into the ASP dotnet core dependency injection. It covers multiple ways to upload data, including opening writable streams or simply uploading a file's data from [IFormFile](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.iformfile?view=aspnetcore-3.1) or similar.

## Installation
There are two  [NuGet](https://www.nuget.org/packages/SimplyWorks.CloudFiles/) packages for Cloudfiles, one for being the actual service, installed with:

`dotnet add package Simplyworks.CloudFiles`

While the other is used to integrate it into the dependency injection, with:

`dotnet add package Simplyworks.CloudFiles.Extensions`

## Getting Started 
To register *Cloudfiles*, use the service collection extension method. Add *Cloudfiles* to your startup file and pass the configuration in one of these two ways:

1. Configure your details in the **AppSettings.json** file and then call **.AddCloudFiles()** in your Startup file. 
Here's how:

```json
 "CloudFiles": {
    "AccessKeyId": "",
    "SecretAccessKey": "",
    "BucketName": "",
    "ServiceUrl": ""
  }, 
  ```
  2. Use the **AddCloudFiles** function in your Startup file and specify your parameters like so:

   ```csharp
   .AddCloudFiles( config =>
            config.AccessKeyId = ""
            config.SecretAccessKey = "";
            config.ServiceUrl = "";
            config.BucketName = "";
    ) 
```

Then simply add the *ICoudFilesService* interface (from [PrimitiveTypes](https://github.com/simplify9/primitivetypes)) in the constructor of a controller for it to be injected, then use the functions provided!

## Examples

### Reading from Cloud bucket example:

We initialize this function with its corresponding primitive type, and it then reads a file from the bucket and writes it onto the local disk. 

``` C#
async public Task TestOpenReadAcync()
{
    var cloudFiles = server.Host.Services.GetService<ICloudFilesService>();

    using var stream = await cloudFiles.OpenReadAsync("test/TestWriteAcync.txt");
    using var diskFile = File.OpenWrite(@"c:\temp\sample.txt");
    await stream.CopyToAsync(diskFile);
}
```
### CloudFiles used in an ASP Controller endpoint:

```C#
[HttpPost]
[Route("{**directory}")]
public async Task<IActionResult> UploadBlobToCloud([FromRoute]string directory, [FromForm]IFormFile file)
{
        string directoryPath = directory.EndsWith('/') ? directory : directory + '/';
        if(_contentTypeProvider.TryGetContentType(file.FileName, out string mimeType))
        {
                var blob = await cloudFilesService.WriteAsync(file.OpenReadStream(), new PrimitiveTypes.WriteFileSettings
                {
                        ContentType = mimeType,
                        Key = directoryPath +  file.FileName,
                        CloseInputStream = false,
                        Public = true,
                        Metadata = new Dictionary<string, string>()
                });
                return Ok(blob.Location);
        } throw new Exception("Invalid form file");
}
```

## Getting support 👷
If you encounter any bugs, don't hesitate to submit an [issue](https://github.com/simplify9/CloudFiles/issues). We'll get back to you promptly! 







