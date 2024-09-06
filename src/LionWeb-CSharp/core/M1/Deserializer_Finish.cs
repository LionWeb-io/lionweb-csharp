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
    public override IEnumerable<INode> Finish()
    {
        foreach (var compressedId in _deserializedNodesById.Keys)
        {
            InstallParent(compressedId);
            InstallContainments(compressedId);
            InstallReferences(compressedId);
            InstallAnnotations(compressedId);
        }

        return FilterRootNodes()
            .ToList();
    }

    private IEnumerable<INode> FilterRootNodes() =>
        _deserializedNodesById
            .Values
            .Where(node => node.GetParent() == null);

    #region Parent

    private void InstallParent(CompressedId compressedId)
    {
        if (!_parentByNodeId.TryGetValue(compressedId, out var parentCompressedId))
            return;

        INode? parent = FindParent(compressedId, parentCompressedId);

        if (parent != null)
            _deserializedNodesById[compressedId].SetParent(parent);
    }

    private INode? FindParent(CompressedId compressedId, CompressedId parentId) =>
        _deserializedNodesById.TryGetValue(parentId, out INode? existingParent)
            ? existingParent
            : Handler.UnknownParent(parentId, _deserializedNodesById[compressedId]);

    #endregion

    #region Containments

    private void InstallContainments(CompressedId compressedId)
    {
        if (!_containmentsByOwnerId.TryGetValue(compressedId, out var containments))
            return;

        INode node = _deserializedNodesById[compressedId];

        foreach ((var compressedMetaPointer, var compressedChildrenIds) in containments)
        {
            var containment = _deserializerMetaInfo.FindFeature<Containment>(node, compressedMetaPointer);
            if (containment == null)
                continue;

            var children = compressedChildrenIds
                .Select(childId => FindChild(node, childId))
                .Where(c => c != null)
                .ToList();

            if (children.Count != 0)
            {
                foreach (INode child in children.OfType<INode>()
                             .Where(child => !child.GetClassifier().IsGeneralization(containment.Type)))
                {
                    throw new UnsupportedClassifierException(child.GetClassifier().ToMetaPointer());
                }

                if (children.Count > 1 && !containment.Multiple)
                {
                    throw new InvalidValueException(containment,
                        "single containment is expected, but multiple children encountered");
                }

                node.Set(
                    containment,
                    containment.Multiple ? children : children.FirstOrDefault()
                );
            }
        }
    }

    private INode? FindChild(INode node, CompressedId childId) =>
        _deserializedNodesById.TryGetValue(childId, out var existingChild)
            ? existingChild
            : Handler.UnknownChild(childId, node);

    #endregion

    #region References

    private void InstallReferences(CompressedId compressedId)
    {
        if (!_referencesByOwnerId.TryGetValue(compressedId, out var references))
            return;

        INode node = _deserializedNodesById[compressedId];
        foreach ((var compressedMetaPointer, var targetEntries) in references)
        {
            var reference = _deserializerMetaInfo.FindFeature<Reference>(node, compressedMetaPointer);
            if (reference == null)
                continue;

            var targets = targetEntries
                .Select(target => FindReferenceTarget(node, target.Item1, target.Item2))
                .Where(c => c != null)
                .ToList();

            if (targets.Count != 0)
            {
                node.Set(
                    reference,
                    reference.Multiple ? targets : targets.FirstOrDefault()
                );
            }
        }
    }

    private IReadableNode? FindReferenceTarget(INode node, CompressedId targetId, string? resolveInfo)
    {
        if (_deserializedNodesById.TryGetValue(targetId, out var ownNode))
            return ownNode;

        if (_dependentNodesById.TryGetValue(targetId, out var dependentNode))
            return dependentNode;

        return Handler.UnknownReference(targetId, resolveInfo, node);
    }

    #endregion

    #region Annotations

    private void InstallAnnotations(CompressedId compressedId)
    {
        if (!_annotationsByOwnerId.TryGetValue(compressedId, out var annotationIds))
            return;

        INode node = _deserializedNodesById[compressedId];

        List<INode> annotations = annotationIds
            .Select(annId => FindAnnotation(node, annId))
            .Where(c => c != null)
            .ToList()!;

        node.AddAnnotations(annotations);
    }

    private INode? FindAnnotation(INode node, CompressedId annotationId)
    {
        if (_deserializedNodesById.TryGetValue(annotationId, out var existingAnnotation))
        {
            if (existingAnnotation.GetClassifier() is not Annotation ann || !ann.CanAnnotate(node.GetClassifier()))
                return Handler.InvalidAnnotation(existingAnnotation, node);
            return existingAnnotation;
        }

        return Handler.UnknownAnnotation(annotationId, node);
    }

    #endregion
}