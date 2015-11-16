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
    // [START create_pubsub_client]

    using Google.Apis.Services;
    using Google.Apis.Pubsub.v1;

    public class PubSubClient
    {
        public static PubsubService Create()
        {
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync().Result;
            credentials = credentials.CreateScoped(new[] { PubsubService.Scope.Pubsub });

            var serviceInitializer = new BaseClientService.Initializer()
            {
                ApplicationName = "PubSub Sample",
                HttpClientInitializer = credentials
            };

            return new PubsubService(serviceInitializer);
        }
    }
    // [END create_pubsub_client]
}