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

public partial class Deserializer
{
    /// <inheritdoc />
    public override IEnumerable<IWritableNode> Finish()
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

    private IEnumerable<IWritableNode> FilterRootNodes() =>
        _deserializedNodesById
            .Values
            .Where(node => node.GetParent() == null);

    #region Parent

    private void InstallParent(CompressedId compressedId)
    {
        if (!_parentByNodeId.TryGetValue(compressedId, out var parentCompressedId))
            return;

        IWritableNode? parent = FindParent(compressedId, parentCompressedId);

        if (parent != null)
            _deserializedNodesById[compressedId].SetParent(parent);
    }

    private IWritableNode? FindParent(CompressedId compressedId, CompressedId parentId) =>
        _deserializedNodesById.TryGetValue(parentId, out IWritableNode? existingParent)
            ? existingParent
            : Handler.UnresolvableParent(parentId, _deserializedNodesById[compressedId]);

    #endregion

    #region Containments

    private void InstallContainments(CompressedId compressedId)
    {
        if (!_containmentsByOwnerId.TryGetValue(compressedId, out var containments))
            return;

        IWritableNode node = _deserializedNodesById[compressedId];

        foreach ((var compressedMetaPointer, var compressedChildrenIds) in containments)
        {
            var containment = _deserializerMetaInfo.FindFeature<Containment>(node, compressedMetaPointer);
            if (containment == null)
                continue;

            List<IWritableNode> children = compressedChildrenIds
                .Select(childId => FindChild(node, containment, childId))
                .Where(c => c != null)
                .ToList()!;

            SetLink(children, node, containment);
        }
    }

    private IWritableNode? FindChild(IWritableNode node, Feature containment, CompressedId childId) =>
        _deserializedNodesById.TryGetValue(childId, out var existingChild)
            ? existingChild
            : Handler.UnresolvableChild(childId, containment, node);

    #endregion

    private void SetLink<T>(List<T> children, IWritableNode node, Feature link) where T : class, IReadableNode
    {
        if (children.Count == 0)
            return;

        var single = link is Link { Multiple: false };
        try
        {
            node.Set(link, single ? children.FirstOrDefault() : children);
        } catch (InvalidValueException)
        {
            List<T>? replacement = Handler.InvalidLinkValue(children, link, node);
            if (replacement != null)
                node.Set(link, single ? replacement.FirstOrDefault() : replacement);
        }
    }

    #region References

    private void InstallReferences(CompressedId compressedId)
    {
        if (!_referencesByOwnerId.TryGetValue(compressedId, out var references))
            return;

        IWritableNode node = _deserializedNodesById[compressedId];
        foreach ((var compressedMetaPointer, var targetEntries) in references)
        {
            var reference = _deserializerMetaInfo.FindFeature<Reference>(node, compressedMetaPointer);
            if (reference == null)
                continue;

            List<IReadableNode> targets = targetEntries
                .Select(target => FindReferenceTarget(node, reference, target.Item1, target.Item2))
                .Where(c => c != null)
                .ToList()!;

            SetLink(targets, node, reference);
        }
    }

    private IReadableNode? FindReferenceTarget(IWritableNode node, Feature reference, CompressedId? targetId,
        string? resolveInfo)
    {
        if (targetId is not null)
        {
            var tid = (CompressedId)targetId;
            if (_deserializedNodesById.TryGetValue(tid, out var ownNode))
                return ownNode;

            if (_dependentNodesById.TryGetValue(tid, out var dependentNode))
                return dependentNode;
        }

        return Handler.UnresolvableReferenceTarget(targetId, resolveInfo, reference, node);
    }

    #endregion

    #region Annotations

    private void InstallAnnotations(CompressedId compressedId)
    {
        if (!_annotationsByOwnerId.TryGetValue(compressedId, out var annotationIds))
            return;

        IWritableNode node = _deserializedNodesById[compressedId];

        List<IWritableNode> annotations = annotationIds
            .Select(annId => FindAnnotation(node, annId))
            .Where(c => c != null)
            .ToList()!;

        node.AddAnnotations(annotations);
    }

    private IWritableNode? FindAnnotation(IWritableNode node, CompressedId annotationId)
    {
        if (!_deserializedNodesById.TryGetValue(annotationId, out var existingAnnotation))
            existingAnnotation = Handler.UnresolvableAnnotation(annotationId, node);

        if (existingAnnotation == null)
            return null;

        if (existingAnnotation.GetClassifier() is not Annotation ann || !ann.CanAnnotate(node.GetClassifier()))
            return Handler.InvalidAnnotation(existingAnnotation, node);
        return existingAnnotation;
    }

    #endregion
}