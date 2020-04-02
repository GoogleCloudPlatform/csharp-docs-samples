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

using Google.Cloud.Spanner.Admin.Database.V1;
using Google.Cloud.Spanner.Common.V1;
using Google.LongRunning;
using log4net;
using System.Linq;
using static GoogleCloudSamples.Spanner.Program;

namespace GoogleCloudSamples.Spanner
{
    public class GetDatabaseOperations
    {
        static readonly ILog s_logger = LogManager.GetLogger(typeof(GetDatabaseOperations));

        // [START spanner_get_database_operations]
        public static object SpannerGetDatabaseOperations(string projectId, string instanceId)
        {
            // Create the DatabaseAdminClient instance.
            DatabaseAdminClient databaseAdminClient = DatabaseAdminClient.Create();

            string parent = InstanceName.Format(projectId, instanceId);
            var filter = "(metadata.@type:type.googleapis.com/google.spanner.admin.database.v1.OptimizeRestoredDatabaseMetadata)";

            // List the optimize restored databases operations on the instance.
            var operations = databaseAdminClient.ListDatabaseOperations(parent, filter).ToList();

            operations.ForEach(operation =>
            {
                OptimizeRestoredDatabaseMetadata metadata =
                    operation.Metadata.Unpack<OptimizeRestoredDatabaseMetadata>();
                s_logger.Info(
                    $"Database {metadata.Name} restored from backup is " +
                    $"{metadata.Progress.ProgressPercent}% optimized");
            });

            return ExitCode.Success;
        }
        // [END spanner_get_database_operations]
    }
}