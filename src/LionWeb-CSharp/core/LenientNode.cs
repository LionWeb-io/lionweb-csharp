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
using System.Collections;

public class LenientNode : NodeBase, INode
{
    private readonly Dictionary<Feature, object?> _features = new();
    private Classifier? _classifier = null;

    public LenientNode(string id, Classifier? classifier) : base(id)
    {
        _classifier = classifier;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() =>
        _classifier ?? throw new UnsupportedNodeTypeException(this, "classifier");

    public void SetClassifier(Classifier? classifier) => _classifier = classifier;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() => _features.Keys;

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (feature == null)
            return false;

        if (_features.TryGetValue(feature, out result))
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
        {
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        if (feature == null)
        {
            var enumerable = M2Extensions.AsNodes<INode>(value).ToList();
            RemoveSelfParent(_annotations.ToList(), _annotations, null);
            AddAnnotations(enumerable);
            return true;
        }

        var oldValue = _features.TryGetValue(feature, out var old) ? old : null;

        switch (value)
        {
            case null:
                if (_features.Remove(feature))
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
                _features[feature] = readableNode;
                return true;

            case string s:
                _features[feature] = value;
                return true;

            case IEnumerable:
                var readableNodes = M2Extensions.AsNodes<IReadableNode>(value).ToList();
                if (!readableNodes.Any())
                {
                    if (_features.Remove(feature))
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

                    _features[feature] = newChildren.ToList();
                } else
                {
                    _features[feature] = readableNodes;
                }

                return true;

            case { } v:
                if (!v.GetType().IsValueType)
                    return false;

                _features[feature] = value;
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
                RemoveSelfParent(oldList?.ToList(), oldList, c);
                return;
        }
    }

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
            return true;

        foreach (var (key, value) in _features.Where(pair => pair.Key is Containment))
        {
            switch (value)
            {
                case IReadableNode readable:
                    _features.Remove(key);
                    return true;
                case IList list:
                    int index = list.IndexOf(child);
                    if (index == -1)
                        return false;

                    list.RemoveAt(index);
                    if (list.Count == 0)
                        _features.Remove(key);

                    return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public override Containment? GetContainmentOf(INode child) =>
        _features
            .Keys
            .OfType<Containment>()
            .FirstOrDefault(k => k
                .AsNodes<INode>(_features[k])
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