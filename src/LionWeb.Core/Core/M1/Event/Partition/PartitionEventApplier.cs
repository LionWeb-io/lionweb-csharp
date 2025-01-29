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

namespace LionWeb.Core.M1.Event.Partition;

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
            PauseCommands(() => OnRemotePropertyAdded(sender, args.Node, args.Property, args.NewValue));
        listener.PropertyDeleted += (sender, args) =>
            PauseCommands(() => OnRemotePropertyDeleted(sender, args.Node, args.Property, args.OldValue));
        listener.PropertyChanged += (sender, args) =>
            PauseCommands(() => OnRemotePropertyChanged(sender, args.Node, args.Property, args.NewValue, args.OldValue));

        listener.ChildAdded += (sender, args) =>
            PauseCommands(() => OnRemoteChildAdded(sender, args.Parent, args.NewChild, args.Containment, args.Index));
        listener.ChildDeleted += (sender, args) =>
            PauseCommands(() => OnRemoteChildDeleted(sender, args.DeletedChild, args.Parent, args.Containment, args.Index));
        listener.ChildReplaced += (sender, args) =>
            PauseCommands(() => OnRemoteChildReplaced(sender, args.NewChild, args.ReplacedChild, args.Parent, args.Containment, args.Index));
        listener.ChildMovedFromOtherContainment += (sender, args) =>
            PauseCommands(() => OnRemoteChildMovedFromOtherContainment(sender, args.NewParent, args.NewContainment, args.NewIndex,
                args.MovedChild, args.OldParent, args.OldContainment, args.OldIndex));
        listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) =>
            PauseCommands(() => OnRemoteChildMovedFromOtherContainmentInSameParent(sender, args.NewContainment, args.NewIndex,
                args.MovedChild, args.Parent, args.OldContainment, args.OldIndex));
        listener.ChildMovedInSameContainment += (sender, args) =>
            PauseCommands(() => OnRemoteChildMovedInSameContainment(sender, args.NewIndex, args.MovedChild, args.Parent, args.Containment,
                args.OldIndex));

        listener.AnnotationAdded += (sender, args) =>
            PauseCommands(() => OnRemoteAnnotationAdded(sender, args.Parent, args.NewAnnotation, args.Index));
        listener.AnnotationDeleted += (sender, args) =>
            PauseCommands(() => OnRemoteAnnotationDeleted(sender, args.DeletedAnnotation, args.Parent, args.Index));
        listener.AnnotationMovedFromOtherParent += (sender, args) =>
            PauseCommands(() => OnRemoteAnnotationMovedFromOtherParent(sender, args.NewParent, args.NewIndex, args.MovedAnnotation,
                args.OldParent, args.OldIndex));
        listener.AnnotationMovedInSameParent += (sender, args) =>
            PauseCommands(() => OnRemoteAnnotationMovedInSameParent(sender, args.NewIndex, args.MovedAnnotation, args.Parent,
                args.OldIndex));

        listener.ReferenceAdded += (sender, args) =>
            PauseCommands(() => OnRemoteReferenceAdded(sender, args.Parent, args.Reference, args.Index, args.NewTarget));
        listener.ReferenceDeleted += (sender, args) =>
            PauseCommands(() => OnRemoteReferenceDeleted(sender, args.Parent, args.Reference, args.Index, args.DeletedTarget));
        listener.ReferenceChanged += (sender, args) =>
            PauseCommands(() => OnRemoteReferenceChanged(sender, args.Parent, args.Reference, args.Index, args.NewTarget,
                args.ReplacedTarget));
    }

    private void Init()
    {
        RegisterNode(_localPartition);

        var listener = _localPartition.Listener;
        if (listener == null)
            return;

        listener.ChildAdded += (sender, args) =>
            RegisterNode(args.NewChild);
        listener.ChildDeleted += (sender, args) =>
            UnregisterNode(args.DeletedChild);

        listener.AnnotationAdded += (sender, args) =>
            RegisterNode(args.NewAnnotation);
        listener.AnnotationDeleted += (sender, args) =>
            UnregisterNode(args.DeletedAnnotation);
    }

    private void RegisterNode(IReadableNode newNode)
    {
        foreach (var node in M1Extensions.Descendants(newNode, true, true))
        {
            if (!_nodeById.TryAdd(node.GetId(), node))
                throw new DuplicateNodeIdException(node, _nodeById[node.GetId()]);
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

    private void PauseCommands(Func<Action?> action)
    {
        IPartitionCommander? previousDelegate = null;
        Action? postAction = null;
        try
        {
            previousDelegate = DisableCommands();

            postAction = action();
        } finally
        {
            ReenableCommands(previousDelegate);
            postAction?.Invoke();
        }
    }
    
    private IPartitionCommander? DisableCommands()
    {
        IPartitionCommander? previousDelegate = null;
        if (_localPartition.Commander is IOverridableCommander<IPartitionCommander> oc)
        {
            previousDelegate = oc.Delegate;
            oc.Delegate = new NoOpPartitionCommander();
        }

        return previousDelegate;
    }

    private void ReenableCommands(IPartitionCommander? previousDelegate)
    {
        if (_localPartition.Commander is IOverridableCommander<IPartitionCommander> oc)
        {
            oc.Delegate = previousDelegate;
        }
    }

    #region Remote

    #region Properties

    private Action? OnRemotePropertyAdded(object? sender, IWritableNode node, Property property,
        SemanticPropertyValue newValue)
    {
        Lookup(node.GetId()).Set(property, newValue);
        return null;
    }

    private Action? OnRemotePropertyDeleted(object? sender, IWritableNode node, Property property,
        SemanticPropertyValue oldValue)
    {
        Lookup(node.GetId()).Set(property, null);
        return null;
    }

    private Action? OnRemotePropertyChanged(object? sender, IWritableNode node, Property property,
        SemanticPropertyValue newValue, SemanticPropertyValue oldValue)
    {
        Lookup(node.GetId()).Set(property, newValue);
        return null;
    }

    #endregion

    #region Children

    private Action? OnRemoteChildAdded(object? sender, IWritableNode parent, IWritableNode newChild,
        Containment containment, Index index)
    {
        var localParent = Lookup(parent.GetId());
        var newChildNode = (INode)newChild;

        var clone = Clone(newChildNode);
        RegisterNode(clone);

        var newValue = InsertContainment(localParent, containment, index, clone);

        localParent.Set(containment, newValue);
        return null;
    }

    private object InsertContainment(INode localParent, Containment containment, Index index, INode nodeToInsert)
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

    private Action? OnRemoteChildDeleted(object? sender, IWritableNode deletedChild, IWritableNode parent,
        Containment containment, Index index)
    {
        var localParent = Lookup(parent.GetId());

        object? newValue = null;
        if (containment.Multiple)
        {
            var existingChildren = localParent.Get(containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                UnregisterNode(children[index]);
                children.RemoveAt(index);
                newValue = children;
            }
        }

        localParent.Set(containment, newValue);
        return null;
    }

    private Action? OnRemoteChildReplaced(object? sender, IWritableNode newChild, IWritableNode replacedChild,
        IWritableNode parent,
        Containment containment, Index index)
    {
        var localParent = Lookup(parent.GetId());

        object newValue = Clone((INode)newChild);
        if (containment.Multiple)
        {
            var existingChildren = localParent.Get(containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                var newValueNode = (IWritableNode)newValue;
                children.Insert(index, newValueNode);
                var removeIndex = index + 1;
                children.RemoveAt(removeIndex);
                UnregisterNode(children[removeIndex]);
                RegisterNode(newValueNode);
                newValue = children;
            }
        }

        localParent.Set(containment, newValue);
        return null;
    }

    private Action? OnRemoteChildMovedFromOtherContainment(object? sender, IWritableNode newParent,
        Containment newContainment,
        Index newIndex,
        IWritableNode movedChild,
        IWritableNode oldParent,
        Containment oldContainment,
        Index oldIndex)
    {
        var localNewParent = Lookup(newParent.GetId());
        var newValue = InsertContainment(localNewParent, newContainment, newIndex, Lookup(movedChild.GetId()));

        localNewParent.Set(newContainment, newValue);
        return null;
    }

    private Action? OnRemoteChildMovedFromOtherContainmentInSameParent(object? sender,
        Containment newContainment,
        Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent,
        Containment oldContainment,
        Index oldIndex)
    {
        var localParent = Lookup(parent.GetId());
        var newValue = InsertContainment(localParent, newContainment, newIndex, Lookup(movedChild.GetId()));

        localParent.Set(newContainment, newValue);
        return null;
    }

    private Action? OnRemoteChildMovedInSameContainment(object? sender,
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
        return null;
    }

    #endregion

    #region Annotations

    private Action? OnRemoteAnnotationAdded(object? sender, IWritableNode parent, IWritableNode newAnnotation, Index index)
    {
        var localParent = Lookup(parent.GetId());
        var clone = Clone((INode)newAnnotation);
        RegisterNode(clone);
        localParent.InsertAnnotations(index, [clone]);
        return null;
    }

    private Action? OnRemoteAnnotationDeleted(object? sender, IWritableNode deletedAnnotation, IWritableNode parent,
        Index index)
    {
        var localParent = Lookup(parent.GetId());
        var localDeleted = Lookup(deletedAnnotation.GetId());
        UnregisterNode(localDeleted);
        localParent.RemoveAnnotations([localDeleted]);
        return null;
    }

    private Action? OnRemoteAnnotationMovedFromOtherParent(object? sender, IWritableNode newParent,
        Index newIndex,
        IWritableNode movedAnnotation,
        IWritableNode oldParent,
        Index oldIndex)
    {
        var localNewParent = Lookup(newParent.GetId());
        localNewParent.InsertAnnotations(newIndex, [Lookup(movedAnnotation.GetId())]);
        return null;
    }

    private Action? OnRemoteAnnotationMovedInSameParent(object? sender,
        Index newIndex,
        IWritableNode movedAnnotation,
        IWritableNode parent,
        Index oldIndex)
    {
        var localParent = Lookup(parent.GetId());
        INode nodeToInsert = Lookup(movedAnnotation.GetId());
        localParent.InsertAnnotations(newIndex, [nodeToInsert]);
        return null;
    }

    #endregion

    #region References

    private Action? OnRemoteReferenceAdded(object? sender, IWritableNode parent, Reference reference, Index index,
        IReferenceTarget newTarget)
    {
        var localParent = Lookup(parent.GetId());
        INode target = Lookup(newTarget.Reference.GetId());
        var newValue = InsertReference(localParent, reference, index, target);

        localParent.Set(reference, newValue);
        return null;
    }

    private object InsertReference(INode localParent, Reference reference, Index index, IReadableNode target)
    {
        object newValue = target;
        if (reference.Multiple)
        {
            if (localParent.CollectAllSetFeatures().Contains(reference))
            {
                var existingTargets = localParent.Get(reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.Insert(index, target);
                    newValue = targets;
                }
            } else
            {
                newValue = new List<IReadableNode>() { target };
            }
        }

        return newValue;
    }

    private Action? OnRemoteReferenceDeleted(object? sender, IWritableNode parent, Reference reference, Index index,
        IReferenceTarget deletedTarget)
    {
        var localParent = Lookup(parent.GetId());

        object newValue = null;
        if (reference.Multiple)
        {
            var existingTargets = localParent.Get(reference);
            if (existingTargets is IList l)
            {
                var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                targets.RemoveAt(index);
                newValue = targets;
            }
        }

        localParent.Set(reference, newValue);
        return null;
    }

    private Action? OnRemoteReferenceChanged(object? sender, IWritableNode parent, Reference reference, Index index,
        IReferenceTarget newTarget, IReferenceTarget replacedTarget)
    {
        var localParent = Lookup(parent.GetId());

        object newValue = Lookup(newTarget.Reference.GetId());
        if (reference.Multiple)
        {
            var existingTargets = localParent.Get(reference);
            if (existingTargets is IList l)
            {
                var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                targets.Insert(index, (IReadableNode)newValue);
                targets.RemoveAt(index + 1);
                newValue = targets;
            }
        }

        localParent.Set(reference, newValue);
        return null;
    }

    #endregion

    #endregion
}