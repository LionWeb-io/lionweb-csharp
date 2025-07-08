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
using CompressedReference = (CompressedMetaPointer, List<(ICompressedId?, ResolveInfo?)>);

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

    /// Installs all of <paramref name="references"/> into <paramref name="nodeId"/>, if the target can be found.
    /// Takes care of <see cref="IDeserializerHandler.UnresolvableReferenceTarget"/>.
    /// and <see cref="IDeserializerHandler.InvalidReference"/>.
    protected internal void InstallNodeReferences(ICompressedId nodeId, IEnumerable<CompressedReference> references);
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

/// Extensions to <see cref="IDeserializer"/>.
public static class IDeserializerExtensions
{
    /// <summary>Deserializes <paramref name="serializationChunk"/>.</summary>
    /// <param name="deserializer">Deserializer to use.</param>
    /// <param name="serializationChunk">Chunk to deserialize.</param>
    /// <param name="dependentNodes">Nodes that should be referencable during deserialization.</param>
    /// <returns>The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializationChunk">serialization chunk</paramref>.</returns>
    /// <exception cref="DeserializerException"/>
    /// <exception cref="VersionMismatchException">If <paramref name="serializationChunk"/>'s LionWeb version is not compatible with <paramref name="deserializer"/>'s <see cref="IDeserializer.LionWebVersion"/>.</exception>
    public static List<IReadableNode> Deserialize(this IDeserializer deserializer,
        SerializationChunk serializationChunk,
        IEnumerable<INode>? dependentNodes = null)
    {
        deserializer.LionWebVersion.AssureCompatible(serializationChunk.SerializationFormatVersion);
        return deserializer.Deserialize(serializationChunk.Nodes, dependentNodes ?? []);
    }

    /// <summary>Deserializes all of <paramref name="serializedNodes"/>.</summary>
    /// <param name="deserializer">Deserializer to use.</param>
    /// <param name="serializedNodes">Low-level nodes to deserialize.</param>
    /// <param name="dependentNodes">High-level nodes that should be referencable during deserialization.</param>
    /// <returns>The root (i.e.: parent-less) nodes among the deserialization of the given <paramref name="serializedNodes"/>.</returns>
    /// <exception cref="DeserializerException"/>
    public static List<IReadableNode> Deserialize(this IDeserializer deserializer,
        IEnumerable<SerializedNode> serializedNodes,
        IEnumerable<IReadableNode>? dependentNodes = null)
    {
        deserializer.RegisterDependentNodes(dependentNodes ?? []);
        foreach (SerializedNode serializedNode in serializedNodes)
        {
            deserializer.Process(serializedNode);
        }

        return deserializer.Finish().ToList();
    }
}