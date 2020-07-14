﻿// Copyright (c) 2020 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START bigtable_reads_row_ranges]

using Google.Cloud.Bigtable.Common.V2;
using Google.Cloud.Bigtable.V2;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ReadRowRangesSample
{
    public async Task<List<Row>> ReadRowRangesAsync(string projectId, string instanceId, string tableId)
    {
        BigtableClient bigtableClient = BigtableClient.Create();
        TableName tableName = new TableName(projectId, instanceId, tableId);
        RowSet rowSet = RowSet.FromRowRanges(RowRange.ClosedOpen("phone#4c410523#20190501", "phone#4c410523#20190601"),
        RowRange.ClosedOpen("phone#5c10102#20190501", "phone#5c10102#20190601"));
        ReadRowsStream readRowsStream = bigtableClient.ReadRows(tableName, rowSet);

        var enumerator = readRowsStream.GetAsyncEnumerator(default);

        var result = new List<Row>();
        while (await enumerator.MoveNextAsync())
        {
            var row = enumerator.Current;
            result.Add(row);
        }
        await enumerator.DisposeAsync();
        return result;
    }
}
// [END bigtable_reads_row_ranges]
