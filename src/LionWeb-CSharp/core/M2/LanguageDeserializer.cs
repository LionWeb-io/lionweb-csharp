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

namespace LionWeb.Core.M2;

using M1;
using M3;
using Serialization;

/// <summary>
/// A deserializer that deserializes serializations of <see cref="Language"/>s.
/// The generic deserializer isn't aware of the LionCore M3-types (and their idiosyncrasies),
/// so that can't be used.
/// </summary>
public class LanguageDeserializer
{
    private readonly SerializedNode[] _serializedNodes;
    private readonly Dictionary<string, SerializedNode> _serializedNodesById;
    private readonly Dictionary<string, IKeyed> _nodesById = new();
    private readonly Dictionary<string, IKeyed> _dependentNodesById;

    private LanguageDeserializer(SerializationChunk serializationChunk, params Language[] dependentLanguages)
    {
        _serializedNodes = serializationChunk.Nodes;
        _serializedNodesById = _serializedNodes.ToDictionary(
            serializedNode => serializedNode.Id
        );
        _dependentNodesById =
            dependentLanguages.Concat([BuiltInsLanguage.Instance])
                .SelectMany(language => M1Extensions.Descendants<IKeyed>(language, true))
                .ToDictionary(node => node.GetId());
    }

    private IKeyed MemoisedDeserialization(string id)
    {
        var serializedNode = _serializedNodesById[id];
        IKeyed node;
        if (!_nodesById.TryGetValue(id, out IKeyed? value))
        {
            var parentId = serializedNode.Parent;
            node = CreateNodeFromWithProperties(serializedNode,
                (parentId == null || !_nodesById.ContainsKey(parentId))
                    ? null
                    : MemoisedDeserialization(parentId)
            );
            _nodesById[node.GetId()] = node;
        } else
        {
            node = value;
        }

        return node;
    }

    private T LookupNode<T>(string id) where T : class, IKeyed
    {
        if (_dependentNodesById.TryGetValue(id, out var node))
            return (T)node;
        return (T)_nodesById[id];
    }

    private IEnumerable<DynamicLanguage> DeserializedLanguages()
    {
        foreach (var serializedNode in _serializedNodes)
        {
            var id = serializedNode.Id;
            _nodesById[id] = MemoisedDeserialization(id);
        }

        foreach (var serializedNode in _serializedNodes)
        {
            InstallContainments(serializedNode);
            InstallReferences(serializedNode);
        }

        return _nodesById.Values.OfType<DynamicLanguage>();
    }

    /// <summary>
    /// Deserializes the given <paramref name="serializationChunk">serialization chunk</paramref> as an iterable collection of <see cref="Language"/>s.
    /// The <paramref name="dependentLanguages">dependent languages</paramref> should contain all languages that are referenced by the top-level
    /// <c>languages</c> property of the serialization chunk.
    /// </summary>
    /// 
    /// <returns>The deserialization of the language definitions present in the given <paramref name="serializationChunk"/>.</returns>
    public static IEnumerable<DynamicLanguage> Deserialize(SerializationChunk serializationChunk,
        params Language[] dependentLanguages)
        => new LanguageDeserializer(serializationChunk, dependentLanguages).DeserializedLanguages();

    private static DynamicIKeyed CreateNodeFromWithProperties(SerializedNode serializedNode, IKeyed? parent)
    {
        var serializedPropertiesByKey = serializedNode.Properties.ToDictionary(
            serializedProperty => serializedProperty.Property.Key,
            serializedProperty => serializedProperty.Value
        );
        var id = serializedNode.Id;
        var key = serializedPropertiesByKey["IKeyed-key"];
        var name = serializedPropertiesByKey["LionCore-builtins-INamed-name"];
        return serializedNode.Classifier.Key switch
        {
            "Annotation" => new DynamicAnnotation(id, parent as DynamicLanguage) { Key = key, Name = name },
            "Concept" => new DynamicConcept(id, parent as DynamicLanguage)
            {
                Key = key,
                Name = name,
                Abstract = serializedPropertiesByKey["Concept-abstract"] == "true",
                Partition = serializedPropertiesByKey["Concept-partition"] == "true"
            },
            "Containment" => new DynamicContainment(id, parent as DynamicClassifier)
            {
                Key = key,
                Name = name,
                Optional = IsOptional(serializedPropertiesByKey),
                Multiple = IsMultiple(serializedPropertiesByKey)
            },
            "Enumeration" => new DynamicEnumeration(id, parent as DynamicLanguage) { Key = key, Name = name },
            "EnumerationLiteral" => new DynamicEnumerationLiteral(id, parent as DynamicEnumeration)
            {
                Key = key, Name = name
            },
            "Interface" => new DynamicInterface(id, parent as DynamicLanguage) { Key = key, Name = name },
            "Language" => new DynamicLanguage(id)
            {
                Key = key, Name = name, Version = serializedPropertiesByKey["Language-version"]
            },
            "PrimitiveType" => new DynamicPrimitiveType(id, parent as DynamicLanguage) { Key = key, Name = name },
            "Property" => new DynamicProperty(id, parent as DynamicClassifier)
            {
                Key = key, Name = name, Optional = IsOptional(serializedPropertiesByKey)
            },
            "Reference" => new DynamicReference(id, parent as DynamicClassifier)
            {
                Key = key,
                Name = name,
                Optional = IsOptional(serializedPropertiesByKey),
                Multiple = IsMultiple(serializedPropertiesByKey)
            },
            _ => throw new UnsupportedClassifierException(serializedNode.Classifier)
        };
    }

