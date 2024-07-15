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
using Utilities;

public interface ISerializer
{
    ISerializerHandler Handler { get; init; }

    /// <param name="allNodes">Collection of nodes to be serialized. Does not transform the collection, i.e. does not consider descendants.</param>
    /// <remarks>
    /// We want to keep any transformation outside the serializer, as it might lead to duplicate nodes.
    /// Calling <c>Distinct()</c> implicitly creates a HashSet of the elements, violating the idea to stream nodes with minimal memory overhead. 
    /// </remarks>
    IEnumerable<SerializedNode> SerializeToNodes(IEnumerable<INode> allNodes);

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Lazily populated while processing <i>allNodes</i> of <see cref="SerializeToNodes"/>.
    /// </remarks>
    IEnumerable<SerializedLanguageReference> UsedLanguages { get; }
}

public static class ISerializerExtensions
{
    /// <param name="allNodes">Collection of nodes to be serialized. Does not transform the collection, i.e. does not consider descendants.</param>
    /// <remarks>
    /// We want to keep any transformation outside the serializer, as it might lead to duplicate nodes.
    /// Calling <c>Distinct()</c> implicitly creates a HashSet of the elements, violating the idea to stream nodes with minimal memory overhead. 
    /// </remarks>
    public static SerializationChunk Serialize(this ISerializer serializer, IEnumerable<INode> allNodes)
    {
        SerializedNode[] serializedNodes = serializer.SerializeToNodes(allNodes).ToArray();
        return new SerializationChunk
        {
            SerializationFormatVersion = "asf",
            Languages = serializer.UsedLanguages.ToArray(),
            Nodes = serializedNodes
        };
    }

    public static IEnumerable<SerializedNode> SerializeDescendants(this ISerializer serializer, IEnumerable<INode> nodes) =>
        serializer.SerializeToNodes(nodes.SelectMany(n => n.Descendants(true, true)));
}

public interface ISerializerHandler
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>The language to use, if any.</returns>
    Language? DuplicateUsedLanguage(Language a, Language b);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <remarks>
    /// It makes no sense to allow "healing" a duplicate id (e.g. by creating a new id).
    /// If we allowed this, we'd need to keep a map of all nodes to their (potentially "healed") id,
    /// in case we wanted to refer to it.
    /// This would clash with streaming nodes with minimal memory overhead.
    /// </remarks>
    void DuplicateNodeId(INode n);
}

public class Serializer : SerializerBase, ISerializer
{
    private readonly HashSet<Language> _usedLanguages = new();
    private readonly DuplicateIdChecker _duplicateIdChecker = new();

    /// <inheritdoc />
    public ISerializerHandler Handler { get; init; } = new SerializerExceptionHandler();

    /// <summary>
    /// Serializes a given <paramref name="nodes">iterable collection of nodes</paramref>, including all descendants and annotations.
    /// Disregards duplicate nodes, but fails on duplicate node ids.
    /// </summary>
    /// 
    /// <returns>A data structure that can be directly serialized/unparsed to JSON.</returns>
    public static SerializationChunk SerializeToChunk(IEnumerable<INode> nodes) =>
        new Serializer()
            .Serialize(nodes
                .SelectMany(n => n.Descendants(true, true))
                .Distinct()
            );

    public SerializationChunk Serialize(IEnumerable<INode> allNodes)
    {
        var serializedNodes = SerializeToNodes(allNodes)
            .ToArray();
        var languagesUsed = UsedLanguages
            .ToArray();
        return new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current, Languages = languagesUsed, Nodes = serializedNodes
        };
    }

    public IEnumerable<SerializedNode> SerializeToNodes(IEnumerable<INode> allNodes)
    {
        foreach (INode node in allNodes)
        {
            RegisterUsedLanguage(node);
            yield return SerializeNode(node);
        }
    }

    public IEnumerable<SerializedLanguageReference> UsedLanguages =>
        _usedLanguages
            .Select(SerializeLanguageReference);

    private void RegisterUsedLanguage(INode node)
    {
        Language language = node.GetClassifier().GetLanguage();
        if (language.Key == BuiltInsLanguage.LionCoreBuiltInsIdAndKey)
            return;

        Language? existingLanguage = _usedLanguages.FirstOrDefault(l => l != language && l.EqualsIdentity(language));
        if (existingLanguage != null)
        {
            Language? altLanguage = Handler.DuplicateUsedLanguage(existingLanguage, language);
            _usedLanguages.Add(altLanguage ??
                               throw new InvalidOperationException(
                                   $"Duplicate UsedLanguage: '{language}' vs. '{existingLanguage}'"));
        } else
        {
            _usedLanguages.Add(language);
        }
    }

    private SerializedNode SerializeNode(INode node)
    {
        var id = node.GetId();
        if (_duplicateIdChecker.IsIdDuplicate(id))
        {
            Handler.DuplicateNodeId(node);
            throw new InvalidOperationException($"Duplicate node id '{id}': {node}");
        }

        return new()
        {
            Id = id,
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
    }

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

public class SerializerExceptionHandler : ISerializerHandler
{
    public void DuplicateNodeId(INode n) =>
        throw new ArgumentException($"nodes have same id '{n.GetId()}': {n}");

    public virtual Language? DuplicateUsedLanguage(Language a, Language b) =>
        throw new ArgumentException(
            $"different languages with same key '{a?.Key ?? b?.Key}' / version '{a?.Version ?? b?.Version}': {a}, {b}");
}