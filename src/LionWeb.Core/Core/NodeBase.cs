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

namespace LionWeb.Core;

using M1;
using M2;
using M3;
using Notification;
using Notification.Partition;
using Notification.Partition.Emitter;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

/// Base implementation of <see cref="INode"/>.
public abstract partial class NodeBase : ReadableNodeBase<INode>, INode
{
    /// <summary>
    /// Initializes <c>this</c> node's <see cref="IReadableNode.GetId">id</see>.
    /// </summary>
    /// <param name="id"><c>this</c> node's <see cref="IReadableNode.GetId">id</see>.</param>
    /// <exception cref="InvalidIdException">If <paramref name="id"/> is not a <see cref="IReadableNode.GetId">valid identifier</see>.</exception>
    protected NodeBase(NodeId id) : base(id, null) { }

    /// <inheritdoc />
    void IWritableNode<INode>.SetParent(INode? parent) =>
        _parent = parent;

    /// <inheritdoc />
    bool IWritableNode<INode>.DetachChild(INode child) =>
        DetachChild(child);

    /// <inheritdoc cref="IWritableNode.DetachChild"/>
    protected virtual bool DetachChild(INode child) =>
        _annotations.Remove(child);

    /// <inheritdoc />
    public void DetachFromParent()
    {
        if (_parent != null)
        {
            _parent.DetachChild(this);
            _parent = null;
        }
    }

