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

namespace LionWeb.Core.Notification.Partition;

using LionWeb.Core.M1.Event.Processor;
using LionWeb.Core.M3;
using System.Collections;
using System.Diagnostics;

/// Replicates events for a <i>local</i> <see cref="IPartitionInstance">partition</see>.
/// <inheritdoc cref="RemoteEventReplicatorBase{TEvent}"/>
public static class PartitionNotificationReplicator
{
    public static IEventProcessor<IPartitionNotification> Create(IPartitionInstance localPartition,
        SharedNodeMap sharedNodeMap, object? sender)
    {
        var internalSender = sender ?? localPartition.GetId();
        var filter = new EventIdFilteringEventProcessor<IPartitionNotification>(internalSender);
        var remoteReplicator =
            new RemotePartitionEventReplicator(localPartition, sharedNodeMap, filter, internalSender);
        var localReplicator = new LocalPartitionEventReplicator(sharedNodeMap, internalSender);

        var result = new CompositeEventProcessor<IPartitionNotification>([remoteReplicator, filter],
            sender ?? $"Composite of {nameof(PartitionNotificationReplicator)} {localPartition.GetId()}");

        var partitionProcessor = localPartition.GetProcessor();
        if (partitionProcessor != null)
        {
            IProcessor.Connect(partitionProcessor, localReplicator);
            IProcessor.Connect(localReplicator, filter);
        }

        return result;
    }
}

public class RemotePartitionEventReplicator : RemoteEventReplicatorBase<IPartitionNotification>
{
    private readonly IPartitionInstance _localPartition;

    protected internal RemotePartitionEventReplicator(IPartitionInstance localPartition, SharedNodeMap sharedNodeMap,
        EventIdFilteringEventProcessor<IPartitionNotification> filter, object? sender) : base(sharedNodeMap, filter, sender)
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
    protected override void ProcessEvent(IPartitionNotification? partitionEvent)
    {
        Debug.WriteLine($"processing event {partitionEvent}");
        switch (partitionEvent)
        {
            case PropertyAddedNotification e:
                OnRemotePropertyAdded(e);
                break;
            case PropertyDeletedNotification e:
                OnRemotePropertyDeleted(e);
                break;
            case PropertyChangedNotification e:
                OnRemotePropertyChanged(e);
                break;
            case ChildAddedNotification e:
                OnRemoteChildAdded(e);
                break;
            case ChildDeletedNotification e:
                OnRemoteChildDeleted(e);
                break;
            case ChildReplacedNotification e:
                OnRemoteChildReplaced(e);
                break;
            case ChildMovedFromOtherContainmentNotification e:
                OnRemoteChildMovedFromOtherContainment(e);
                break;
            case ChildMovedFromOtherContainmentInSameParentNotification e:
                OnRemoteChildMovedFromOtherContainmentInSameParent(e);
                break;
            case ChildMovedInSameContainmentNotification e:
                OnRemoteChildMovedInSameContainment(e);
                break;
            case ChildMovedAndReplacedFromOtherContainmentNotification e:
                OnRemoteChildMovedAndReplacedFromOtherContainment(e);
                break;
            case ChildMovedAndReplacedFromOtherContainmentInSameParentNotification e:
                OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(e);
                break;
            case AnnotationAddedNotification e:
                OnRemoteAnnotationAdded(e);
                break;
            case AnnotationDeletedNotification e:
                OnRemoteAnnotationDeleted(e);
                break;
            case AnnotationMovedFromOtherParentNotification e:
                OnRemoteAnnotationMovedFromOtherParent(e);
                break;
            case AnnotationMovedInSameParentNotification e:
                OnRemoteAnnotationMovedInSameParent(e);
                break;
            case ReferenceAddedNotification e:
                OnRemoteReferenceAdded(e);
                break;
            case ReferenceDeletedNotification e:
                OnRemoteReferenceDeleted(e);
                break;
            case ReferenceChangedNotification e:
                OnRemoteReferenceChanged(e);
                break;
            default:
                throw new ArgumentException($"Can not process event due to unknown {partitionEvent}!");
        }
    }

    #region Properties

    private void OnRemotePropertyAdded(PropertyAddedNotification propertyAddedEvent) =>
        SuppressEventForwarding(propertyAddedEvent, () =>
        {
            Debug.WriteLine(
                $"Node {propertyAddedEvent.Node.PrintIdentity()}: Setting {propertyAddedEvent.Property} to {propertyAddedEvent.NewValue}");
            Lookup(propertyAddedEvent.Node.GetId()).Set(propertyAddedEvent.Property, propertyAddedEvent.NewValue,
                propertyAddedEvent.NotificationId);
        });

