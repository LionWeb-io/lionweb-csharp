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
using PropertyValue = object;
using SemanticPropertyValue = object;
using TargetNode = IReadableNode;

/// Forwards <see cref="IForestCommander"/> commands to <see cref="IForestListener"/> events.
public class ForestEventHandler : IForestListener, IForestCommander
{
    private readonly object _sender;

    /// <inheritdoc cref="ForestEventHandler"/>
    /// <param name="sender">Optional sender of the events.</param>
    public ForestEventHandler(object? sender)
    {
        _sender = sender ?? this;
    }

    /// <inheritdoc />
    public event EventHandler<IForestListener.NewPartitionArgs> NewPartition
    {
        add => _newPartition.Add(value);
        remove => _newPartition.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IForestListener.NewPartitionArgs> _newPartition = new();

    /// <inheritdoc />
    public void AddPartition(IPartitionInstance newPartition) =>
        _newPartition.Invoke(_sender, new(newPartition));

    /// <inheritdoc />
    public bool CanRaiseAddPartition => _newPartition.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IForestListener.PartitionDeletedArgs> PartitionDeleted
    {
        add => _partitionDeleted.Add(value);
        remove => _partitionDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IForestListener.PartitionDeletedArgs> _partitionDeleted = new();

    /// <inheritdoc />
    public void DeletePartition(IPartitionInstance deletedPartition) =>
        _partitionDeleted.Invoke(_sender, new(deletedPartition));

    /// <inheritdoc />
    public bool CanRaiseDeletePartition => _partitionDeleted.HasSubscribers;
}

/// Forwards <see cref="IPartitionCommander"/> commands to <see cref="IPartitionListener"/> events.
public class PartitionEventHandler : IPartitionListener, IPartitionCommander
{
    private readonly object _sender;

    /// <inheritdoc cref="PartitionEventHandler"/>
    /// <param name="sender">Optional sender of the events.</param>
    public PartitionEventHandler(object? sender)
    {
        _sender = sender ?? this;
    }

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ClassifierChangedArgs> ClassifierChanged
    {
        add => _classifierChanged.Add(value);
        remove => _classifierChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ClassifierChangedArgs> _classifierChanged = new();

    /// <inheritdoc />
    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier) =>
        _classifierChanged.Invoke(_sender, new(node, newClassifier, oldClassifier));

    /// <inheritdoc />
    public bool CanRaiseChangeClassifier => _classifierChanged.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.PropertyAddedArgs> PropertyAdded
    {
        add => _propertyAdded.Add(value);
        remove => _propertyAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.PropertyAddedArgs> _propertyAdded = new();

    /// <inheritdoc />
    public void AddProperty(IWritableNode node, Property property, PropertyValue newValue) =>
        _propertyAdded.Invoke(_sender, new(node, property, newValue));

    /// <inheritdoc />
    public bool CanRaiseAddProperty => _propertyAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.PropertyDeletedArgs> PropertyDeleted
    {
        add => _propertyDeleted.Add(value);
        remove => _propertyDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.PropertyDeletedArgs> _propertyDeleted = new();

    /// <inheritdoc />
    public void DeleteProperty(IWritableNode node, Property property, PropertyValue oldValue) =>
        _propertyDeleted.Invoke(_sender, new(node, property, oldValue));

    /// <inheritdoc />
    public bool CanRaiseDeleteProperty => _propertyDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.PropertyChangedArgs> PropertyChanged
    {
        add => _propertyChanged.Add(value);
        remove => _propertyChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.PropertyChangedArgs> _propertyChanged = new();

    /// <inheritdoc />
    public void ChangeProperty(IWritableNode node, Property property, PropertyValue newValue, PropertyValue oldValue) =>
        _propertyChanged.Invoke(_sender, new(node, property, newValue, oldValue));

    /// <inheritdoc />
    public bool CanRaiseChangeProperty => _propertyChanged.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildAddedArgs> ChildAdded
    {
        add => _childAdded.Add(value);
        remove => _childAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ChildAddedArgs> _childAdded = new();

    /// <inheritdoc />
    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index) =>
        _childAdded.Invoke(_sender, new(parent, newChild, containment, index));

    /// <inheritdoc />
    public bool CanRaiseAddChild => _childAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildDeletedArgs> ChildDeleted
    {
        add => _childDeleted.Add(value);
        remove => _childDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ChildDeletedArgs> _childDeleted = new();

    /// <inheritdoc />
    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index) =>
        _childDeleted.Invoke(_sender, new(deletedChild, parent, containment, index));

    /// <inheritdoc />
    public bool CanRaiseDeleteChild => _childDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildReplacedArgs> ChildReplaced
    {
        add => _childReplaced.Add(value);
        remove => _childReplaced.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ChildReplacedArgs> _childReplaced = new();

    /// <inheritdoc />
    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment,
        Index index) =>
        _childReplaced.Invoke(_sender, new(newChild, replacedChild, parent, containment, index));

    /// <inheritdoc />
    public bool CanRaiseReplaceChild => _childReplaced.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildMovedFromOtherContainmentArgs> ChildMovedFromOtherContainment
    {
        add => _childMovedFromOtherContainment.Add(value);
        remove => _childMovedFromOtherContainment.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ChildMovedFromOtherContainmentArgs>
        _childMovedFromOtherContainment = new();

    /// <inheritdoc />
    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex) =>
        _childMovedFromOtherContainment.Invoke(_sender,
            new(newParent, newContainment, newIndex, movedChild, oldParent, oldContainment, oldIndex));

    /// <inheritdoc />
    public bool CanRaiseMoveChildFromOtherContainment => _childMovedFromOtherContainment.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildMovedFromOtherContainmentInSameParentArgs>
        ChildMovedFromOtherContainmentInSameParent
        {
            add => _childMovedFromOtherContainmentInSameParent.Add(value);
            remove => _childMovedFromOtherContainmentInSameParent.Remove(value);
        }

    private readonly ShortCircuitEventHandler<IPartitionListener.ChildMovedFromOtherContainmentInSameParentArgs>
        _childMovedFromOtherContainmentInSameParent = new();

    /// <inheritdoc />
    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex) =>
        _childMovedFromOtherContainmentInSameParent.Invoke(_sender,
            new(newContainment, newIndex, movedChild, parent, oldContainment, oldIndex));

    /// <inheritdoc />
    public bool CanRaiseMoveChildFromOtherContainmentInSameParent =>
        _childMovedFromOtherContainmentInSameParent.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ChildMovedInSameContainmentArgs> ChildMovedInSameContainment
    {
        add => _childMovedInSameContainment.Add(value);
        remove => _childMovedInSameContainment.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ChildMovedInSameContainmentArgs>
        _childMovedInSameContainment = new();

    /// <inheritdoc />
    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment,
        Index oldIndex) =>
        _childMovedInSameContainment.Invoke(_sender, new(newIndex, movedChild, parent, containment, oldIndex));

    /// <inheritdoc />
    public bool CanRaiseMoveChildInSameContainment => _childMovedInSameContainment.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationAddedArgs> AnnotationAdded
    {
        add => _annotationAdded.Add(value);
        remove => _annotationAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.AnnotationAddedArgs> _annotationAdded = new();

    /// <inheritdoc />
    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index) =>
        _annotationAdded.Invoke(_sender, new(parent, newAnnotation, index));

    /// <inheritdoc />
    public bool CanRaiseAddAnnotation => _annotationAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationDeletedArgs> AnnotationDeleted
    {
        add => _annotationDeleted.Add(value);
        remove => _annotationDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.AnnotationDeletedArgs> _annotationDeleted = new();

    /// <inheritdoc />
    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index) =>
        _annotationDeleted.Invoke(_sender, new(deletedAnnotation, parent, index));

    /// <inheritdoc />
    public bool CanRaiseDeleteAnnotation => _annotationDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationReplacedArgs> AnnotationReplaced
    {
        add => _annotationReplaced.Add(value);
        remove => _annotationReplaced.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.AnnotationReplacedArgs> _annotationReplaced = new();

    /// <inheritdoc />
    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index) =>
        _annotationReplaced.Invoke(_sender, new(newAnnotation, replacedAnnotation, parent, index));

    /// <inheritdoc />
    public bool CanRaiseReplaceAnnotation => _annotationReplaced.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationMovedFromOtherParentArgs> AnnotationMovedFromOtherParent
    {
        add => _annotationMovedFromOtherParent.Add(value);
        remove => _annotationMovedFromOtherParent.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.AnnotationMovedFromOtherParentArgs>
        _annotationMovedFromOtherParent = new();

    /// <inheritdoc />
    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex) =>
        _annotationMovedFromOtherParent.Invoke(_sender, new(newParent, newIndex, movedAnnotation, oldParent, oldIndex));

    /// <inheritdoc />
    public bool CanRaiseMoveAnnotationFromOtherParent => _annotationMovedFromOtherParent.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.AnnotationMovedInSameParentArgs> AnnotationMovedInSameParent
    {
        add => _annotationMovedInSameParent.Add(value);
        remove => _annotationMovedInSameParent.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.AnnotationMovedInSameParentArgs>
        _annotationMovedInSameParent = new();

    /// <inheritdoc />
    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex) =>
        _annotationMovedInSameParent.Invoke(_sender, new(newIndex, movedAnnotation, parent, oldIndex));

    /// <inheritdoc />
    public bool CanRaiseMoveAnnotationInSameParent => _annotationMovedInSameParent.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceAddedArgs> ReferenceAdded
    {
        add => _referenceAdded.Add(value);
        remove => _referenceAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ReferenceAddedArgs> _referenceAdded = new();

    /// <inheritdoc />
    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget) =>
        _referenceAdded.Invoke(_sender, new(parent, reference, index, newTarget));

    /// <inheritdoc />
    public bool CanRaiseAddReference => _referenceAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceDeletedArgs> ReferenceDeleted
    {
        add => _referenceDeleted.Add(value);
        remove => _referenceDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ReferenceDeletedArgs> _referenceDeleted = new();

    /// <inheritdoc />
    public void DeleteReference(IWritableNode parent, Reference reference, Index index,
        IReferenceTarget deletedTarget) =>
        _referenceDeleted.Invoke(_sender, new(parent, reference, index, deletedTarget));

    /// <inheritdoc />
    public bool CanRaiseDeleteReference => _referenceDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceChangedArgs> ReferenceChanged
    {
        add => _referenceChanged.Add(value);
        remove => _referenceChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ReferenceChangedArgs> _referenceChanged = new();

    /// <inheritdoc />
    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget) =>
        _referenceChanged.Invoke(_sender, new(parent, reference, index, newTarget, replacedTarget));

    /// <inheritdoc />
    public bool CanRaiseChangeReference => _referenceChanged.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.EntryMovedFromOtherReferenceArgs> EntryMovedFromOtherReference
    {
        add => _entryMovedFromOtherReference.Add(value);
        remove => _entryMovedFromOtherReference.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.EntryMovedFromOtherReferenceArgs>
        _entryMovedFromOtherReference = new();

    /// <inheritdoc />
    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target) =>
        _entryMovedFromOtherReference.Invoke(_sender,
            new(newParent, newReference, newIndex, oldParent, oldReference, oldIndex, target));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryFromOtherReference => _entryMovedFromOtherReference.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.EntryMovedFromOtherReferenceInSameParentArgs>
        EntryMovedFromOtherReferenceInSameParent
        {
            add => _entryMovedFromOtherReferenceInSameParent.Add(value);
            remove => _entryMovedFromOtherReferenceInSameParent.Remove(value);
        }

    private readonly ShortCircuitEventHandler<IPartitionListener.EntryMovedFromOtherReferenceInSameParentArgs>
        _entryMovedFromOtherReferenceInSameParent = new();

    /// <inheritdoc />
    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target) =>
        _entryMovedFromOtherReferenceInSameParent.Invoke(_sender,
            new(parent, newReference, newIndex, oldReference, oldIndex, target));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryFromOtherReferenceInSameParent =>
        _entryMovedFromOtherReferenceInSameParent.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.EntryMovedInSameReferenceArgs> EntryMovedInSameReference
    {
        add => _entryMovedInSameReference.Add(value);
        remove => _entryMovedInSameReference.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.EntryMovedInSameReferenceArgs>
        _entryMovedInSameReference =
            new();

    /// <inheritdoc />
    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target) =>
        _entryMovedInSameReference.Invoke(_sender, new(parent, reference, newIndex, oldIndex, target));

    /// <inheritdoc />
    public bool CanRaiseMoveEntryInSameReference => _entryMovedInSameReference.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceResolveInfoAddedArgs> ReferenceResolveInfoAdded
    {
        add => _referenceResolveInfoAdded.Add(value);
        remove => _referenceResolveInfoAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ReferenceResolveInfoAddedArgs>
        _referenceResolveInfoAdded =
            new();

    /// <inheritdoc />
    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        TargetNode target) =>
        _referenceResolveInfoAdded.Invoke(_sender, new(parent, reference, index, newResolveInfo, target));

    /// <inheritdoc />
    public bool CanRaiseAddReferenceResolveInfo => _referenceResolveInfoAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceResolveInfoDeletedArgs> ReferenceResolveInfoDeleted
    {
        add => _referenceResolveInfoDeleted.Add(value);
        remove => _referenceResolveInfoDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ReferenceResolveInfoDeletedArgs>
        _referenceResolveInfoDeleted = new();

    /// <inheritdoc />
    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, TargetNode target,
        ResolveInfo deletedResolveInfo) =>
        _referenceResolveInfoDeleted.Invoke(_sender, new(parent, reference, index, target, deletedResolveInfo));

    /// <inheritdoc />
    public bool CanRaiseDeleteReferenceResolveInfo => _referenceResolveInfoDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceResolveInfoChangedArgs> ReferenceResolveInfoChanged
    {
        add => _referenceResolveInfoChanged.Add(value);
        remove => _referenceResolveInfoChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ReferenceResolveInfoChangedArgs>
        _referenceResolveInfoChanged = new();

    /// <inheritdoc />
    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        TargetNode? target, ResolveInfo replacedResolveInfo) =>
        _referenceResolveInfoChanged.Invoke(_sender,
            new(parent, reference, index, newResolveInfo, target, replacedResolveInfo));

    /// <inheritdoc />
    public bool CanRaiseChangeReferenceResolveInfo => _referenceResolveInfoChanged.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceTargetAddedArgs> ReferenceTargetAdded
    {
        add => _referenceTargetAdded.Add(value);
        remove => _referenceTargetAdded.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ReferenceTargetAddedArgs>
        _referenceTargetAdded = new();

    /// <inheritdoc />
    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo resolveInfo) =>
        _referenceTargetAdded.Invoke(_sender, new(parent, reference, index, newTarget, resolveInfo));

    /// <inheritdoc />
    public bool CanRaiseAddReferenceTarget => _referenceTargetAdded.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceTargetDeletedArgs> ReferenceTargetDeleted
    {
        add => _referenceTargetDeleted.Add(value);
        remove => _referenceTargetDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ReferenceTargetDeletedArgs>
        _referenceTargetDeleted = new();

    /// <inheritdoc />
    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        TargetNode deletedTarget) =>
        _referenceTargetDeleted.Invoke(_sender, new(parent, reference, index, resolveInfo, deletedTarget));

    /// <inheritdoc />
    public bool CanRaiseDeleteReferenceTarget => _referenceTargetDeleted.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IPartitionListener.ReferenceTargetChangedArgs> ReferenceTargetChanged
    {
        add => _referenceTargetChanged.Add(value);
        remove => _referenceTargetChanged.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IPartitionListener.ReferenceTargetChangedArgs>
        _referenceTargetChanged = new();

    /// <inheritdoc />
    public void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo? resolveInfo, TargetNode oldTarget) =>
        _referenceTargetChanged.Invoke(_sender, new(parent, reference, index, newTarget, resolveInfo, oldTarget));

    /// <inheritdoc />
    public bool CanRaiseChangeReferenceTarget => _referenceTargetChanged.HasSubscribers;
}

/// Event handler that allows to check for existing subscribers.
/// Used to avoid expensive event argument preparation if nobody is listening.
internal class ShortCircuitEventHandler<T>
{
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
        }
    }

    public void Remove(EventHandler<T> handler)
    {
        lock (this)
        {
            Event -= handler;
        }
    }

    public bool HasSubscribers
    {
        get
        {
            lock (this)
            {
                return Event != null;
            }
        }
    }
}

public class CallSensitivePartitionCommander : IPartitionCommander, IOverridableCommander<IPartitionCommander>
{
    private readonly IPartitionCommander _defaultDelegate;
    private readonly AsyncLocal<IPartitionCommander?> _delegate = new();

    public IPartitionCommander? Delegate
    {
        get => _delegate.Value;
        set => _delegate.Value = value;
    }

    public CallSensitivePartitionCommander(IPartitionCommander defaultDelegate)
    {
        _defaultDelegate = defaultDelegate;
    }

    private IPartitionCommander TargetDelegate => Delegate ?? _defaultDelegate;

    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier) =>
        TargetDelegate.ChangeClassifier(node, newClassifier, oldClassifier);

    public bool CanRaiseChangeClassifier => TargetDelegate.CanRaiseChangeClassifier;

    public void AddProperty(IWritableNode node, Property property, SemanticPropertyValue newValue) =>
        TargetDelegate.AddProperty(node, property, newValue);

    public bool CanRaiseAddProperty => TargetDelegate.CanRaiseAddProperty;

    public void DeleteProperty(IWritableNode node, Property property, SemanticPropertyValue oldValue) =>
        TargetDelegate.DeleteProperty(node, property, oldValue);

    public bool CanRaiseDeleteProperty => TargetDelegate.CanRaiseDeleteProperty;

    public void ChangeProperty(IWritableNode node, Property property, SemanticPropertyValue newValue,
        SemanticPropertyValue oldValue) =>
        TargetDelegate.ChangeProperty(node, property, newValue, oldValue);

    public bool CanRaiseChangeProperty => TargetDelegate.CanRaiseChangeProperty;

    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index) =>
        TargetDelegate.AddChild(parent, newChild, containment, index);

