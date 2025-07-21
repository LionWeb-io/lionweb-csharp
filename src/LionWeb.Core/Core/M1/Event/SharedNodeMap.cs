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

using System.Diagnostics.CodeAnalysis;

public class SharedNodeMap : IDisposable
{
    private readonly Dictionary<NodeId, IReadableNode> _map = [];

    public event EventHandler<IReadableNode>? OnAdded;
    public event EventHandler<IReadableNode>? OnRemoved;

    public void RegisterNode(IReadableNode newNode)
    {
        foreach (var node in M1Extensions.Descendants(newNode, true, true))
        {
            if (!TryAdd(node.GetId(), node))
                throw new DuplicateNodeIdException(node, this[node.GetId()]);
        }
    }

    public void UnregisterNode(IReadableNode newNode)
    {
        foreach (var node in M1Extensions.Descendants(newNode, true, true))
        {
            TryRemove(node.GetId());
        }
    }

    public IReadableNode this[NodeId nodeId]
    {
        get => _map[nodeId];
    }

    public IReadableNode? GetValueOrDefault(NodeId nodeId)
        => _map.GetValueOrDefault(nodeId);

    public bool TryGetValue(NodeId targetNode, [NotNullWhen(true)] out IReadableNode? node)
        => _map.TryGetValue(targetNode, out node);

    public IEnumerable<IReadableNode> Values => _map.Values;

    public bool ContainsKey(NodeId nodeId)
        => _map.ContainsKey(nodeId);

    /// <inheritdoc />
    public virtual void Dispose() { }

    private bool TryAdd(NodeId nodeId, IReadableNode node)
    {
        var result = _map.TryAdd(nodeId, node);
        if (result)
            OnAdded?.Invoke(this, node);
        return result;
    }

    private bool TryRemove(NodeId nodeId)
    {
        if (_map.Remove(nodeId, out var node))
        {
            OnRemoved?.Invoke(this, node);
            return true;
        }

        ;
        return false;
    }
}

public class PartitionSharedNodeMap : SharedNodeMap
{
    private readonly Dictionary<NodeId, IPartitionInstance> _owningPartition = [];

    // public event EventHandler<IPartitionInstance>? OnPartitionAdded;
    // public event EventHandler<IPartitionInstance>? OnPartitionRemoved;

    public PartitionSharedNodeMap()
    {
        OnAdded += OnNodeAdded;
        OnRemoved += OnNodeRemoved;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        OnAdded -= OnNodeAdded;
        OnRemoved -= OnNodeRemoved;
        base.Dispose();
    }

    private void OnNodeAdded(object? _, IReadableNode node)
    {
        var partition = node.GetPartition();
        if (partition is null)
            return;

        _owningPartition[node.GetId()] = partition;
        // if(!_owningPartition.ContainsValue(partition))
        //     OnPartitionAdded?.Invoke(this, partition);
    }

    private void OnNodeRemoved(object? _, IReadableNode node)
    {
        _owningPartition.Remove(node.GetId());
        // if (_owningPartition.Remove(node.GetId(), out var removed))
        // OnPartitionRemoved?.Invoke(this, removed);
    }

    public bool TryGetPartition(NodeId nodeId, [NotNullWhen(true)] out IPartitionInstance? partition)
        => _owningPartition.TryGetValue(nodeId, out partition);
}