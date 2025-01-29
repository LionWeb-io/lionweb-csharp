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

public class PartitionEventApplier : EventApplierBase
{
    private readonly IPartitionInstance _localPartition;
    private readonly List<IPartitionListener> _listeners = [];

    public PartitionEventApplier(IPartitionInstance localPartition,
        Dictionary<NodeId, IReadableNode>? sharedNodeMap = null) : base(sharedNodeMap)
    {
        _localPartition = localPartition;
        Init();
    }

    public void Subscribe(IPartitionListener listener)
    {
        _listeners.Add(listener);

        listener.PropertyAdded += OnRemotePropertyAdded;
        listener.PropertyDeleted += OnRemotePropertyDeleted;
        listener.PropertyChanged += OnRemotePropertyChanged;

        listener.ChildAdded += OnRemoteChildAdded;
        listener.ChildDeleted += OnRemoteChildDeleted;
        listener.ChildReplaced += OnRemoteChildReplaced;
        listener.ChildMovedFromOtherContainment += OnRemoteChildMovedFromOtherContainment;
        listener.ChildMovedFromOtherContainmentInSameParent += OnRemoteChildMovedFromOtherContainmentInSameParent;
        listener.ChildMovedInSameContainment += OnRemoteChildMovedInSameContainment;

        listener.AnnotationAdded += OnRemoteAnnotationAdded;
        listener.AnnotationDeleted += OnRemoteAnnotationDeleted;
        listener.AnnotationMovedFromOtherParent += OnRemoteAnnotationMovedFromOtherParent;
        listener.AnnotationMovedInSameParent += OnRemoteAnnotationMovedInSameParent;

        listener.ReferenceAdded += OnRemoteReferenceAdded;
        listener.ReferenceDeleted += OnRemoteReferenceDeleted;
        listener.ReferenceChanged += OnRemoteReferenceChanged;
    }

    private void Init()
    {
        RegisterNode(_localPartition);

        var listener = _localPartition.Listener;
        if (listener == null)
            return;

        listener.ChildAdded += OnLocalChildAdded;
        listener.ChildDeleted += OnLocalChildDeleted;

        listener.AnnotationAdded += OnLocalAnnotationAdded;
        listener.AnnotationDeleted += OnLocalAnnotationDeleted;
    }

    public override void Dispose()
    {
        foreach (var listener in _listeners)
        {
            listener.PropertyAdded -= OnRemotePropertyAdded;
            listener.PropertyDeleted -= OnRemotePropertyDeleted;
            listener.PropertyChanged -= OnRemotePropertyChanged;

            listener.ChildAdded -= OnRemoteChildAdded;
            listener.ChildDeleted -= OnRemoteChildDeleted;
            listener.ChildReplaced -= OnRemoteChildReplaced;
            listener.ChildMovedFromOtherContainment -= OnRemoteChildMovedFromOtherContainment;
            listener.ChildMovedFromOtherContainmentInSameParent -= OnRemoteChildMovedFromOtherContainmentInSameParent;
            listener.ChildMovedInSameContainment -= OnRemoteChildMovedInSameContainment;

            listener.AnnotationAdded -= OnRemoteAnnotationAdded;
            listener.AnnotationDeleted -= OnRemoteAnnotationDeleted;
            listener.AnnotationMovedFromOtherParent -= OnRemoteAnnotationMovedFromOtherParent;
            listener.AnnotationMovedInSameParent -= OnRemoteAnnotationMovedInSameParent;

            listener.ReferenceAdded -= OnRemoteReferenceAdded;
            listener.ReferenceDeleted -= OnRemoteReferenceDeleted;
            listener.ReferenceChanged -= OnRemoteReferenceChanged;
        }

        UnregisterNode(_localPartition);

        var localListener = _localPartition.Listener;
        if (localListener == null)
            return;

        localListener.ChildAdded -= OnLocalChildAdded;
        localListener.ChildDeleted -= OnLocalChildDeleted;

        localListener.AnnotationAdded -= OnLocalAnnotationAdded;
        localListener.AnnotationDeleted -= OnLocalAnnotationDeleted;
    }

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

    #region Local

    private void OnLocalChildAdded(object? sender, IPartitionListener.ChildAddedArgs args) =>
        RegisterNode(args.NewChild);

    private void OnLocalChildDeleted(object? sender, IPartitionListener.ChildDeletedArgs args) =>
        UnregisterNode(args.DeletedChild);

    private void OnLocalAnnotationAdded(object? sender, IPartitionListener.AnnotationAddedArgs args) =>
        RegisterNode(args.NewAnnotation);

    private void OnLocalAnnotationDeleted(object? sender, IPartitionListener.AnnotationDeletedArgs args) =>
        UnregisterNode(args.DeletedAnnotation);

    #endregion


    #region Remote

    #region Properties

    private void OnRemotePropertyAdded(object? sender, IPartitionListener.PropertyAddedArgs args) =>
        PauseCommands(() =>
        {
            Lookup(args.Node.GetId()).Set(args.Property, args.NewValue);
            return null;
        });

    private void OnRemotePropertyDeleted(object? sender, IPartitionListener.PropertyDeletedArgs args) =>
        PauseCommands(() =>
        {
            Lookup(args.Node.GetId()).Set(args.Property, null);
            return null;
        });

    private void OnRemotePropertyChanged(object? sender, IPartitionListener.PropertyChangedArgs args) =>
        PauseCommands(() =>
        {
            Lookup(args.Node.GetId()).Set(args.Property, args.NewValue);
            return null;
        });