    public bool CanRaiseAddChild => TargetDelegate.CanRaiseAddChild;

    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index) =>
        TargetDelegate.DeleteChild(deletedChild, parent, containment, index);

    public bool CanRaiseDeleteChild => TargetDelegate.CanRaiseDeleteChild;

    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment,
        Index index) =>
        TargetDelegate.ReplaceChild(newChild, replacedChild, parent, containment, index);

    public bool CanRaiseReplaceChild => TargetDelegate.CanRaiseReplaceChild;

    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex) =>
        TargetDelegate.MoveChildFromOtherContainment(newParent, newContainment, newIndex, movedChild, oldParent,
            oldContainment, oldIndex);

    public bool CanRaiseMoveChildFromOtherContainment => TargetDelegate.CanRaiseMoveChildFromOtherContainment;

    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex) =>
        TargetDelegate.MoveChildFromOtherContainmentInSameParent(newContainment, newIndex, movedChild, parent,
            oldContainment, oldIndex);

    public bool CanRaiseMoveChildFromOtherContainmentInSameParent =>
        TargetDelegate.CanRaiseMoveChildFromOtherContainmentInSameParent;

    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment,
        Index oldIndex) =>
        TargetDelegate.MoveChildInSameContainment(newIndex, movedChild, parent, containment, oldIndex);

    public bool CanRaiseMoveChildInSameContainment => TargetDelegate.CanRaiseMoveChildInSameContainment;

    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index) =>
        TargetDelegate.AddAnnotation(parent, newAnnotation, index);

    public bool CanRaiseAddAnnotation => TargetDelegate.CanRaiseAddAnnotation;

    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index) =>
        TargetDelegate.DeleteAnnotation(deletedAnnotation, parent, index);

    public bool CanRaiseDeleteAnnotation => TargetDelegate.CanRaiseDeleteAnnotation;

    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index) =>
        TargetDelegate.ReplaceAnnotation(newAnnotation, replacedAnnotation, parent, index);

    public bool CanRaiseReplaceAnnotation => TargetDelegate.CanRaiseReplaceAnnotation;

    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex) =>
        TargetDelegate.MoveAnnotationFromOtherParent(newParent, newIndex, movedAnnotation, oldParent, oldIndex);

    public bool CanRaiseMoveAnnotationFromOtherParent => TargetDelegate.CanRaiseMoveAnnotationFromOtherParent;

    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex) => TargetDelegate.MoveAnnotationInSameParent(newIndex, movedAnnotation, parent, oldIndex);

    public bool CanRaiseMoveAnnotationInSameParent => TargetDelegate.CanRaiseMoveAnnotationInSameParent;

    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget) =>
        TargetDelegate.AddReference(parent, reference, index, newTarget);

    public bool CanRaiseAddReference => TargetDelegate.CanRaiseAddReference;

    public void DeleteReference(IWritableNode parent, Reference reference, Index index,
        IReferenceTarget deletedTarget) => TargetDelegate.DeleteReference(parent, reference, index, deletedTarget);

    public bool CanRaiseDeleteReference => TargetDelegate.CanRaiseDeleteReference;

    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget) =>
        TargetDelegate.ChangeReference(parent, reference, index, newTarget, replacedTarget);

    public bool CanRaiseChangeReference => TargetDelegate.CanRaiseChangeReference;

    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target) =>
        TargetDelegate.MoveEntryFromOtherReference(newParent, newReference, newIndex, oldParent, oldReference, oldIndex,
            target);

    public bool CanRaiseMoveEntryFromOtherReference => TargetDelegate.CanRaiseMoveEntryFromOtherReference;

    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target) =>
        TargetDelegate.MoveEntryFromOtherReferenceInSameParent(parent, newReference, newIndex, oldReference, oldIndex,
            target);

    public bool CanRaiseMoveEntryFromOtherReferenceInSameParent =>
        TargetDelegate.CanRaiseMoveEntryFromOtherReferenceInSameParent;

    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target) =>
        TargetDelegate.MoveEntryInSameReference(parent, reference, oldIndex, newIndex, target);

    public bool CanRaiseMoveEntryInSameReference => TargetDelegate.CanRaiseMoveEntryInSameReference;

    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        TargetNode target) =>
        TargetDelegate.AddReferenceResolveInfo(parent, reference, index, newResolveInfo, target);

    public bool CanRaiseAddReferenceResolveInfo => TargetDelegate.CanRaiseAddReferenceResolveInfo;

    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, TargetNode target,
        ResolveInfo deletedResolveInfo) =>
        TargetDelegate.DeleteReferenceResolveInfo(parent, reference, index, target, deletedResolveInfo);

    public bool CanRaiseDeleteReferenceResolveInfo => TargetDelegate.CanRaiseDeleteReferenceResolveInfo;

    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo,
        TargetNode? target, ResolveInfo replacedResolveInfo) =>
        TargetDelegate.ChangeReferenceResolveInfo(parent, reference, index, newResolveInfo, target,
            replacedResolveInfo);

    public bool CanRaiseChangeReferenceResolveInfo => TargetDelegate.CanRaiseChangeReferenceResolveInfo;

    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo resolveInfo) =>
        TargetDelegate.AddReferenceTarget(parent, reference, index, newTarget, resolveInfo);

    public bool CanRaiseAddReferenceTarget => TargetDelegate.CanRaiseAddReferenceTarget;

    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        TargetNode deletedTarget) =>
        TargetDelegate.DeleteReferenceTarget(parent, reference, index, resolveInfo, deletedTarget);

    public bool CanRaiseDeleteReferenceTarget => TargetDelegate.CanRaiseDeleteReferenceTarget;

    public void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo? resolveInfo, TargetNode oldTarget) =>
        TargetDelegate.ChangeReferenceTarget(parent, reference, index, newTarget, resolveInfo, oldTarget);

    public bool CanRaiseChangeReferenceTarget => TargetDelegate.CanRaiseChangeReferenceTarget;
}

