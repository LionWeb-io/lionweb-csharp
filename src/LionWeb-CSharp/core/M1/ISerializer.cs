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

using M3;
using Serialization;

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
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = serializer.UsedLanguages.ToArray(),
            Nodes = serializedNodes
        };
    }

    public static IEnumerable<SerializedNode> SerializeDescendants(this ISerializer serializer,
        IEnumerable<INode> nodes) =>
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