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
using Serialization;

public class Serializer : SerializerBase
{
    private readonly IEnumerable<INode> _nodes;

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
        var allNodes = _nodes
            .SelectMany(node => node.Descendants(true, true))
            .Distinct()
            .ToList();
        var languagesUsed = allNodes
            .Select(node => node.GetClassifier().GetLanguage())
            .Distinct();
        return new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages = languagesUsed
                .Where(language => language.Key != BuiltInsLanguage.LionCoreBuiltInsIdAndKey)
                .Select(SerializedLanguageReference)
                .ToArray(),
            Nodes = allNodes
                .Select(SerializeSimpleNode)
                .ToArray()
        };
    }
}