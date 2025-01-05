// Copyright 2024 TRUMPF Laser SE and other contributors
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
using Index = int;
using PropertyValue = object;
using ResolveInfo = string;
using TargetNode = IReadableNode;

public class ForestEventHandler : IForestListener, IForestCommander
{
    private readonly object _sender;

    public ForestEventHandler(object? sender = null)
    {
        _sender = sender ?? this;
    }

    /// <inheritdoc />
    public event EventHandler<IForestListener.NewPartitionArgs>? NewPartition;

    /// <inheritdoc />
    public void AddPartition(IReadableNode newPartition) =>
        NewPartition?.Invoke(_sender, new(newPartition));

    /// <inheritdoc />
    public event EventHandler<IForestListener.PartitionDeletedArgs>? PartitionDeleted;

    /// <inheritdoc />
    public void DeletePartition(IReadableNode deletedPartition) =>
        PartitionDeleted?.Invoke(_sender, new(deletedPartition));
}

public class PartitionEventHandler : IPartitionListener, IPartitionCommander
{
    private readonly object _sender;

    public PartitionEventHandler(object? sender = null)
    {
        _sender = sender ?? this;
    }

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ClassifierChangedArgs>? ClassifierChanged;

    /// <inheritdoc />
    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier) =>
        ClassifierChanged?.Invoke(_sender, new(node, newClassifier, oldClassifier));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.PropertyAddedArgs>? PropertyAdded;

    /// <inheritdoc />
    public void AddProperty(IWritableNode node, Property property, PropertyValue newValue) =>
        PropertyAdded?.Invoke(_sender, new(node, property, newValue));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.PropertyDeletedArgs>? PropertyDeleted;

    /// <inheritdoc />
    public void DeleteProperty(IWritableNode node, Property property, PropertyValue oldValue) =>
        PropertyDeleted?.Invoke(_sender, new(node, property, oldValue));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.PropertyChangedArgs>? PropertyChanged;

    /// <inheritdoc />
    public void ChangeProperty(IWritableNode node, Property property, PropertyValue newValue, PropertyValue oldValue) =>
        PropertyChanged?.Invoke(_sender, new(node, property, newValue, oldValue));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildAddedArgs>? ChildAdded;

    /// <inheritdoc />
    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index) =>
        ChildAdded?.Invoke(_sender, new(parent, newChild, containment, index));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildDeletedArgs>? ChildDeleted;

    /// <inheritdoc />
    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index) =>
        ChildDeleted?.Invoke(_sender, new(deletedChild, parent, containment, index));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildReplacedArgs>? ChildReplaced;

    /// <inheritdoc />
    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment,
        Index index) =>
        ChildReplaced?.Invoke(_sender, new(newChild, replacedChild, parent, containment, index));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildMovedFromOtherContainmentArgs>? ChildMovedFromOtherContainment;

    /// <inheritdoc />
    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex) =>
        ChildMovedFromOtherContainment?.Invoke(_sender,
            new(newParent, newContainment, newIndex, movedChild, oldParent, oldContainment, oldIndex));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildMovedFromOtherContainmentInSameParentArgs>?
        ChildMovedFromOtherContainmentInSameParent;

    /// <inheritdoc />
    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex) =>
        ChildMovedFromOtherContainmentInSameParent?.Invoke(_sender,
            new(newContainment, newIndex, movedChild, parent, oldContainment, oldIndex));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildMovedInSameContainmentArgs>? ChildMovedInSameContainment;

    /// <inheritdoc />
    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment,
        Index oldIndex) =>
        ChildMovedInSameContainment?.Invoke(_sender, new(newIndex, movedChild, parent, containment, oldIndex));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationAddedArgs>? AnnotationAdded;

    /// <inheritdoc />
    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index) =>
        AnnotationAdded?.Invoke(_sender, new(parent, newAnnotation, index));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationDeletedArgs>? AnnotationDeleted;

    /// <inheritdoc />
    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index) =>
        AnnotationDeleted?.Invoke(_sender, new(deletedAnnotation, parent, index));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationReplacedArgs>? AnnotationReplaced;

    /// <inheritdoc />
    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index) =>
        AnnotationReplaced?.Invoke(_sender, new(newAnnotation, replacedAnnotation, parent, index));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationMovedFromOtherParentArgs>? AnnotationMovedFromOtherParent;

    /// <inheritdoc />
    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex) =>
        AnnotationMovedFromOtherParent?.Invoke(_sender, new(newParent, newIndex, movedAnnotation, oldParent, oldIndex));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationMovedInSameParentArgs>? AnnotationMovedInSameParent;

    /// <inheritdoc />
    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex) =>
        AnnotationMovedInSameParent?.Invoke(_sender, new(newIndex, movedAnnotation, parent, oldIndex));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceAddedArgs> ReferenceAdded
    {
        add => _referenceAdded.Add(value);
        remove => _referenceAdded.Remove(value);
    }

    private readonly CountingEventHandler<IPartitionListener.ReferenceAddedArgs> _referenceAdded = new();

    /// <inheritdoc />
    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget) =>
        _referenceAdded.Invoke(_sender, new(parent, reference, index, newTarget));

    /// <inheritdoc />
    public bool CanRaiseAddReference() => _referenceAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceDeletedArgs> ReferenceDeleted
    {
        add => _referenceDeleted.Add(value);
        remove => _referenceDeleted.Remove(value);
    }

    private readonly CountingEventHandler<IPartitionListener.ReferenceDeletedArgs> _referenceDeleted = new();

    /// <inheritdoc />
    public void DeleteReference(IWritableNode parent, Reference reference, Index index,
        IReferenceTarget deletedTarget) =>
        _referenceDeleted.Invoke(_sender, new(parent, reference, index, deletedTarget));

    /// <inheritdoc />
    public bool CanRaiseDeleteReference() => _referenceDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceChangedArgs> ReferenceChanged
    {
        add => _referenceChanged.Add(value);
        remove => _referenceChanged.Remove(value);
    }

    private readonly CountingEventHandler<IPartitionListener.ReferenceChangedArgs> _referenceChanged = new();

    /// <inheritdoc />
    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget) =>
        _referenceChanged.Invoke(_sender, new(parent, reference, index, newTarget, replacedTarget));

    /// <inheritdoc />
    public bool CanRaiseChangeReference() => _referenceChanged.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.EntryMovedFromOtherReferenceArgs>? EntryMovedFromOtherReference;

    /// <inheritdoc />
    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target) =>
        EntryMovedFromOtherReference?.Invoke(_sender,
            new(newParent, newReference, newIndex, oldParent, oldReference, oldIndex, target));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.EntryMovedFromOtherReferenceInSameParentArgs>?
        EntryMovedFromOtherReferenceInSameParent;

    /// <inheritdoc />
    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target) =>
        EntryMovedFromOtherReferenceInSameParent?.Invoke(_sender,
            new(parent, newReference, newIndex, oldReference, oldIndex, target));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.EntryMovedInSameReferenceArgs> EntryMovedInSameReference
    {
        add => _entryMovedInSameReference.Add(value);
        remove => _entryMovedInSameReference.Remove(value);
    }

    private readonly CountingEventHandler<IPartitionListener.EntryMovedInSameReferenceArgs> _entryMovedInSameReference =
        new();

    /// <inheritdoc />
    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target) =>
        _entryMovedInSameReference.Invoke(_sender, new(parent, reference, newIndex, oldIndex, target));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryInSameReference() => _entryMovedInSameReference.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceResolveInfoAddedArgs>? ReferenceResolveInfoAdded;

    /// <inheritdoc />
    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        TargetNode target) =>
        ReferenceResolveInfoAdded?.Invoke(_sender, new(parent, reference, index, newResolveInfo, target));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceResolveInfoDeletedArgs>? ReferenceResolveInfoDeleted;

    /// <inheritdoc />
    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, TargetNode target,
        ResolveInfo deletedResolveInfo) =>
        ReferenceResolveInfoDeleted?.Invoke(_sender, new(parent, reference, index, target, deletedResolveInfo));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceResolveInfoChangedArgs>? ReferenceResolveInfoChanged;

    /// <inheritdoc />
    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        TargetNode? target, ResolveInfo replacedResolveInfo) =>
        ReferenceResolveInfoChanged?.Invoke(_sender,
            new(parent, reference, index, newResolveInfo, target, replacedResolveInfo));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceTargetAddedArgs>? ReferenceTargetAdded;

    /// <inheritdoc />
    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo resolveInfo) =>
        ReferenceTargetAdded?.Invoke(_sender, new(parent, reference, index, newTarget, resolveInfo));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceTargetDeletedArgs>? ReferenceTargetDeleted;

    /// <inheritdoc />
    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        TargetNode deletedTarget) =>
        ReferenceTargetDeleted?.Invoke(_sender, new(parent, reference, index, resolveInfo, deletedTarget));

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceTargetChangedArgs>? ReferenceTargetChanged;

    /// <inheritdoc />
    public void ChangedReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo? resolveInfo, TargetNode oldTarget) =>
        ReferenceTargetChanged?.Invoke(_sender, new(parent, reference, index, newTarget, resolveInfo, oldTarget));
}

class CountingEventHandler<T>
{
    private int _subscriberCount = 0;
    public event EventHandler<T>? Event;

    public void Invoke(object sender, T args)
    {
        if (HasSubscribers && Event != null)
            Event.Invoke(sender, args);
    }

    public void Add(EventHandler<T> handler)
    {
        lock (this)
        {
            Event += handler;
            _subscriberCount++;
        }
    }

    public void Remove(EventHandler<T> handler)
    {
        lock (this)
        {
            Event -= handler;
            _subscriberCount--;
        }
    }

    public bool HasSubscribers
    {
        get
        {
            lock (this)
            {
                return _subscriberCount > 0;
            }
        }
    }
}