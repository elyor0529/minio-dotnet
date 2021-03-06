﻿/*
 * MinIO .NET Library for Amazon S3 Compatible Cloud Storage,
 * (C) 2017, 2018, 2019, 2020 MinIO, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Minio.Exceptions;

namespace Minio.Tests
{
    [TestClass]
    public class NegativeTest
    {
        [TestMethod]
        public async Task TestNoConnectionError()
        {
            // invalid uri
            var minio = new MinioClient()
                                .WithEndpoint("localhost", 12121)
                                .WithCredentials("minio", "minio")
                                .Build();

            var ex = await Assert.ThrowsExceptionAsync<ConnectionException>(() => minio.BucketExistsAsync(new BucketExistsArgs("test")));
            Assert.IsNotNull(ex.ServerResponse);
        }

        [TestMethod]
        public async Task TestInvalidBucketNameError()
        {
            var badName = new string('A', 260);
            var minio = new MinioClient()
                                .WithEndpoint("play.min.io")
                                .WithCredentials("Q3AM3UQ867SPQQA43P2F", "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG")
                                .Build();
            await Assert.ThrowsExceptionAsync<InvalidBucketNameException>(() => minio.BucketExistsAsync(new BucketExistsArgs(badName)));
        }

        [TestMethod]
        public async Task TestInvalidObjectNameError()
        {
            var badName = new string('A', 260);
            var bucketName = Guid.NewGuid().ToString("N");
            var minio = new MinioClient()
                                .WithEndpoint("play.min.io")
                                .WithCredentials("Q3AM3UQ867SPQQA43P2F", "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG")
                                .Build();

            try
            {
                await minio.MakeBucketAsync(new MakeBucketArgs(bucketName));

                var ex = await Assert.ThrowsExceptionAsync<InvalidObjectNameException>(
                    () => minio.StatObjectAsync(bucketName, badName));
                Assert.AreEqual(ex.Response.Code, "InvalidObjectName");

                ex = await Assert.ThrowsExceptionAsync<InvalidObjectNameException>(
                    () => minio.GetObjectAsync(bucketName, badName, s => { }));
                Assert.AreEqual(ex.Response.Code, "InvalidObjectName");
            }
            finally
            {
                await minio.RemoveBucketAsync(new RemoveBucketArgs(bucketName));
            }
        }
    }
}
