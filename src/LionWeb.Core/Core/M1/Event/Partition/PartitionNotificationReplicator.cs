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

/// Replicates notifications for a <i>local</i> <see cref="IPartitionInstance">partition</see>.
/// <inheritdoc cref="NotificationReplicatorBase{TEvent,TPublisher}"/>
public class PartitionNotificationReplicator : NotificationReplicatorBase<IPartitionNotification, IPartitionPublisher>, IPartitionPublisher
{
    private readonly IPartitionInstance _localPartition;

    public PartitionNotificationReplicator(IPartitionInstance localPartition,
        SharedNodeMap sharedNodeMap = null) : base(localPartition.GetPublisher(),
        localPartition.GetCommander(), sharedNodeMap)
    {
        _localPartition = localPartition;
        Init();
    }

    /// <inheritdoc />
    protected override void ProcessEvent(object? sender, IPartitionNotification? partitionEvent)
    {
        Debug.WriteLine($"{sender}: processing event {partitionEvent}");
        switch (partitionEvent)
        {
            case PropertyAddedNotification a:
                OnRemotePropertyAdded(a);
                break;
            case PropertyDeletedNotification a:
                OnRemotePropertyDeleted(a);
                break;
            case PropertyChangedNotification a:
                OnRemotePropertyChanged(a);
                break;
            case ChildAddedNotification a:
                OnRemoteChildAdded(a);
                break;
            case ChildDeletedNotification a:
                OnRemoteChildDeleted(a);
                break;
            case ChildReplacedNotification a:
                OnRemoteChildReplaced(a);
                break;
            case ChildMovedFromOtherContainmentNotification a:
                OnRemoteChildMovedFromOtherContainment(a);
                break;
            case ChildMovedFromOtherContainmentInSameParentNotification a:
                OnRemoteChildMovedFromOtherContainmentInSameParent(a);
                break;
            case ChildMovedInSameContainmentNotification a:
                OnRemoteChildMovedInSameContainment(a);
                break;
            case ChildMovedAndReplacedFromOtherContainmentNotification a:
                OnRemoteChildMovedAndReplacedFromOtherContainment(a);
                break;
            case ChildMovedAndReplacedFromOtherContainmentInSameParentNotification a:
                OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(a);
                break;
            case AnnotationAddedNotification a:
                OnRemoteAnnotationAdded(a);
                break;
            case AnnotationDeletedNotification a:
                OnRemoteAnnotationDeleted(a);
                break;
            case AnnotationMovedFromOtherParentNotification a:
                OnRemoteAnnotationMovedFromOtherParent(a);
                break;
            case AnnotationMovedInSameParentNotification a:
                OnRemoteAnnotationMovedInSameParent(a);
                break;
            case ReferenceAddedNotification a:
                OnRemoteReferenceAdded(a);
                break;
            case ReferenceDeletedNotification a:
                OnRemoteReferenceDeleted(a);
                break;
            case ReferenceChangedNotification a:
                OnRemoteReferenceChanged(a);
                break;
            default:
                throw new ArgumentException($"Can not process event due to unknown {partitionEvent}!");
        }
    }

    private void Init()
    {
        RegisterNode(_localPartition);

        var publisher = _localPartition.GetPublisher();
        if (publisher == null)
            return;

        publisher.Subscribe<IPartitionNotification>(LocalHandler);
    }

    /// <see cref="NotificationReplicatorBase{TEvent,TPublisher}.UnregisterNode">Unregisters</see> all nodes of the <i>local</i> partition,
    /// and <see cref="IPublisher{TEvent}.Unsubscribe{TSubscribedEvent}">unsubscribes</see> from it.
    public override void Dispose()
    {
        base.Dispose();

        UnregisterNode(_localPartition);

        var localPublisher = _localPartition.GetPublisher();
        if (localPublisher == null)
            return;

        localPublisher.Unsubscribe<IPartitionNotification>(LocalHandler);

        GC.SuppressFinalize(this);
    }

    #region Local

