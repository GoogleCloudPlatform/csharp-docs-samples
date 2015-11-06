﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace PubSubSample
{

  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine(@"Usage: PubSubSample.exe [command] [args]

       ListTopics
       ListSubscriptions
       CreateTopic            [name]
       CreateSubscription     [topic] [name]
       PublishMessage         [topic] [message]
       Pull                   [subscription]
       GetTopicPolicy         [topic]
       GetSubscriptionPolicy  [subscription]
       SetTopicPolicy         [topic]        [role=member,member...] [role=...]
       SetSubscriptionPolicy  [subscription] [role=member,member...] [role=...]
       TestTopicPolicy        [topic]        [permission] [permission...]
       TestSubscriptionPolicy [subscription] [permission] [permission...]
");
        Environment.Exit(0);
      }

      var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

      var commandName = args.FirstOrDefault();
      var commandArguments = args.Skip(1).ToArray();

      switch (commandName)
      {
        case "ListTopics":
          new ListTopicsSample().ListTopics(projectId);
          break;

        case "ListSubscriptions":
          new ListSubscriptionsSample().ListSubscriptions(projectId);
          break;

        case "CreateTopic":
          new CreateTopicSample().CreateTopic(projectId,
            topicName: commandArguments[0]
          );
          break;

        case "CreateSubscription":
          new CreateSubscriptionSample().CreateSubscription(projectId,
            topicName: commandArguments[0],
            subscriptionName: commandArguments[1]
          );
          break;

        case "PublishMessage":
          new PublishMessageSample().PublishMessage(projectId,
            topicName: commandArguments[0],
            message: commandArguments[1]
          );
          break;

        case "Pull":
          new PullSample().Pull(projectId, subscriptionName: commandArguments[0]);
          break;

        case "GetTopicPolicy":
          new GetTopicPolicySample().GetTopicPolicy(projectId, topicName: commandArguments[0]);
          break;

        case "GetSubscriptionPolicy":
          new GetSubscriptionPolicySample().GetSubscriptionPolicy(projectId, subscriptionName: commandArguments[0]);
          break;

        case "SetTopicPolicy":
          var topicName = commandArguments[0];
          var topicPolicyArguments = commandArguments.Skip(1);

          var topicRolesAndMembers = new Dictionary<string, string[]>();

          foreach (var arg in topicPolicyArguments)
          {
            var roleName = arg.Split('"')[0];
            var memberNames = arg.Split('"')[1].Split(',');
            topicRolesAndMembers[roleName] = memberNames;
          }

          new SetTopicPolicySample().SetTopicPolicy(projectId,
            topicName: topicName,
            rolesAndMembers: topicRolesAndMembers
          );
          break;

        case "SetSubscriptionPolicy":
          var subscriptionName = commandArguments[0];
          var subscriptionPolicyArguments = commandArguments.Skip(1);

          var subscriptionRolesAndMembers = new Dictionary<string, string[]>();

          foreach (var arg in subscriptionPolicyArguments)
          {
            var roleName = arg.Split('"')[0];
            var memberNames = arg.Split('"')[1].Split(',');
            subscriptionRolesAndMembers[roleName] = memberNames;
          }

          new SetSubscriptionPolicySample().SetSubscriptionPolicy(projectId,
            subscriptionName: subscriptionName,
            rolesAndMembers: subscriptionRolesAndMembers
          );
          break;

        case "TestTopicPolicy":
          new TestTopicPermissionsSample().TestTopicPermissions(projectId,
            topicName: commandArguments[0],
            permissions: commandArguments.Skip(1).ToList()
          );
          break;

        case "TestSubscriptionPolicy":
          new TestSubscriptionPermissionsSample().TestSubscriptionPermissions(projectId,
            subscriptionName: commandArguments[0],
            permissions: commandArguments.Skip(1).ToList()
          );
          break;

        default:
          Console.WriteLine($"Command not found: {commandName}");
          break;
      }
    }
  }
}
