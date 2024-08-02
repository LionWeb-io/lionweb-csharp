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

namespace LionWeb.Core.M1;

using M2;
using M3;
using Serialization;

/// <summary>
/// Instances of this class can deserialize a <see cref="SerializationChunk"/> as a list of <see cref="INode"/>s that are root nodes.
/// An instance is parametrized with a collection of <see cref="Language"/> definitions with a corresponding <see cref="INodeFactory"/>.
/// </summary>
public class Deserializer : IDeserializer
{
    private readonly Dictionary<Language, INodeFactory> _language2NodeFactory = new();
    private readonly Dictionary<MetaPointer, Classifier> _classifiers = new();
    private readonly Dictionary<MetaPointer, Feature> _features = new();
    private readonly Dictionary<string, NodePair> _nodesById = new();

    private readonly Dictionary<string, IReadableNode> _dependentNodesById=new ();

    /// <inheritdoc />
    public IDeserializerHandler Handler { get; init; } = new DeserializerExceptionHandler();

    public Deserializer()
    {
        RegisterLanguage(BuiltInsLanguage.Instance, BuiltInsLanguage.Instance.GetFactory());
    }

    /// <inheritdoc />
    public void RegisterLanguage(Language language, INodeFactory factory)
    {
        _language2NodeFactory[language] = factory;

        foreach (Classifier classifier in language.Entities.OfType<Classifier>())
        {
            _classifiers[classifier.ToMetaPointer()] = classifier;
            foreach (Feature feature in classifier.Features)
            {
                _features[feature.ToMetaPointer()] = feature;
            }
        }
    }

    /// <inheritdoc />
    public List<INode> Deserialize(IEnumerable<SerializedNode> serializedNodes,
        IEnumerable<IReadableNode> dependentNodes)
    {
        foreach (var nodePair in NodesWithProperties(serializedNodes))
        {
            _nodesById[nodePair.SerializedNode.Id] = nodePair;
        }

        foreach (var dependentNode in dependentNodes)
        {
            _dependentNodesById[dependentNode.GetId()] = dependentNode;
        }

        foreach (var nodePair in _nodesById.Values)
        {
            Parent(nodePair);
            Containments(nodePair);
            References(nodePair);
            Annotations(nodePair);
        }

        return FilterRootNodes()
            .ToList();
    }

    #region NodesWithProperties

    private IEnumerable<NodePair> NodesWithProperties(IEnumerable<SerializedNode> serializedNodes) =>
        serializedNodes.Select(serializedNode =>
        {
            var id = serializedNode.Id;
            var node = Instantiate(id, serializedNode.Classifier);
            foreach (var serializedProperty in serializedNode.Properties.Where(p => p.Value != null))
            {
                var property = FindFeature<Property>(node.GetClassifier(), serializedProperty.Property, id);
                var type = property.Type;
                node.Set(
                    property,
                    type switch
                    {
                        PrimitiveType => FromString(serializedProperty.Value, property),
                        Enumeration enumeration => _language2NodeFactory[enumeration.GetLanguage()]
                            .GetEnumerationLiteral(enumeration.Literals.First(literal =>
                                literal.Key == serializedProperty.Value)),
                        _ => throw new UnsupportedClassifierException(serializedNode.Classifier,
                            $"On node with id={id}: can't deserialize property {property.Name} (key={property.Key}) from a <{type.GetType().Name}> datatype \"{type.Name}\"")
                    }
                );
            }

            return new NodePair(serializedNode, node);
        });

    private INode Instantiate(string id, MetaPointer metaPointer)
    {
        if (!_classifiers.TryGetValue(metaPointer, out var classifier))
        {
            classifier = Handler.UnknownClassifier(id, metaPointer);
            if (classifier == null)
                throw new UnsupportedClassifierException(metaPointer, $"On node with id={id}:");
        }

        return _language2NodeFactory[classifier.GetLanguage()].CreateNode(id, classifier);
    }

    /// <summary>
    /// Deserializes the given <paramref name="value">string value</paramref> as a runtime value
    /// corresponding to the given <paramref name="property">property's primitive type</paramref>.
    /// Only primitive types of the LionCore built-ins language (defined here) are understood.
    /// </summary>
    /// <exception cref="InvalidValueException">Thrown when the given primitive type is not known/handled.</exception>
    private object FromString(string value, Property property)
    {
        var primitiveType = property.Type;
        if (primitiveType == BuiltInsLanguage.Instance.Boolean)
        {
            return value == "true";
        }

