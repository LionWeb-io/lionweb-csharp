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
using CompressedContainment = (CompressedMetaPointer, List<ICompressedId>);

public partial class Deserializer
{
    /// <inheritdoc />
    public override void Process(SerializedNode serializedNode)
    {
        var processed = ProcessInternal(serializedNode, (id) =>
        {
            INode? node = _deserializerMetaInfo.Instantiate(id, serializedNode.Classifier);
            if (node == null)
                return null;

            DeserializeProperties(serializedNode, node);
            return node;
        });

        if (processed is not { } compressedId)
            return;

        RegisterContainments(serializedNode, compressedId);
        RegisterReferences(serializedNode, compressedId);
        RegisterAnnotations(serializedNode, compressedId);
    }

    private void DeserializeProperties(SerializedNode serializedNode, IWritableNode node)
    {
        foreach (var serializedProperty in serializedNode.Properties)
        {
            var property = _deserializerMetaInfo.FindFeature<Property>(node, Compress(serializedProperty.Property));
            if (property == null)
                continue;

            var convertedValue = _versionSpecifics.ConvertDatatype(
                node,
                property,
                property.GetFeatureType(),
                serializedProperty.Value
            );

            if (convertedValue == null)
                continue;

            node.Set(property, convertedValue);
        }
    }

    private void RegisterAnnotations(SerializedNode serializedNode, ICompressedId compressedId)
    {
        if (serializedNode.Annotations.Length == 0)
            return;

        _annotationsByOwnerId[compressedId] = serializedNode
            .Annotations
            .Select(Compress)
            .ToList();
    }

    private void RegisterReferences(SerializedNode serializedNode, ICompressedId compressedId)
    {
        if (serializedNode.References.Length == 0)
            return;

        _referencesByOwnerId[compressedId] = serializedNode
            .References
            .Select(Compress)
            .ToList();
    }

    private void RegisterContainments(SerializedNode serializedNode, ICompressedId compressedId)
    {
        if (serializedNode.Containments.Length == 0)
            return;

        _containmentsByOwnerId[compressedId] = serializedNode
            .Containments
            .Select(Compress)
            .ToList();
    }

    private CompressedContainment Compress(SerializedContainment c) =>
    (
        Compress(c.Containment),
        c
            .Children
            .Select(Compress)
            .ToList()
    );
}