    /// <inheritdoc />
    public virtual void AddAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null)
    {
        var safeAnnotations = AssureAnnotations(annotations?.ToList());
        int index = Math.Max(_annotations.Count - 1, 0);
        foreach (var safeAnnotation in safeAnnotations)
        {
            AnnotationAddMultipleNotificationEmitter emitter = new(this, safeAnnotation, _annotations, index++);
            emitter.CollectOldData();
            if (AddAnnotationsRaw(safeAnnotation))
                emitter.Notify();
        }
    }

    bool IWritableNodeRaw.AddAnnotationsRaw(IWritableNode annotation) =>
        AddAnnotationsRaw(annotation);

    /// <inheritdoc cref="IWritableNodeRaw.AddAnnotationsRaw"/>
    protected internal bool AddAnnotationsRaw(IWritableNode annotation)
    {
        if (annotation is not IAnnotationInstance ann || !ann.GetAnnotation().CanAnnotate(GetClassifier()))
            return false;

        var node = (INode)annotation;
        AttachChild(node);

        _annotations.Add(node);
        return true;
    }

    /// <inheritdoc />
    public virtual void InsertAnnotations(Index index, IEnumerable<INode> annotations,
        INotificationId? notificationId = null)
    {
        AssureInRange(index, _annotations);
        var safeAnnotations = AssureAnnotations(annotations?.ToList());
        foreach (var safeAnnotation in safeAnnotations)
        {
            AnnotationAddMultipleNotificationEmitter notification = new(this, safeAnnotation, _annotations,
                startIndex: index);
            notification.CollectOldData();
            if (InsertAnnotationsRaw(index++, safeAnnotation))
                notification.Notify();
        }
    }

    bool IWritableNodeRaw.InsertAnnotationsRaw(Index index, IWritableNode annotation) =>
        InsertAnnotationsRaw(index, annotation);

    /// <inheritdoc cref="IWritableNodeRaw.InsertAnnotationsRaw"/>
    protected internal bool InsertAnnotationsRaw(Index index, IWritableNode annotation)
    {
        if (!IsInRange(index, _annotations) || annotation is not IAnnotationInstance ann || !ann.GetAnnotation().CanAnnotate(GetClassifier()))
            return false;

        var node = (INode)annotation;
        AttachChild(node);

        _annotations.Insert(index, node);
        return true;
    }


    /// <inheritdoc />
    public virtual bool RemoveAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null) =>
        RemoveSelfParent(annotations?.ToList(), _annotations, null, AnnotationRemover, notificationId);


    bool IWritableNodeRaw.RemoveAnnotationsRaw(IWritableNode annotation) =>
        RemoveAnnotationsRaw(annotation);

    /// <inheritdoc cref="IWritableNodeRaw.RemoveAnnotationsRaw"/>
    protected internal bool RemoveAnnotationsRaw(IWritableNode annotation)
    {
        if (annotation is not IAnnotationInstance ann || !ann.GetAnnotation().CanAnnotate(GetClassifier()))
            return false;

        return RemoveSelfParent((INode)annotation, _annotations, null);
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() => [];

    /// <inheritdoc />
    public virtual Containment? GetContainmentOf(INode child) => null;

    /// <inheritdoc />
    public sealed override object? Get(Feature? feature)
    {
        if (GetInternal(feature, out var result))
            return result;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc cref="IReadableNode.Get"/>
    protected virtual bool GetInternal(Feature? feature, out object? result)
    {
        if (feature == null)
        {
            result = GetAnnotations();
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value) =>
        GetInternal(feature, out value);

    /// <inheritdoc />
    public void Set(Feature feature, object? value, INotificationId? notificationId = null)
    {
        if (SetInternal(feature, value, notificationId))
            return;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc cref="IWritableNode.Set"/>
    protected virtual bool SetInternal(Feature? feature, object? value, INotificationId? notificationId = null)
    {
        if (feature != null)
            return false;

        if (value is not IEnumerable)
            throw new InvalidValueException(feature, value);
        var safeNodes = M2Extensions.AsNodes<INode>(value, feature).ToList();
        AssureAnnotations(safeNodes);
        AnnotationSetNotificationEmitter notification = new(this, safeNodes, _annotations);
        notification.CollectOldData();
        RemoveSelfParent(_annotations.ToList(), _annotations, null);
        _annotations.AddRange(SetSelfParent(safeNodes, null));
        notification.Notify();
        
        return true;

    }

    bool IWritableNodeRaw.SetRaw(Feature feature, object? value) =>
        SetRaw(feature, value);

    /// <inheritdoc cref="IWritableNodeRaw.SetRaw"/>
    protected internal virtual bool SetRaw(Feature feature, object? value)
    {
        switch (feature, value)
        {
            case (Property f, _):
                return SetPropertyRaw(f, value);

            case (Containment { Multiple: false } f, IWritableNode v):
                return SetContainmentRaw(f, v);
            
            case (Reference { Multiple: false } f, ReferenceTarget v):
                return SetReferenceRaw(f, v);
            
            case (Containment { Multiple: true } f, IEnumerable<IWritableNode> v):
                return TryGetContainmentsRaw(f, out var deletedChildren)
                       && ((IReadOnlyList<IWritableNode>)deletedChildren).All(d => RemoveContainmentsRaw(f, d))
                       && v.All(a => AddContainmentsRaw(f, a));
            
            case (Reference { Multiple: true } f, IEnumerable<IReferenceTarget> v):
                return TryGetReferencesRaw(f, out var deletedTargets)
                       && deletedTargets.All(d => RemoveReferencesRaw(f, (ReferenceTarget)d))
                       && v.All(a => AddReferencesRaw(f, (ReferenceTarget)a));
            
            default:
                return false;
        }
    }

    bool IWritableNodeRaw.SetPropertyRaw(Property property, object? value) =>
        SetPropertyRaw(property, value);

    /// <inheritdoc cref="IWritableNodeRaw.SetPropertyRaw"/>
    protected internal virtual bool SetPropertyRaw(Property property, object? value) =>
        false;

    bool IWritableNodeRaw.SetContainmentRaw(Containment containment, IWritableNode? node) =>
        SetContainmentRaw(containment, node);

    /// <inheritdoc cref="IWritableNodeRaw.SetContainmentRaw"/>
    protected internal virtual bool SetContainmentRaw(Containment containment, IWritableNode? node) =>
        false;

    bool IWritableNodeRaw.SetReferenceRaw(Reference reference, ReferenceTarget? targets) =>
        SetReferenceRaw(reference, targets);

    /// <inheritdoc cref="IWritableNodeRaw.SetReferenceRaw"/>
    protected internal virtual bool SetReferenceRaw(Reference reference, ReferenceTarget? target) =>
        false;

    /// <inheritdoc />
    public void Add(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (AddInternal(link, nodes))
            return;

        throw new UnknownFeatureException(GetClassifier(), link);
    }

    /// <inheritdoc cref="Add"/>
    protected virtual bool AddInternal(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (link == null)
        {
            AddAnnotations(nodes.Cast<INode>());
            return true;
        }

        return false;
    }

    bool IWritableNodeRaw.AddContainmentsRaw(Containment containment, IWritableNode node) =>
        AddContainmentsRaw(containment, node);

    /// <inheritdoc cref="IWritableNodeRaw.AddContainmentsRaw"/>
    protected internal virtual bool AddContainmentsRaw(Containment containment, IWritableNode node) =>
        false;

    bool IWritableNodeRaw.AddReferencesRaw(Reference reference, ReferenceTarget target) =>
        AddReferencesRaw(reference, target);

    /// <inheritdoc cref="IWritableNodeRaw.AddReferencesRaw"/>
    protected internal virtual bool AddReferencesRaw(Reference reference, ReferenceTarget target) =>
        false;

    /// <inheritdoc />
    public void Insert(Link? link, Index index, IEnumerable<IReadableNode> nodes)
    {
        if (InsertInternal(link, index, nodes))
            return;

        throw new UnknownFeatureException(GetClassifier(), link);
    }

    /// <inheritdoc cref="Insert"/>
    protected virtual bool InsertInternal(Link? link, Index index, IEnumerable<IReadableNode> nodes)
    {
        if (link == null)
        {
            InsertAnnotations(index, nodes.Cast<INode>());
            return true;
        }

        return false;
    }

    bool IWritableNodeRaw.InsertContainmentsRaw(Containment containment, Index index, IWritableNode node) =>
        InsertContainmentsRaw(containment, index, node);

    /// <inheritdoc cref="IWritableNodeRaw.InsertContainmentsRaw"/>
    protected internal virtual bool InsertContainmentsRaw(Containment containment, Index index, IWritableNode node) =>
        false;

    bool IWritableNodeRaw.InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target) =>
        InsertReferencesRaw(reference, index, target);

    /// <inheritdoc cref="IWritableNodeRaw.InsertReferencesRaw"/>
    protected internal virtual bool InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target) =>
        false;

    /// <inheritdoc />
    public void Remove(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (RemoveInternal(link, nodes))
            return;

        throw new UnknownFeatureException(GetClassifier(), link);
    }

    /// <inheritdoc cref="Remove"/>
    protected virtual bool RemoveInternal(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (link == null)
        {
            RemoveAnnotations(nodes.Cast<INode>());
            return true;
        }

        return false;
    }

    bool IWritableNodeRaw.RemoveContainmentsRaw(Containment containment, IWritableNode node) =>
        RemoveContainmentsRaw(containment, node);

    /// <inheritdoc cref="IWritableNodeRaw.RemoveContainmentsRaw"/>
    protected internal virtual bool RemoveContainmentsRaw(Containment containment, IWritableNode node) =>
        false;

    bool IWritableNodeRaw.RemoveReferencesRaw(Reference reference, ReferenceTarget target) =>
        RemoveReferencesRaw(reference, target);

    /// <inheritdoc cref="IWritableNodeRaw.RemoveReferencesRaw"/>
    protected internal virtual bool RemoveReferencesRaw(Reference reference, ReferenceTarget target) =>
        false;

    /// <summary>
    /// Tries to retrieve the <see cref="IPartitionInstance.GetNotificationProducer"/> from this node's <see cref="Concept.Partition"/>.
    /// </summary>
    /// <returns>This node's <see cref="IPartitionNotificationProducer"/>, if available.</returns>
    protected virtual IPartitionNotificationProducer? GetPartitionNotificationProducer() =>
        this.GetPartition()?.GetNotificationProducer();

    protected void AddOptionalMultipleContainment<T>(IEnumerable<T> nodes, Containment containment, List<T> storage,
        Func<T, bool> adder) where T : INode
    {
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, containment);
        AssureNotNullMembers(safeNodes, containment);
        if (storage.SequenceEqual(safeNodes))
            return;
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<T> emitter = new(containment, this, value, storage, null);
            emitter.CollectOldData();
            if (adder(value))
                emitter.Notify();
        }
    }

    protected void InsertOptionalMultipleContainment<T>(int index, IEnumerable<T> nodes, Containment containment,List<T> storage, 
        Func<int, T?, bool> inserter) where T : INode
    {
        AssureInRange(index, storage);
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, containment);
        AssureNoSelfMove(index, safeNodes, storage);
        AssureNotNullMembers(safeNodes, containment);
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<T> emitter = new(containment, this, value, storage, index);
            emitter.CollectOldData();
            if (inserter(index++, value))
                emitter.Notify();
        }
    }

    protected void RemoveOptionalMultipleContainment<T>(IEnumerable<T>? nodes, Containment containment,
        List<T> storage, Func<T?, bool> remover) where T : INode
    {
        RemoveSelfParent(nodes?.ToList(), storage, containment, ContainmentRemover<T>(containment));
    }

    protected void AddRequiredMultipleContainment<T>(IEnumerable<T>? nodes, Containment containment, List<T> storage, Func<T, bool> adder) where T : INode
    {
        var safeNodes = nodes?.ToList();
        AssureNonEmpty(safeNodes, storage, containment);
        if (storage.SequenceEqual(safeNodes))
            return;
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<T> emitter = new(containment, this, value, storage, null);
            emitter.CollectOldData();
            if (adder(value))
                emitter.Notify();
        }
    }

    protected void InsertRequiredMultipleContainment<T>(int index, IEnumerable<T>? nodes, Containment containment, List<T> storage,
        Func<int, T, bool> inserter) where T : INode
    {
        AssureInRange(index, storage);
        var safeNodes = nodes?.ToList();
        AssureNonEmpty(safeNodes, storage, containment);
        AssureNoSelfMove(index, safeNodes, storage);
        foreach (var value in safeNodes)
        {
            ContainmentAddMultipleNotificationEmitter<T> emitter = new(containment, this, value, storage, index);
            emitter.CollectOldData();
            if (inserter(index++, value))
                emitter.Notify();
        }
    }
    
    protected void RemoveRequiredMultipleContainment<T>(IEnumerable<T>? nodes, Containment containment,
        List<T> storage, Func<T?, bool> remover) where T : INode
    {
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, containment);
        AssureNotClearing(safeNodes, storage, containment);
        RemoveSelfParent(safeNodes, storage, containment, ContainmentRemover<T>(containment));
    }

    protected void SetRequiredSingleContainment<T>(T value, Containment containment, T? storage,
        Func<T?, bool> setter) where T : INode
    {
        AssureNotNull(value, containment);
        ContainmentSingleNotificationEmitter<T> emitter = new(containment, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    protected void SetOptionalSingleContainment<T>(T? value, Containment containment, T? storage,
        Func<T?, bool> setter) where T : INode
    {
        ContainmentSingleNotificationEmitter<T> emitter = new(containment, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    protected void SetOptionalSingleReference<T>(ReferenceTarget? value, Reference reference, ReferenceTarget? storage,
        Func<ReferenceTarget?, bool> setter) where T : IReadableNode
    {
        AssureNullableInstance<T>(value, reference);
        ReferenceSingleNotificationEmitter<T> emitter = new(reference, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    protected void AddOptionalMultipleReference<T>(IEnumerable<T>? nodes, Reference reference, List<ReferenceTarget> storage, Func<ReferenceTarget, bool> adder) where T : IReadableNode
    {
        var safeNodes = nodes?.Select(node => ReferenceTarget.FromNode(node)).ToList();
        AssureNotNull(safeNodes, reference);
        AssureNotNullMembers(safeNodes, reference);
        foreach (var value in safeNodes)
        {
            ReferenceAddMultipleNotificationEmitter<T> emitter = new(reference, this, value, storage.Count);
            emitter.CollectOldData();
            if (adder(value))
                emitter.Notify();
        }
    }

    protected void InsertOptionalMultipleReference<T>(int index, IEnumerable<T>? nodes, Reference reference, List<ReferenceTarget> storage,
        Func<int, ReferenceTarget, bool> inserter) where T : IReadableNode
    {
        AssureInRange(index, storage);
        var safeNodes = nodes?.Select(node => ReferenceTarget.FromNode(node)).ToList();
        AssureNotNull(safeNodes, reference);
        AssureNotNullMembers(safeNodes, reference);
        foreach (var value in safeNodes)
        {
            ReferenceAddMultipleNotificationEmitter<T> emitter = new(reference, this, value, index);
            emitter.CollectOldData();
            if (inserter(index++, value))
                emitter.Notify();
        }
    }
    
    protected void RemoveOptionalMultipleReference<T>(IEnumerable<T>? nodes, Reference reference,
        List<ReferenceTarget> storage, Func<ReferenceTarget, bool> remover) where T : IReadableNode
    {
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, reference);
        AssureNotNullMembers(safeNodes, reference);
        RemoveAll(safeNodes, storage, ReferenceRemover<T>(reference));
    }

    protected void SetRequiredSingleReference<T>(ReferenceTarget? value, Reference reference, ReferenceTarget? storage,
        Func<ReferenceTarget?, bool> setter) where T : IReadableNode
    {
        AssureNotNullInstance<T>(value, reference);
        ReferenceSingleNotificationEmitter<T> emitter = new(reference, this, value, storage);
        emitter.CollectOldData();
        if (setter(value))
            emitter.Notify();
    }

    protected void AddRequiredMultipleReference<T>(IEnumerable<T> nodes, Reference reference, List<ReferenceTarget> storage, Func<ReferenceTarget, bool> adder) where T : IReadableNode
    {
        var safeNodes = nodes?.Select(node => ReferenceTarget.FromNode(node)).ToList();
        AssureNotNull(safeNodes, reference);
        AssureNonEmpty(safeNodes, storage, reference);
        foreach (var value in safeNodes)
        {
            ReferenceAddMultipleNotificationEmitter<T> emitter = new(reference, this, value, storage.Count);
            emitter.CollectOldData();
            if (adder(value))
                emitter.Notify();
        }
    }

    protected void InsertRequiredMultipleReference<T>(int index, IEnumerable<T> nodes, Reference reference, List<ReferenceTarget> storage,
        Func<int, ReferenceTarget, bool> inserter) where T : IReadableNode
    {
        AssureInRange(index, storage);
        var safeNodes = nodes?.Select(node => ReferenceTarget.FromNode(node)).ToList();
        AssureNotNull(safeNodes, reference);
        AssureNonEmpty(safeNodes, storage, reference);
        foreach (var value in safeNodes)
        {
            ReferenceAddMultipleNotificationEmitter<T> emitter = new(reference, this, value, index);
            emitter.CollectOldData();
            if (inserter(index++, value))
                emitter.Notify();
        }
    }
    
    protected void RemoveRequiredMultipleReference<T>(IEnumerable<T>? nodes, Reference reference,
        List<ReferenceTarget> storage, Func<ReferenceTarget, bool> remover) where T : IReadableNode
    {
        var safeNodes = nodes?.ToList();
        AssureNotNull(safeNodes, reference);
        AssureNonEmpty(safeNodes, storage, reference);
        AssureNotClearing(safeNodes, ReferenceTargetNullableTargets<T>(storage, reference), reference);
        RemoveAll(safeNodes, storage, ReferenceRemover<T>(reference));
    }
}