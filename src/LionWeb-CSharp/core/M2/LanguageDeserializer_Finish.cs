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

namespace LionWeb.Core.M2;

using M1;
using M3;

public partial class LanguageDeserializer
{
    /// <inheritdoc />
    public override IEnumerable<DynamicLanguage> Finish()
    {
        InstallLanguageLinks();

        List<IWritableNode> deserializedAnnotationInstances = DeserializeAnnotations();
        InstallAnnotationParents(deserializedAnnotationInstances);
        InstallAnnotationReferences();

        return _deserializedNodesById.Values.OfType<DynamicLanguage>();
    }

    private List<IWritableNode> DeserializeAnnotations()
    {
        _deserializerBuilder
            .WithHandler(new AnnotationDeserializerHandler(Handler))
            .WithUncompressedIds(StoreUncompressedIds)
            .WithLanguages(_deserializedNodesById.Values.OfType<Language>())
            .WithDependentNodes(_deserializedNodesById.Values)
            .WithLionWebVersion(LionWebVersion);

        var deserializer = _deserializerBuilder.Build();

        var annotationNodes = _serializedNodesById
            .Where(p => !_deserializedNodesById.ContainsKey(p.Key))
            .Where(p => !IsInDependentNodes(p.Key))
            .Select(p => p.Value);
        List<IReadableNode> deserializedAnnotations = deserializer.Deserialize(annotationNodes);

        return deserializedAnnotations
            .Select(deserializedAnnotation =>
            {
                if (deserializedAnnotation is not IWritableNode node)
                    node = Handler.InvalidAnnotation(deserializedAnnotation, null);

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
            var serializedAnnotation = _serializedNodesById[Compress(deserializedAnnotation.GetId())];
            var parentId = serializedAnnotation.Parent;
            if (parentId == null)
                continue;

            CompressedId compressedParentId = Compress(parentId);
            if(IsInDependentNodes(compressedParentId))
                continue;
            
            if (!_deserializedNodesById.TryGetValue(compressedParentId, out var parent) ||
                parent is not IWritableNode writableParent)
            {
                Handler.InvalidAnnotationParent(deserializedAnnotation, parent);
                continue;
            }

            IWritableNode? annotation = deserializedAnnotation;
            if (deserializedAnnotation.GetClassifier() is not Annotation ann ||
                !ann.CanAnnotate(writableParent.GetClassifier()))
            {
                annotation = Handler.InvalidAnnotation(deserializedAnnotation, writableParent);
                if (annotation == null)
                    continue;
            }

            writableParent.AddAnnotations([annotation]);
        }
    }

    private void InstallAnnotationReferences()
    {
        foreach (var serializedNode in _serializedNodesById.Values)
        {
            InstallReferences(serializedNode);
        }
    }

    private T LookupNode<T>(string id) where T : class, IKeyed
    {
        CompressedId compressedId = Compress(id);
        if (_dependentNodesById.TryGetValue(compressedId, out var node))
            return (T)node;
        return (T)_deserializedNodesById[compressedId];
    }
}