    private static bool IsMultiple(Dictionary<string, string> serializedPropertiesByKey) =>
        serializedPropertiesByKey["Link-multiple"] == "true";

    private static bool IsOptional(Dictionary<string, string> serializedPropertiesByKey) =>
        serializedPropertiesByKey["Feature-optional"] == "true";


    private void InstallContainments(SerializedNode serializedNode)
    {
        var node = _nodesById[serializedNode.Id];

        ILookup<string, IKeyed> serializedContainmentsByKey = serializedNode
            .Containments
            .SelectMany(containment => containment.Children.Select(child => (containment, child)))
            .ToLookup(pair => pair.containment.Containment.Key, pair => LookupNode<IKeyed>(pair.child));

        switch (node)
        {
            case DynamicAnnotation annotation:
                {
                    annotation.AddFeatures(serializedContainmentsByKey["Classifier-features"].Cast<Feature>());
                    break;
                }
            case DynamicConcept concept:
                {
                    concept.AddFeatures(serializedContainmentsByKey["Classifier-features"].Cast<Feature>());
                    break;
                }
            case DynamicContainment:
                {
                    break;
                }
            case DynamicEnumeration enumeration:
                {
                    enumeration.AddLiterals(serializedContainmentsByKey["Enumeration-literals"]
                        .Cast<EnumerationLiteral>());
                    break;
                }
            case DynamicEnumerationLiteral:
                {
                    break;
                }
            case DynamicInterface @interface:
                {
                    @interface.AddFeatures(serializedContainmentsByKey["Classifier-features"].Cast<Feature>());
                    break;
                }
            case DynamicLanguage language:
                {
                    language.AddEntities(serializedContainmentsByKey["Language-entities"].Cast<LanguageEntity>());
                    break;
                }
            case DynamicPrimitiveType:
                {
                    break;
                }
            case DynamicProperty:
                {
                    break;
                }
            case DynamicReference:
                {
                    break;
                }
            default:
                {
                    logError($"installing containments in node of meta-concept {node.GetType().Name} not implemented");
                    break;
                }
        }
    }


    private T? ResolveSingleRef<T>(string key, ILookup<string, IKeyed> serializedReferencesByKey)
        where T : class, IKeyed
    {
        if (serializedReferencesByKey.Contains(key) && serializedReferencesByKey[key].Count() == 1)
            return serializedReferencesByKey[key].Cast<T>().First();

        return null;
    }

    private IEnumerable<T> ResolveMultiRef<T>(string key, ILookup<string, IKeyed> serializedReferencesByKey)
        where T : class, IKeyed
    {
        if (serializedReferencesByKey.Contains(key))
            return serializedReferencesByKey[key].Cast<T>();

        return [];
    }

    private void InstallReferences(SerializedNode serializedNode)
    {
        var node = _nodesById[serializedNode.Id];
        ILookup<string, IKeyed> serializedReferencesByKey = serializedNode
            .References
            .SelectMany(reference => reference.Targets.Select(target => (reference, target)))
            .ToLookup(pair => pair.reference.Reference.Key, pair => LookupNode<IKeyed>(pair.target.Reference));

        switch (node)
        {
            case DynamicAnnotation annotation:
                {
                    annotation.Extends = ResolveSingleRef<Annotation>("Annotation-extends", serializedReferencesByKey);
                    annotation.AddImplements(ResolveMultiRef<Interface>("Annotation-implements",
                        serializedReferencesByKey));
                    annotation.Annotates =
                        ResolveSingleRef<Classifier>("Annotation-annotates", serializedReferencesByKey);
                    break;
                }
            case DynamicConcept concept:
                {
                    concept.Extends = ResolveSingleRef<Concept>("Concept-extends", serializedReferencesByKey);
                    concept.AddImplements(ResolveMultiRef<Interface>("Concept-implements", serializedReferencesByKey));
                    break;
                }
            case DynamicContainment containment:
                {
                    containment.Type = ResolveSingleRef<Classifier>("Link-type", serializedReferencesByKey);
                    break;
                }
            case DynamicEnumeration:
                {
                    break;
                }
            case DynamicEnumerationLiteral:
                {
                    break;
                }
            case DynamicInterface @interface:
                {
                    @interface.AddExtends(ResolveMultiRef<Interface>("Interface-extends", serializedReferencesByKey));
                    break;
                }
            case DynamicLanguage:
                {
                    // TODO  dependsOn? -> dependent languages (insofar present and != BuiltIns)
                    break;
                }
            case DynamicPrimitiveType:
                {
                    break;
                }
            case DynamicProperty property:
                {
                    property.Type = ResolveSingleRef<Datatype>("Property-type", serializedReferencesByKey);
                    break;
                }
            case DynamicReference reference:
                {
                    reference.Type = ResolveSingleRef<Classifier>("Link-type", serializedReferencesByKey);
                    break;
                }
            default:
                {
                    logError($"installing references in node of meta-concept {node.GetType().Name} not implemented");
                    break;
                }
        }
    }

    protected virtual void logError(string message) =>
        Console.Error.WriteLine(message);
}