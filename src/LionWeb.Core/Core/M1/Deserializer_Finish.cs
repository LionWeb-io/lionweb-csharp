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
            InstallContainments(compressedId);
            InstallReferences(compressedId);
            InstallAnnotations(compressedId);
        }

        return FilterRootNodes();
    }

    private IEnumerable<IWritableNode> FilterRootNodes() =>
        _deserializedNodesById
            .Values
            .Where(node => node.GetParent() == null);

    #region Containments

    private void InstallContainments(ICompressedId compressedId)
    {
        if (!_containmentsByOwnerId.TryGetValue(compressedId, out var containments))
            return;

        IWritableNode node = _deserializedNodesById[compressedId];

        foreach (var (compressedMetaPointer, compressedChildrenIds) in containments)
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

    private IWritableNode? FindChild(IWritableNode node, Feature containment, ICompressedId childId)
    {
        IWritableNode? result = _deserializedNodesById.TryGetValue(childId, out var existingChild)
            ? existingChild as IWritableNode
            : _handler.UnresolvableChild(childId, containment, node);

        return PreventCircularContainment(node, result);
    }

    #endregion

    #region References

    private void InstallReferences(ICompressedId compressedId)
    {
        if (!_referencesByOwnerId.TryGetValue(compressedId, out var references))
            return;

        InstallReferences(compressedId, references);
    }

    #endregion

    #region Annotations

    private void InstallAnnotations(ICompressedId compressedId)
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

    private IWritableNode? FindAnnotation(IWritableNode node, ICompressedId annotationId)
    {
        if (!_deserializedNodesById.TryGetValue(annotationId, out var result))
            result = _handler.UnresolvableAnnotation(annotationId, node);

        if (result == null)
            return null;

        if (result.GetClassifier() is not Annotation ann || !ann.CanAnnotate(node.GetClassifier()))
            result = _handler.InvalidAnnotation(result, node);

        return PreventCircularContainment(node, result);
    }

    #endregion
}