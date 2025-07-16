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
using System.Diagnostics;

/// Replicates events for a <i>local</i> <see cref="IPartitionInstance">partition</see>.
/// <inheritdoc cref="EventReplicatorBase{TEvent,TPublisher}"/>
public class PartitionEventReplicator : EventReplicatorBase<IPartitionEvent, IPartitionPublisher>, IPartitionPublisher
{
    private readonly IPartitionInstance _localPartition;

    public PartitionEventReplicator(IPartitionInstance localPartition,
        Dictionary<NodeId, IReadableNode>? sharedNodeMap = null) : base(localPartition.GetPublisher(),
        localPartition.GetCommander(), sharedNodeMap)
    {
        _localPartition = localPartition;
        Init();
    }

    /// <inheritdoc />
    protected override void ProcessEvent(object? sender, IPartitionEvent? partitionEvent)
    {
        Debug.WriteLine($"{sender}: processing event {partitionEvent}");
        switch (partitionEvent)
        {
            case PropertyAddedEvent a:
                OnRemotePropertyAdded(a);
                break;
            case PropertyDeletedEvent a:
                OnRemotePropertyDeleted(a);
                break;
            case PropertyChangedEvent a:
                OnRemotePropertyChanged(a);
                break;
            case ChildAddedEvent a:
                OnRemoteChildAdded(a);
                break;
            case ChildDeletedEvent a:
                OnRemoteChildDeleted(a);
                break;
            case ChildReplacedEvent a:
                OnRemoteChildReplaced(a);
                break;
            case ChildMovedFromOtherContainmentEvent a:
                OnRemoteChildMovedFromOtherContainment(a);
                break;
            case ChildMovedFromOtherContainmentInSameParentEvent a:
                OnRemoteChildMovedFromOtherContainmentInSameParent(a);
                break;
            case ChildMovedInSameContainmentEvent a:
                OnRemoteChildMovedInSameContainment(a);
                break;
            case ChildMovedAndReplacedFromOtherContainmentEvent a:
                OnRemoteChildMovedAndReplacedFromOtherContainment(a);
                break;
            case AnnotationAddedEvent a:
                OnRemoteAnnotationAdded(a);
                break;
            case AnnotationDeletedEvent a:
                OnRemoteAnnotationDeleted(a);
                break;
            case AnnotationMovedFromOtherParentEvent a:
                OnRemoteAnnotationMovedFromOtherParent(a);
                break;
            case AnnotationMovedInSameParentEvent a:
                OnRemoteAnnotationMovedInSameParent(a);
                break;
            case ReferenceAddedEvent a:
                OnRemoteReferenceAdded(a);
                break;
            case ReferenceDeletedEvent a:
                OnRemoteReferenceDeleted(a);
                break;
            case ReferenceChangedEvent a:
                OnRemoteReferenceChanged(a);
                break;
        }
    }

    private void Init()
    {
        RegisterNode(_localPartition);

        var publisher = _localPartition.GetPublisher();
        if (publisher == null)
            return;

        publisher.Subscribe<IPartitionEvent>(LocalHandler);
    }

    /// <see cref="EventReplicatorBase{TEvent,TPublisher}.UnregisterNode">Unregisters</see> all nodes of the <i>local</i> partition,
    /// and <see cref="IPublisher{TEvent}.Unsubscribe{TSubscribedEvent}">unsubscribes</see> from it.
    public override void Dispose()
    {
        base.Dispose();

        UnregisterNode(_localPartition);

        var localPublisher = _localPartition.GetPublisher();
        if (localPublisher == null)
            return;

        localPublisher.Unsubscribe<IPartitionEvent>(LocalHandler);

        GC.SuppressFinalize(this);
    }

    #region Local

