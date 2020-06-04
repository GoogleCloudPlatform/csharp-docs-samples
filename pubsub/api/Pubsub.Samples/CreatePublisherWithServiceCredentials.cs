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

using Google.Cloud.PubSub.V1;

public class CreatePublisherWithServiceCredentialsSample
{
    /// <summary>
    /// Creates a PublisherClient given a path to a downloaded json service
    /// credentials file.
    /// </summary>
    /// <param name="jsonPath">The path to the downloaded json file.</param>
    /// <returns>A new publisher client.</returns>
    public PublisherServiceApiClient CreatePublisherWithServiceCredentials(
        string jsonPath)
    {
        PublisherServiceApiClientBuilder builder = new PublisherServiceApiClientBuilder
        {
            CredentialsPath = jsonPath
        };
        return builder.Build();
    }
}