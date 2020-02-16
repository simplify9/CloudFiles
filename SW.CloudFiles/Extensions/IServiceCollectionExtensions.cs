
using Microsoft.Extensions.DependencyInjection;
using System;


namespace SW.CloudFiles
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCloudFiles(this IServiceCollection serviceCollection, Action<CloudFilesOptions> configure)
        {
            var cloudFilesOptions = new CloudFilesOptions();
            configure.Invoke(cloudFilesOptions);
            serviceCollection.AddSingleton(cloudFilesOptions);
            serviceCollection.AddTransient<CloudFilesService>();
            //serviceCollection.AddHttpClient();

            return serviceCollection;
        }



    }
}
