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
        listener.PropertyAdded += (sender, args) =>
            OnRemotePropertyAdded(sender, args.Node, args.Property, args.NewValue);
        listener.PropertyDeleted += (sender, args) =>
            OnRemotePropertyDeleted(sender, args.Node, args.Property, args.OldValue);
        listener.PropertyChanged += (sender, args) =>
            OnRemotePropertyChanged(sender, args.Node, args.Property, args.NewValue, args.OldValue);

        listener.ChildAdded += (sender, args) =>
            OnRemoteChildAdded(sender, args.Parent, args.NewChild, args.Containment, args.Index);
        listener.ChildDeleted += (sender, args) =>
            OnRemoteChildDeleted(sender, args.DeletedChild, args.Parent, args.Containment, args.Index);
        listener.ChildMovedFromOtherContainment += (sender, args) =>
            OnRemoteChildMovedFromOtherContainment(sender, args.NewParent, args.NewContainment, args.NewIndex,
                args.MovedChild, args.OldParent, args.OldContainment, args.OldIndex);
        listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) =>
            OnRemoteChildMovedFromOtherContainmentInSameParent(sender, args.NewContainment, args.NewIndex,
                args.MovedChild, args.Parent, args.OldContainment, args.OldIndex);
        listener.ChildMovedInSameContainment += (sender, args) =>
            OnRemoteChildMovedInSameContainment(sender, args.NewIndex, args.MovedChild, args.Parent, args.Containment,
                args.OldIndex);
    }

    private void Init()
    {
        RegisterNode(_localPartition);

        var listener = _localPartition.Listener;
        if (listener == null)
            return;

        listener.ChildAdded += (sender, args) =>
            OnLocalChildAdded(sender, args.Parent, args.NewChild, args.Containment, args.Index);
        listener.ChildDeleted += (sender, args) =>
            OnLocalChildDeleted(sender, args.DeletedChild, args.Parent, args.Containment, args.Index);
    }

    private void RegisterNode(IReadableNode newNode)
    {
        foreach (var node in M1Extensions.Descendants(newNode, true, true))
        {
            _nodeById[node.GetId()] = node;
        }
    }

    private void UnregisterNode(IReadableNode newNode)
    {
        foreach (var node in M1Extensions.Descendants(newNode, true, true))
        {
            _nodeById.Remove(node.GetId());
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

    private void OnRemotePropertyDeleted(object? sender, IWritableNode node, Property property,
        SemanticPropertyValue oldValue) =>
        Lookup(node.GetId()).Set(property, null);

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

        var newValue = Insert(localParent, containment, index, clone);

        localParent.Set(containment, newValue);
    }

    private static object Insert(INode localParent, Containment containment, Index index, INode nodeToInsert)
    {
        object newValue = nodeToInsert;
        if (containment.Multiple)
        {
            if (localParent.CollectAllSetFeatures().Contains(containment))
            {
                var existingChildren = localParent.Get(containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    children.Insert(index, nodeToInsert);
                    newValue = children;
                }
            } else
            {
                newValue = new List<IWritableNode>() { nodeToInsert };
            }
        }

        return newValue;
    }

    private void OnRemoteChildDeleted(object? sender, IWritableNode deletedChild, IWritableNode parent,
        Containment containment, Index index)
    {
        var localParent = Lookup(parent.GetId());

        object newValue = null;
        if (containment.Multiple)
        {
            var existingChildren = localParent.Get(containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                children.RemoveAt(index);
                newValue = children;
            }
        }

        localParent.Set(containment, newValue);
    }

    private void OnRemoteChildMovedFromOtherContainment(object? sender, IWritableNode newParent,
        Containment newContainment,
        Index newIndex,
        IWritableNode movedChild,
        IWritableNode oldParent,
        Containment oldContainment,
        Index oldIndex)
    {
        var localNewParent = Lookup(newParent.GetId());
        var newValue = Insert(localNewParent, newContainment, newIndex, Lookup(movedChild.GetId()));

        localNewParent.Set(newContainment, newValue);
    }

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(object? sender,
        Containment newContainment,
        Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent,
        Containment oldContainment,
        Index oldIndex)
    {
        var localParent = Lookup(parent.GetId());
        var newValue = Insert(localParent, newContainment, newIndex, Lookup(movedChild.GetId()));

        localParent.Set(newContainment, newValue);
    }

    private void OnRemoteChildMovedInSameContainment(object? sender,
        Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent,
        Containment containment,
        Index oldIndex)
    {
        var localParent = Lookup(parent.GetId());
        INode nodeToInsert = Lookup(movedChild.GetId());
        object newValue = nodeToInsert;
        var existingChildren = localParent.Get(containment);
        if (existingChildren is IList l)
        {
            var children = new List<IWritableNode>(l.Cast<IWritableNode>());
            children.RemoveAt(oldIndex);
            children.Insert(newIndex, nodeToInsert);
            newValue = children;
        }

        localParent.Set(containment, newValue);
    }

    #endregion

    #endregion

    #region Local

    private void OnLocalChildAdded(object? sender, IWritableNode parent, IWritableNode newChild,
        Containment containment, Index index) =>
        RegisterNode(newChild);

    private void OnLocalChildDeleted(object? sender, IWritableNode deletedChild, IWritableNode parent,
        Containment containment, Index index) =>
        UnregisterNode(deletedChild);

    #endregion
}