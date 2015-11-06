﻿using System;
using System.Linq;
using System.Collections.Generic;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

// Usage: pubsub.subscriptions.consume
class TestSubscriptionPermissionsSample
{
  public void TestSubscriptionPermissions(string projectId, string subscriptionName, IList<string> permissions)
  {
    PubsubService PubSub = PubSubClient.Create();

    TestIamPermissionsResponse response = PubSub.Projects.Subscriptions.TestIamPermissions(
      resource: $"projects/{projectId}/subscriptions/{subscriptionName}",
      body: new TestIamPermissionsRequest() { Permissions = permissions }
    ).Execute();

    foreach (var permission in permissions)
    {
      if (response.Permissions.Contains(permission))
      {
        Console.WriteLine($"Caller has permission {permission}");
      }
      else
      {
        Console.WriteLine($"Caller does not have persmission {permission}");
      }
    }
  }
}