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
using Processor;
using System.Collections;
using System.Diagnostics;

/// Replicates events for a <i>local</i> <see cref="IPartitionInstance">partition</see>.
/// <inheritdoc cref="RemoteEventReplicatorBase{TEvent}"/>
public static class PartitionEventReplicator
{
    public static IEventProcessor<IPartitionEvent> Create(IPartitionInstance localPartition,
        SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localPartition.GetId();
        var filter = new EventIdFilteringEventProcessor<IPartitionEvent>(internalSender);
        var remoteReplicator =
            new RemotePartitionEventReplicator(localPartition, sharedNodeMap, filter, internalSender);
        var localReplicator = new LocalPartitionEventReplicator(sharedNodeMap, internalSender);

        var result = new CompositeEventProcessor<IPartitionEvent>([remoteReplicator, filter],
            sender ?? $"Composite of {nameof(PartitionEventReplicator)} {localPartition.GetId()}");

        var partitionProcessor = localPartition.GetProcessor();
        if (partitionProcessor != null)
        {
            IProcessor.Connect(partitionProcessor, localReplicator);
            IProcessor.Connect(localReplicator, filter);
        }

        return result;
    }
}

public class RemotePartitionEventReplicator : RemoteEventReplicatorBase<IPartitionEvent>
{
    private readonly IPartitionInstance _localPartition;

    protected internal RemotePartitionEventReplicator(IPartitionInstance localPartition, SharedNodeMap sharedNodeMap,
        EventIdFilteringEventProcessor<IPartitionEvent> filter, object? sender) : base(sharedNodeMap, filter, sender)
    {
        _localPartition = localPartition;
        SharedNodeMap.RegisterNode(_localPartition);
    }

    /// <see cref="SharedNodeMap.UnregisterNode">Unregisters</see> all nodes of the <i>local</i> partition,
    /// and <see cref="IPublisher{TEvent}.Unsubscribe{TSubscribedEvent}">unsubscribes</see> from it.
    // public override void Dispose()
    // {
    //     base.Dispose();
    //
    //     SharedNodeMap.UnregisterNode(_localPartition);
    //
    //     var localPublisher = _localPartition.GetPublisher();
    //     if (localPublisher == null)
    //         return;
    //
    //     localPublisher.Unsubscribe<IPartitionEvent>(LocalHandler);
    //
    //     GC.SuppressFinalize(this);
    // }

    /// <inheritdoc />
    protected override void ProcessEvent(IPartitionEvent? partitionEvent)
    {
        Debug.WriteLine($"processing event {partitionEvent}");
        switch (partitionEvent)
        {
            case PropertyAddedEvent e:
                OnRemotePropertyAdded(e);
                break;
            case PropertyDeletedEvent e:
                OnRemotePropertyDeleted(e);
                break;
            case PropertyChangedEvent e:
                OnRemotePropertyChanged(e);
                break;
            case ChildAddedEvent e:
                OnRemoteChildAdded(e);
                break;
            case ChildDeletedEvent e:
                OnRemoteChildDeleted(e);
                break;
            case ChildReplacedEvent e:
                OnRemoteChildReplaced(e);
                break;
            case ChildMovedFromOtherContainmentEvent e:
                OnRemoteChildMovedFromOtherContainment(e);
                break;
            case ChildMovedFromOtherContainmentInSameParentEvent e:
                OnRemoteChildMovedFromOtherContainmentInSameParent(e);
                break;
            case ChildMovedInSameContainmentEvent e:
                OnRemoteChildMovedInSameContainment(e);
                break;
            case ChildMovedAndReplacedFromOtherContainmentEvent e:
                OnRemoteChildMovedAndReplacedFromOtherContainment(e);
                break;
            case ChildMovedAndReplacedFromOtherContainmentInSameParentEvent e:
                OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(e);
                break;
            case AnnotationAddedEvent e:
                OnRemoteAnnotationAdded(e);
                break;
            case AnnotationDeletedEvent e:
                OnRemoteAnnotationDeleted(e);
                break;
            case AnnotationMovedFromOtherParentEvent e:
                OnRemoteAnnotationMovedFromOtherParent(e);
                break;
            case AnnotationMovedInSameParentEvent e:
                OnRemoteAnnotationMovedInSameParent(e);
                break;
            case ReferenceAddedEvent e:
                OnRemoteReferenceAdded(e);
                break;
            case ReferenceDeletedEvent e:
                OnRemoteReferenceDeleted(e);
                break;
            case ReferenceChangedEvent e:
                OnRemoteReferenceChanged(e);
                break;
            default:
                throw new ArgumentException($"Can not process event due to unknown {partitionEvent}!");
        }
    }

