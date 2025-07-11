﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.M2;

using M1;
using M3;
using Serialization;

public partial class LanguageDeserializer
{
    /// <inheritdoc />
    public override IEnumerable<DynamicLanguage> Finish()
    {
        InstallLanguageLinks();

        _deserializerBuilder
            .WithHandler(_handler)
            .WithCompressedIds(CompressedIdConfig)
            .WithLanguages(_deserializedNodesById.Values.OfType<Language>())
            .WithDependentNodes(_deserializedNodesById.Values)
            .WithLionWebVersion(LionWebVersion);

        var deserializer = _deserializerBuilder.Build();

        List<IWritableNode> deserializedAnnotationInstances = DeserializeAnnotations(deserializer);
        InstallAnnotationParents(deserializedAnnotationInstances);
        InstallAnnotationReferences(deserializer);

        return _deserializedNodesById.Values.OfType<DynamicLanguage>();
    }

    private List<IWritableNode> DeserializeAnnotations(IDeserializer deserializer)
    {
        var annotationNodes = _serializedNodesById
            .Where(p => !_deserializedNodesById.ContainsKey(p.Key))
            .Where(p => !IsInDependentNodes(p.Key))
            .Select(p => p.Value);
        List<IReadableNode> deserializedAnnotations = deserializer.Deserialize(annotationNodes);

        return deserializedAnnotations
            .Select(deserializedAnnotation =>
            {
                IWritableNode? node = deserializedAnnotation as IWritableNode ??
                                      _handler.InvalidAnnotation(deserializedAnnotation, null);

                if (node != null)
                    _deserializedNodesById[Compress(node.GetId())] = node;

                return node;
            })
            .Where(n => n != null)
            .ToList()!;
    }


    private void InstallAnnotationParents(List<IWritableNode> deserializedAnnotationInstances)
    {
        foreach (var deserializedAnnotation in deserializedAnnotationInstances)
        {
            IReadableNode? readableParent = null;
            IWritableNode? writableParent = null;

            var serializedAnnotation = _serializedNodesById[Compress(deserializedAnnotation.GetId())];
            var parentId = serializedAnnotation.Parent;
            if (parentId != null)
            {
                ICompressedId compressedParentId = Compress(parentId);

                if (_deserializedNodesById.TryGetValue(compressedParentId, out readableParent) &&
                    readableParent is IWritableNode w)
                {
                    writableParent = w;
                }
            }

            if (writableParent == null)
            {
                _handler.InvalidAnnotationParent(deserializedAnnotation, readableParent);
                continue;
            }

            IWritableNode? annotation = deserializedAnnotation;
            if (deserializedAnnotation.GetClassifier() is not Annotation ann ||
                !ann.CanAnnotate(writableParent.GetClassifier()))
            {
                annotation = _handler.InvalidAnnotation(deserializedAnnotation, writableParent);
                if (annotation == null)
                    continue;
            }

            annotation = PreventCircularContainment(writableParent, annotation);

            if (annotation != null)
                writableParent.AddAnnotations([annotation]);
        }
    }

    private void InstallAnnotationReferences(IDeserializer deserializer)
    {
        foreach (var serializedNode in _serializedNodesById.Values.Where(n => !IsLanguageNode(n)))
        {
            InstallAnnotationReferences(deserializer, serializedNode);
        }
    }

    private void InstallAnnotationReferences(IDeserializer deserializer, SerializedNode serializedNode) =>
        deserializer.InstallNodeReferences(Compress(serializedNode.Id), serializedNode.References.Select(Compress));
}