    private void LocalHandler(object? sender, IPartitionNotification partitionNotification)
    {
        switch (partitionNotification)
        {
            case ChildAddedNotification a:
                OnLocalChildAdded(a);
                break;
            case ChildDeletedNotification a:
                OnLocalChildDeleted(a);
                break;
            case AnnotationAddedNotification a:
                OnLocalAnnotationAdded(a);
                break;
            case AnnotationDeletedNotification a:
                OnLocalAnnotationDeleted(a);
                break;
        }
    }

    private void OnLocalChildAdded(ChildAddedNotification childAddedNotification) =>
        RegisterNode(childAddedNotification.NewChild);

    private void OnLocalChildDeleted(ChildDeletedNotification childDeletedNotification) =>
        UnregisterNode(childDeletedNotification.DeletedChild);

    private void OnLocalAnnotationAdded(AnnotationAddedNotification annotationAddedNotification) =>
        RegisterNode(annotationAddedNotification.NewAnnotation);

    private void OnLocalAnnotationDeleted(AnnotationDeletedNotification annotationDeletedNotification) =>
        UnregisterNode(annotationDeletedNotification.DeletedAnnotation);

    #endregion


    #region Remote

    #region Properties

    private void OnRemotePropertyAdded(PropertyAddedNotification propertyAddedNotification) =>
        SuppressEventForwarding(propertyAddedNotification, () =>
        {
            Debug.WriteLine(
                $"Node {propertyAddedNotification.Node.PrintIdentity()}: Setting {propertyAddedNotification.Property} to {propertyAddedNotification.NewValue}");
            Lookup(propertyAddedNotification.Node.GetId()).Set(propertyAddedNotification.Property, propertyAddedNotification.NewValue, propertyAddedNotification.NotificationId);
        });

    private void OnRemotePropertyDeleted(PropertyDeletedNotification propertyDeletedNotification) =>
        SuppressEventForwarding(propertyDeletedNotification, () =>
        {
            Lookup(propertyDeletedNotification.Node.GetId()).Set(propertyDeletedNotification.Property, null, propertyDeletedNotification.NotificationId);
        });

    private void OnRemotePropertyChanged(PropertyChangedNotification propertyChangedNotification) =>
        SuppressEventForwarding(propertyChangedNotification, () =>
        {
            Lookup(propertyChangedNotification.Node.GetId()).Set(propertyChangedNotification.Property, propertyChangedNotification.NewValue, propertyChangedNotification.NotificationId);
        });

    #endregion

    #region Children

    private void OnRemoteChildAdded(ChildAddedNotification childAddedNotification) =>
        SuppressEventForwarding(childAddedNotification, () =>
        {
            var localParent = Lookup(childAddedNotification.Parent.GetId());
            var newChildNode = (INode)childAddedNotification.NewChild;

            var clone = Clone(newChildNode);

            Debug.WriteLine(
                $"Parent {localParent.PrintIdentity()}: Adding {clone.PrintIdentity()} to {childAddedNotification.Containment} at {childAddedNotification.Index}");
            var newValue = InsertContainment(localParent, childAddedNotification.Containment, childAddedNotification.Index, clone);

            localParent.Set(childAddedNotification.Containment, newValue, childAddedNotification.NotificationId);
        });

    private void OnRemoteChildDeleted(ChildDeletedNotification childDeletedNotification) =>
        SuppressEventForwarding(childDeletedNotification, () =>
        {
            var localParent = Lookup(childDeletedNotification.Parent.GetId());

            object? newValue = null;
            if (childDeletedNotification.Containment.Multiple)
            {
                var existingChildren = localParent.Get(childDeletedNotification.Containment);
                if (existingChildren is IList l)
                {
                    var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                    children.RemoveAt(childDeletedNotification.Index);
                    newValue = children;
                }
            }

            localParent.Set(childDeletedNotification.Containment, newValue, childDeletedNotification.NotificationId);
        });

