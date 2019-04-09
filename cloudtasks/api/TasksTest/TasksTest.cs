﻿// Copyright 2018 Google Inc.
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

using Google.Apis.Auth.OAuth2;
using System;
using Xunit;

namespace GoogleCloudSamples
{
    public class TasksTestFixture
    {
        public readonly string ProjectId;
        public readonly string QueueId;
        public readonly string LocationId;

        public readonly CommandLineRunner CommandLineRunner = new CommandLineRunner
        {
            VoidMain = Tasks.Main,
        };

        public TasksTestFixture()
        {
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            QueueId = Environment.GetEnvironmentVariable("GCP_QUEUE") ?? "my-appengine-queue";
            LocationId = Environment.GetEnvironmentVariable("LOCATION_ID") ?? "us-central1";
            Url = $"https://{ProjectId}.appspot.com/log_payload";
        }
    }

    public partial class TasksTest : IClassFixture<TasksTestFixture>
    {
        private readonly TasksTestFixture _fixture;
        private string ProjectId { get { return _fixture.ProjectId; } }
        private string QueueId { get { return _fixture.QueueId; } }
        private string LocationId { get { return _fixture.LocationId; } }
        private string Url { get { return _fixture.Url; } }

        private readonly CommandLineRunner _tasks = new CommandLineRunner()
        {
            VoidMain = Tasks.Main,
            Command = "Tasks"
        };

        public TasksTest(TasksTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Dispose()
        {
            // TODO add cleanup logic here
        }

        [Fact]
        public void TestCreateTask()
        {
            ConsoleOutput output = _tasks.Run(
                "createTask",
                "--projectId", ProjectId,
                "--location", LocationId,
                "--queue", QueueId,
                "--payload", "Test Payload"
            );
            Assert.Contains("Created Task", output.Stdout);
        }

        [Fact]
        public void TestCreateHttpTask()
        {
            ConsoleOutput output = _tasks.Run(
                "createHttpTask",
                "--projectId", ProjectId,
                "--location", LocationId,
                "--queue", QueueId,
                "--url", Url,
                "--payload", "Test Payload"
            );
            Assert.Contains("Created Task", output.Stdout);
        }
    }
}
