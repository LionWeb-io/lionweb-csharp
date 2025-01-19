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

namespace LionWeb.Core.M1;

using M3;
using System.Collections;
using Utilities;
using SemanticPropertyValue = object;

public class PartitionEventApplier
{
    private readonly IPartitionInstance _localPartition;
    private readonly Dictionary<NodeId, IReadableNode> _nodeById;

    public PartitionEventApplier(IPartitionInstance localPartition,
        Dictionary<NodeId, IReadableNode>? sharedNodeMap = null)
    {
        _localPartition = localPartition;
        _nodeById = sharedNodeMap ?? new();
        Init();
    }

    public void Subscribe(IPartitionListener listener)
    {
        listener.PropertyAdded +=
            (sender, args) => OnRemotePropertyAdded(sender, args.Node, args.Property, args.NewValue);
        listener.PropertyChanged += (sender, args) =>
            OnRemotePropertyChanged(sender, args.Node, args.Property, args.NewValue, args.OldValue);

        listener.ChildAdded += (sender, args) =>
            OnRemoteChildAdded(sender, args.Parent, args.NewChild, args.Containment, args.Index);
    }

    private void Init()
    {
        RegisterNode(_localPartition);
        _localPartition.Listener.ChildAdded += (sender, args) =>
            OnLocalChildAdded(sender, args.Parent, args.NewChild, args.Containment, args.Index);
    }

    private void RegisterNode(IReadableNode newNode)
    {
        foreach (var node in M1Extensions.Descendants(newNode, true, true))
        {
            _nodeById[node.GetId()] = node;
        }
    }

    protected virtual INode Lookup(NodeId remoteNodeId) =>
        (INode)_nodeById[remoteNodeId];

    protected virtual INode Clone(INode remoteNode) =>
        new SameIdCloner(remoteNode.Descendants(true, true)).Clone()[remoteNode];

    protected class SameIdCloner : Cloner
    {
        public SameIdCloner(IEnumerable<INode> inputNodes) : base(inputNodes)
        {
        }

        protected override string GetNewId(INode remoteNode) =>
            remoteNode.GetId();
    }

    #region Remote

    #region Properties

    private void OnRemotePropertyAdded(object? sender, IWritableNode node, Property property,
        SemanticPropertyValue newValue) =>
        Lookup(node.GetId()).Set(property, newValue);

    private void OnRemotePropertyChanged(object? sender, IWritableNode node, Property property,
        SemanticPropertyValue newValue, SemanticPropertyValue oldValue) =>
        Lookup(node.GetId()).Set(property, newValue);

    #endregion

    #region Children

    private void OnRemoteChildAdded(object? sender, IWritableNode parent, IWritableNode newChild,
        Containment containment, Index index)
    {
        var localParent = Lookup(parent.GetId());
        var newChildNode = (INode)newChild;

        var clone = Clone(newChildNode);

        object newValue = clone;
        if (containment.Multiple)
        {
            var existingChildren = localParent.Get(containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                children.Insert(index, clone);
                newValue = children;
            }
        }

        localParent.Set(containment, newValue);
    }

    #endregion

    #endregion

    #region Local

    private void OnLocalChildAdded(object? sender, IWritableNode parent, IWritableNode newChild,
        Containment containment, Index index) =>
        RegisterNode(newChild);

    #endregion
}