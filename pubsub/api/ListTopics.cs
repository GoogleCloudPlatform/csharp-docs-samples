﻿/*
 * Copyright (c) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

namespace PubSubSample
{
    // [START list_topics]
    using Google.Apis.Pubsub.v1;
    using Google.Apis.Pubsub.v1.Data;

    using System.Collections.Generic;

    public class ListTopicsSample
    {
        public void ListTopics(string projectId)
        {
            PubsubService PubSub = PubSubClient.Create();

            ListTopicsResponse response = PubSub.Projects.Topics.List(
              project: $"projects/{projectId}"
            ).Execute();

            if (response.Topics != null)
            {
                IList<Topic> topics = response.Topics;

                foreach (var topic in topics)
                {
                    System.Console.WriteLine($"Found topics: {topic.Name}");
                }
            }
        }
    }
    // [END list_topics]
}