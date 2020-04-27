// Copyright 2019 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.LongRunning;
using System.Collections.Generic;
using Xunit;

namespace GoogleCloudSamples
{
    [Collection(nameof(AutoMLFixture))]
    public class ListOperationStatusTest
    {
        private readonly AutoMLFixture _fixture;
        private readonly AutoMLListOperationStatus _sample;
        public ListOperationStatusTest(AutoMLFixture fixture)
        {
            _fixture = fixture;
            _sample = new AutoMLListOperationStatus();
        }

        [Fact]
        public void TestListOpetationStatus()
        {
            // Act
            _sample.ListOperationStatus(_fixture.ProjectId);

            Operation firstOper;
            using (IEnumerator<Operation> iter = _sample.ListOperationStatus(_fixture.ProjectId).GetEnumerator())
            {
                iter.MoveNext();
                firstOper = iter.Current;
            }

            // Assert
            Assert.Contains("projects/", firstOper.Name);
        }
    }
}