    private void OnRemoteChildReplaced(ChildReplacedNotification childReplacedNotification) =>
        SuppressEventForwarding(childReplacedNotification, () =>
        {
            var localParent = Lookup(childReplacedNotification.Parent.GetId());
            var substituteNode = Clone((INode)childReplacedNotification.NewChild);
            var newValue = ReplaceContainment(localParent, childReplacedNotification.Containment, childReplacedNotification.Index, substituteNode);

            localParent.Set(childReplacedNotification.Containment, newValue, childReplacedNotification.NotificationId);
        });

    private void OnRemoteChildMovedFromOtherContainment(ChildMovedFromOtherContainmentNotification childMovedNotification) =>
        SuppressEventForwarding(childMovedNotification, () =>
        {
            var localNewParent = Lookup(childMovedNotification.NewParent.GetId());
            var nodeToInsert = LookupOpt(childMovedNotification.MovedChild.GetId()) ?? Clone((INode)childMovedNotification.MovedChild);
            var newValue = InsertContainment(localNewParent, childMovedNotification.NewContainment, childMovedNotification.NewIndex, nodeToInsert);

            localNewParent.Set(childMovedNotification.NewContainment, newValue, childMovedNotification.NotificationId);
        });

    private void OnRemoteChildMovedAndReplacedFromOtherContainment(ChildMovedAndReplacedFromOtherContainmentNotification childMovedAndReplacedNotification) => SuppressEventForwarding(childMovedAndReplacedNotification, () =>
    {
        var localNewParent = Lookup(childMovedAndReplacedNotification.NewParent.GetId());
        var substituteNode = Lookup(childMovedAndReplacedNotification.MovedChild.GetId());
        var newValue = ReplaceContainment(localNewParent, childMovedAndReplacedNotification.NewContainment, childMovedAndReplacedNotification.NewIndex,
            substituteNode);

        localNewParent.Set(childMovedAndReplacedNotification.NewContainment, newValue, childMovedAndReplacedNotification.NotificationId);
    });

    private void OnRemoteChildMovedAndReplacedFromOtherContainmentInSameParent(ChildMovedAndReplacedFromOtherContainmentInSameParentNotification childMovedAndReplacedNotification)
    {
        var parent = Lookup(childMovedAndReplacedNotification.Parent.GetId());
        var substituteNode = Lookup(childMovedAndReplacedNotification.MovedChild.GetId());
        var newValue = ReplaceContainment(parent, childMovedAndReplacedNotification.NewContainment, childMovedAndReplacedNotification.NewIndex, substituteNode);
        
        parent.Set(childMovedAndReplacedNotification.NewContainment, newValue);
    }
    
    private void OnRemoteChildMovedFromOtherContainmentInSameParent(ChildMovedFromOtherContainmentInSameParentNotification childMovedNotification) =>
        SuppressEventForwarding(childMovedNotification, () =>
        {
            var localParent = Lookup(childMovedNotification.Parent.GetId());
            var newValue = InsertContainment(localParent, childMovedNotification.NewContainment, childMovedNotification.NewIndex,
                Lookup(childMovedNotification.MovedChild.GetId()));

            localParent.Set(childMovedNotification.NewContainment, newValue, childMovedNotification.NotificationId);
        });

