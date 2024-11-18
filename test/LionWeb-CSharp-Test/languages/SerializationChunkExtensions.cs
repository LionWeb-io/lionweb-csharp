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

/*
  serializationChunk.Node("A").Classifier("key-Shapes", "1", "key-OffsetDuplicate")
  serializationChunk.Node("A").Classifier(ShapesLanguage.Instance.OffsetDuplicate)
  serializationChunk.Node("A").Classifier(typeof(OffsetDuplicate)) //reflection
  serializationChunk.Node("A").Properties().Property(...).Value(...)

*/

public static class SerializationChunkExtensions
{
    public static SerializedNode Node(this SerializationChunk serializationChunk, string id)
    {
        var serializedNode = new SerializedNode
        {
            Id = id,
            Classifier = null!,
            Properties = new SerializedProperty[] { },
            Containments = new SerializedContainment[] { },
            References = new SerializedReference[] { },
            Annotations = new string[] { }
        };

        SerializedNode[] serializationChunkNodes = serializationChunk.Nodes;
        Array.Resize(ref serializationChunkNodes, serializationChunkNodes.Length + 1);
        serializationChunkNodes[^1] = serializedNode;
        serializationChunk.Nodes = serializationChunkNodes;
        return serializedNode;
    }

    public static SerializedNode Classifier(this SerializedNode serializedNode, MetaPointer metaPointer)
    {
        serializedNode.Classifier = metaPointer;
        return serializedNode;
    }

    public static SerializedNode Classifier(this SerializedNode serializedNode, Concept concept)
    {
        serializedNode.Classifier = concept.ToMetaPointer();
        return serializedNode;
    }

    public static SerializedNode Property(this SerializedNode serializedNode, MetaPointer metaPointer)
    {
        var property = new SerializedProperty { Property = metaPointer };
        SerializedProperty[] serializedNodeProperties = serializedNode.Properties;
        Array.Resize(ref serializedNodeProperties, serializedNodeProperties.Length + 1);
        serializedNodeProperties[^1] = property;
        serializedNode.Properties = serializedNodeProperties;
        return serializedNode;
    }


    public static SerializedNode Property(this SerializedNode serializedNode, MetaPointer metaPointer, string? value)
    {
        var property = new SerializedProperty { Property = metaPointer, Value = value };
        SerializedProperty[] serializedNodeProperties = serializedNode.Properties;
        Array.Resize(ref serializedNodeProperties, serializedNodeProperties.Length + 1);
        serializedNodeProperties[^1] = property;
        serializedNode.Properties = serializedNodeProperties;
        return serializedNode;
    }

    public static SerializedNode Property(this SerializedNode serializedNode, Property property)
    {
        var serializedProperty = new SerializedProperty { Property = property.ToMetaPointer() };
        SerializedProperty[] serializedNodeProperties = serializedNode.Properties;
        Array.Resize(ref serializedNodeProperties, serializedNodeProperties.Length + 1);
        serializedNodeProperties[^1] = serializedProperty;
        serializedNode.Properties = serializedNodeProperties;
        return serializedNode;
    }

    public static SerializedNode Property(this SerializedNode serializedNode, Property property, string? value)
    {
        var serializedProperty = new SerializedProperty { Property = property.ToMetaPointer(), Value = value };
        SerializedProperty[] serializedNodeProperties = serializedNode.Properties;
        Array.Resize(ref serializedNodeProperties, serializedNodeProperties.Length + 1);
        serializedNodeProperties[^1] = serializedProperty;
        serializedNode.Properties = serializedNodeProperties;
        return serializedNode;
    }
}