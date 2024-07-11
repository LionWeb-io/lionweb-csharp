// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

// ReSharper disable once CheckNamespace
namespace LionWeb.Core.Utilities;

using M3;

/// <summary>
/// Information about a source and target node related through a <see cref="Reference"/>.
/// </summary>
/// <param name="SourceNode">The source <see cref="IReadableNode"/> of the reference (relation)</param>
/// <param name="Reference">The <see cref="LionWeb.Core.M3.Reference"/> feature that has the reference to the target node</param>
/// <param name="Index">The index within the (intrinsically-ordered) multi-value of the reference feature on the <paramref name="SourceNode"/> of the reference,
/// or <c>null</c> if the reference feature is not multivalued
/// <param name="TargetNode">The target <see cref="IReadableNode"/> of the reference (relation)</param>
/// </param>
// ReSharper disable NotAccessedPositionalProperty.Global
public record ReferenceValue(IReadableNode SourceNode, Reference Reference, int? Index, IReadableNode TargetNode);

/// <summary>
/// Extension methods to deal with references, i.e. values of <see cref="Reference"/> features.
/// </summary>
public static class ReferenceUtils
{
    private static IEnumerable<ReferenceValue> GatherReferenceValues(IReadableNode sourceNode, Reference reference)
    {
        if (reference.Multiple)
        {
            var sourceNodes = sourceNode.Get(reference) as IEnumerable<IReadableNode> ?? [];
            return sourceNodes
                .Select((targetNode, index) =>
                    new ReferenceValue(sourceNode, reference, index, targetNode)
                );
        }

        // singular:
        return [new ReferenceValue(sourceNode, reference, null, (sourceNode.Get(reference) as IReadableNode)!)];
    }

    /// <summary>
    /// Finds all references within the given <paramref name="scope"/>, as <see cref="ReferenceValue"/>s.
    /// To search within all nodes under a collection of root nodes,
    /// pass <c>rootNodes.SelectMany(rootNode => rootNode.Descendants(true, true)</c> as scope.
    /// </summary>
    /// <param name="scope">The <see cref="IReadableNode"/>s that are searched for references</param>
    /// <returns>An enumeration of references, as <see cref="ReferenceValue"/>s.</returns>
    public static IEnumerable<ReferenceValue> ReferenceValues(IEnumerable<IReadableNode> scope)
        => scope
            .Distinct()
            .SelectMany(sourceNode =>                                                       // for all nodes in the scope:
                sourceNode
                    .CollectAllSetFeatures()                                                // for all set features
                    .OfType<Reference>()                                                    //  that are references:
                    .SelectMany(reference => GatherReferenceValues(sourceNode, reference))  // gather all reference values
            );

    /// <summary>
    /// Finds all references coming into any of the given <paramref name="targetNodes"/>
    /// within the given <paramref name="scope"/>, as <see cref="ReferenceValue"/>s.
    /// To search within all nodes under a collection of root nodes,
    /// pass <c>rootNodes.SelectMany(rootNode => rootNode.Descendants(true, true)</c> as scope.
    /// </summary>
    /// <param name="targetNodes">The target nodes for which the incoming references are searched</param>
    /// <param name="scope">The <see cref="IReadableNode"/>s that are searched for references</param>
    /// <returns>An enumeration of references, as <see cref="ReferenceValue"/>s.</returns>
    public static IEnumerable<ReferenceValue> FindIncomingReferences(IEnumerable<IReadableNode> targetNodes,
        IEnumerable<IReadableNode> scope)
    {
        var targetNodesAsSet = new HashSet<IReadableNode>(targetNodes);
        return ReferenceValues(scope)
            .Where(referenceValue => targetNodesAsSet.Contains(referenceValue.TargetNode));
    }

    /// <summary>
    /// Finds all references coming into the given <paramref name="targetNode"/>
    /// within the given <paramref name="scope"/>, as <see cref="ReferenceValue"/>s.
    /// To search within all nodes under a collection of root nodes,
    /// pass <c>rootNodes.SelectMany(rootNode => rootNode.Descendants(true, true)</c> as scope.
    /// </summary>
    /// <param name="targetNode">A target <see cref="IReadableNode"/></param>
    /// <param name="scope">The <see cref="IReadableNode"/>s that form the scope of the search</param>
    /// <returns>An enumeration of references, as <see cref="ReferenceValue"/>s.</returns>
    public static IEnumerable<ReferenceValue> FindIncomingReferences(IReadableNode targetNode,
        IEnumerable<IReadableNode> scope)
        => FindIncomingReferences([targetNode], scope);
}