    #endregion

    #region Children

    private void OnRemoteChildAdded(object? sender, IPartitionListener.ChildAddedArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());
            var newChildNode = (INode)args.NewChild;

            var clone = Clone(newChildNode);
            RegisterNode(clone);

            var newValue = InsertContainment(localParent, args.Containment, args.Index, clone);

            localParent.Set(args.Containment, newValue);
            return null;
        });

    private void OnRemoteChildDeleted(object? sender, IPartitionListener.ChildDeletedArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());

            object? newValue = null;
            if (args.Containment.Multiple)
            {
                var existingChildren = localParent.Get(args.Containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    UnregisterNode(children[args.Index]);
                    children.RemoveAt(args.Index);
                    newValue = children;
                }
            }

            localParent.Set(args.Containment, newValue);
            return null;
        });

    private void OnRemoteChildReplaced(object? sender, IPartitionListener.ChildReplacedArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());

            object newValue = Clone((INode)args.NewChild);
            if (args.Containment.Multiple)
            {
                var existingChildren = localParent.Get(args.Containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    var newValueNode = (IWritableNode)newValue;
                    children.Insert(args.Index, newValueNode);
                    var removeIndex = args.Index + 1;
                    children.RemoveAt(removeIndex);
                    UnregisterNode(children[removeIndex]);
                    RegisterNode(newValueNode);
                    newValue = children;
                }
            }

            localParent.Set(args.Containment, newValue);
            return null;
        });

    private void OnRemoteChildMovedFromOtherContainment(object? sender,
        IPartitionListener.ChildMovedFromOtherContainmentArgs args) =>
        PauseCommands(() =>
        {
            var localNewParent = Lookup(args.NewParent.GetId());
            var nodeToInsert = LookupOpt(args.MovedChild.GetId()) ?? Clone((INode)args.MovedChild);
            var newValue = InsertContainment(localNewParent, args.NewContainment, args.NewIndex, nodeToInsert);

            localNewParent.Set(args.NewContainment, newValue);
            return null;
        });

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(object? sender,
        IPartitionListener.ChildMovedFromOtherContainmentInSameParentArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());
            var newValue = InsertContainment(localParent, args.NewContainment, args.NewIndex,
                Lookup(args.MovedChild.GetId()));

            localParent.Set(args.NewContainment, newValue);
            return null;
        });

    private void OnRemoteChildMovedInSameContainment(object? sender,
        IPartitionListener.ChildMovedInSameContainmentArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());
            INode nodeToInsert = Lookup(args.MovedChild.GetId());
            object newValue = nodeToInsert;
            var existingChildren = localParent.Get(args.Containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                children.RemoveAt(args.OldIndex);
                children.Insert(args.NewIndex, nodeToInsert);
                newValue = children;
            }

            localParent.Set(args.Containment, newValue);
            return null;
        });

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

    #endregion

    #region Annotations

    private void OnRemoteAnnotationAdded(object? sender, IPartitionListener.AnnotationAddedArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());
            var clone = Clone((INode)args.NewAnnotation);
            RegisterNode(clone);
            localParent.InsertAnnotations(args.Index, [clone]);
            return null;
        });

    private void OnRemoteAnnotationDeleted(object? sender, IPartitionListener.AnnotationDeletedArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());
            var localDeleted = Lookup(args.DeletedAnnotation.GetId());
            UnregisterNode(localDeleted);
            localParent.RemoveAnnotations([localDeleted]);
            return null;
        });

    private void OnRemoteAnnotationMovedFromOtherParent(object? sender,
        IPartitionListener.AnnotationMovedFromOtherParentArgs args) =>
        PauseCommands(() =>
        {
            var localNewParent = Lookup(args.NewParent.GetId());
            var moved = LookupOpt(args.MovedAnnotation.GetId()) ?? Clone((INode)args.MovedAnnotation);
            localNewParent.InsertAnnotations(args.NewIndex, [moved]);
            return null;
        });

    private void OnRemoteAnnotationMovedInSameParent(object? sender,
        IPartitionListener.AnnotationMovedInSameParentArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());
            INode nodeToInsert = Lookup(args.MovedAnnotation.GetId());
            localParent.InsertAnnotations(args.NewIndex, [nodeToInsert]);
            return null;
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(object? sender, IPartitionListener.ReferenceAddedArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());
            INode target = Lookup(args.NewTarget.Reference.GetId());
            var newValue = InsertReference(localParent, args.Reference, args.Index, target);

            localParent.Set(args.Reference, newValue);
            return null;
        });

    private void OnRemoteReferenceDeleted(object? sender, IPartitionListener.ReferenceDeletedArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());

            object newValue = null;
            if (args.Reference.Multiple)
            {
                var existingTargets = localParent.Get(args.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.RemoveAt(args.Index);
                    newValue = targets;
                }
            }

            localParent.Set(args.Reference, newValue);
            return null;
        });

    private void OnRemoteReferenceChanged(object? sender, IPartitionListener.ReferenceChangedArgs args) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(args.Parent.GetId());

            object newValue = Lookup(args.NewTarget.Reference.GetId());
            if (args.Reference.Multiple)
            {
                var existingTargets = localParent.Get(args.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.Insert(args.Index, (IReadableNode)newValue);
                    targets.RemoveAt(args.Index + 1);
                    newValue = targets;
                }
            }

            localParent.Set(args.Reference, newValue);
            return null;
        });

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

    #endregion

    #endregion
}