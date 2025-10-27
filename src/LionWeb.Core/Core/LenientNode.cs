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

namespace LionWeb.Core;

using M2;
using M3;
using Notification;
using Notification.Partition;
using Notification.Pipe;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Utilities;

/// <summary>
/// A LenientNode can handle any kind of <i>feature value</i>
/// or <see cref="IReadableNode.GetAnnotations()">annotations</see>,
/// even if the <i>feature</i> is unknown to the <see cref="GetClassifier">classifier</see>
/// or the <i>value</i> does not adhere to the <i>feature</i>.
///
/// It strikes the following compromises:
/// <list type="bullet">
/// <item>
/// <inheritdoc cref="SetInternal"/>
/// </item>
/// <item>
/// <see cref="IWritableNode.AddAnnotations"/>, <see cref="IWritableNode.InsertAnnotations"/>,
/// and <see cref="IWritableNode.RemoveAnnotations"/> accept any node, even if the node is not an annotation,
/// or does not annotate the target node.
/// </item>
/// <item>
/// <inheritdoc cref="GetInternal"/>
/// </item>
/// <item>
/// <inheritdoc cref="CollectAllSetFeatures"/>
/// </item>
/// </list>
/// <remarks>
/// The main use case for this class are migration scenarios,
/// where the metamodel (M2) might not fit the instance (M1) nodes.
///
/// <p><b>Comparison to <see cref="DynamicNode"/></b></p>
/// <see cref="DynamicNode"/> completely adheres to the <see cref="INode"/> interface semantics:
/// Setting a feature unknown to the classifier fails,
/// setting a required feature to null or empty list fails,
/// setting a link with type building to instance of tree fails.
/// <see cref="LenientNode"/> accepts all that.
/// </remarks>
/// </summary>
public class LenientNode : NodeBase, INode
{
    private readonly List<(Feature feature, object? value)> _featureValues = [];
    private Classifier? _classifier;

    /// <summary>
    /// Constructs a new node.
    /// </summary>
    /// <param name="id"><c>this</c> node's <see cref="IReadableNode.GetId">id</see>.</param>
    /// <param name="classifier"><c>this</c> node's <see cref="IReadableNode.GetClassifier()">classifier</see>.</param>
    /// <exception cref="InvalidIdException">If <paramref name="id"/> is not a <see cref="IReadableNode.GetId">valid identifier</see>.</exception>
    public LenientNode(NodeId id, Classifier? classifier) : base(id)
    {
        _classifier = classifier;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() =>
        _classifier ?? throw new UnsupportedNodeTypeException(this, "classifier");

    /// <summary>
    /// Sets this node's <see cref="IReadableNode.GetClassifier">classifier</see>.
    /// </summary>
    /// <param name="classifier">The new classifier.</param>
    public void SetClassifier(Classifier? classifier) => _classifier = classifier;

    /// <summary>
    /// <see cref="IReadableNode.CollectAllSetFeatures"/> returns all set features, even if the feature is unknown to
    /// the target node's classifier, or the feature's value doesn't adhere to the feature's type.
    /// </summary>
    public override IEnumerable<Feature> CollectAllSetFeatures() => FeatureKeys;

    /// <summary>
    /// <see cref="IReadableNode.Get"/> returns single values as-is, and any enumerable nodes as
    /// <c>List&lt;INode&gt;</c> (for containments) or <c>List&gt;IReadableNode&lt;</c> (for references or properties).
    /// Throws <see cref="UnsetFeatureException"/> for unset <i>required features</i>.
    /// If set, returns feature values for features unknown to the target node's classifier.
    /// If set, returns values that might not fit the feature's type.
    /// </summary>
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (feature == null)
            return false;

        if (TryGet(feature, out result))
        {
            if (feature.Optional)
            {
                return true;
            }

            if (result == null || (result is IEnumerable enumerable && !enumerable.OfType<object?>().Any()))
            {
                throw new UnsetFeatureException(feature);
            }

            return true;
        }

        if (!feature.Optional)
            throw new UnsetFeatureException(feature);

        return false;
    }

    /// <inheritdoc />
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (feature == null)
        {
            value = null;
            return false;
        }

