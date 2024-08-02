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

namespace LionWeb.Core.M1;

using M2;
using M3;
using Serialization;

public class DeserializerBuilder
{
    private IDeserializerHandler? _handler;
    private Dictionary<Language, INodeFactory> _languages = new();


    public DeserializerBuilder WithHandler(IDeserializerHandler handler)
    {
        _handler = handler;
        return this;
    }

    public DeserializerBuilder WithLanguage(Language language)
    {
        WithCustomFactory(language, language.GetFactory());
        return this;
    }

    public DeserializerBuilder WithLanguages(IEnumerable<Language> languages)
    {
        foreach (Language language in languages)
        {
            WithLanguage(language);
        }

        return this;
    }

    public DeserializerBuilder WithCustomFactory(Language language, INodeFactory factory)
    {
        _languages[language] = factory;
        return this;
    }

    public IDeserializer Build()
    {
        Deserializer result = _handler != null ? new Deserializer { Handler = _handler } : new Deserializer();

        foreach ((Language language, INodeFactory factory) in _languages)
        {
            result.RegisterLanguage(language, factory);
        }

        return result;
    }
}

public interface IDeserializer
{
    IDeserializerHandler Handler { get; init; }

    void RegisterLanguage(Language language, INodeFactory factory);

    /// <returns>
    /// The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializedNodes">serialized nodes</paramref>.
    /// References to any of the given dependent nodes are resolved as well.
    /// </returns>
    /// <exception cref="InvalidDataException">Thrown when the serialization references a <see cref="Concept"/> that couldn't be found in the languages this instance is parametrized with.</exception>
    List<INode> Deserialize(IEnumerable<SerializedNode> serializedNodes, IEnumerable<IReadableNode> dependentNodes);
}

public static class IDeserializerExtensions
{
    /// <returns>The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializationChunk">serialization chunk</paramref>.</returns>
    /// <exception cref="InvalidDataException">Thrown when the serialization references a <see cref="Concept"/> that couldn't be found in the languages this instance is parametrized with.</exception>
    public static List<INode> Deserialize(this IDeserializer deserializer, SerializationChunk serializationChunk) =>
        deserializer.Deserialize(serializationChunk, Enumerable.Empty<INode>());


    /// <returns>
    /// The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializationChunk">serialization chunk</paramref>.
    /// References to any of the given dependent nodes are resolved as well.
    /// </returns>
    /// <exception cref="InvalidDataException">Thrown when the serialization references a <see cref="Concept"/> that couldn't be found in the languages this instance is parametrized with.</exception>
    public static List<INode> Deserialize(this IDeserializer deserializer, SerializationChunk serializationChunk,
        IEnumerable<INode> dependentNodes) =>
        deserializer.Deserialize(serializationChunk.Nodes, dependentNodes);
}

public interface IDeserializerHandler
{
    Classifier? UnknownClassifier(string id, MetaPointer metaPointer);
    Feature? UnknownFeature(string id, Classifier classifier, MetaPointer metaPointer);
    INode? UnknownParent(string parentId, SerializedNode serializedNode, INode node);
    INode? UnknownChild(string childId, SerializedNode serializedNode, INode node);
    IReadableNode? UnknownReference(SerializedReferenceTarget target, SerializedNode serializedNode, INode node);
    INode? UnknownAnnotation(string annotationId, SerializedNode serializedNode, INode node);
}