﻿using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SW.CloudFiles.S3
{
    internal class ReadWrapper : IDisposable
    {
        private readonly GetObjectResponse getObjectResponse;

        public ReadWrapper(GetObjectResponse getObjectResponse)
        {
            Stream = getObjectResponse.ResponseStream;
            this.getObjectResponse = getObjectResponse;
        }

        public Stream Stream { get; }

        public void Dispose()
        {
            Stream.Dispose();
            getObjectResponse.Dispose();
        }
    }
}
