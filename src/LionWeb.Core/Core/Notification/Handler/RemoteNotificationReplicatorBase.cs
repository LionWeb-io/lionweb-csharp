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

namespace LionWeb.Core.Notification.Handler;

using Utilities;

/// Replicates <see cref="Receive">received</see> notifications on a <i>local</i> equivalent.
/// 
/// <para>
/// Example: We receive a <see cref="PropertyAddedNotification" /> for a node that we know <i>locally</i>.
/// This class adds the same property value to the <i>locally</i> known node.
/// </para>
public abstract class RemoteNotificationReplicatorBase : NotificationHandlerBase
{
    protected readonly SharedNodeMap SharedNodeMap;
    protected readonly IdFilteringNotificationHandler Filter;

    protected RemoteNotificationReplicatorBase(SharedNodeMap sharedNodeMap,
        IdFilteringNotificationHandler filter,
        object? sender) : base(sender)
    {
        SharedNodeMap = sharedNodeMap;
        Filter = filter;
    }

    /// unsubscribes from all <see cref="ReplicateFrom">replicated publishers</see>.
    // public override void Dispose()
    // {
    //     foreach (var publisher in _publishers)
    //     {
    //         publisher.Unsubscribe<TEvent>(ProcessEvent);
    //     }
    //
    //     GC.SuppressFinalize(this);
    // }
    public override void Receive(INotification message) =>
        ProcessNotification(message);

    protected abstract void ProcessNotification(INotification? notification);

    protected INode Lookup(NodeId nodeId) =>
        (INode)SharedNodeMap[nodeId];

    protected INode? LookupOpt(NodeId nodeId) =>
        SharedNodeMap.TryGetValue(nodeId, out var result) ? (INode?)result : null;

    /// Uses <see cref="IdFilteringNotificationHandler"/> to suppress forwarding notifications raised during executing <paramref name="action"/>. 
    protected virtual void SuppressNotificationForwarding(INotification notification, Action action)
    {
        var notificationId = notification.NotificationId;
        Filter.RegisterNotificationId(notificationId);

        try
        {
            action();
        } finally
        {
            Filter.UnregisterNotificationId(notificationId);
        }
    }
}

public class SameIdCloner : Cloner
{
    public SameIdCloner(IEnumerable<INode> inputNodes) : base(inputNodes)
    {
    }

    /// <inheritdoc cref="Cloner.Clone()"/>
    public static new T Clone<T>(T node) where T : class, INode =>
        (T)new SameIdCloner([node]).Clone()[node];

    protected override NodeId GetNewId(INode remoteNode) =>
        remoteNode.GetId();
}