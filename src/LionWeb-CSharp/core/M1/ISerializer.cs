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

using Serialization;

public interface ISerializer
{
    ISerializerHandler Handler { get; init; }

    /// <param name="allNodes">Collection of nodes to be serialized. Does not transform the collection, i.e. does not consider descendants.</param>
    /// <remarks>
    /// We want to keep any transformation outside the serializer, as it might lead to duplicate nodes.
    /// Calling <c>Distinct()</c> implicitly creates a HashSet of the elements, violating the idea to stream nodes with minimal memory overhead. 
    /// </remarks>
    IEnumerable<SerializedNode> Serialize(IEnumerable<IReadableNode> allNodes);

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Lazily populated while processing <i>allNodes</i> of <see cref="Serialize"/>.
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
    public static SerializationChunk Serialize(this ISerializer serializer, IEnumerable<IReadableNode> allNodes)
    {
        SerializedNode[] serializedNodes = serializer.Serialize(allNodes).ToArray();
        return new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = serializer.UsedLanguages.ToArray(),
            Nodes = serializedNodes
        };
    }

    public static IEnumerable<SerializedNode> SerializeDescendants(this ISerializer serializer,
        IEnumerable<IReadableNode> nodes) => serializer.Serialize(nodes.AllNodes());

    /// <summary>
    /// Serializes a given <paramref name="nodes">iterable collection of nodes</paramref>, including all descendants and annotations.
    /// Disregards duplicate nodes, but fails on duplicate node ids.
    /// </summary>
    /// 
    /// <returns>A data structure that can be directly serialized/unparsed to JSON.</returns>
    public static SerializationChunk SerializeToChunk(this ISerializer serializer, IEnumerable<IReadableNode> nodes) =>
        Serialize(serializer, nodes.AllNodes().Distinct());

    private static IEnumerable<IReadableNode> AllNodes(this IEnumerable<IReadableNode> nodes) =>
        nodes.SelectMany(n => M1Extensions.Descendants(n, [], true, true));
}