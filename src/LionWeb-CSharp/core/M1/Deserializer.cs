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
public class Deserializer
{
    public Deserializer(IEnumerable<Language> languages)
    {
        var list = languages.ToList();
        _language2NodeFactory = list.ToDictionary(
            l => l,
            l => l.GetFactory()
        );
        _classifiers = list.SelectMany(
            l => l.Entities.OfType<Classifier>()
        );
        // TODO  pre-compute dictionaries Concept-key -> Concept[], Concept -> Features[] (all features), or even key this on the meta-pointer? -- for performance
    }

    private readonly IDictionary<Language, INodeFactory> _language2NodeFactory;
    private readonly IEnumerable<Classifier> _classifiers;

    private readonly Dictionary<string, INode> _nodesById = new();
    private Dictionary<string, IReadableNode> dependentNodesById;

    public void RegisterCustomFactory(Language language, INodeFactory factory) =>
        _language2NodeFactory[language] = factory;

    /// <returns>The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializationChunk">serialization chunk</paramref>.</returns>
    /// <exception cref="InvalidDataException">Thrown when the serialization references a <see cref="Concept"/> that couldn't be found in the languages this instance is parametrized with.</exception>
    public List<INode> Deserialize(SerializationChunk serializationChunk) =>
        Deserialize(serializationChunk, Enumerable.Empty<INode>());

    /// <returns>
    /// The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializationChunk">serialization chunk</paramref>.
    /// References to any of the given dependent nodes are resolved as well.
    /// </returns>
    /// <exception cref="InvalidDataException">Thrown when the serialization references a <see cref="Concept"/> that couldn't be found in the languages this instance is parametrized with.</exception>
    public List<INode> Deserialize(SerializationChunk serializationChunk, IEnumerable<INode> dependentNodes) =>
        Deserialize(serializationChunk.Nodes, dependentNodes);
    
    /// <returns>
    /// The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializedNodes">serialized nodes</paramref>.
    /// References to any of the given dependent nodes are resolved as well.
    /// </returns>
    /// <exception cref="InvalidDataException">Thrown when the serialization references a <see cref="Concept"/> that couldn't be found in the languages this instance is parametrized with.</exception>
    public List<INode> Deserialize(IEnumerable<SerializedNode> serializedNodes, IEnumerable<IReadableNode> dependentNodes)
    {
        var serializedNodesList = serializedNodes.ToList();
        NodesWithProperties(serializedNodesList);

        dependentNodesById = dependentNodes
            .SelectMany(node => M1Extensions.Descendants<IReadableNode>(node, true, true))
            .Distinct()
            .ToDictionary(node => node.GetId());

        foreach (var serializedNode in serializedNodesList)
        {
            var id = serializedNode.Id;
            var node = _nodesById[id];
            Parent(serializedNode, node, id);
            Containments(serializedNode, node, id);
            References(serializedNode, node, id);
            Annotations(serializedNode, node, id);
        }

        return FilterRootNodes()
            .ToList();
    }

    private void NodesWithProperties(IEnumerable<SerializedNode> serializedNodes)
    {
        foreach (var serializedNode in serializedNodes)
        {
            var id = serializedNode.Id;
            var node = Instantiate(id, serializedNode.Classifier);
            _nodesById[id] = node;
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
        }
    }

    private void Parent(SerializedNode serializedNode, INode node, string id)
    {
        if (serializedNode.Parent != null)
        {
            if (_nodesById.TryGetValue(serializedNode.Parent, out INode? parent))
            {
                node.SetParent(parent);
            } else
            {
                LogError($"On node with id={id}: couldn't find specified parent - leaving this node orphaned.");
            }
        }
    }

    private void Containments(SerializedNode serializedNode, INode node, string id)
    {
        foreach (var serializedContainment in serializedNode.Containments)
        {
            var containment = FindFeature<Containment>(node.GetClassifier(), serializedContainment.Containment, id);
            var children = serializedContainment
                .Children
                .Select(childId =>
                {
                    if (_nodesById.TryGetValue(childId, out var existing))
                        return existing;
                    LogError($"On node with id={id}: couldn't find child with id={childId} - skipping.");
                    return null;
                })
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

    private void References(SerializedNode serializedNode, INode node, string id)
    {
        foreach (var serializedReference in serializedNode.References)
        {
            var reference = FindFeature<Reference>(node.GetClassifier(), serializedReference.Reference, id);
            var targets = serializedReference.Targets.Select(target =>
                {
                    var targetId = target.Reference;
                    if (_nodesById.TryGetValue(targetId, out INode? ownNode))
                        return ownNode;

                    if (dependentNodesById.TryGetValue(targetId, out var dependentNode))
                        return dependentNode;

                    LogError($"On node with id={id}: couldn't find reference with id={targetId} - skipping.");
                    return null;
                })
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

    private void Annotations(SerializedNode serializedNode, INode node, string id)
    {
        foreach (var annotationId in serializedNode.Annotations)
        {
            if (_nodesById.TryGetValue(annotationId, out INode? ownNode))
            {
                node.AddAnnotations([ownNode]);
            } else if (dependentNodesById.TryGetValue(annotationId, out var dependentNode) && dependentNode is IWritableNode writableNode)
            {
                node.AddAnnotations([writableNode]);
            } else
            {
                LogError($"On node with id={id}: couldn't find annotation with id={annotationId} - skipping.");
            }
        }
    }

    private IEnumerable<INode> FilterRootNodes() =>
        _nodesById
            .Values
            .Where(node => node.GetParent() == null);

    private INode Instantiate(string id, MetaPointer metaPointer)
    {
        var candidateClassifiers = _classifiers.Where(metaPointer.Matches).ToList();
        switch (candidateClassifiers.Count)
        {
            case 0:
                throw new UnsupportedClassifierException(metaPointer, $"On node with id={id}:");
            case > 1:
                LogError($"On node with id={id}: multiple classifiers found matching meta-pointer {metaPointer}.");
                break;
        }

        var classifier = candidateClassifiers.First();

        return _language2NodeFactory[classifier.GetLanguage()].CreateNode(id, classifier);
    }

    private TFeature FindFeature<TFeature>(Classifier classifier, MetaPointer metaPointer, string id)
        where TFeature : Feature
    {
        var candidateFeatures = classifier
            .AllFeatures()
            .OfType<TFeature>()
            .Where(f => metaPointer.Matches(f))
            .ToList();
        switch (candidateFeatures.Count)
        {
            case 0:
                throw new UnknownFeatureException(classifier, metaPointer, $"On node with id={id}:");
            case > 1:
                LogError(
                    $"On node with id={id}: multiple {typeof(TFeature).Name.ToLower()} features found matching meta-pointer {metaPointer}.");
                break;
        }

        return candidateFeatures.First();
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


    protected virtual void LogError(string message) =>
        Console.Error.WriteLine(message);
}