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
    public void DetachFromParent()
    {
        if (_parent != null)
        {
            _parent.DetachChild(this);
            _parent = null;
        }
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

    #region Annotations

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

    #endregion

    #region Multiple mutators

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

    #endregion

    /// <summary>
    /// Tries to retrieve the <see cref="IPartitionInstance.GetNotificationProducer"/> from this node's <see cref="Concept.Partition"/>.
    /// </summary>
    /// <returns>This node's <see cref="IPartitionNotificationProducer"/>, if available.</returns>
    protected virtual IPartitionNotificationProducer? GetPartitionNotificationProducer() =>
        this.GetPartition()?.GetNotificationProducer();
}