    #region Properties

    private void OnRemotePropertyAdded(PropertyAddedEvent propertyAddedEvent) =>
        SuppressEventForwarding(propertyAddedEvent, () =>
        {
            Debug.WriteLine(
                $"Node {propertyAddedEvent.Node.PrintIdentity()}: Setting {propertyAddedEvent.Property} to {propertyAddedEvent.NewValue}");
            Lookup(propertyAddedEvent.Node.GetId()).Set(propertyAddedEvent.Property, propertyAddedEvent.NewValue,
                propertyAddedEvent.EventId);
        });

    private void OnRemotePropertyDeleted(PropertyDeletedEvent propertyDeletedEvent) =>
        SuppressEventForwarding(propertyDeletedEvent, () =>
        {
            Lookup(propertyDeletedEvent.Node.GetId())
                .Set(propertyDeletedEvent.Property, null, propertyDeletedEvent.EventId);
        });

    private void OnRemotePropertyChanged(PropertyChangedEvent propertyChangedEvent) =>
        SuppressEventForwarding(propertyChangedEvent, () =>
        {
            Lookup(propertyChangedEvent.Node.GetId()).Set(propertyChangedEvent.Property, propertyChangedEvent.NewValue,
                propertyChangedEvent.EventId);
        });

    #endregion

    #region Children

    private void OnRemoteChildAdded(ChildAddedEvent childAddedEvent) =>
        SuppressEventForwarding(childAddedEvent, () =>
        {
            var localParent = Lookup(childAddedEvent.Parent.GetId());
            var newChildNode = (INode)childAddedEvent.NewChild;

            Debug.WriteLine(
                $"Parent {localParent.PrintIdentity()}: Adding {newChildNode.PrintIdentity()} to {childAddedEvent.Containment} at {childAddedEvent.Index}");
            var newValue = InsertContainment(localParent, childAddedEvent.Containment, childAddedEvent.Index,
                newChildNode);

            localParent.Set(childAddedEvent.Containment, newValue, childAddedEvent.EventId);
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

            localParent.Set(childDeletedEvent.Containment, newValue, childDeletedEvent.EventId);
        });

    private void OnRemoteChildReplaced(ChildReplacedEvent childReplacedEvent) =>
        SuppressEventForwarding(childReplacedEvent, () =>
        {
            var localParent = Lookup(childReplacedEvent.Parent.GetId());
            var substituteNode = (INode)childReplacedEvent.NewChild;
            var newValue = ReplaceContainment(localParent, childReplacedEvent.Containment, childReplacedEvent.Index,
                substituteNode);

            localParent.Set(childReplacedEvent.Containment, newValue, childReplacedEvent.EventId);
        });

