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
        var safeAnnotations = annotations?.ToList();
        AssureAnnotations(safeAnnotations);
        AnnotationAddMultipleNotificationEmitter notification = new(this, safeAnnotations, _annotations,
            startIndex: null, notificationId: notificationId);
        notification.CollectOldData();
        AddAnnotationsRaw(safeAnnotations);
        notification.Notify();
    }

    bool IWritableNodeRaw<INode>.AddAnnotationsRaw(List<INode> annotations) =>
        AddAnnotationsRaw(annotations);

    protected internal bool AddAnnotationsRaw(List<INode> annotations)
    {
        if (annotations.Count == 0 || annotations.Any(a => !CanAnnotate(a)))
            return false;

        _annotations.AddRange(SetSelfParent(annotations, null));
        return true;
    }

    /// <inheritdoc />
    public virtual void InsertAnnotations(Index index, IEnumerable<INode> annotations,
        INotificationId? notificationId = null)
    {
        AssureInRange(index, _annotations);
        var safeAnnotations = annotations?.ToList();
        AssureAnnotations(safeAnnotations);
        AnnotationAddMultipleNotificationEmitter notification = new(this, safeAnnotations, _annotations,
            startIndex: index, notificationId: notificationId);
        notification.CollectOldData();
        InsertAnnotationsRaw(index, safeAnnotations);
        notification.Notify();
    }

    bool IWritableNodeRaw<INode>.InsertAnnotationsRaw(Index index, List<INode> annotations) =>
        InsertAnnotationsRaw(index, annotations);
    
    protected internal bool InsertAnnotationsRaw(Index index, List<INode> annotations)
    {
        if(annotations.Count == 0 || !IsInRange(index, _annotations) || annotations.Any(a => !CanAnnotate(a)))
            return false;
        
        AssureAnnotations(annotations);
        _annotations.InsertRange(index, SetSelfParent(annotations, null));
        return true;
    }


    /// <inheritdoc />
    public virtual bool RemoveAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null) =>
        RemoveSelfParent(annotations?.ToList(), _annotations, null, AnnotationRemover, notificationId);


    bool IWritableNodeRaw<INode>.RemoveAnnotationsRaw(HashSet<INode> annotations) => 
        RemoveAnnotationsRaw(annotations);

    protected internal bool RemoveAnnotationsRaw(HashSet<INode> annotations)
    {
        if (annotations.Count == 0 || annotations.Any(a => !CanAnnotate(a)))
            return false;

        return RemoveSelfParent(annotations.ToList(), _annotations, null);
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
        if (feature == null)
        {
            if (value is not IEnumerable)
                throw new InvalidValueException(feature, value);
            var safeNodes = M2Extensions.AsNodes<INode>(value, feature).ToList();
            AssureAnnotations(safeNodes);
            AnnotationSetNotificationEmitter notification = new(this, safeNodes, _annotations, notificationId);
            notification.CollectOldData();
            RemoveSelfParent(_annotations.ToList(), _annotations, null);
            _annotations.AddRange(SetSelfParent(safeNodes, null));
            notification.Notify();
            return true;
        }

        return false;
    }

    bool IWritableNodeRaw.SetRaw(Feature feature, object? value) => 
        SetRaw(feature, value);

    protected internal virtual bool SetRaw(Feature feature, object? value)
    {
        return false;
    }

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


    bool IWritableNodeRaw<INode>.AddRaw(Link link, List<INode> nodes) => 
        AddRaw(link, nodes);

    protected internal virtual bool AddRaw(Link link, List<INode> nodes) =>
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

    bool IWritableNodeRaw<INode>.InsertRaw(Link link, Index index, List<INode> nodes) =>
        InsertRaw(link, index, nodes);

    protected internal virtual bool InsertRaw(Link link, Index index, List<INode> nodes) =>
        false;

    /// <inheritdoc />
    public void Remove(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (RemoveInternal(link, nodes))
            return;

        throw new UnknownFeatureException(GetClassifier(), link);
    }

    /// <inheritdoc cref="Insert"/>
    protected virtual bool RemoveInternal(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (link == null)
        {
            RemoveAnnotations(nodes.Cast<INode>());
            return true;
        }

        return false;
    }

    bool IWritableNodeRaw<INode>.RemoveRaw(Link link, List<INode> nodes) =>
        RemoveRaw(link, nodes);

    protected internal virtual bool RemoveRaw(Link link, List<INode> nodes) =>
        false;

    /// <summary>
    /// Tries to retrieve the <see cref="IPartitionInstance.GetNotificationProducer"/> from this node's <see cref="Concept.Partition"/>.
    /// </summary>
    /// <returns>This node's <see cref="IPartitionNotificationProducer"/>, if available.</returns>
    protected virtual IPartitionNotificationProducer? GetPartitionNotificationProducer() =>
        this.GetPartition()?.GetNotificationProducer();
}