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

namespace LionWeb.Core.M1.Event;

using Partition;
using Utilities;

/// Replicates events received from <see cref="ReplicateFrom">other publishers</see> on a <i>local</i> equivalent.
/// 
/// <para>
/// Example: We receive a <see cref="PropertyAddedNotification"/> for a node that we know <i>locally</i>.
/// This class adds the same property value to the <i>locally</i> known node.
/// </para>
///
/// <para>
/// This class is <i>also</i> a <see cref="IPublisher{TEvent}"/> itself.
/// We <see cref="NotificationIdFilteringNotificationForwarder{TEvent,TPublisher}">forward</see> all events from our <i>local</i>,
/// except the events that stem from replicating <see cref="ReplicateFrom">other publishers</see>.
/// Therefore, two instances with different <i>locals</i> can replicate each other, keeping both <i>locals</i> in sync.
/// </para>
public abstract class NotificationReplicatorBase<TEvent, TPublisher> : NotificationIdFilteringNotificationForwarder<TEvent, TPublisher>
    where TEvent : class, INotification where TPublisher : IPublisher<TEvent>
{
    protected readonly ICommander<TEvent>? _localCommander;
    private readonly List<TPublisher> _publishers = [];

    protected readonly SharedNodeMap NodeById;

    protected NotificationReplicatorBase(TPublisher? localPublisher, ICommander<TEvent>? localCommander,
        SharedNodeMap sharedNodeMap = null) : base(localPublisher)
    {
        _localCommander = localCommander;
        NodeById = sharedNodeMap ?? new();
    }

    /// Replicate events raised by <paramref name="publisher"/>. 
    public virtual void ReplicateFrom(TPublisher publisher)
    {
        publisher.Subscribe<TEvent>(ProcessEvent);
        _publishers.Add(publisher);
    }

    /// unsubscribes from all <see cref="ReplicateFrom">replicated publishers</see>.
    public override void Dispose()
    {
        foreach (var publisher in _publishers)
        {
            publisher.Unsubscribe<TEvent>(ProcessEvent);
        }
        
        GC.SuppressFinalize(this);
    }

    protected abstract void ProcessEvent(object? sender, TEvent @event);

    protected void RegisterNode(IReadableNode newNode)
    {
        NodeById.RegisterNode(newNode);
    }

    protected void UnregisterNode(IReadableNode newNode)
    {
        NodeById.UnregisterNode(newNode);
    }

    protected virtual INode Lookup(NodeId remoteNodeId) =>
        (INode)NodeById[remoteNodeId];

    protected virtual INode? LookupOpt(NodeId remoteNodeId) =>
        (INode?)NodeById.GetValueOrDefault(remoteNodeId);

    protected virtual INode Clone(INode remoteNode) =>
        new SameIdCloner(remoteNode.Descendants(true, true)).Clone()[remoteNode];

    /// Uses <see cref="NotificationIdFilteringNotificationForwarder{TEvent,TPublisher}"/> to suppress forwarding events raised during executing <paramref name="action"/>. 
    protected virtual void SuppressEventForwarding(TEvent @event, Action action)
    {
        INotificationId? notificationId = null;
        if (_localCommander != null)
        {
            notificationId = @event.NotificationId;
            RegisterNotificationId(notificationId);
        }

        try
        {
            action();
        } finally
        {
            if (notificationId != null)
                UnregisterNotificationId(notificationId);
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