public class NoOpPartitionCommander : IPartitionCommander
{
    public void ChangeClassifier(IWritableNode node, Classifier newClassifier, Classifier oldClassifier) { }

    public bool CanRaiseChangeClassifier => false;

    public void AddProperty(IWritableNode node, Property property, SemanticPropertyValue newValue) { }

    public bool CanRaiseAddProperty => false;

    public void DeleteProperty(IWritableNode node, Property property, SemanticPropertyValue oldValue) { }

    public bool CanRaiseDeleteProperty => false;

    public void ChangeProperty(IWritableNode node, Property property, SemanticPropertyValue newValue,
        SemanticPropertyValue oldValue)
    {
    }

    public bool CanRaiseChangeProperty => false;

    public void AddChild(IWritableNode parent, IWritableNode newChild, Containment containment, Index index) { }

    public bool CanRaiseAddChild => false;

    public void DeleteChild(IWritableNode deletedChild, IWritableNode parent, Containment containment, Index index) { }

    public bool CanRaiseDeleteChild => false;

    public void ReplaceChild(IWritableNode newChild, IWritableNode replacedChild, IWritableNode parent,
        Containment containment, Index index)
    {
    }

    public bool CanRaiseReplaceChild => false;

    public void MoveChildFromOtherContainment(IWritableNode newParent, Containment newContainment, Index newIndex,
        IWritableNode movedChild, IWritableNode oldParent, Containment oldContainment, Index oldIndex)
    {
    }

