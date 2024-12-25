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

namespace LionWeb_CSharp_Test.languages;

using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

public static class SerializationChunkExtensions
{
    private static List<SerializedNode> _nodes;
    private static Dictionary<string, List<SerializedProperty>> _properties;

    public static void Initialize(this SerializationChunk serializationChunk)
    {
        _nodes = [];
        _properties = [];
    }

    public static SerializedNode Node(this SerializationChunk serializationChunk, string id, MetaPointer metaPointer)
    {
        var serializedNode = new SerializedNode { Id = id, Classifier = metaPointer };
        _nodes.Add(serializedNode);
        return serializedNode;
    }

    public static SerializedNode Node(this SerializationChunk serializationChunk, string id, Concept concept)
    {
        var serializedNode = new SerializedNode { Id = id, Classifier = concept.ToMetaPointer() };
        _nodes.Add(serializedNode);
        return serializedNode;
    }

    public static SerializedNode[] Nodes(this SerializationChunk serializationChunk)
        => _nodes.ToArray();

    public static SerializedProperty[] Properties(this SerializedNode serializedNode)
        => _properties[serializedNode.Id].ToArray();

    public static SerializedNode Property(this SerializedNode serializedNode, Property property)
    {
        var serializedProperty = new SerializedProperty { Property = property.ToMetaPointer() };
        AddProperty(serializedNode, serializedProperty);
        return serializedNode;
    }

    public static SerializedNode Property(this SerializedNode serializedNode, MetaPointer metaPointer)
    {
        var property = new SerializedProperty { Property = metaPointer };
        AddProperty(serializedNode, property);
        return serializedNode;
    }

    public static SerializedNode Property(this SerializedNode serializedNode, MetaPointer metaPointer, string? value)
    {
        var property = new SerializedProperty { Property = metaPointer, Value = value };
        AddProperty(serializedNode, property);
        return serializedNode;
    }

    public static SerializedNode Property(this SerializedNode serializedNode, Property property, string? value)
    {
        var serializedProperty = new SerializedProperty { Property = property.ToMetaPointer(), Value = value };
        AddProperty(serializedNode, serializedProperty);
        return serializedNode;
    }

    private static void AddProperty(SerializedNode serializedNode, SerializedProperty serializedProperty)
    {
        if (!_properties.TryGetValue(serializedNode.Id, out List<SerializedProperty> valueList))
        {
            valueList = new List<SerializedProperty>();
            _properties.Add(serializedNode.Id, valueList);
        }

        valueList.Add(serializedProperty);
    }
}