using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SW.CloudFiles
{
    public class ReadWrapper : IDisposable
    {
        private readonly object getObjectResponse;

        public ReadWrapper(object getObjectResponse)
        {
            Stream = ((dynamic)getObjectResponse).ResponseStream;
            this.getObjectResponse = getObjectResponse;
        }

        public Stream Stream { get; }

        public void Dispose()
        {
            Stream.Dispose();
            ((dynamic)getObjectResponse).Dispose();
        }
    }
}
