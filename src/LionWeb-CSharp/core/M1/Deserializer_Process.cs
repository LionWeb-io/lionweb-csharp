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

public partial class Deserializer
{
    /// <inheritdoc />
    public override void Process(SerializedNode serializedNode)
    {
        var id = serializedNode.Id;

        var node = _deserializerMetaInfo.Instantiate(id, serializedNode.Classifier);
        if (node == null)
            return;

        var compressedId = Compress(id);
        _deserializedNodesById[compressedId] = node;

        DeserializeProperties(serializedNode, node);
        RegisterContainments(serializedNode, compressedId);
        RegisterReferences(serializedNode, compressedId);
        RegisterAnnotations(serializedNode, compressedId);
        RegisterParent(serializedNode, compressedId);
    }

    private void DeserializeProperties(SerializedNode serializedNode, INode node)
    {
        var id = serializedNode.Id;

        foreach (var serializedProperty in serializedNode.Properties)
        {
            var property = _deserializerMetaInfo.FindFeature<Property>(node, Compress(serializedProperty.Property));
            if (property == null)
                continue;

            var value = serializedProperty.Value;
            var convertedValue = (property, value) switch
            {
                (_, null) => null,
                (Property { Type: PrimitiveType } p, { } v) => ConvertPrimitiveType(id, p, v),
                (Property { Type: Enumeration enumeration }, { } v) => _deserializerMetaInfo.ConvertEnumeration(id,
                    enumeration, v),
                _ => Handler.UnknownDatatype(id, property, value)
            };

            if (convertedValue == null)
                continue;

            node.Set(property, convertedValue);
        }
    }

    private object? ConvertPrimitiveType(string nodeId, Property property, string value) => property.Type switch
    {
        var b when b == BuiltInsLanguage.Instance.Boolean && bool.TryParse(value, out var result) => result,
        var i when i == BuiltInsLanguage.Instance.Integer && int.TryParse(value, out var result) => result,
        // leave both a String and JSON value as a string:
        var s when s == BuiltInsLanguage.Instance.String || s == BuiltInsLanguage.Instance.Json => value,
        _ => Handler.UnknownDatatype(nodeId, property, value)
    };

    private void RegisterParent(SerializedNode serializedNode, CompressedId compressedId)
    {
        if (serializedNode.Parent == null)
            return;

        _parentByNodeId[compressedId] = Compress(serializedNode.Parent);
    }

    private void RegisterAnnotations(SerializedNode serializedNode, CompressedId compressedId)
    {
        if (serializedNode.Annotations.Length == 0)
            return;

        _annotationsByOwnerId[compressedId] = serializedNode
            .Annotations
            .Select(Compress)
            .ToList();
    }

    private void RegisterReferences(SerializedNode serializedNode, CompressedId compressedId)
    {
        if (serializedNode.References.Length == 0)
            return;

        _referencesByOwnerId[compressedId] = serializedNode
            .References
            .Select(r => (
                Compress(r.Reference),
                r
                    .Targets
                    .Select(t => (Compress(t.Reference), t.ResolveInfo))
                    .ToList()
            ))
            .ToList();
    }

    private void RegisterContainments(SerializedNode serializedNode, CompressedId compressedId)
    {
        if (serializedNode.Containments.Length == 0)
            return;

        _containmentsByOwnerId[compressedId] = serializedNode
            .Containments
            .Select(c => (
                Compress(c.Containment),
                c
                    .Children
                    .Select(Compress)
                    .ToList()
            ))
            .ToList();
    }
}