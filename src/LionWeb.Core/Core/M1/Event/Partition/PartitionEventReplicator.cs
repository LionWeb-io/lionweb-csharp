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

public class PartitionEventReplicator : EventReplicatorBase<IPartitionEvent, IPartitionPublisher>
{
    private readonly IPartitionInstance _localPartition;

    public PartitionEventReplicator(IPartitionInstance localPartition,
        Dictionary<NodeId, IReadableNode>? sharedNodeMap = null) : base(sharedNodeMap)
    {
        _localPartition = localPartition;
        Init();
    }

    /// <inheritdoc />
    protected override void ProcessEvent(object? sender, IPartitionEvent? @event)
    {
        switch (@event)
        {
            case PropertyAddedEvent a:
                OnRemotePropertyAdded(sender, a);
                break;
            case PropertyDeletedEvent a:
                OnRemotePropertyDeleted(sender, a);
                break;
            case PropertyChangedEvent a:
                OnRemotePropertyChanged(sender, a);
                break;
            case ChildAddedEvent a:
                OnRemoteChildAdded(sender, a);
                break;
            case ChildDeletedEvent a:
                OnRemoteChildDeleted(sender, a);
                break;
            case ChildReplacedEvent a:
                OnRemoteChildReplaced(sender, a);
                break;
            case ChildMovedFromOtherContainmentEvent a:
                OnRemoteChildMovedFromOtherContainment(sender, a);
                break;
            case ChildMovedFromOtherContainmentInSameParentEvent a:
                OnRemoteChildMovedFromOtherContainmentInSameParent(sender, a);
                break;
            case ChildMovedInSameContainmentEvent a:
                OnRemoteChildMovedInSameContainment(sender, a);
                break;
            case AnnotationAddedEvent a:
                OnRemoteAnnotationAdded(sender, a);
                break;
            case AnnotationDeletedEvent a:
                OnRemoteAnnotationDeleted(sender, a);
                break;
            case AnnotationMovedFromOtherParentEvent a:
                OnRemoteAnnotationMovedFromOtherParent(sender, a);
                break;
            case AnnotationMovedInSameParentEvent a:
                OnRemoteAnnotationMovedInSameParent(sender, a);
                break;
            case ReferenceAddedEvent a:
                OnRemoteReferenceAdded(sender, a);
                break;
            case ReferenceDeletedEvent a:
                OnRemoteReferenceDeleted(sender, a);
                break;
            case ReferenceChangedEvent a:
                OnRemoteReferenceChanged(sender, a);
                break;
        }
    }

    private void Init()
    {
        RegisterNode(_localPartition);

        var publisher = _localPartition.Publisher;
        if (publisher == null)
            return;

        publisher.Subscribe<IPartitionEvent>(LocalHandler);
    }