        if (TryGetFeature(feature, out value))
        {
            switch (feature)
            {
                case Containment { Multiple: true } when value is INode node:
                    value = new List<INode> { node }.AsReadOnly();
                    break;
                case Reference { Multiple: true }:
                    switch (value)
                    {
                        case INode iNode:
                            value = new List<INode> { iNode }.AsReadOnly();
                            break;
                        case IReadableNode readableNode:
                            value = new List<IReadableNode> { readableNode }.AsReadOnly();
                            break;
                    }

                    break;
            }

            return true;
        }

        switch (feature)
        {
            case Containment { Multiple: true }:
                value = new List<INode>().AsReadOnly();
                break;
            case Reference { Multiple: true }:
                value = new List<IReadableNode>().AsReadOnly();
                break;
        }

        if (_classifier != null && _classifier.AllFeatures().Contains(feature))
            return true;

        return false;
    }

    /// <summary>
    /// <see cref="IWritableNode.Set"/> never fails, it accepts any feature and feature value that makes sense in LW context.
    /// So nodes, enumerables of nodes, value types. Enumerable of value types doesn't work, and arbitrary objects do not work.
    /// Features unknown to the target node's classifier do work as well as <c>null</c> for <i>required features</i>.
    /// For containments, sets the target node as parent of the value, even if the value doesn't fit the containment's type.
    /// For containments, the target node MUST implement <see cref="INode"/>; for references, the target node MUST implement <see cref="IReadableNode"/>. 
    /// </summary>
    protected override bool SetInternal(Feature? feature, object? value, INotificationId? notificationId = null)
    {
        if (feature == null)
        {
            var annotations = M2Extensions.AsNodes<INode>(value).ToList();
            RemoveSelfParent(_annotations.ToList(), _annotations, null, null);
            AddAnnotations(annotations);
            return true;
        }

        var oldValue = TryGetFeature(feature, out var old) ? old : null;

        switch (value)
        {
            case null:
                if (RemoveFeature(feature))
                    return true;
                if (_classifier != null && _classifier.AllFeatures().Contains(feature))
                    return true;
                return false;

            case IReadableNode readableNode:
                if (feature is Containment c && readableNode is INode node)
                {
                    RemoveExistingChildren(c, oldValue);
                    AttachChild(node);
                }

                SetFeature(feature, readableNode);
                return true;

            case string:
                SetFeature(feature, value);
                return true;

            case IEnumerable:
                var readableNodes = M2Extensions.AsNodes<IReadableNode>(value).ToList();
                if (readableNodes.Count == 0)
                {
                    if (RemoveFeature(feature))
                        return true;
                    if (_classifier != null && _classifier.AllFeatures().Contains(feature))
                        return true;
                    return false;
                }

                if (feature is Containment cont)
                {
                    RemoveExistingChildren(cont, oldValue);
                    var newChildren = M2Extensions.AsNodes<INode>(readableNodes).ToList();
                    foreach (var newChild in newChildren)
                    {
                        AttachChild(newChild);
                    }

                    SetFeature(feature, newChildren.ToList());
                } else
                {
                    SetFeature(feature, readableNodes);
                }

                return true;

            case { } v:
                if (!v.GetType().IsValueType)
                    throw new InvalidValueException(feature, value);

                SetFeature(feature, value);
                return true;
        }
    }

    /// <inheritdoc />
    protected override bool AddInternal(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (link == null)
        {
            var annotations = M2Extensions.AsNodes<INode>(nodes).ToList();
            AddAnnotations(annotations);
            return true;
        }
        
        //TODO: the code block below is this method is not tested properly 
        
        var oldValue = TryGetFeature(link, out var old) ? old : null;
        
        var readableNodes = M2Extensions.AsNodes<IReadableNode>(nodes).ToList();
        if (readableNodes.Count == 0)
        {
            if (RemoveFeature(link))
                return true;
            if (_classifier != null && _classifier.AllFeatures().Contains(link))
                return true;
            return false;
        }

        if (link is Containment cont)
        {
            RemoveExistingChildren(cont, oldValue);
            var newChildren = M2Extensions.AsNodes<INode>(readableNodes).ToList();
            foreach (var newChild in newChildren)
            {
                AttachChild(newChild);
            }

            SetFeature(link, newChildren.ToList());
        } else
        {
            SetFeature(link, readableNodes);
        }
        
        return true;
    }
    
    /// <inheritdoc />
    protected override bool InsertInternal(Link? link, Index index, IEnumerable<IReadableNode> nodes)
    {
        if (link == null)
        {
            var annotations = M2Extensions.AsNodes<INode>(nodes).ToList();
            InsertAnnotations(index, annotations);
            return true;
        }

        //TODO: not complete
        
        return true;
    }

    /// <inheritdoc />
    protected override bool RemoveInternal(Link? link, IEnumerable<IReadableNode> nodes)
    {
        if (link is null)
        {
            var annotations= M2Extensions.AsNodes<INode>(nodes).ToList();
            RemoveAnnotations(annotations);
            return true;
        }

        //TODO: not complete
        
        return true;
    }

    private void RemoveExistingChildren(Containment c, object? oldValue)
    {
        switch (oldValue)
        {
            case INode n:
                SetParentNull(n);
                return;
            case List<INode> oldList:
                RemoveSelfParent(oldList.ToList(), oldList, c, null);
                return;
        }
    }

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
            return true;

        foreach (var (key, value) in _featureValues.Where(pair => pair.feature is Containment))
        {
            switch (value)
            {
                case IReadableNode:
                    RemoveFeature(key);
                    return true;
                case IList list:
                    Index index = list.IndexOf(child);
                    if (index == -1)
                        return false;

                    list.RemoveAt(index);
                    if (list.Count == 0)
                        RemoveFeature(key);

                    return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public override Containment? GetContainmentOf(INode child) =>
        FeatureKeys
            .OfType<Containment>()
            .FirstOrDefault(k => k
                .AsNodes<INode>(TryGetFeature(k, out var value) ? value : null)
                .Contains(child));

    /// <inheritdoc />
    public override void AddAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null)
    {
        var safeAnnotations = annotations?.ToList();
        _annotations.AddRange(SetSelfParent(safeAnnotations, null));
    }

    /// <inheritdoc />
    public override void InsertAnnotations(Index index, IEnumerable<INode> annotations, INotificationId? notificationId = null)
    {
        AssureInRange(index, _annotations);
        var safeAnnotations = annotations?.ToList();
        _annotations.InsertRange(index, SetSelfParent(safeAnnotations, null));
    }

    /// <inheritdoc />
    public override bool RemoveAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null) =>
        RemoveSelfParent(annotations?.ToList(), _annotations, null, null);

    private IEnumerable<Feature> FeatureKeys => _featureValues.Select(f => f.feature);

    private bool TryGetFeature(Feature featureToFind, out object? value)
    {
        var result = _featureValues.Find(f => featureToFind.EqualsIdentity(f.feature));
        if (result != default)
        {
            value = result.value;
            return true;
        }

        value = null;
        return false;
    }

    private bool RemoveFeature(Feature featureToRemove)
    {
        var result = _featureValues.Find(f => featureToRemove.EqualsIdentity(f.feature));
        if (result != default)
        {
            _featureValues.Remove(result);
            return true;
        }

        return false;
    }

    private void SetFeature(Feature featureToSet, object? value)
    {
        var result = _featureValues.Find(f => featureToSet.EqualsIdentity(f.feature));
        if (result != default)
        {
            var index = _featureValues.IndexOf(result);
            _featureValues[index] = (featureToSet, value);
        } else
        {
            _featureValues.Add((featureToSet, value));
        }
    }
}

public class LenientPartition : LenientNode, IPartitionInstance
{
    private readonly IPartitionNotificationProducer _notificationProducer;

    public LenientPartition(NodeId id, Classifier? classifier) : base(id, classifier)
    {
        _notificationProducer = new PartitionNotificationProducer(this);
    }

    /// <inheritdoc />
    public INotificationSender? GetNotificationSender() => _notificationProducer;

    /// <inheritdoc />
    IPartitionNotificationProducer? IPartitionInstance.GetNotificationProducer() => _notificationProducer;

    /// <inheritdoc />
    public Concept GetConcept() => (Concept)GetClassifier();
}