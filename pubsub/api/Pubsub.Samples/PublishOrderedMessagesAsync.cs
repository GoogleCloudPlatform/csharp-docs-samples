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

// [START pubsub_quickstart_ordered_publisher]
// [START pubsub_publish]

using Google.Cloud.PubSub.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PublishOrderedMessagesAsyncSample
{
    public async Task<int> PublishOrderedMessagesAsync(string projectId, string topicId, IEnumerable<KeyValuePair<string, string>> messagesAndKeys)
    {
        TopicName topicName = TopicName.FromProjectTopic(projectId, topicId);

        var customSettings = new PublisherClient.Settings
        {
            EnableMessageOrdering = true
        };

        PublisherClient publisher = await PublisherClient.CreateAsync(topicName, settings: customSettings);

        int publishedMessageCount = 0;
        var publishTasks = messagesAndKeys.Select(async messageAndKey =>
        {
            try
            {
                string message = await publisher.PublishAsync(messageAndKey.Value, messageAndKey.Key);
                Console.WriteLine($"Published message {message}");
                Interlocked.Increment(ref publishedMessageCount);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error occurred when publishing message {messageAndKey.Key}: {exception.Message}");
            }
        });
        await Task.WhenAll(publishTasks);
        return publishedMessageCount;
    }
}
// [END pubsub_publish]
// [END pubsub_quickstart_ordered_publisher]