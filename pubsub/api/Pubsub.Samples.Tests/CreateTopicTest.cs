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

[Collection(nameof(PubsubFixture))]
public class CreateTopicTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly GetTopicSample _getTopicSample;
    public CreateTopicTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _getTopicSample = new GetTopicSample();
    }
    [Fact]
    public void CreateTopic()
    {
        string topicId = "testTopicForTopicCreation" + _pubsubFixture.RandomName();
        var newlyCreatedTopic = _pubsubFixture.CreateTopic(topicId);
        var topic = _getTopicSample.GetTopic(_pubsubFixture.ProjectId, topicId);
        Assert.Equal(newlyCreatedTopic, topic);
    }
}