    private void OnRemotePropertyDeleted(PropertyDeletedNotification propertyDeletedEvent) =>
        SuppressEventForwarding(propertyDeletedEvent, () =>
        {
            Lookup(propertyDeletedEvent.Node.GetId())
                .Set(propertyDeletedEvent.Property, null, propertyDeletedEvent.NotificationId);
        });

    private void OnRemotePropertyChanged(PropertyChangedNotification propertyChangedEvent) =>
        SuppressEventForwarding(propertyChangedEvent, () =>
        {
            Lookup(propertyChangedEvent.Node.GetId()).Set(propertyChangedEvent.Property, propertyChangedEvent.NewValue,
                propertyChangedEvent.NotificationId);
        });

    #endregion

    #region Children

    private void OnRemoteChildAdded(ChildAddedNotification childAddedEvent) =>
        SuppressEventForwarding(childAddedEvent, () =>
        {
            var localParent = Lookup(childAddedEvent.Parent.GetId());
            var newChildNode = (INode)childAddedEvent.NewChild;

            Debug.WriteLine(
                $"Parent {localParent.PrintIdentity()}: Adding {newChildNode.PrintIdentity()} to {childAddedEvent.Containment} at {childAddedEvent.Index}");
            var newValue = InsertContainment(localParent, childAddedEvent.Containment, childAddedEvent.Index,
                newChildNode);

            localParent.Set(childAddedEvent.Containment, newValue, childAddedEvent.NotificationId);
        });

    private void OnRemoteChildDeleted(ChildDeletedNotification childDeletedEvent) =>
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