    public bool CanRaiseMoveChildFromOtherContainment => false;

    public void MoveChildFromOtherContainmentInSameParent(Containment newContainment, Index newIndex,
        IWritableNode movedChild,
        IWritableNode parent, Containment oldContainment, Index oldIndex)
    {
    }

    public bool CanRaiseMoveChildFromOtherContainmentInSameParent => false;

    public void MoveChildInSameContainment(Index newIndex, IWritableNode movedChild, IWritableNode parent,
        Containment containment, Index oldIndex)
    {
    }

    public bool CanRaiseMoveChildInSameContainment => false;

    public void AddAnnotation(IWritableNode parent, IWritableNode newAnnotation, Index index) { }

    public bool CanRaiseAddAnnotation => false;

    public void DeleteAnnotation(IWritableNode deletedAnnotation, IWritableNode parent, Index index) { }

    public bool CanRaiseDeleteAnnotation => false;

    public void ReplaceAnnotation(IWritableNode newAnnotation, IWritableNode replacedAnnotation, IWritableNode parent,
        Index index)
    {
    }

    public bool CanRaiseReplaceAnnotation => false;

    public void MoveAnnotationFromOtherParent(IWritableNode newParent, Index newIndex, IWritableNode movedAnnotation,
        IWritableNode oldParent, Index oldIndex)
    {
    }

