﻿// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google;
using Xunit;

[Collection(nameof(BucketFixture))]
public class MoveFileTest
{
    private readonly BucketFixture _bucketFixture;

    public MoveFileTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void MoveFile()
    {
        MoveFileSample moveFileSample = new MoveFileSample();
        UploadFileSample uploadFileSample = new UploadFileSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();

        uploadFileSample.UploadFile(_bucketFixture.BucketName, _bucketFixture.FilePath, _bucketFixture.Collect("HelloMove.txt"));
        // Make sure the file doesn't exist until we move it there.
        Assert.Throws<GoogleApiException>(() => getMetadataSample.GetMetadata(_bucketFixture.BucketName, "ByeMove.txt"));

        moveFileSample.MoveFile(_bucketFixture.BucketName, "HelloMove.txt", _bucketFixture.Collect("ByeMove.txt"));

        // If we try to clean up "HelloMove.txt", it will fail because it moved.
        _bucketFixture.TempBucketFiles[_bucketFixture.BucketName].Remove("HelloMove.txt");

        var metadata = getMetadataSample.GetMetadata(_bucketFixture.BucketName, "ByeMove.txt");
        Assert.NotNull(metadata);
    }
}
