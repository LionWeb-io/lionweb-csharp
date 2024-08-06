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

using Serialization;

/// <inheritdoc cref="ISerializer"/>
public class Serializer : SerializerBase, ISerializer
{
    private readonly DuplicateIdChecker _duplicateIdChecker = new();

    /// <inheritdoc cref="ISerializerExtensions.SerializeToChunk"/>
    public static SerializationChunk SerializeToChunk(IEnumerable<IReadableNode> nodes) =>
        new Serializer().SerializeToChunk(nodes);

    /// <inheritdoc />
    public override IEnumerable<SerializedNode> Serialize(IEnumerable<IReadableNode> allNodes)
    {
        foreach (var node in allNodes)
        {
            RegisterUsedLanguage(node);
            var result = SerializeNode(node);
            if (result != null)
                yield return result;
        }
    }

    private SerializedNode? SerializeNode(IReadableNode node)
    {
        var id = node.GetId();
        if (_duplicateIdChecker.IsIdDuplicate(Compress(id)))
        {
            Handler.DuplicateNodeId(node);
            return null;
        }

        return SerializeSimpleNode(node);
    }
}