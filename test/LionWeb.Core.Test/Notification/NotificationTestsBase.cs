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

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Pipe;
using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;

public abstract class NotificationTestsBase
{
    protected T ClonePartition<T>(T node) where T : IPartitionInstance, INode =>
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

internal class NotificationForwarder() : NotificationPipeBase(null), INotificationProducer
{
    public void ProduceNotification(INotification notification) => Send(notification);
}

public interface IReplicatorCreator
{
    Geometry CreateReplicator(Geometry node);
}

public class NotificationObserver() : NotificationPipeBase(null), INotificationReceiver
{
    public int Count => Notifications.Count;

    public List<INotification> Notifications { get; } = [];

    public void Receive(INotificationSender correspondingSender, INotification notification) =>
        Notifications.Add(notification);
}

[Obsolete("Will be removed after tests are refactored with proper ConnectTo() calls")]
public static class NotificationExtensions
{
    // TODO: Replace tests by proper ConnectTo() calls
    public static void Subscribe<T>(this INotificationSender sender, Action<object?, T> handler) =>
        sender.ConnectTo(new FilteredReceiver<T>(handler));
}

[Obsolete("Will be removed after tests are refactored with proper ConnectTo() calls")]
internal class FilteredReceiver<T>(Action<object?, T> handler) : INotificationReceiver
{
    public bool Handles(params Type[] notificationTypes) =>
        notificationTypes.Any(t => typeof(T).IsAssignableFrom(t));

    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        if (notification is T t)
            handler.Invoke(correspondingSender, t);
    }
}