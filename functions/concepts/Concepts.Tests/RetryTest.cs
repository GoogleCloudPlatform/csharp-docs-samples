﻿// Copyright 2020 Google LLC
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

using CloudNative.CloudEvents;
using Google.Cloud.Functions.Invoker.Testing;
using Google.Events;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Concepts.Tests
{
    public class RetryTest : FunctionTestBase<Retry.Function>
    {
        // This just makes the tests a little briefer.
        private const string EventType = MessagePublishedData.MessagePublishedCloudEventType;

        public static TheoryData<PubsubMessage> NonRetryMessages = new TheoryData<PubsubMessage>
        {
            // Explicit retry=false
            new PubsubMessage { TextData = "{ \"retry\": false }" },
            // No retry property at all
            new PubsubMessage { TextData = "{ \"not-retry\": 123 }" },
            // No text at all
            new PubsubMessage { Attributes = { { "key", "value" } } }
        };

        [Fact]
        public async Task RetryTrue()
        {
            var cloudEvent = new CloudEvent(MessagePublishedData.MessagePublishedCloudEventType, new Uri("//pubsub.googleapis.com"));
            var data = new MessagePublishedData
            {
                Message = new PubsubMessage { TextData = "{ \"retry\": true }" }
            };
            CloudEventConverters.PopulateCloudEvent(cloudEvent, data);

            // The test server propagates the exception to the caller. The real server would respond
            // with a status code of 500.
            await Assert.ThrowsAsync<InvalidOperationException>(() => ExecuteCloudEventRequestAsync(cloudEvent));
            Assert.Empty(GetFunctionLogEntries());
        }

        [Theory]
        [MemberData(nameof(NonRetryMessages))]
        public async Task NoRetry(PubsubMessage message)
        {
            var cloudEvent = new CloudEvent(MessagePublishedData.MessagePublishedCloudEventType, new Uri("//pubsub.googleapis.com"));
            var data = new MessagePublishedData { Message = message };
            CloudEventConverters.PopulateCloudEvent(cloudEvent, data);

            await ExecuteCloudEventRequestAsync(cloudEvent);
            var logEntry = Assert.Single(GetFunctionLogEntries());
            Assert.Equal(LogLevel.Information, logEntry.Level);
            Assert.Equal("Not retrying...", logEntry.Message);
        }
    }
}