    public bool CanRaiseMoveAnnotationFromOtherParent => false;

    public void MoveAnnotationInSameParent(Index newIndex, IWritableNode movedAnnotation, IWritableNode parent,
        Index oldIndex)
    {
    }

    public bool CanRaiseMoveAnnotationInSameParent => false;

    public void AddReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget) { }

    public bool CanRaiseAddReference => false;

    public void DeleteReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget deletedTarget)
    {
    }

    public bool CanRaiseDeleteReference => false;

    public void ChangeReference(IWritableNode parent, Reference reference, Index index, IReferenceTarget newTarget,
        IReferenceTarget replacedTarget)
    {
    }

    public bool CanRaiseChangeReference => false;

    public void MoveEntryFromOtherReference(IWritableNode newParent, Reference newReference, Index newIndex,
        IWritableNode oldParent, Reference oldReference, Index oldIndex, IReferenceTarget target)
    {
    }

    public bool CanRaiseMoveEntryFromOtherReference => false;

    public void MoveEntryFromOtherReferenceInSameParent(IWritableNode parent, Reference newReference, Index newIndex,
        Reference oldReference, Index oldIndex, IReferenceTarget target)
    {
    }

    public bool CanRaiseMoveEntryFromOtherReferenceInSameParent => false;

    public void MoveEntryInSameReference(IWritableNode parent, Reference reference, Index oldIndex, Index newIndex,
        IReferenceTarget target)
    {
    }

    public bool CanRaiseMoveEntryInSameReference => false;

    public void AddReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo, TargetNode target)
    {
    }

    public bool CanRaiseAddReferenceResolveInfo => false;

    public void DeleteReferenceResolveInfo(IWritableNode parent, Reference reference, Index index, TargetNode target,
        ResolveInfo deletedResolveInfo)
    {
    }

    public bool CanRaiseDeleteReferenceResolveInfo => false;

    public void ChangeReferenceResolveInfo(IWritableNode parent, Reference reference, Index index,
        ResolveInfo newResolveInfo, TargetNode? target, ResolveInfo replacedResolveInfo)
    {
    }

    public bool CanRaiseChangeReferenceResolveInfo => false;

    public void AddReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo resolveInfo)
    {
    }

    public bool CanRaiseAddReferenceTarget => false;

    public void DeleteReferenceTarget(IWritableNode parent, Reference reference, Index index, ResolveInfo resolveInfo,
        TargetNode deletedTarget)
    {
    }

    public bool CanRaiseDeleteReferenceTarget => false;

    public void ChangeReferenceTarget(IWritableNode parent, Reference reference, Index index, TargetNode newTarget,
        ResolveInfo? resolveInfo, TargetNode oldTarget)
    {
    }

    public bool CanRaiseChangeReferenceTarget => false;
}