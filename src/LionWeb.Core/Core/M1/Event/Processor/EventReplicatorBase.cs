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

namespace LionWeb.Core.M1.Event.Processor;

using Partition;
using Utilities;

/// Replicates <see cref="Receive">received</see> events on a <i>local</i> equivalent.
/// 
/// <para>
/// Example: We receive a <see cref="PropertyAddedEvent"/> for a node that we know <i>locally</i>.
/// This class adds the same property value to the <i>locally</i> known node.
/// </para>
public abstract class EventReplicatorBase<TEvent> : EventProcessorBase<TEvent> where TEvent : class, IEvent
{
    protected readonly SharedNodeMap SharedNodeMap;
    protected readonly EventIdFilteringEventProcessor<TEvent> Filter;

    protected EventReplicatorBase(SharedNodeMap sharedNodeMap, EventIdFilteringEventProcessor<TEvent> filter,
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
    public override void Receive(TEvent message) =>
        ProcessEvent(message);

    protected abstract void ProcessEvent(TEvent? @event);

    protected INode Lookup(NodeId nodeId) =>
        (INode)SharedNodeMap[nodeId];

    protected INode? LookupOpt(NodeId nodeId) =>
        SharedNodeMap.TryGetValue(nodeId, out var result) ? (INode?)result : null;

    protected virtual INode AdjustRemoteNode(INode remoteNode) =>
        remoteNode;

    /// Uses <see cref="EventIdFilteringEventProcessor{TEvent}"/> to suppress forwarding events raised during executing <paramref name="action"/>. 
    protected virtual void SuppressEventForwarding(TEvent @event, Action action)
    {
        IEventId eventId = @event.EventId;
        Filter.RegisterEventId(eventId);

        try
        {
            action();
        } finally
        {
            Filter.UnregisterEventId(eventId);
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