    private void OnRemoteChildMovedInSameContainment(ChildMovedInSameContainmentNotification childMovedNotification) =>
        SuppressEventForwarding(childMovedNotification, () =>
        {
            var localParent = Lookup(childMovedNotification.Parent.GetId());
            INode nodeToInsert = Lookup(childMovedNotification.MovedChild.GetId());
            object newValue = nodeToInsert;
            var existingChildren = localParent.Get(childMovedNotification.Containment);
            if (existingChildren is IList l)
            {
                var children = new List<IWritableNode>(l.Cast<IWritableNode>());
                children.RemoveAt(childMovedNotification.OldIndex);
                children.Insert(childMovedNotification.NewIndex, nodeToInsert);
                newValue = children;
            }

            localParent.Set(childMovedNotification.Containment, newValue, childMovedNotification.NotificationId);
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

    private void OnRemoteAnnotationAdded(AnnotationAddedNotification annotationAddedNotification) =>
        SuppressEventForwarding(annotationAddedNotification, () =>
        {
            var localParent = Lookup(annotationAddedNotification.Parent.GetId());
            var clone = Clone((INode)annotationAddedNotification.NewAnnotation);
            localParent.InsertAnnotations(annotationAddedNotification.Index, [clone], annotationAddedNotification.NotificationId);
        });

    private void OnRemoteAnnotationDeleted(AnnotationDeletedNotification annotationDeletedNotification) =>
        SuppressEventForwarding(annotationDeletedNotification, () =>
        {
            var localParent = Lookup(annotationDeletedNotification.Parent.GetId());
            var localDeleted = Lookup(annotationDeletedNotification.DeletedAnnotation.GetId());
            localParent.RemoveAnnotations([localDeleted], annotationDeletedNotification.NotificationId);
        });

    private void OnRemoteAnnotationMovedFromOtherParent(AnnotationMovedFromOtherParentNotification annotationMovedNotification) =>
        SuppressEventForwarding(annotationMovedNotification, () =>
        {
            var localNewParent = Lookup(annotationMovedNotification.NewParent.GetId());
            var moved = LookupOpt(annotationMovedNotification.MovedAnnotation.GetId()) ?? Clone((INode)annotationMovedNotification.MovedAnnotation);
            localNewParent.InsertAnnotations(annotationMovedNotification.NewIndex, [moved], annotationMovedNotification.NotificationId);
        });

    private void OnRemoteAnnotationMovedInSameParent(AnnotationMovedInSameParentNotification annotationMovedNotification) =>
        SuppressEventForwarding(annotationMovedNotification, () =>
        {
            var localParent = Lookup(annotationMovedNotification.Parent.GetId());
            INode nodeToInsert = Lookup(annotationMovedNotification.MovedAnnotation.GetId());
            localParent.InsertAnnotations(annotationMovedNotification.NewIndex, [nodeToInsert], annotationMovedNotification.NotificationId);
        });

    #endregion

    #region References

    private void OnRemoteReferenceAdded(ReferenceAddedNotification referenceAddedNotification) =>
        SuppressEventForwarding(referenceAddedNotification, () =>
        {
            var localParent = Lookup(referenceAddedNotification.Parent.GetId());
            INode target = Lookup(referenceAddedNotification.NewTarget.Reference.GetId());
            var newValue = InsertReference(localParent, referenceAddedNotification.Reference, referenceAddedNotification.Index, target);

            localParent.Set(referenceAddedNotification.Reference, newValue, referenceAddedNotification.NotificationId);
        });

    private void OnRemoteReferenceDeleted(ReferenceDeletedNotification referenceDeletedNotification) =>
        SuppressEventForwarding(referenceDeletedNotification, () =>
        {
            var localParent = Lookup(referenceDeletedNotification.Parent.GetId());

            object newValue = null;
            if (referenceDeletedNotification.Reference.Multiple)
            {
                var existingTargets = localParent.Get(referenceDeletedNotification.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.RemoveAt(referenceDeletedNotification.Index);
                    newValue = targets;
                }
            }

            localParent.Set(referenceDeletedNotification.Reference, newValue, referenceDeletedNotification.NotificationId);
        });

    private void OnRemoteReferenceChanged(ReferenceChangedNotification referenceChangedNotification) =>
        SuppressEventForwarding(referenceChangedNotification, () =>
        {
            var localParent = Lookup(referenceChangedNotification.Parent.GetId());

            object newValue = Lookup(referenceChangedNotification.NewTarget.Reference.GetId());
            if (referenceChangedNotification.Reference.Multiple)
            {
                var existingTargets = localParent.Get(referenceChangedNotification.Reference);
                if (existingTargets is IList l)
                {
                    var targets = new List<IReadableNode>(l.Cast<IReadableNode>());
                    targets.Insert(referenceChangedNotification.Index, (IReadableNode)newValue);
                    targets.RemoveAt(referenceChangedNotification.Index + 1);
                    newValue = targets;
                }
            }

            localParent.Set(referenceChangedNotification.Reference, newValue,  referenceChangedNotification.NotificationId);
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