    private void LocalHandler(object? sender, IPartitionEvent partitionEvent)
    {
        switch (partitionEvent)
        {
            case ChildAddedEvent a:
                OnLocalChildAdded(a);
                break;
            case ChildDeletedEvent a:
                OnLocalChildDeleted(a);
                break;
            case AnnotationAddedEvent a:
                OnLocalAnnotationAdded(a);
                break;
            case AnnotationDeletedEvent a:
                OnLocalAnnotationDeleted(a);
                break;
        }
    }

    private void OnLocalChildAdded(ChildAddedEvent childAddedEvent) =>
        RegisterNode(childAddedEvent.NewChild);

    private void OnLocalChildDeleted(ChildDeletedEvent childDeletedEvent) =>
        UnregisterNode(childDeletedEvent.DeletedChild);

    private void OnLocalAnnotationAdded(AnnotationAddedEvent annotationAddedEvent) =>
        RegisterNode(annotationAddedEvent.NewAnnotation);

    private void OnLocalAnnotationDeleted(AnnotationDeletedEvent annotationDeletedEvent) =>
        UnregisterNode(annotationDeletedEvent.DeletedAnnotation);

    #endregion


    #region Remote

    #region Properties

    private void OnRemotePropertyAdded(PropertyAddedEvent propertyAddedEvent) =>
        SuppressEventForwarding(propertyAddedEvent, () =>
        {
            Debug.WriteLine(
                $"Node {propertyAddedEvent.Node.PrintIdentity()}: Setting {propertyAddedEvent.Property} to {propertyAddedEvent.NewValue}");
            Lookup(propertyAddedEvent.Node.GetId()).Set(propertyAddedEvent.Property, propertyAddedEvent.NewValue);
        });

    private void OnRemotePropertyDeleted(PropertyDeletedEvent propertyDeletedEvent) =>
        SuppressEventForwarding(propertyDeletedEvent, () =>
        {
            Lookup(propertyDeletedEvent.Node.GetId()).Set(propertyDeletedEvent.Property, null);
        });

    private void OnRemotePropertyChanged(PropertyChangedEvent propertyChangedEvent) =>
        SuppressEventForwarding(propertyChangedEvent, () =>
        {
            Lookup(propertyChangedEvent.Node.GetId()).Set(propertyChangedEvent.Property, propertyChangedEvent.NewValue);
        });

    #endregion

    #region Children

    private void OnRemoteChildAdded(ChildAddedEvent childAddedEvent) =>
        SuppressEventForwarding(childAddedEvent, () =>
        {
            var localParent = Lookup(childAddedEvent.Parent.GetId());
            var newChildNode = (INode)childAddedEvent.NewChild;

            var clone = Clone(newChildNode);

            Debug.WriteLine(
                $"Parent {localParent.PrintIdentity()}: Adding {clone.PrintIdentity()} to {childAddedEvent.Containment} at {childAddedEvent.Index}");
            var newValue = InsertContainment(localParent, childAddedEvent.Containment, childAddedEvent.Index, clone);

            localParent.Set(childAddedEvent.Containment, newValue);
        });

    private void OnRemoteChildDeleted(ChildDeletedEvent childDeletedEvent) =>
        SuppressEventForwarding(childDeletedEvent, () =>
        {
            var localParent = Lookup(childDeletedEvent.Parent.GetId());

            object? newValue = null;
            if (childDeletedEvent.Containment.Multiple)
            {
                var existingChildren = localParent.Get(childDeletedEvent.Containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    children.RemoveAt(childDeletedEvent.Index);
                    newValue = children;
                }
            }

            localParent.Set(childDeletedEvent.Containment, newValue);
        });

    private void OnRemoteChildReplaced(ChildReplacedEvent childReplacedEvent) =>
        SuppressEventForwarding(childReplacedEvent, () =>
        {
            var localParent = Lookup(childReplacedEvent.Parent.GetId());

            object newValue = Clone((INode)childReplacedEvent.NewChild);
            if (childReplacedEvent.Containment.Multiple)
            {
                var existingChildren = localParent.Get(childReplacedEvent.Containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    var newValueNode = (IWritableNode)newValue;
                    children.Insert(childReplacedEvent.Index, newValueNode);
                    var removeIndex = childReplacedEvent.Index + 1;
                    children.RemoveAt(removeIndex);
                    newValue = children;
                }
            }

            localParent.Set(childReplacedEvent.Containment, newValue);
        });

