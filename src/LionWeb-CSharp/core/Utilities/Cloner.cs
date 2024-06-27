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

namespace LionWeb.Core.Utilities;

using M3;

/// <summary>
/// Clones a set of <see cref="INode"/>s, properly handling interconnections.
/// Does not change the input nodes.
/// <para>
/// Refer to <see cref="IncludingProperties"/>, <see cref="IncludingChildren"/>, <see cref="IncludingAnnotations"/>, <see cref="IncludingReferences"/>, <see cref="IncludingParents"/>, and <see cref="KeepExternalReferences"/> for configuration options.
/// </para>
/// </summary>
public class Cloner : ClonerBase
{
    /// <summary>
    /// Whether we clone <see cref="Property">Properties</see>.
    /// Defaults to <c>true</c>.
    /// If <c>false</c>, properties of cloned nodes are set to their default value. 
    /// </summary>
    public bool IncludingProperties { get; set; } = true;

    /// <summary>
    /// Whether we clone <see cref="Containment">Containments</see>.
    /// Defaults to <c>true</c>.
    /// If <c>false</c>, containments of cloned nodes are unset (even if the contained node has been cloned). 
    /// </summary>
    public bool IncludingChildren { get; set; } = true;

    /// <summary>
    /// Whether we clone <see cref="Annotation">Annotations</see>.
    /// Defaults to <c>true</c>.
    /// If <c>false</c>, annotations of cloned nodes are unset (even if the annotating node has been cloned). 
    /// </summary>
    public bool IncludingAnnotations { get; set; } = true;

    /// <summary>
    /// Whether we clone <see cref="Reference">References</see>.
    /// Defaults to <c>false</c>.
    /// If <c>false</c>, references of cloned nodes are populated if the target node has been cloned. 
    /// </summary>
    public bool IncludingReferences { get; set; } = false;

    /// <summary>
    /// Whether we clone <see cref="IReadableNode.GetParent()">Parents</see>.
    /// Defaults to <c>false</c>. 
    /// If <c>false</c>, parents of cloned nodes are populated if the parent node has been cloned. 
    /// </summary>
    public bool IncludingParents { get; set; } = false;

    /// <summary>
    /// Whether we keep references from cloned nodes to not cloned nodes.
    /// Defaults to <c>true</c>.
    /// If <c>false</c>, references to external nodes are excluded from cloned nodes. 
    /// </summary>
    public bool KeepExternalReferences { get; set; } = true;

    /// <summary>
    /// Initializes the Cloner based on <paramref name="inputNodes"/>.
    /// </summary>
    /// <param name="inputNodes">
    /// Nodes this Cloner should start from.
    /// We clone each node once, even if we mention them multiple times. 
    /// </param>
    public Cloner(IEnumerable<INode> inputNodes) : base(inputNodes)
    {
    }

    /// <summary>
    /// Clones <paramref name="node"/> with default configuration.
    /// </summary>
    /// <param name="node">INode to clone.</param>
    /// <returns>The cloned instance of <paramref name="node"/>.</returns>
    public static T Clone<T>(T node) where T : class, INode =>
        (T)new Cloner(new HashSet<INode> { node }).Clone()[node];

    /// <summary>
    /// Clones <paramref name="nodes"/> with default configuration.
    /// </summary>
    /// <param name="nodes">
    /// Nodes to clone.
    /// We clone each node once, even if we mention them multiple times. 
    /// </param>
    /// <returns>
    /// The cloned node for each of <paramref name="nodes"/>.
    /// The order of returned clones is undefined.
    /// </returns>
    /// <seealso cref="Clone{T}(T)">Use instance method to retrieve a mapping of input to cloned nodes.</seealso>
    public static IEnumerable<INode> Clone(IEnumerable<INode> nodes)
    {
        HashSet<INode> hashSet;
        if (nodes is HashSet<INode> set)
        {
            hashSet = set;
        } else
        {
            hashSet = [..nodes];
        }

        return FilterCorresponding(hashSet, new Cloner(hashSet).Clone());
    }

    /// Returns only values of <paramref name="result"/> if their key is contained in <paramref name="input"/>.
    protected static IEnumerable<INode>
        FilterCorresponding(IEnumerable<INode> input, Dictionary<INode, INode> result) =>
        result.Where(p => input.Contains(p.Key)).Select(p => p.Value);

    /// <inheritdoc />
    protected override bool IncludeProperty(INode inputNode, Property prop) => IncludingProperties;

    /// <inheritdoc />
    protected override bool IncludeChild(INode inputNode, Containment cont) => IncludingChildren;

    /// <inheritdoc />
    protected override bool IncludeReference(INode inputNode, Reference refer) => IncludingReferences;

    /// <inheritdoc />
    protected override bool IncludeAnnotation(INode inputNode, INode annotation) => IncludingAnnotations;

    /// <inheritdoc />
    protected override bool IncludeParent(INode inputNode) => IncludingParents;

    /// <inheritdoc />
    protected override bool KeepExternalReference(INode inputNode, Reference externalRef) => KeepExternalReferences;
}