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

using Xunit;

[Collection(nameof(BucketFixture))]
public class UploadFileTest
{
    private readonly BucketFixture _bucketFixture;

    public UploadFileTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void UploadFile()
    {
        UploadFileSample uploadFileSample = new UploadFileSample();
        ListFilesSample listFilesSample = new ListFilesSample();

        // upload file
        uploadFileSample.UploadFile(_bucketFixture.BucketName, _bucketFixture.FilePath, _bucketFixture.Collect("UploadTest.txt"));

        var files = listFilesSample.ListFiles(_bucketFixture.BucketName);
        Assert.Contains(files, c => c.Name == "UploadTest.txt");
    }
}
