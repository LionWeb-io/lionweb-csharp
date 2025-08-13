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

namespace LionWeb.Core.Notification;

using M1;
using System.Diagnostics.CodeAnalysis;

/// <see cref="NodeId">NodeId</see> -&gt; <see cref="IReadableNode"/> mapping,
/// shared between all notification handlers in one client or repository.  
public class SharedNodeMap : IDisposable
{
    private readonly Dictionary<NodeId, IReadableNode> _map = [];

    /// Registers <paramref name="newNode"/> (and all its <see cref="M1Extensions.Descendants">descendants</see>) with this map.
    /// <exception cref="DuplicateNodeIdException">If <paramref name="newNode"/>'s node id already has been registered.</exception>
    public void RegisterNode(IReadableNode newNode)
    {
        foreach (var node in M1Extensions.Descendants(newNode, true, true))
        {
            if (!TryAdd(node.GetId(), node))
                throw new DuplicateNodeIdException(node, this[node.GetId()]);
        }
    }

    /// Removes <paramref name="removedNode"/> (and all its <see cref="M1Extensions.Descendants">descendants</see>) from this map.
    public void UnregisterNode(IReadableNode removedNode)
    {
        foreach (var node in M1Extensions.Descendants(removedNode, true, true))
        {
            TryRemove(node.GetId(), out _);
        }
    }

    /// Retrieves the node mapped to <paramref name="nodeId"/>.
    /// <exception cref="InvalidIdException">If <paramref name="nodeId"/> has not been <see cref="RegisterNode">registered</see>.</exception>
    public IReadableNode this[NodeId nodeId]
    {
        get => _map[nodeId] ?? throw new InvalidIdException(nodeId);
    }

    /// Retrieves the node mapped to <paramref name="nodeId"/>, if any.
    public bool TryGetValue(NodeId nodeId, [NotNullWhen(true)] out IReadableNode? node)
        => _map.TryGetValue(nodeId, out node);

    /// Retrieves all mapped nodes.
    public IEnumerable<IReadableNode> Values
        => _map.Values;

    /// Checks whether <paramref name="nodeId"/> has been mapped.
    public bool ContainsKey(NodeId nodeId)
        => _map.ContainsKey(nodeId);

    /// <inheritdoc />
    public virtual void Dispose() { }

    protected virtual bool TryAdd(NodeId nodeId, IReadableNode node) =>
        _map.TryAdd(nodeId, node);

    protected virtual bool TryRemove(NodeId nodeId, [MaybeNullWhen(false)] out IReadableNode node)
    {
        if (_map.Remove(nodeId, out node))
        {
            return true;
        }

        return false;
    }
}

/// <inheritdoc />
/// Also keeps track of the owning <see cref="IPartitionInstance">partition</see> for each <see cref="NodeId">nodeId</see>. 
public class PartitionSharedNodeMap : SharedNodeMap
{
    private readonly Dictionary<NodeId, IPartitionInstance> _owningPartition = [];

    /// <inheritdoc />
    public override void Dispose()
    {
        base.Dispose();
    }

    /// Retrieves the partition owning <paramref name="nodeId"/>, if any.
    public bool TryGetPartition(NodeId nodeId, [NotNullWhen(true)] out IPartitionInstance? partition)
        => _owningPartition.TryGetValue(nodeId, out partition);

    /// <inheritdoc />
    protected override bool TryAdd(NodeId nodeId, IReadableNode node)
    {
        if (!base.TryAdd(nodeId, node))
            return false;

        var partition = node.GetPartition();
        if (partition is not null)
            _owningPartition[node.GetId()] = partition;

        return true;
    }

    /// <inheritdoc />
    protected override bool TryRemove(NodeId nodeId, [MaybeNullWhen(false)] out IReadableNode node)
    {
        if (!base.TryRemove(nodeId, out node))
            return false;

        _owningPartition.Remove(node.GetId());

        return true;
    }
}