            localParent.Set(childDeletedEvent.Containment, newValue, childDeletedEvent.NotificationId);
        });

    private void OnRemoteChildReplaced(ChildReplacedNotification childReplacedEvent) =>
        SuppressEventForwarding(childReplacedEvent, () =>
        {
            var localParent = Lookup(childReplacedEvent.Parent.GetId());
            var substituteNode = (INode)childReplacedEvent.NewChild;
            var newValue = ReplaceContainment(localParent, childReplacedEvent.Containment, childReplacedEvent.Index,
                substituteNode);

            localParent.Set(childReplacedEvent.Containment, newValue, childReplacedEvent.NotificationId);
        });

    private void OnRemoteChildMovedFromOtherContainment(ChildMovedFromOtherContainmentNotification childMovedEvent) =>
        SuppressEventForwarding(childMovedEvent, () =>
        {
            var localNewParent = Lookup(childMovedEvent.NewParent.GetId());
            var nodeToInsert = LookupOpt(childMovedEvent.MovedChild.GetId()) ?? (INode)childMovedEvent.MovedChild;
            var newValue = InsertContainment(localNewParent, childMovedEvent.NewContainment, childMovedEvent.NewIndex,
                nodeToInsert);

            localNewParent.Set(childMovedEvent.NewContainment, newValue, childMovedEvent.NotificationId);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainment(
        ChildMovedAndReplacedFromOtherContainmentNotification childMovedAndReplacedEvent) => SuppressEventForwarding(
        childMovedAndReplacedEvent, () =>
        {
            var localNewParent = Lookup(childMovedAndReplacedEvent.NewParent.GetId());
            var substituteNode = Lookup(childMovedAndReplacedEvent.MovedChild.GetId());
            var newValue = ReplaceContainment(localNewParent, childMovedAndReplacedEvent.NewContainment,
                childMovedAndReplacedEvent.NewIndex,
                substituteNode);

            localNewParent.Set(childMovedAndReplacedEvent.NewContainment, newValue, childMovedAndReplacedEvent.NotificationId);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(
        ChildMovedAndReplacedFromOtherContainmentInSameParentNotification childMovedAndReplacedEvent)
    {
        var parent = Lookup(childMovedAndReplacedEvent.Parent.GetId());
        var substituteNode = Lookup(childMovedAndReplacedEvent.MovedChild.GetId());
        var newValue = ReplaceContainment(parent, childMovedAndReplacedEvent.NewContainment,
            childMovedAndReplacedEvent.NewIndex, substituteNode);

        parent.Set(childMovedAndReplacedEvent.NewContainment, newValue);
    }

    private void OnRemoteChildMovedFromOtherContainmentInSameParent(
        ChildMovedFromOtherContainmentInSameParentNotification childMovedEvent) =>
        SuppressEventForwarding(childMovedEvent, () =>
        {
            var localParent = Lookup(childMovedEvent.Parent.GetId());
            var newValue = InsertContainment(localParent, childMovedEvent.NewContainment, childMovedEvent.NewIndex,
                Lookup(childMovedEvent.MovedChild.GetId()));

            localParent.Set(childMovedEvent.NewContainment, newValue, childMovedEvent.NotificationId);
        });

    private void OnRemoteChildMovedInSameContainment(ChildMovedInSameContainmentNotification childMovedEvent) =>
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

            localParent.Set(childMovedEvent.Containment, newValue, childMovedEvent.NotificationId);
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

    private void OnRemoteAnnotationAdded(AnnotationAddedNotification annotationAddedEvent) =>
        SuppressEventForwarding(annotationAddedEvent, () =>
        {
            var localParent = Lookup(annotationAddedEvent.Parent.GetId());
            var annotation = (INode)annotationAddedEvent.NewAnnotation;
            localParent.InsertAnnotations(annotationAddedEvent.Index, [annotation], annotationAddedEvent.NotificationId);
        });

    private void OnRemoteAnnotationDeleted(AnnotationDeletedNotification annotationDeletedEvent) =>
        SuppressEventForwarding(annotationDeletedEvent, () =>
        {
            var localParent = Lookup(annotationDeletedEvent.Parent.GetId());
            var localDeleted = Lookup(annotationDeletedEvent.DeletedAnnotation.GetId());
            localParent.RemoveAnnotations([localDeleted], annotationDeletedEvent.NotificationId);
        });

    private void OnRemoteAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentNotification annotationMovedEvent) =>
        SuppressEventForwarding(annotationMovedEvent, () =>
        {
            var localNewParent = Lookup(annotationMovedEvent.NewParent.GetId());
            var moved = LookupOpt(annotationMovedEvent.MovedAnnotation.GetId()) ??
                        (INode)annotationMovedEvent.MovedAnnotation;
            localNewParent.InsertAnnotations(annotationMovedEvent.NewIndex, [moved], annotationMovedEvent.NotificationId);
        });

    private void OnRemoteAnnotationMovedInSameParent(AnnotationMovedInSameParentNotification annotationMovedEvent) =>
        SuppressEventForwarding(annotationMovedEvent, () =>
        {
            var localParent = Lookup(annotationMovedEvent.Parent.GetId());
            INode nodeToInsert = Lookup(annotationMovedEvent.MovedAnnotation.GetId());
            localParent.InsertAnnotations(annotationMovedEvent.NewIndex, [nodeToInsert], annotationMovedEvent.NotificationId);
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(ReferenceAddedNotification referenceAddedEvent) =>
        SuppressEventForwarding(referenceAddedEvent, () =>
        {
            var localParent = Lookup(referenceAddedEvent.Parent.GetId());
            INode target = Lookup(referenceAddedEvent.NewTarget.Reference.GetId());
            var newValue = InsertReference(localParent, referenceAddedEvent.Reference, referenceAddedEvent.Index,
                target);

            localParent.Set(referenceAddedEvent.Reference, newValue, referenceAddedEvent.NotificationId);
        });

    private void OnRemoteReferenceDeleted(ReferenceDeletedNotification referenceDeletedEvent) =>
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

            localParent.Set(referenceDeletedEvent.Reference, newValue, referenceDeletedEvent.NotificationId);
        });

    private void OnRemoteReferenceChanged(ReferenceChangedNotification referenceChangedEvent) =>
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

            localParent.Set(referenceChangedEvent.Reference, newValue, referenceChangedEvent.NotificationId);
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
    : EventProcessorBase<IPartitionNotification>(sender)
{
    /// <inheritdoc />
    public override void Receive(IPartitionNotification message)
    {
        switch (message)
        {
            case ChildAddedNotification e:
                OnLocalChildAdded(e);
                break;
            case ChildDeletedNotification e:
                OnLocalChildDeleted(e);
                break;
            case AnnotationAddedNotification e:
                OnLocalAnnotationAdded(e);
                break;
            case AnnotationDeletedNotification e:
                OnLocalAnnotationDeleted(e);
                break;
        }

        Send(message);
    }

    private void OnLocalChildAdded(ChildAddedNotification childAddedEvent) =>
        sharedNodeMap.RegisterNode(childAddedEvent.NewChild);

    private void OnLocalChildDeleted(ChildDeletedNotification childDeletedEvent) =>
        sharedNodeMap.UnregisterNode(childDeletedEvent.DeletedChild);

    private void OnLocalAnnotationAdded(AnnotationAddedNotification annotationAddedEvent) =>
        sharedNodeMap.RegisterNode(annotationAddedEvent.NewAnnotation);

    private void OnLocalAnnotationDeleted(AnnotationDeletedNotification annotationDeletedEvent) =>
        sharedNodeMap.UnregisterNode(annotationDeletedEvent.DeletedAnnotation);
}