    private void OnRemoteChildMovedFromOtherContainment(ChildMovedFromOtherContainmentEvent childMovedEvent) =>
        SuppressEventForwarding(childMovedEvent, () =>
        {
            var localNewParent = Lookup(childMovedEvent.NewParent.GetId());
            var nodeToInsert = LookupOpt(childMovedEvent.MovedChild.GetId()) ?? Clone((INode)childMovedEvent.MovedChild);
            var newValue = InsertContainment(localNewParent, childMovedEvent.NewContainment, childMovedEvent.NewIndex, nodeToInsert);

            localNewParent.Set(childMovedEvent.NewContainment, newValue);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainment(ChildMovedAndReplacedFromOtherContainmentEvent childMovedAndReplacedEvent) => SuppressEventForwarding(childMovedAndReplacedEvent, () =>
    {
        var localNewParent = Lookup(childMovedAndReplacedEvent.NewParent.GetId());
        var substituteNode = Lookup(childMovedAndReplacedEvent.MovedChild.GetId());
        var newValue = ReplaceContainment(localNewParent, childMovedAndReplacedEvent.NewContainment, childMovedAndReplacedEvent.NewIndex,
            substituteNode);

        localNewParent.Set(childMovedAndReplacedEvent.NewContainment, newValue);
    });

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(ChildMovedFromOtherContainmentInSameParentEvent childMovedEvent) =>
        SuppressEventForwarding(childMovedEvent, () =>
        {
            var localParent = Lookup(childMovedEvent.Parent.GetId());
            var newValue = InsertContainment(localParent, childMovedEvent.NewContainment, childMovedEvent.NewIndex,
                Lookup(childMovedEvent.MovedChild.GetId()));

            localParent.Set(childMovedEvent.NewContainment, newValue);
        });

    private void OnRemoteChildMovedInSameContainment(ChildMovedInSameContainmentEvent childMovedEvent) =>
        SuppressEventForwarding(childMovedEvent, () =>
        {
            var localParent = Lookup(childMovedEvent.Parent.GetId());
            INode nodeToInsert = Lookup(childMovedEvent.MovedChild.GetId());
            object newValue = nodeToInsert;
            var existingChildren = localParent.Get(childMovedEvent.Containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                children.RemoveAt(childMovedEvent.OldIndex);
                children.Insert(childMovedEvent.NewIndex, nodeToInsert);
                newValue = children;
            }

            localParent.Set(childMovedEvent.Containment, newValue);
        });

    private static object ReplaceContainment(INode localParent, Containment containment, Index index, INode substituteNode)
    {
        if (localParent.TryGet(containment, out var existingChildren))
        {
            switch (existingChildren)
            {
                case IList l:
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    children.Insert(index, substituteNode);
                    children.RemoveAt(index + 1);
                    return children;
                case IWritableNode _ when index == 0:
                    return substituteNode;
                default:
                    // when containment data is corrupted or assigned to an invalid value after its creation
                    throw new InvalidValueException(containment, existingChildren);
            }
        }

        if (containment.Multiple)
        {
            return new List<IWritableNode> { substituteNode };
        }

        return substituteNode;
    }

    private static object InsertContainment(INode localParent, Containment containment, Index index, INode nodeToInsert)
    {
        if (localParent.CollectAllSetFeatures().Contains(containment))
        {
            var existingChildren = localParent.Get(containment);
            switch (existingChildren)
            {
                case IList l:
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    children.Insert(index, nodeToInsert);
                    return children;
                case IWritableNode _:
                    return nodeToInsert;
                default:
                    // when containment data is corrupted or assigned to an invalid value after its creation 
                    throw new InvalidValueException(containment, existingChildren);
            }
        }

        if (containment.Multiple)
        {
            return new List<IWritableNode> { nodeToInsert };
        }

        return nodeToInsert;
    }

    #endregion

    #region Annotations

    private void OnRemoteAnnotationAdded(AnnotationAddedEvent annotationAddedEvent) =>
        SuppressEventForwarding(annotationAddedEvent, () =>
        {
            var localParent = Lookup(annotationAddedEvent.Parent.GetId());
            var clone = Clone((INode)annotationAddedEvent.NewAnnotation);
            localParent.InsertAnnotations(annotationAddedEvent.Index, [clone]);
        });

    private void OnRemoteAnnotationDeleted(AnnotationDeletedEvent annotationDeletedEvent) =>
        SuppressEventForwarding(annotationDeletedEvent, () =>
        {
            var localParent = Lookup(annotationDeletedEvent.Parent.GetId());
            var localDeleted = Lookup(annotationDeletedEvent.DeletedAnnotation.GetId());
            localParent.RemoveAnnotations([localDeleted]);
        });

    private void OnRemoteAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentEvent annotationMovedEvent) =>
        SuppressEventForwarding(annotationMovedEvent, () =>
        {
            var localNewParent = Lookup(annotationMovedEvent.NewParent.GetId());
            var moved = LookupOpt(annotationMovedEvent.MovedAnnotation.GetId()) ?? Clone((INode)annotationMovedEvent.MovedAnnotation);
            localNewParent.InsertAnnotations(annotationMovedEvent.NewIndex, [moved]);
        });

    private void OnRemoteAnnotationMovedInSameParent(AnnotationMovedInSameParentEvent annotationMovedEvent) =>
        SuppressEventForwarding(annotationMovedEvent, () =>
        {
            var localParent = Lookup(annotationMovedEvent.Parent.GetId());
            INode nodeToInsert = Lookup(annotationMovedEvent.MovedAnnotation.GetId());
            localParent.InsertAnnotations(annotationMovedEvent.NewIndex, [nodeToInsert]);
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(ReferenceAddedEvent referenceAddedEvent) =>
        SuppressEventForwarding(referenceAddedEvent, () =>
        {
            var localParent = Lookup(referenceAddedEvent.Parent.GetId());
            INode target = Lookup(referenceAddedEvent.NewTarget.Reference.GetId());
            var newValue = InsertReference(localParent, referenceAddedEvent.Reference, referenceAddedEvent.Index, target);

            localParent.Set(referenceAddedEvent.Reference, newValue);
        });

    private void OnRemoteReferenceDeleted(ReferenceDeletedEvent referenceDeletedEvent) =>
        SuppressEventForwarding(referenceDeletedEvent, () =>
        {
            var localParent = Lookup(referenceDeletedEvent.Parent.GetId());

            object newValue = null;
            if (referenceDeletedEvent.Reference.Multiple)
            {
                var existingTargets = localParent.Get(referenceDeletedEvent.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.RemoveAt(referenceDeletedEvent.Index);
                    newValue = targets;
                }
            }

            localParent.Set(referenceDeletedEvent.Reference, newValue);
        });

    private void OnRemoteReferenceChanged(ReferenceChangedEvent referenceChangedEvent) =>
        SuppressEventForwarding(referenceChangedEvent, () =>
        {
            var localParent = Lookup(referenceChangedEvent.Parent.GetId());

            object newValue = Lookup(referenceChangedEvent.NewTarget.Reference.GetId());
            if (referenceChangedEvent.Reference.Multiple)
            {
                var existingTargets = localParent.Get(referenceChangedEvent.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.Insert(referenceChangedEvent.Index, (IReadableNode)newValue);
                    targets.RemoveAt(referenceChangedEvent.Index + 1);
                    newValue = targets;
                }
            }

            localParent.Set(referenceChangedEvent.Reference, newValue);
        });

    private static object InsertReference(INode localParent, Reference reference, Index index, IReadableNode target)
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