    private void OnRemoteChildMovedFromOtherContainment(ChildMovedFromOtherContainmentEvent childMovedEvent) =>
        SuppressEventForwarding(childMovedEvent, () =>
        {
            var localNewParent = Lookup(childMovedEvent.NewParent.GetId());
            var nodeToInsert = LookupOpt(childMovedEvent.MovedChild.GetId()) ?? (INode)childMovedEvent.MovedChild;
            var newValue = InsertContainment(localNewParent, childMovedEvent.NewContainment, childMovedEvent.NewIndex,
                nodeToInsert);

            localNewParent.Set(childMovedEvent.NewContainment, newValue, childMovedEvent.EventId);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainment(
        ChildMovedAndReplacedFromOtherContainmentEvent childMovedAndReplacedEvent) => SuppressEventForwarding(
        childMovedAndReplacedEvent, () =>
        {
            var localNewParent = Lookup(childMovedAndReplacedEvent.NewParent.GetId());
            var substituteNode = Lookup(childMovedAndReplacedEvent.MovedChild.GetId());
            var newValue = ReplaceContainment(localNewParent, childMovedAndReplacedEvent.NewContainment,
                childMovedAndReplacedEvent.NewIndex,
                substituteNode);

            localNewParent.Set(childMovedAndReplacedEvent.NewContainment, newValue, childMovedAndReplacedEvent.EventId);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(
        ChildMovedAndReplacedFromOtherContainmentInSameParentEvent childMovedAndReplacedEvent)
    {
        var parent = Lookup(childMovedAndReplacedEvent.Parent.GetId());
        var substituteNode = Lookup(childMovedAndReplacedEvent.MovedChild.GetId());
        var newValue = ReplaceContainment(parent, childMovedAndReplacedEvent.NewContainment,
            childMovedAndReplacedEvent.NewIndex, substituteNode);

        parent.Set(childMovedAndReplacedEvent.NewContainment, newValue);
    }

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentEvent childMovedEvent) =>
        SuppressEventForwarding(childMovedEvent, () =>
        {
            var localParent = Lookup(childMovedEvent.Parent.GetId());
            var newValue = InsertContainment(localParent, childMovedEvent.NewContainment, childMovedEvent.NewIndex,
                Lookup(childMovedEvent.MovedChild.GetId()));

            localParent.Set(childMovedEvent.NewContainment, newValue, childMovedEvent.EventId);
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

            localParent.Set(childMovedEvent.Containment, newValue, childMovedEvent.EventId);
        });

    private static object ReplaceContainment(INode localParent, Containment containment, Index index,
        INode substituteNode)
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
            var annotation = (INode)annotationAddedEvent.NewAnnotation;
            localParent.InsertAnnotations(annotationAddedEvent.Index, [annotation], annotationAddedEvent.EventId);
        });

    private void OnRemoteAnnotationDeleted(AnnotationDeletedEvent annotationDeletedEvent) =>
        SuppressEventForwarding(annotationDeletedEvent, () =>
        {
            var localParent = Lookup(annotationDeletedEvent.Parent.GetId());
            var localDeleted = Lookup(annotationDeletedEvent.DeletedAnnotation.GetId());
            localParent.RemoveAnnotations([localDeleted], annotationDeletedEvent.EventId);
        });

    private void OnRemoteAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentEvent annotationMovedEvent) =>
        SuppressEventForwarding(annotationMovedEvent, () =>
        {
            var localNewParent = Lookup(annotationMovedEvent.NewParent.GetId());
            var moved = LookupOpt(annotationMovedEvent.MovedAnnotation.GetId()) ??
                        (INode)annotationMovedEvent.MovedAnnotation;
            localNewParent.InsertAnnotations(annotationMovedEvent.NewIndex, [moved], annotationMovedEvent.EventId);
        });

    private void OnRemoteAnnotationMovedInSameParent(AnnotationMovedInSameParentEvent annotationMovedEvent) =>
        SuppressEventForwarding(annotationMovedEvent, () =>
        {
            var localParent = Lookup(annotationMovedEvent.Parent.GetId());
            INode nodeToInsert = Lookup(annotationMovedEvent.MovedAnnotation.GetId());
            localParent.InsertAnnotations(annotationMovedEvent.NewIndex, [nodeToInsert], annotationMovedEvent.EventId);
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(ReferenceAddedEvent referenceAddedEvent) =>
        SuppressEventForwarding(referenceAddedEvent, () =>
        {
            var localParent = Lookup(referenceAddedEvent.Parent.GetId());
            INode target = Lookup(referenceAddedEvent.NewTarget.Reference.GetId());
            var newValue = InsertReference(localParent, referenceAddedEvent.Reference, referenceAddedEvent.Index,
                target);

            localParent.Set(referenceAddedEvent.Reference, newValue, referenceAddedEvent.EventId);
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

            localParent.Set(referenceDeletedEvent.Reference, newValue, referenceDeletedEvent.EventId);
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

            localParent.Set(referenceChangedEvent.Reference, newValue, referenceChangedEvent.EventId);
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
}

public class LocalPartitionEventReplicator(SharedNodeMap sharedNodeMap, object? sender)
    : EventProcessorBase<IPartitionEvent>(sender)
{
    /// <inheritdoc />
    public override void Receive(IPartitionEvent message)
    {
        switch (message)
        {
            case ChildAddedEvent e:
                OnLocalChildAdded(e);
                break;
            case ChildDeletedEvent e:
                OnLocalChildDeleted(e);
                break;
            case AnnotationAddedEvent e:
                OnLocalAnnotationAdded(e);
                break;
            case AnnotationDeletedEvent e:
                OnLocalAnnotationDeleted(e);
                break;
        }

        Send(message);
    }

    private void OnLocalChildAdded(ChildAddedEvent childAddedEvent) =>
        sharedNodeMap.RegisterNode(childAddedEvent.NewChild);

    private void OnLocalChildDeleted(ChildDeletedEvent childDeletedEvent) =>
        sharedNodeMap.UnregisterNode(childDeletedEvent.DeletedChild);

    private void OnLocalAnnotationAdded(AnnotationAddedEvent annotationAddedEvent) =>
        sharedNodeMap.RegisterNode(annotationAddedEvent.NewAnnotation);

    private void OnLocalAnnotationDeleted(AnnotationDeletedEvent annotationDeletedEvent) =>
        sharedNodeMap.UnregisterNode(annotationDeletedEvent.DeletedAnnotation);
}