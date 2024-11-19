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

public interface IDeserializer
{
    IDeserializerHandler Handler { get; init; }

    LionWebVersions LionWebVersion { get; }

    void RegisterInstantiatedLanguage(Language language, INodeFactory factory);

    void RegisterDependentNodes(IEnumerable<IReadableNode> dependentNodes);

    /// <returns>
    /// The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializedNodes">serialized nodes</paramref>.
    /// References to any of the given dependent nodes are resolved as well.
    /// </returns>
    /// <exception cref="InvalidDataException">Thrown when the serialization references a <see cref="Concept"/> that couldn't be found in the languages this instance is parametrized with.</exception>
    void Process(SerializedNode serializedNode);

    IEnumerable<IReadableNode> Finish();
}

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

    // var primitiveTypeConverter = _lionWebVersion switch
    // {
    //     LionWebVersions.Version2023_1 => PrimitiveTypeConverter_2023_1,
    //     LionWebVersions.Version2024_1 => PrimitiveTypeConverter_2024_1,
    //     _ => throw new UnsupportedVersionException(_lionWebVersion)
    // };
    //     
    // IDeserializer result = _handler != null
    //     ? new Deserializer(_lionWebVersion) { Handler = _handler, StoreUncompressedIds = _storeUncompressedIds, PrimitiveTypeConverter = primitiveTypeConverter}
    //     : new Deserializer(_lionWebVersion) { StoreUncompressedIds = _storeUncompressedIds, PrimitiveTypeConverter = primitiveTypeConverter };

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

    private IDeserializer CreateDeserializer<T>(T lionWebVersion) where T : LionWebVersions
    {
        IDeserializer result = Create(lionWebVersion,
            _storeUncompressedIds, _handler);
        return result;
    }

    public static Deserializer<TVersion> Create<TVersion>(TVersion lionWebVersion, bool storeUncompressedIds,
        IDeserializerHandler? handler = null) where TVersion : LionWebVersions
    {
        object deserializerDelegates = (lionWebVersion switch
        {
            LionWebVersions.Version2023_1 v => new DeserializerDelegates_2023_1(),

            LionWebVersions.Version2024_1 v => new DeserializerDelegates_2024_1(),
            _ => throw new UnsupportedVersionException(lionWebVersion)
        });
        if (handler == null)
            return new Deserializer<TVersion>(lionWebVersion)
            {
                DeserializerDelegates = (IDeserializerDelegates<TVersion>)(object)deserializerDelegates, StoreUncompressedIds = storeUncompressedIds
            };
        return new Deserializer<TVersion>(lionWebVersion)
        {
            DeserializerDelegates = (IDeserializerDelegates<TVersion>)(object)deserializerDelegates,
            StoreUncompressedIds = storeUncompressedIds,
            Handler = handler
        };
    }

    private class DeserializerDelegates_2023_1 : IDeserializerDelegates<LionWebVersions.IVersion2023_1>
    {
        // public IDeserializerDelegates<T> Create<T>() where T : LionWebVersions.IVersion2023_1, new() => new DeserializerDelegates_2023_1();

        public object? ConvertPrimitiveType(DeserializerBase<LionWebVersions.IVersion2023_1> self, IWritableNode node,
            Property property, string value)
        {
            CompressedId compressedId = self.Compress(node.GetId());
            return property.Type switch
            {
                var b when b == BuiltInsLanguage_2023_1.Instance.Boolean => bool.TryParse(value, out var result)
                    ? result
                    : self.Handler.InvalidPropertyValue<bool>(value, property, compressedId),
                var i when i == BuiltInsLanguage_2023_1.Instance.Integer => int.TryParse(value, out var result)
                    ? result
                    : self.Handler.InvalidPropertyValue<int>(value, property, compressedId),
                // leave both a String and JSON value as a string:
                var s when s == BuiltInsLanguage_2023_1.Instance.String ||
                           s == BuiltInsLanguage_2023_1.Instance.Json => value,
                _ => self.Handler.UnknownDatatype(property, value, node)
            };
        }
    }

    private class DeserializerDelegates_2024_1 : IDeserializerDelegates<LionWebVersions.IVersion2024_1>
    {
        public object? ConvertPrimitiveType(DeserializerBase<LionWebVersions.IVersion2024_1> self, IWritableNode node,
            Property property, string value)
        {
            CompressedId compressedId = self.Compress(node.GetId());
            return property.Type switch
            {
                var b when b == BuiltInsLanguage_2024_1.Instance.Boolean => bool.TryParse(value, out var result)
                    ? result
                    : self.Handler.InvalidPropertyValue<bool>(value, property, compressedId),
                var i when i == BuiltInsLanguage_2024_1.Instance.Integer => int.TryParse(value, out var result)
                    ? result
                    : self.Handler.InvalidPropertyValue<int>(value, property, compressedId),
                // leave a String value as a string:
                var s when s == BuiltInsLanguage_2024_1.Instance.String => value,
                _ => self.Handler.UnknownDatatype(property, value, node)
            };
        }
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
        IEnumerable<INode> dependentNodes) =>
        deserializer.Deserialize(serializationChunk.Nodes, dependentNodes);

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