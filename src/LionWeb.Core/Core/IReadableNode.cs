// Copyright 2025 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core;

using M3;
using System.Diagnostics.CodeAnalysis;

/// An interface that LionWeb AST nodes implement to provide <em>read</em> access.
public interface IReadableNode
{
    /// The <em>unique within the repository</em> ID of the node.
    /// A node id has no "meaning". 
    /// An id must be a valid <i>identifier</i>, i.e. a non-empty string of arbitrary length
    /// comprised of uppercase or lowercase A to Z, 1 to 9, - (dash) and _ (underscore).
    public NodeId GetId();

    /// <summary>
    /// The parent of the node, or <c>null</c> if the node is a root node.
    /// The parent of <c>this</c> node should have a containment of <c>this</c> node among its <see cref="Containment"/> features.
    /// </summary>
    /// <seealso cref="IWritableNode.SetParent"/>
    public IReadableNode? GetParent();

    /// <summary>
    /// Contains all annotations.
    /// </summary>
    /// <seealso cref="IWritableNode.AddAnnotations"/>
    /// <seealso cref="IWritableNode.InsertAnnotations"/>
    /// <seealso cref="IWritableNode.RemoveAnnotations"/>
    public IReadOnlyList<IReadableNode> GetAnnotations();

    /// The <see cref="Classifier"/> that <c>this</c> node is an instance of.
    public Classifier GetClassifier();

    /// Returns all features for which a value has been set on <c>this</c> node.
    public IEnumerable<Feature> CollectAllSetFeatures();

    /// <summary>
    /// Gets the value of the given <paramref name="feature"/> on <c>this</c> node.
    /// </summary>
    /// <exception cref="UnsetFeatureException">If <paramref name="feature"/> has not been set or is empty.</exception>
    /// <seealso cref="IWritableNode.Set"/>
    /// <see cref="CollectAllSetFeatures"/>
    public object? Get(Feature feature);

    /// <summary>
    /// Tries to get the value of the given <paramref name="feature"/> on <c>this</c> node.
    /// </summary>
    /// <returns>
    /// If <paramref name="feature"/> is _single_: <c>true</c> if <paramref name="feature"/> is set on <c>this</c>; <c>false</c> otherwise.
    /// If <paramref name="feature"/> is _multiple_: <c>true</c> if <paramref name="feature"/> is not empty on <c>this</c>; <c>false</c> otherwise.
    /// </returns>
    /// <seealso cref="Get"/>
    /// <see cref="CollectAllSetFeatures"/>
    public bool TryGet(Feature feature, [NotNullWhen(true)] out object? value);
}

/// The type-parametrized twin of the non-generic <see cref="IReadableNode"/> interface.
public interface IReadableNode<out T> : IReadableNode where T : IReadableNode
{
    /// <inheritdoc/>
    IReadableNode? IReadableNode.GetParent() => GetParent();

    /// <inheritdoc cref="IReadableNode.GetParent()"/>
    public new T? GetParent();

    /// <inheritdoc/>
    IReadOnlyList<IReadableNode> IReadableNode.GetAnnotations() => (IReadOnlyList<IReadableNode>)GetAnnotations();

    /// <inheritdoc cref="IReadableNode.GetAnnotations()"/>
    public new IReadOnlyList<T> GetAnnotations();
}