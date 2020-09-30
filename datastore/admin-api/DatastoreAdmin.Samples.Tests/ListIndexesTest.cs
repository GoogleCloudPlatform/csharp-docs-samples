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

using System.Linq;
using Xunit;

[Collection(nameof(DatastoreAdminFixture))]
public class ListIndexesTest
{
    private readonly DatastoreAdminFixture _datastoreAdminFixture;

    public ListIndexesTest(DatastoreAdminFixture datastoreAdminFixture)
    {
        _datastoreAdminFixture = datastoreAdminFixture;
    }

    [Fact]
    public void TestListIndexes()
    {
        // Currently, we don't have any API to create index.
        // It Just verify that ListIndexes does not throw any Exception.
        ListIndexesSample listIndexesSample = new ListIndexesSample();
        var exception = Record.Exception(() => listIndexesSample.ListIndexes(_datastoreAdminFixture.ProjectId).ToList());
        Assert.Null(exception);
    }
}
