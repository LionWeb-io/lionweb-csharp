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
    private readonly Dictionary<CompressedMetaPointer, Classifier> _classifiers = new();
    private readonly Dictionary<CompressedMetaPointer, Feature> _features = new();
    private readonly Dictionary<CompressedId, IReadableNode> _dependentNodesById = new();

    private readonly Dictionary<CompressedId, INode> _nodesById = new();

    private readonly Dictionary<CompressedId, List<(CompressedMetaPointer, List<CompressedId>)>>
        _containmentsByOwnerId =
            new();

    private readonly Dictionary<CompressedId, List<(CompressedMetaPointer, List<(CompressedId, string?)>)>>
        _referencesByOwnerId = new
            ();

    private readonly Dictionary<CompressedId, List<CompressedId>> _annotationsByOwnerId = new();
    private readonly Dictionary<CompressedId, CompressedId> _parentByNodeId = new();


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
            _classifiers[CompressedMetaPointer.Create(classifier.ToMetaPointer())] = classifier;
            foreach (Feature feature in classifier.Features)
            {
                _features[CompressedMetaPointer.Create(feature.ToMetaPointer())] = feature;
            }
        }
    }

    public void RegisterDependentNodes(IEnumerable<IReadableNode> dependentNodes)
    {
        foreach (var dependentNode in dependentNodes)
        {
            _dependentNodesById[CompressedId.Create(dependentNode.GetId())] = dependentNode;
        }
    }

    public void Process(SerializedNode serializedNode) =>
        Deserialize(serializedNode);

    public IEnumerable<INode> Finish()
    {
        foreach (var compressedId in _nodesById.Keys)
        {
            Parent(compressedId);
            Containments(compressedId);
            References(compressedId);
            Annotations(compressedId);
        }

        return FilterRootNodes()
            .ToList();
    }

    private void Deserialize(SerializedNode serializedNode)
    {
        var id = serializedNode.Id;
        var compressedId = CompressedId.Create(id);

        var node = Instantiate(id, CompressedMetaPointer.Create(serializedNode.Classifier));
        _nodesById[compressedId] = node;

        Properties(serializedNode, node);

        if (serializedNode.Containments.Length != 0)
        {
            _containmentsByOwnerId[compressedId] = serializedNode
                .Containments
                .Select(c => (
                    CompressedMetaPointer.Create(c.Containment),
                    c
                        .Children
                        .Select(CompressedId.Create)
                        .ToList()
                ))
                .ToList();
        }

        if (serializedNode.References.Length != 0)
        {
            _referencesByOwnerId[compressedId] = serializedNode
                .References
                .Select(r => (
                    CompressedMetaPointer.Create(r.Reference),
                    r
                        .Targets
                        .Select(t => (CompressedId.Create(t.Reference), t.ResolveInfo))
                        .ToList()
                ))
                .ToList();
        }

        if (serializedNode.Annotations.Length != 0)
        {
            _annotationsByOwnerId[compressedId] = serializedNode
                .Annotations
                .Select(CompressedId.Create)
                .ToList();
        }

        if (serializedNode.Parent != null)
        {
            _parentByNodeId[compressedId] = CompressedId.Create(serializedNode.Parent);
        }
    }

    private void Properties(SerializedNode serializedNode, INode node)
    {
        var id = serializedNode.Id;

        foreach (var serializedProperty in serializedNode.Properties.Where(p => p.Value != null))
        {
            var property = FindFeature<Property>(node, CompressedMetaPointer.Create(serializedProperty.Property));
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
    }

    #region NodesWithProperties

    private INode Instantiate(string id, CompressedMetaPointer compressedMetaPointer)
    {
        if (!_classifiers.TryGetValue(compressedMetaPointer, out var classifier))
        {
            classifier = Handler.UnknownClassifier(id, compressedMetaPointer);
            if (classifier == null)
                throw new DeserializerException($"On node with id={id}:");
            // throw new UnsupportedClassifierException(compressedMetaPointer, $"On node with id={id}:");
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

    private void Parent(CompressedId compressedId)
    {
        if (!_parentByNodeId.TryGetValue(compressedId, out var parentCompressedId))
            return;

        INode? parent = FindParent(compressedId, parentCompressedId);

        if (parent != null)
            _nodesById[compressedId].SetParent(parent);
    }

    private INode? FindParent(CompressedId compressedId, CompressedId parentId)
    {
        if (_nodesById.TryGetValue(parentId, out INode? existingParent))
            return existingParent;

        var parent = Handler.UnknownParent(parentId, _nodesById[compressedId]);

        return parent;
    }

    #endregion

    #region Containments

    private void Containments(CompressedId compressedId)
    {
        if (!_containmentsByOwnerId.TryGetValue(compressedId, out var containments))
            return;

        INode node = _nodesById[compressedId];

        foreach ((var compressedMetaPointer, var compressedChildrenIds) in containments)
        {
            var containment = FindFeature<Containment>(node, compressedMetaPointer);
            var children = compressedChildrenIds
                .Select(childId => FindChild(node, childId))
                .Where(c => c != null)
                .ToList();

            if (children.Count != 0)
            {
                node.Set(
                    containment,
                    containment.Multiple ? children : children.FirstOrDefault()
                );
            }
        }
    }

    private INode? FindChild(INode node, CompressedId childId)
    {
        if (_nodesById.TryGetValue(childId, out var existingPair))
            return existingPair;

        var healed = Handler.UnknownChild(childId, node);
        return healed;
    }

    #endregion

    #region References

    private void References(CompressedId compressedId)
    {
        if (!_referencesByOwnerId.TryGetValue(compressedId, out var references))
            return;

        INode node = _nodesById[compressedId];
        foreach ((var compressedMetaPointer, var targetEntries) in references)
        {
            var reference = FindFeature<Reference>(node, compressedMetaPointer);
            var targets = targetEntries
                .Select(target => FindReferenceTarget(node, target.Item1, target.Item2))
                .Where(c => c != null)
                .ToList();

            if (targets.Count != 0)
            {
                node.Set(
                    reference,
                    reference.Multiple ? targets : targets.FirstOrDefault()
                );
            }
        }
    }

    private IReadableNode? FindReferenceTarget(INode node, CompressedId targetId, string? resolveInfo)
    {
        if (_nodesById.TryGetValue(targetId, out var ownNode))
            return ownNode;

        if (_dependentNodesById.TryGetValue(targetId, out var dependentNode))
            return dependentNode;

        IReadableNode? healed = Handler.UnknownReference(targetId, resolveInfo, node);
        return healed;
    }

    #endregion

    #region Annotations

    private void Annotations(CompressedId compressedId)
    {
        if (!_annotationsByOwnerId.TryGetValue(compressedId, out var annotationIds))
            return;

        INode node = _nodesById[compressedId];

        var annotations = annotationIds
            .Select(annId => FindAnnotation(node, annId))
            .Where(c => c != null)
            .ToList();

        node.AddAnnotations(annotations);
    }

    private INode? FindAnnotation(INode node, CompressedId annotationId)
    {
        if (_nodesById.TryGetValue(annotationId, out var existing))
            return existing;

        var healed = Handler.UnknownAnnotation(annotationId, node);
        return healed;
    }

    #endregion

    private IEnumerable<INode> FilterRootNodes() =>
        _nodesById
            .Values
            .Where(node => node.GetParent() == null);

    private TFeature FindFeature<TFeature>(INode node, CompressedMetaPointer compressedMetaPointer)
        where TFeature : Feature
    {
        Classifier classifier = node.GetClassifier();
        var id = node.GetId();
        if (!_features.TryGetValue(compressedMetaPointer, out var feature))
        {
            feature = Handler.UnknownFeature(compressedMetaPointer, node);
            if (feature == null)
                // throw new UnknownFeatureException(classifier, compressedMetaPointer, $"On node with id={id}:");
                throw new DeserializerException($"On node with id={id}:");
        }

        if (feature is not TFeature f)
            // throw new UnknownFeatureException(classifier, compressedMetaPointer, $"On node with id={id}:");
            throw new DeserializerException($"On node with id={id}:");

        return f;
    }
}

internal class DeserializerException(string? message) : LionWebExceptionBase(message);