        if (primitiveType == BuiltInsLanguage.Instance.Integer)
        {
            return int.Parse(value);
        }

        // leave both a String and JSON value as a string:
        if (primitiveType == BuiltInsLanguage.Instance.String || primitiveType == BuiltInsLanguage.Instance.Json)
        {
            return value;
        }

        throw new InvalidValueException(property, value);
    }

    #endregion


    #region Parent

    private void Parent(NodePair nodePair)
    {
        string? parentId = nodePair.SerializedNode.Parent;
        if (parentId == null)
            return;

        INode? parent = FindParent(nodePair, parentId);

        if (parent != null)
            nodePair.Node.SetParent(parent);
    }

    private INode? FindParent(NodePair nodePair, string parentId)
    {
        if (_nodesById.TryGetValue(parentId, out NodePair? parentPair))
            return parentPair.Node;

        var parent = Handler.UnknownParent(parentId, nodePair.SerializedNode, nodePair.Node);

        return parent;
    }

    #endregion

    #region Containments

    private void Containments(NodePair nodePair)
    {
        foreach (var serializedContainment in nodePair.SerializedNode.Containments)
        {
            var containment = FindFeature<Containment>(nodePair.Node.GetClassifier(), serializedContainment.Containment,
                nodePair.SerializedNode.Id);
            var children = serializedContainment
                .Children
                .Select(childId => FindChild(nodePair, childId))
                .Where(c => c != null)
                .ToList();

            if (children.Count != 0)
            {
                nodePair.Node.Set(
                    containment,
                    containment.Multiple ? children : children.FirstOrDefault()
                );
            }
        }
    }

    private INode? FindChild(NodePair nodePair, string childId)
    {
        if (_nodesById.TryGetValue(childId, out var existingPair))
            return existingPair.Node;

        var healed = Handler.UnknownChild(childId, nodePair.SerializedNode, nodePair.Node);
        return healed;
    }

    #endregion

    #region References

    private void References(NodePair nodePair)
    {
        foreach (var serializedReference in nodePair.SerializedNode.References)
        {
            var reference = FindFeature<Reference>(nodePair.Node.GetClassifier(), serializedReference.Reference,
                nodePair.SerializedNode.Id);
            var targets = serializedReference
                .Targets
                .Select(target => FindReferenceTarget(nodePair, target))
                .Where(c => c != null)
                .ToList();

            if (targets.Count != 0)
            {
                nodePair.Node.Set(
                    reference,
                    reference.Multiple ? targets : targets.FirstOrDefault()
                );
            }
        }
    }

    private IReadableNode? FindReferenceTarget(NodePair nodePair, SerializedReferenceTarget target)
    {
        var targetId = target.Reference;
        if (_nodesById.TryGetValue(targetId, out var ownNodePair))
            return ownNodePair.Node;

        if (_dependentNodesById.TryGetValue(targetId, out var dependentNode))
            return dependentNode;

        IReadableNode? healed = Handler.UnknownReference(target, nodePair.SerializedNode, nodePair.Node);
        return healed;
    }

    #endregion

    #region Annotations

    private void Annotations(NodePair nodePair)
    {
        var annotations = nodePair.SerializedNode
            .Annotations
            .Select(childId => FindAnnotation(nodePair, childId))
            .Where(c => c != null)
            .ToList();

        nodePair.Node.AddAnnotations(annotations);
    }

    private INode? FindAnnotation(NodePair nodePair, string annotationId)
    {
        if (_nodesById.TryGetValue(annotationId, out var existingPair))
            return existingPair.Node;

        var healed = Handler.UnknownAnnotation(annotationId, nodePair.SerializedNode, nodePair.Node);
        return healed;
    }

    #endregion

    private IEnumerable<INode> FilterRootNodes() =>
        _nodesById
            .Values
            .Select(p => p.Node)
            .Where(node => node.GetParent() == null);

    private TFeature FindFeature<TFeature>(Classifier classifier, MetaPointer metaPointer, string id)
        where TFeature : Feature
    {
        if (!_features.TryGetValue(metaPointer, out var feature))
        {
            feature = Handler.UnknownFeature(id, classifier, metaPointer);
            if (feature == null)
                throw new UnknownFeatureException(classifier, metaPointer, $"On node with id={id}:");
        }

        if (feature is not TFeature f)
            throw new UnknownFeatureException(classifier, metaPointer, $"On node with id={id}:");

        return f;
    }

    private record NodePair(SerializedNode SerializedNode, INode Node);
}