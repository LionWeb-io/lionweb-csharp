﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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
using System.Collections;
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
    private readonly Dictionary<Feature, object?> _featureValues = new(new FeatureIdentityComparer());
    private Classifier? _classifier;

    /// <summary>
    /// Constructs a new node.
    /// </summary>
    /// <param name="id"><c>this</c> node's <see cref="IReadableNode.GetId">id</see>.</param>
    /// <param name="classifier"><c>this</c> node's <see cref="IReadableNode.GetClassifier()">classifier</see>.</param>
    /// <exception cref="InvalidIdException">If <paramref name="id"/> is not a <see cref="IReadableNode.GetId">valid identifier</see>.</exception>
    public LenientNode(string id, Classifier? classifier) : base(id)
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
    public override IEnumerable<Feature> CollectAllSetFeatures() => _featureValues.Keys;

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

        if (_featureValues.TryGetValue(feature, out result))
        {
            switch (feature)
            {
                case Containment { Multiple: true } when result is INode node:
                    result = new List<INode> { node }.AsReadOnly();
                    break;
                case Reference { Multiple: true }:
                    switch (result)
                    {
                        case INode iNode:
                            result = new List<INode> { iNode }.AsReadOnly();
                            break;
                        case IReadableNode readableNode:
                            result = new List<IReadableNode> { readableNode }.AsReadOnly();
                            break;
                    }

                    break;
            }

            return true;
        }

        if (!feature.Optional)
            throw new UnsetFeatureException(feature);

        switch (feature)
        {
            case Containment { Multiple: true }:
                result = new List<INode>().AsReadOnly();
                break;
            case Reference { Multiple: true }:
                result = new List<IReadableNode>().AsReadOnly();
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
    protected override bool SetInternal(Feature? feature, object? value)
    {
        if (feature == null)
        {
            var enumerable = M2Extensions.AsNodes<INode>(value).ToList();
            RemoveSelfParent(_annotations.ToList(), _annotations, null);
            AddAnnotations(enumerable);
            return true;
        }

        var oldValue = _featureValues.TryGetValue(feature, out var old) ? old : null;

        switch (value)
        {
            case null:
                if (_featureValues.Remove(feature))
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

                _featureValues[feature] = readableNode;
                return true;

            case string:
                _featureValues[feature] = value;
                return true;

            case IEnumerable:
                var readableNodes = M2Extensions.AsNodes<IReadableNode>(value).ToList();
                if (readableNodes.Count == 0)
                {
                    if (_featureValues.Remove(feature))
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

                    _featureValues[feature] = newChildren.ToList();
                } else
                {
                    _featureValues[feature] = readableNodes;
                }

                return true;

            case { } v:
                if (!v.GetType().IsValueType)
                    throw new InvalidValueException(feature, value);

                _featureValues[feature] = value;
                return true;
        }
    }

    private void RemoveExistingChildren(Containment c, object? oldValue)
    {
        switch (oldValue)
        {
            case INode n:
                SetParentNull(n);
                return;
            case List<INode> oldList:
                RemoveSelfParent(oldList.ToList(), oldList, c);
                return;
        }
    }

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
            return true;

        foreach (var (key, value) in _featureValues.Where(pair => pair.Key is Containment))
        {
            switch (value)
            {
                case IReadableNode:
                    _featureValues.Remove(key);
                    return true;
                case IList list:
                    int index = list.IndexOf(child);
                    if (index == -1)
                        return false;

                    list.RemoveAt(index);
                    if (list.Count == 0)
                        _featureValues.Remove(key);

                    return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public override Containment? GetContainmentOf(INode child) =>
        _featureValues
            .Keys
            .OfType<Containment>()
            .FirstOrDefault(k => k
                .AsNodes<INode>(_featureValues[k])
                .Contains(child));

    /// <inheritdoc />
    public override void AddAnnotations(IEnumerable<INode> annotations)
    {
        var safeAnnotations = annotations?.ToList();
        _annotations.AddRange(SetSelfParent(safeAnnotations, null));
    }

    /// <inheritdoc />
    public override void InsertAnnotations(int index, IEnumerable<INode> annotations)
    {
        AssureInRange(index, _annotations);
        var safeAnnotations = annotations?.ToList();
        _annotations.InsertRange(index, SetSelfParent(safeAnnotations, null));
    }

    /// <inheritdoc />
    public override bool RemoveAnnotations(IEnumerable<INode> annotations) =>
        RemoveSelfParent(annotations?.ToList(), _annotations, null);
}