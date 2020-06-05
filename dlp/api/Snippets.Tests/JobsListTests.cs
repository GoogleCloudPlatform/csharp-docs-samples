// Copyright 2020 Google Inc.
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

using Google.Api.Gax;
using Google.Cloud.Dlp.V2;
using GoogleCloudSamples;
using System.Linq;
using Xunit;

public class JobsListTests : IClassFixture<DlpTestFixture>
{
    private RetryRobot TestRetryRobot { get; } = new RetryRobot();
    private DlpTestFixture Fixture { get; }
    public JobsListTests(DlpTestFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void TestListDlpJobs()
    {
        // Create job.
        DlpServiceClient dlp = DlpServiceClient.Create();
        DlpJob dlpJob = dlp.CreateDlpJob(Fixture.GetTestRiskAnalysisJobRequest());

        TestRetryRobot.ShouldRetry = ex => true;
        TestRetryRobot.Eventually(() =>
        {
            PagedEnumerable<ListDlpJobsResponse, DlpJob> response = JobsList.ListDlpJobs(Fixture.ProjectId, "state=DONE", "RiskAnalysisJob");

            Assert.True(response.Any());
        });
    }
}
