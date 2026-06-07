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
        foreach (var nodeId in _deserializedNodesById.Keys)
        {
            InstallContainments(nodeId);
            InstallReferences(nodeId);
            InstallAnnotations(nodeId);
        }

        return FilterRootNodes();
    }

    private IEnumerable<IWritableNode> FilterRootNodes() =>
        _deserializedNodesById
            .Values
            .Where(node => node.GetParent() == null);

    #region Containments

    private void InstallContainments(NodeId nodeId)
    {
        if (!_containmentsByOwnerId.TryGetValue(nodeId, out var containments) || containments.Length == 0)
            return;

        IWritableNode node = _deserializedNodesById[nodeId];

        foreach (var serializedContainment in containments)
        {
            var metaPointer = serializedContainment.Containment;
            var childrenIds = serializedContainment.Children;

            var containment = _deserializerMetaInfo.FindFeature<Containment>(node, metaPointer);
            if (containment == null)
                continue;

            InstallContainment(childrenIds, node, containment);
        }
    }

    #endregion

    #region References

    private void InstallReferences(NodeId nodeId)
    {
        if (!_referencesByOwnerId.TryGetValue(nodeId, out var references) || references.Length == 0)
            return;

        InstallReferences(nodeId, references);
    }

    #endregion

    #region Annotations

    private void InstallAnnotations(NodeId nodeId)
    {
        if (!_annotationsByOwnerId.TryGetValue(nodeId, out var annotationIds) || annotationIds.Length == 0)
            return;

        IWritableNode node = _deserializedNodesById[nodeId];

        var annotations = new List<IWritableNode>(annotationIds.Length);
        foreach (var annotationId in annotationIds)
        {
            if (FindAnnotation(node, annotationId) is { } a)
                annotations.Add(a);
        }

        if (annotations.Count == 0)
            return;

        node.AddAnnotations(annotations);
    }

    private IWritableNode? FindAnnotation(IWritableNode node, NodeId annotationId)
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