    private void LocalHandler(object? sender, IPartitionEvent @event)
    {
        switch (@event)
        {
            case ChildAddedEvent a:
                OnLocalChildAdded(sender, a);
                break;
            case ChildDeletedEvent a:
                OnLocalChildDeleted(sender, a);
                break;
            case AnnotationAddedEvent a:
                OnLocalAnnotationAdded(sender, a);
                break;
            case AnnotationDeletedEvent a:
                OnLocalAnnotationDeleted(sender, a);
                break;
        }
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        base.Dispose();

        UnregisterNode(_localPartition);

        var localPublisher = _localPartition.Publisher;
        if (localPublisher == null)
            return;
        
        localPublisher.Unsubscribe<IPartitionEvent>(LocalHandler);
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
            // oc.Delegate = new NoOpPartitionCommander();
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

    private void OnLocalChildAdded(object? sender, ChildAddedEvent @event) =>
        RegisterNode(@event.NewChild);

    private void OnLocalChildDeleted(object? sender, ChildDeletedEvent @event) =>
        UnregisterNode(@event.DeletedChild);

    private void OnLocalAnnotationAdded(object? sender, AnnotationAddedEvent @event) =>
        RegisterNode(@event.NewAnnotation);

    private void OnLocalAnnotationDeleted(object? sender, AnnotationDeletedEvent @event) =>
        UnregisterNode(@event.DeletedAnnotation);

    #endregion


    #region Remote

    #region Properties

    private void OnRemotePropertyAdded(object? sender, PropertyAddedEvent @event) =>
        PauseCommands(() =>
        {
            Lookup(@event.Node.GetId()).Set(@event.Property, @event.NewValue);
            return null;
        });

    private void OnRemotePropertyDeleted(object? sender, PropertyDeletedEvent @event) =>
        PauseCommands(() =>
        {
            Lookup(@event.Node.GetId()).Set(@event.Property, null);
            return null;
        });

    private void OnRemotePropertyChanged(object? sender, PropertyChangedEvent @event) =>
        PauseCommands(() =>
        {
            Lookup(@event.Node.GetId()).Set(@event.Property, @event.NewValue);
            return null;
        });

    #endregion

    #region Children

    private void OnRemoteChildAdded(object? sender, ChildAddedEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());
            var newChildNode = (INode)@event.NewChild;

            var clone = Clone(newChildNode);
            RegisterNode(clone);

            var newValue = InsertContainment(localParent, @event.Containment, @event.Index, clone);

            localParent.Set(@event.Containment, newValue);
            return null;
        });

    private void OnRemoteChildDeleted(object? sender, ChildDeletedEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());

            object? newValue = null;
            if (@event.Containment.Multiple)
            {
                var existingChildren = localParent.Get(@event.Containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    UnregisterNode(children[@event.Index]);
                    children.RemoveAt(@event.Index);
                    newValue = children;
                }
            }

            localParent.Set(@event.Containment, newValue);
            return null;
        });

    private void OnRemoteChildReplaced(object? sender, ChildReplacedEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());

            object newValue = Clone((INode)@event.NewChild);
            if (@event.Containment.Multiple)
            {
                var existingChildren = localParent.Get(@event.Containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    var newValueNode = (IWritableNode)newValue;
                    children.Insert(@event.Index, newValueNode);
                    var removeIndex = @event.Index + 1;
                    children.RemoveAt(removeIndex);
                    UnregisterNode(children[removeIndex]);
                    RegisterNode(newValueNode);
                    newValue = children;
                }
            }

            localParent.Set(@event.Containment, newValue);
            return null;
        });

    private void OnRemoteChildMovedFromOtherContainment(object? sender,
        ChildMovedFromOtherContainmentEvent @event) =>
        PauseCommands(() =>
        {
            var localNewParent = Lookup(@event.NewParent.GetId());
            var nodeToInsert = LookupOpt(@event.MovedChild.GetId()) ?? Clone((INode)@event.MovedChild);
            var newValue = InsertContainment(localNewParent, @event.NewContainment, @event.NewIndex, nodeToInsert);

            localNewParent.Set(@event.NewContainment, newValue);
            return null;
        });

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(object? sender,
        ChildMovedFromOtherContainmentInSameParentEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());
            var newValue = InsertContainment(localParent, @event.NewContainment, @event.NewIndex,
                Lookup(@event.MovedChild.GetId()));

            localParent.Set(@event.NewContainment, newValue);
            return null;
        });

    private void OnRemoteChildMovedInSameContainment(object? sender,
        ChildMovedInSameContainmentEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());
            INode nodeToInsert = Lookup(@event.MovedChild.GetId());
            object newValue = nodeToInsert;
            var existingChildren = localParent.Get(@event.Containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                children.RemoveAt(@event.OldIndex);
                children.Insert(@event.NewIndex, nodeToInsert);
                newValue = children;
            }

            localParent.Set(@event.Containment, newValue);
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

    private void OnRemoteAnnotationAdded(object? sender, AnnotationAddedEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());
            var clone = Clone((INode)@event.NewAnnotation);
            RegisterNode(clone);
            localParent.InsertAnnotations(@event.Index, [clone]);
            return null;
        });

    private void OnRemoteAnnotationDeleted(object? sender, AnnotationDeletedEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());
            var localDeleted = Lookup(@event.DeletedAnnotation.GetId());
            UnregisterNode(localDeleted);
            localParent.RemoveAnnotations([localDeleted]);
            return null;
        });

    private void OnRemoteAnnotationMovedFromOtherParent(object? sender,
        AnnotationMovedFromOtherParentEvent @event) =>
        PauseCommands(() =>
        {
            var localNewParent = Lookup(@event.NewParent.GetId());
            var moved = LookupOpt(@event.MovedAnnotation.GetId()) ?? Clone((INode)@event.MovedAnnotation);
            localNewParent.InsertAnnotations(@event.NewIndex, [moved]);
            return null;
        });

    private void OnRemoteAnnotationMovedInSameParent(object? sender,
        AnnotationMovedInSameParentEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());
            INode nodeToInsert = Lookup(@event.MovedAnnotation.GetId());
            localParent.InsertAnnotations(@event.NewIndex, [nodeToInsert]);
            return null;
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(object? sender, ReferenceAddedEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());
            INode target = Lookup(@event.NewTarget.Reference.GetId());
            var newValue = InsertReference(localParent, @event.Reference, @event.Index, target);

            localParent.Set(@event.Reference, newValue);
            return null;
        });

    private void OnRemoteReferenceDeleted(object? sender, ReferenceDeletedEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());

            object newValue = null;
            if (@event.Reference.Multiple)
            {
                var existingTargets = localParent.Get(@event.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.RemoveAt(@event.Index);
                    newValue = targets;
                }
            }

            localParent.Set(@event.Reference, newValue);
            return null;
        });

    private void OnRemoteReferenceChanged(object? sender, ReferenceChangedEvent @event) =>
        PauseCommands(() =>
        {
            var localParent = Lookup(@event.Parent.GetId());

            object newValue = Lookup(@event.NewTarget.Reference.GetId());
            if (@event.Reference.Multiple)
            {
                var existingTargets = localParent.Get(@event.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.Insert(@event.Index, (IReadableNode)newValue);
                    targets.RemoveAt(@event.Index + 1);
                    newValue = targets;
                }
            }

            localParent.Set(@event.Reference, newValue);
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