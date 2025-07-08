// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Utilities;

using M2;
using M3;
using System.Collections;

/// <summary>
/// Configurable base class for cloning a set of <see cref="INode"/>s.
/// Contains all cloning logic.
/// </summary>
public abstract class ClonerBase
{
    private readonly IEnumerable<INode> _inputNodes;
    private readonly Dictionary<INode, INode> _nodes = new();
    private readonly HashSet<(INode, Reference)> _referencesToCheck = [];

    /// <summary>
    /// Initializes the Cloner based on <paramref name="inputNodes"/>.
    /// </summary>
    /// <param name="inputNodes">
    /// Nodes this Cloner should start from.
    /// We clone each INode once, even if we mention them multiple times. 
    /// </param>
    protected ClonerBase(IEnumerable<INode> inputNodes)
    {
        _inputNodes = inputNodes;
    }

    /// <summary>
    /// Clones <see cref="ClonerBase(System.Collections.Generic.IEnumerable{INode})">Cloner.inputNodes</see>.
    /// </summary>
    /// <returns>
    /// A mapping for each <c>inputNodes</c>, and all other processed input nodes, to their clones.
    /// We interconnect the cloned nodes according to their original nodes' connections (containments, references, parents, etc.)
    /// Refer to <see cref="Cloner"/> for configuration options.
    /// </returns>
    public Dictionary<INode, INode> Clone()
    {
        foreach (INode inputNode in _inputNodes)
        {
            CloneNode(inputNode);
        }

        foreach (var pair in _nodes)
        {
            ConnectParent(pair.Key, pair.Value);
        }

        foreach (var refSource in _referencesToCheck)
        {
            EstablishReference(refSource);
        }

        return _nodes;
    }

    /// Sets the clones of all contained nodes of <paramref name="inputNode"/> as children of <paramref name="resultNode"/>. 
    protected virtual void ConnectParent(INode inputNode, INode resultNode)
    {
        foreach (var containment in inputNode.CollectAllSetFeatures().OfType<Containment>())
        {
            switch (inputNode.Get(containment))
            {
                case INode inputChild:
                    if (_nodes.TryGetValue(inputChild, out INode? resultChild))
                    {
                        resultNode.Set(containment, resultChild);
                    }

                    break;

                case IEnumerable e:
                    resultNode.Set(containment,
                        containment.AsNodes<INode>(e)
                            .Select(inputChild => _nodes.GetValueOrDefault(inputChild))
                            .Where(c => c != null));
                    break;
            }
        }
    }

    private void EstablishReference((INode, Reference) refSource)
    {
        (INode? inputNode, Reference? reference) = refSource;
        INode result = _nodes[inputNode];
        var featureValue = inputNode.Get(reference);

        if (reference.Multiple)
        {
            if (featureValue is not IEnumerable featureValues)
            {
                throw new TypeExpectationFailedException(typeof(IList), featureValue);
            }

            IEnumerable<IReadableNode> resolvedTargets = featureValues.OfType<IReadableNode>().Select(ResolveReference).OfType<IReadableNode>();
            result.Set(reference, resolvedTargets.ToList());
        } else if (featureValue is IReadableNode target)
        {
            IReadableNode? resolvedTarget = ResolveReference(target);
            if (resolvedTarget is not null)
            {
                result.Set(reference, resolvedTarget);
            }
        }

        return;

        IReadableNode? ResolveReference(IReadableNode target)
        {
            if (target is INode iNode && _nodes.TryGetValue(iNode, out INode? found))
            {
                return found;
            }

            if (KeepExternalReference(inputNode, reference))
            {
                return target;
            }

            return null;
        }
    }

    class TypeExpectationFailedException(Type expected, object? actual)
        : ArgumentException($"Expected type: {expected}, actual: {actual?.GetType()}");

    /// Clones <paramref name="inputNode"/>, including applicable properties, containments, references, annotations, and parent.
    protected virtual INode CloneNode(INode inputNode)
    {
        if (_nodes.TryGetValue(inputNode, out INode? resultNode))
        {
            return resultNode;
        }

        INode result = CreateNode(inputNode);
        _nodes[inputNode] = result;

        foreach (Feature feature in inputNode.CollectAllSetFeatures())
        {
            switch (feature)
            {
                case Property prop:
                    if (IncludeProperty(inputNode, prop)) result.Set(feature, inputNode.Get(feature));
                    break;
                case Link link:
                    switch (feature)
                    {
                        case Containment cont when IncludeChild(inputNode, cont):
                        case Reference refer when IncludeReference(inputNode, refer):
                            {
                                var featureValue = inputNode.Get(feature);
                                if (featureValue == null)
                                {
                                    break;
                                }

                                if (link.Multiple)
                                {
                                    if (featureValue is not IEnumerable featureValues)
                                    {
                                        throw new TypeExpectationFailedException(typeof(IList), featureValue);
                                    }

                                    IEnumerable<INode> clonedTargets = featureValues.OfType<INode>().Select(CloneNode);
                                    result.Set(feature, clonedTargets.ToList());
                                } else
                                {
                                    if (featureValue is not INode targetNode)
                                    {
                                        throw new TypeExpectationFailedException(typeof(INode), featureValue);
                                    }

                                    INode clonedTarget = CloneNode(targetNode);
                                    result.Set(feature, clonedTarget);
                                }

                                break;
                            }
                        case Reference externalRef:
                            _referencesToCheck.Add((inputNode, externalRef));
                            break;
                    }

                    break;
            }
        }

        foreach (INode annotation in inputNode.GetAnnotations())
        {
            if (IncludeAnnotation(inputNode, annotation))
            {
                INode clonedAnnotation = CloneNode(annotation);
                result.AddAnnotations([clonedAnnotation]);
            }
        }

        if (inputNode.GetParent() is { } nodeParent && IncludeParent(inputNode))
        {
            CloneNode(nodeParent);
        }

        return result;
    }

    /// Creates a new, empty instance equivalent to <paramref name="inputNode"/>.
    protected virtual INode CreateNode(INode inputNode) =>
        inputNode.GetClassifier().GetLanguage().GetFactory()
            .CreateNode(GetNewId(inputNode), inputNode.GetClassifier()) ??
        throw new InvalidOperationException();

    /// Provides a new id for a new instance equivalent to <paramref name="inputNode"/>.
    protected virtual NodeId GetNewId(INode inputNode) => IdUtils.NewId();

    /// Whether <paramref name="prop"/> of <paramref name="inputNode"/> should be cloned.
    protected abstract bool IncludeProperty(INode inputNode, Property prop);

    /// Whether <paramref name="cont"/> of <paramref name="inputNode"/> should be cloned.
    protected abstract bool IncludeChild(INode inputNode, Containment cont);

    /// Whether <paramref name="refer"/> of <paramref name="inputNode"/> should be cloned.
    protected abstract bool IncludeReference(INode inputNode, Reference refer);

    /// Whether <paramref name="annotation"/> of <paramref name="inputNode"/> should be cloned.
    protected abstract bool IncludeAnnotation(INode inputNode, INode annotation);

    /// Whether the parent of <paramref name="inputNode"/> should be cloned.
    protected abstract bool IncludeParent(INode inputNode);

    /// Whether <paramref name="externalRef"/> of <paramref name="inputNode"/> should be kept.
    protected abstract bool KeepExternalReference(INode inputNode, Reference externalRef);
}