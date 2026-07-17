// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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
// 
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

using LionWeb.Core.M1;
using LionWeb.Core.Notification;
using LionWeb.Core.Notification.Pipe;
using LionWeb.Core.Utilities;

namespace LionWeb.Core.Test.Notification;

public abstract class NotificationTestsBase
{
    protected T ClonePartition<T>(T node) where T : INode =>
        (T)new SameIdCloner([node]) { IncludingReferences = true }.Clone()[node];

    protected static void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new IdComparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.HasCount(0, differences, differences.DescribeAll(new()));
    }

    protected static void AssertEquals(IEnumerable<IReadableNode?> expected, IEnumerable<IReadableNode?> actual)
    {
        List<IDifference> differences = new IdComparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.HasCount(0, differences, differences.DescribeAll(new()));
    }

    protected static void AssertUniqueNodeIds(params INode[] nodes)
    {
        var nodeIds = new HashSet<NodeId>();
        foreach (INode node in nodes)
        {
            foreach (INode descendant in node.Descendants(true, true))
            {
                var nodeId = descendant.GetId();
                if (!nodeIds.Add(nodeId))
                {
                    Assert.Fail($"Duplicate node id found: {nodeId}");
                }
            }
        }
    }
}

public class NotificationObserver : NotificationPipeBase, INotificationReceiver
{
    public int Count => Notifications.Count;

    public List<INotification> Notifications { get; } = [];

    public void Receive(INotificationSender correspondingSender, INotification notification) =>
        Notifications.Add(notification);
}

/// <summary>
/// Forwards the notification to the following pipe 
/// </summary>
internal class NotificationForwarder : NotificationPipeBase, INotificationProducer
{
    public void ProduceNotification(INotification notification) => Send(notification);
}