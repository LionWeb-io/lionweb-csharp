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

public class Serializer : SerializerBase
{
    private readonly IEnumerable<INode> _nodes;
    private readonly HashSet<Language> _usedLanguages = new();

    public ISerializerHandler Handler { get; init; } = new SerializerExceptionHandler();

    public Serializer(IEnumerable<INode> nodes)
    {
        _nodes = nodes;
    }

    /// <summary>
    /// Serializes a given <paramref name="nodes">iterable collection of nodes</paramref>.
    /// </summary>
    /// 
    /// <returns>A data structure that can be directly serialized/unparsed to JSON.</returns>
    public static SerializationChunk Serialize(IEnumerable<INode> nodes) =>
        new Serializer(nodes).Serialize();

    public SerializationChunk Serialize()
    {
        var serializedNodes = SerializeToNodes()
            .ToArray();
        var languagesUsed = UsedLanguages()
            .ToArray();
        return new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current, Languages = languagesUsed, Nodes = serializedNodes
        };
    }

    public IEnumerable<SerializedNode> SerializeToNodes() =>
        AllNodes()
            .Select(RegisterUsedLanguage)
            .Select(SerializedNode);

    public IEnumerable<SerializedLanguageReference> UsedLanguages() =>
        _usedLanguages
            .Select(SerializeLanguageReference);

    private INode RegisterUsedLanguage(INode node)
    {
        Language language = node.GetClassifier().GetLanguage();
        if (language.Key != BuiltInsLanguage.LionCoreBuiltInsIdAndKey)
        {
            Language? existingLanguage = _usedLanguages.FirstOrDefault(l => l != language && l.Key == language.Key && l.Version == language.Version);
            if (existingLanguage != null)
            {
                Handler.DuplicateUsedLanguage(existingLanguage, language);
            } else
            {
                _usedLanguages.Add(language);
            }
        }

        return node;
    }

    private class NodeIdEqualityComparer(ISerializerHandler handler) : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            if (x != y)
                return false;

            handler.DuplicateNodeId(x);
            return true;
        }

        public int GetHashCode(string obj) =>
            obj.GetHashCode();
    }

    private IEnumerable<INode> AllNodes() =>
        _nodes
            .SelectMany(node => node.Descendants(true, true))
            .DistinctBy(n => n.GetId(), new NodeIdEqualityComparer(Handler));

    private SerializedNode SerializedNode(INode node) =>
        new()
        {
            Id = node.GetId(),
            Classifier = node.GetClassifier().ToMetaPointer(),
            Properties = node.GetClassifier().AllFeatures().OfType<Property>()
                .Select(property => SerializePropertySetting(node, property)).ToArray(),
            Containments = node.GetClassifier().AllFeatures().OfType<Containment>()
                .Select(containment => SerializedContainmentSetting(node, containment)).ToArray(),
            References = node.GetClassifier().AllFeatures().OfType<Reference>()
                .Select(reference => SerializedReferenceSetting(node, reference)).ToArray(),
            Annotations = node.GetAnnotations()
                .Select(annotation => annotation.GetId()).ToArray(),
            Parent = node.GetParent()?.GetId()
        };

    private SerializedContainment SerializedContainmentSetting(INode node, Containment containment)
    {
        var value = GetValueIfSet(node, containment);
        return new SerializedContainment
        {
            Containment = containment.ToMetaPointer(),
            Children = value != null
                ? containment.AsNodes<INode>(value).Select((child) => child.GetId()).ToArray()
                : []
        };
    }

    private SerializedReference SerializedReferenceSetting(INode node, Reference reference)
    {
        var value = GetValueIfSet(node, reference);
        return new()
        {
            Reference = reference.ToMetaPointer(),
            Targets = value != null
                ? reference.AsNodes<INode>(value).Select(SerializeReferenceTarget).ToArray()
                : []
        };
    }
}

public interface ISerializerHandler
{
    void DuplicateNodeId(INode a, INode b);
    void DuplicateUsedLanguage(Language a, Language b);
    void DuplicateNodeId(string? s);
}

public class SerializerExceptionHandler : ISerializerHandler
{
    public virtual void DuplicateNodeId(INode a, INode b) =>
        throw new ArgumentException($"nodes have same id '{a?.GetId() ?? b?.GetId()}': {a}, {b}");

    public void DuplicateNodeId(string? s) => 
        throw new ArgumentException($"nodes have same id '{s}'");

    public virtual void DuplicateUsedLanguage(Language a, Language b) =>
        throw new ArgumentException($"different languages with same key '{a?.Key ?? b?.Key}': {a}, {b}");
}