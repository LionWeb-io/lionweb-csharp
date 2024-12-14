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

/// <summary>
/// Converts <see cref="SerializedNode">SerializedNodes</see> into <see cref="IReadableNode">IReadableNodes</see>.
///
/// <para>
/// API contract:
/// <list type="number">
/// <item>Create the deserializer, either via <see cref="DeserializerBuilder"/> (for M1) or <see cref="LanguageDeserializer"/> (M2).</item>
/// <item>Set it up by <see cref="RegisterInstantiatedLanguage"/> and/or <see cref="RegisterDependentNodes"/>.</item>
/// <item>Call <see cref="Process"/> for each SerializedNode.</item>
/// <item>Retrieve the deserialized nodes from <see cref="Finish"/></item>
/// </list>
/// </para>
/// </summary>
/// <seealso cref="JsonUtils">Higher-level API</seealso>
/// <seealso cref="IDeserializerExtensions">Extensions</seealso>
public interface IDeserializer
{
    /// Optional handler to customize this deserializer's behaviour in non-regular situations.
    IDeserializerHandler Handler { get; init; }

    /// <summary>
    /// Enables this deserializer to create instances of <paramref name="language"/>'s entities.
    /// </summary>
    /// <param name="language">Language to make known to this deserializer.</param>
    /// <param name="factory">Special factory to use for instantiating entities of <paramref name="language"/>.</param>
    /// <exception cref="VersionMismatchException">If <paramref name="language"/>'s LionWeb version is not compatible with <see cref="LionWebVersion"/>.</exception>
    void RegisterInstantiatedLanguage(Language language, INodeFactory? factory = null);

    /// <summary>
    /// Enables this deserializer to create references to <paramref name="dependentNodes"/>.
    /// </summary>
    /// <param name="dependentNodes">Nodes that should be referenceable by deserialized nodes.</param>
    void RegisterDependentNodes(IEnumerable<IReadableNode> dependentNodes);

    /// <summary>
    /// Starts internal processing of <paramref name="serializedNode"/>.
    /// </summary>
    /// <param name="serializedNode">Node to process.</param>
    /// <remarks>We separate processing single nodes from <see cref="Finish">finishing</see> to enable streaming.</remarks>
    void Process(SerializedNode serializedNode);

    /// <summary>
    /// Takes all <see cref="Process">processed</see> nodes, shapes them into a (forest of) trees, and establishes references. 
    /// </summary>
    /// <returns>
    /// The root (i.e.: parent-less) nodes among the deserialization of the processed nodes.
    /// References to any of the given dependent nodes are resolved as well.
    /// </returns>
    /// <exception cref="DeserializerException"/>
    IEnumerable<IReadableNode> Finish();

    /// Version of LionWeb standard to use.
    LionWebVersions LionWebVersion { get; }
}

/// <inheritdoc />
/// <typeparam name="T">Type of node to return</typeparam>
public interface IDeserializer<out T> : IDeserializer where T : IReadableNode
{
    IEnumerable<IReadableNode> IDeserializer.Finish() =>
        Finish().Cast<IReadableNode>();

    /// <inheritdoc cref="IDeserializer.Finish"/>
    new IEnumerable<T> Finish();
}

public class DeserializerBuilder
{
    private readonly Dictionary<Language, INodeFactory> _languages = new();
    private readonly HashSet<IReadableNode> _dependentNodes = new();
    private IDeserializerHandler? _handler;
    private bool _storeUncompressedIds = false;
    private LionWebVersions _lionWebVersion = LionWebVersions.Current;

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

    public DeserializerBuilder WithDependentNodes(IEnumerable<IReadableNode> dependentNodes)
    {
        foreach (var dependentNode in dependentNodes)
        {
            _dependentNodes.Add(dependentNode);
        }

        return this;
    }

    public DeserializerBuilder WithUncompressedIds(bool storeUncompressedIds = true)
    {
        _storeUncompressedIds = storeUncompressedIds;
        return this;
    }

    public DeserializerBuilder WithLionWebVersion(LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;
        return this;
    }

    public IDeserializer Build()
    {
        IDeserializer result = CreateDeserializer(_lionWebVersion);
        foreach ((Language language, INodeFactory factory) in _languages)
        {
            result.RegisterInstantiatedLanguage(language, factory);
        }

        result.RegisterDependentNodes(_dependentNodes);

        return result;
    }

    private IDeserializer CreateDeserializer(LionWebVersions lionWebVersion)
    {
        var versionSpecifics =
            IDeserializerVersionSpecifics.Create(lionWebVersion);

        return _handler == null
            ? new Deserializer(versionSpecifics) { StoreUncompressedIds = _storeUncompressedIds }
            : new Deserializer(versionSpecifics) { StoreUncompressedIds = _storeUncompressedIds, Handler = _handler };
    }
}

public static class IDeserializerExtensions
{
    /// <returns>The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializationChunk">serialization chunk</paramref>.</returns>
    /// <exception cref="InvalidDataException">Thrown when the serialization references a <see cref="Concept"/> that couldn't be found in the languages this instance is parametrized with.</exception>
    public static List<IReadableNode> Deserialize(this IDeserializer deserializer,
        SerializationChunk serializationChunk) =>
        deserializer.Deserialize(serializationChunk, []);

    /// <returns>
    /// The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializationChunk">serialization chunk</paramref>.
    /// References to any of the given dependent nodes are resolved as well.
    /// </returns>
    /// <exception cref="InvalidDataException">Thrown when the serialization references a <see cref="Concept"/> that couldn't be found in the languages this instance is parametrized with.</exception>
    public static List<IReadableNode> Deserialize(this IDeserializer deserializer,
        SerializationChunk serializationChunk,
        IEnumerable<INode> dependentNodes)
    {
        if (serializationChunk.SerializationFormatVersion != deserializer.LionWebVersion.VersionString)
            throw new VersionMismatchException(serializationChunk.SerializationFormatVersion, deserializer.LionWebVersion.VersionString);
        return deserializer.Deserialize(serializationChunk.Nodes, dependentNodes);
    }

    public static List<IReadableNode> Deserialize(this IDeserializer deserializer,
        IEnumerable<SerializedNode> serializedNodes) =>
        deserializer.Deserialize(serializedNodes, []);

    public static List<IReadableNode> Deserialize(this IDeserializer deserializer,
        IEnumerable<SerializedNode> serializedNodes,
        IEnumerable<IReadableNode> dependentNodes)
    {
        deserializer.RegisterDependentNodes(dependentNodes);
        foreach (SerializedNode serializedNode in serializedNodes)
        {
            deserializer.Process(serializedNode);
        }

        return deserializer.Finish().ToList();
    }
}