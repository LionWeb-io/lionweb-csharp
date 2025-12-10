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

using M2;
using M3;

/// An interface that LionWeb AST nodes implement to provide <em>write</em> access.
/// <seealso cref="LionWeb.Core.M1.Raw.IWritableNodeRawExtensions"/>
public interface IWritableNode : IReadableNode
{
    /// <inheritdoc cref="IReadableNode.GetParent"/>
    protected internal void SetParent(IWritableNode? value);

    /// <summary>
    /// Internal helper to remove <paramref name="child"/> from <c>this</c> node's containments. 
    /// </summary>
    /// <param name="child">The child node to detach.</param>
    /// <returns><c>true</c> if <paramref name="child"/> was contained in any of <c>this</c> node's containment, <c>false</c> otherwise.</returns>
    protected internal bool DetachChild(IWritableNode child);

    /// Removes <c>this</c> node from its <see cref="IReadableNode.GetParent">parents'</see> containments.
    /// After completion, <c>this</c> node does not have a parent, and the former parent does not contain <c>this</c> node anymore.
    public void DetachFromParent();

    /// <summary>
    /// Returns the <see cref="Containment"/> of <c>this</c> node that contains <paramref name="child"/>, if any.
    /// </summary>
    /// <param name="child">Node to get the containment of.</param>
    /// <returns><see cref="Containment"/> of <c>this</c> node that contains <paramref name="child"/>, if any; <c>null</c> otherwise.</returns>
    public Containment? GetContainmentOf(IWritableNode child);

    /// <summary>
    /// Adds <see cref="Annotation">annotations</see> to <c>this</c> node.
    /// </summary>
    /// <param name="annotations">Instances of <see cref="Annotation"/> to add.</param>
    /// <exception cref="InvalidValueException">
    /// If <paramref name="annotations"/> contains any node that's not an instance of <see cref="Annotation"/>,
    /// or the annotation cannot <see cref="Annotation.Annotates">annotate</see> <c>this</c> node.
    /// </exception>
    /// <seealso cref="IReadableNode.GetAnnotations"/>
    /// <seealso cref="InsertAnnotations"/>
    /// <seealso cref="RemoveAnnotations"/>
    public void AddAnnotations(IEnumerable<IWritableNode> annotations);

    /// <summary>
    /// Inserts <see cref="Annotation">annotations</see> into <c>this</c> node's annotations at <paramref name="index"/>.
    /// </summary>
    /// <param name="index">Position to insert <paramref name="annotations"/> at.</param>
    /// <param name="annotations">Instances of <see cref="Annotation"/> to add.</param>
    /// <exception cref="ArgumentOutOfRangeException">If <c>this</c> node's annotations contains less than <paramref name="index"/> annotations.</exception>
    /// <exception cref="InvalidValueException">
    /// If <paramref name="annotations"/> contains any node that's not an instance of <see cref="Annotation"/>,
    /// or the annotation cannot <see cref="Annotation.Annotates">annotate</see> <c>this</c> node.
    /// </exception>
    /// <seealso cref="IReadableNode.GetAnnotations"/>
    /// <seealso cref="AddAnnotations"/>
    /// <seealso cref="RemoveAnnotations"/>
    public void InsertAnnotations(Index index, IEnumerable<IWritableNode> annotations);

    /// <summary>
    /// Removes <see cref="Annotation">annotations</see> to <c>this</c> node.
    /// </summary>
    /// <param name="annotations">Instances of <see cref="Annotation"/> to add.</param>
    /// <returns><c>true</c> if any of <paramref name="annotations"/> used to be annotations of <c>this</c> node and have been removed; <c>false</c> otherwise.</returns>
    /// <exception cref="InvalidValueException">
    /// If <paramref name="annotations"/> contains any node that's not an instance of <see cref="Annotation"/>,
    /// or the annotation cannot <see cref="Annotation.Annotates">annotate</see> <c>this</c> node.
    /// </exception>
    /// <seealso cref="IReadableNode.GetAnnotations"/>
    /// <seealso cref="AddAnnotations"/>
    /// <seealso cref="InsertAnnotations"/>
    public bool RemoveAnnotations(IEnumerable<IWritableNode> annotations);

    /// <summary>
    /// Sets the given <paramref name="feature"/> on <c>this</c> node to the given <paramref name="value"/>.
    /// </summary>
    /// <exception cref="InvalidValueException">If <paramref name="value"/> does not adhere to <paramref name="feature"/>'s type or constraints</exception>
    /// <seealso cref="IReadableNode.Get"/>
    public void Set(Feature feature, object? value);

    /// <summary>
    /// Adds nodes to the specified <paramref name="link"/> feature of this node.
    /// </summary>
    /// <param name="link">The link feature to which nodes will be added.</param>
    /// <param name="nodes">The nodes to add to the link feature.</param>
    /// <exception cref="UnknownFeatureException">Thrown if the specified <paramref name="link"/> is not recognized by this node.</exception>
    public void Add(Link? link, IEnumerable<IReadableNode> nodes);

    /// <summary>
    /// Inserts nodes into the specified <paramref name="link"/> feature of this node at the given <paramref name="index"/>.
    /// </summary>
    /// <param name="link">The link feature to insert nodes into.</param>
    /// <param name="index">The position at which to insert the nodes.</param>
    /// <param name="nodes">The nodes to insert into the link feature.</param>
    /// <exception cref="UnknownFeatureException">Thrown if the specified <paramref name="link"/> is not recognized by this node.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is in out of range.</exception>
    public void Insert(Link? link, Index index, IEnumerable<IReadableNode> nodes);

    /// <summary>
    /// Removes the specified nodes from the given <paramref name="link"/> feature of this node.
    /// If one of the node in <see cref="nodes"/> is not found in <paramref name="link"/> feature, it is simply ignored.
    /// </summary>
    /// <param name="link">The link feature from which nodes will be removed.</param>
    /// <param name="nodes">The nodes to remove from the link feature.</param>
    /// <exception cref="UnknownFeatureException">Thrown if the specified <paramref name="link"/> is not recognized by this node.</exception>
    public void Remove(Link? link, IEnumerable<IReadableNode> nodes);

    #region raw api

    #region Annotation

    /// <summary>
    /// Adds <paramref name="annotation"/> to <c>this</c> node.
    /// </summary>
    /// <param name="annotation">Annotation to add to <c>this</c> node.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="annotation"/> has been added and that changed the node
    /// (i.e. <paramref name="annotation"/> is a valid annotation for <c>this</c> and not yet the last annotation of <c>this</c>);
    /// <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// <i>Should</i> take an <see cref="IAnnotationInstance"/>,
    /// but for broken models we want to allow invalid annotation instances.
    /// </remarks>
    /// <seealso cref="IWritableNode.AddAnnotations"/>
    protected internal bool AddAnnotationsRaw(IWritableNode annotation);

    /// <summary>
    /// Inserts <paramref name="annotation"/> into <c>this</c> node's annotations at <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The position at which to insert <paramref name="annotation"/>.</param>
    /// <param name="annotation">Annotation to insert into <c>this</c> node.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="annotation"/> has been inserted and that changed the node
    /// (i.e. <paramref name="annotation"/> is a valid annotation for <c>this</c> and not yet the annotation of <c>this</c> at <paramref name="index"/>);
    /// <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// <i>Should</i> take an <see cref="IAnnotationInstance"/>,
    /// but for broken models we want to allow invalid annotation instances.
    /// </remarks>
    /// <seealso cref="IWritableNode.InsertAnnotations"/>
    protected internal bool InsertAnnotationsRaw(Index index, IWritableNode annotation);

    /// <summary>
    /// Removes <paramref name="annotation"/> from <c>this</c> node.
    /// </summary>
    /// <param name="annotation">Annotation to renmove from <c>this</c> node.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="annotation"/> has been removed and that changed the node
    /// (i.e. <paramref name="annotation"/> was an annotation of <c>this</c>);
    /// <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// <i>Should</i> take an <see cref="IAnnotationInstance"/>,
    /// but for broken models we want to allow invalid annotation instances.
    /// </remarks>
    /// <seealso cref="IWritableNode.RemoveAnnotations"/>
    protected internal bool RemoveAnnotationsRaw(IWritableNode annotation);

    #endregion

    /// <summary>
    /// Sets the given <paramref name="feature"/> on <c>this</c> node to the given <paramref name="value"/>.
    /// </summary>
    /// <param name="feature">The <see cref="Feature"/> to set.</param>
    /// <param name="value">Value to set for <paramref name="feature"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="feature"/> has been set to <paramref name="value"/> that changed the node;
    /// <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// For broken models, we might allow invalid values for a feature (e.g. a string for a containment).
    /// </remarks>
    /// <seealso cref="IWritableNode.Set"/>
    protected internal bool SetRaw(Feature feature, object? value);

    #region Property

    /// <summary>
    /// Sets the given <paramref name="property"/> on <c>this</c> node to the given <paramref name="value"/>.
    /// </summary>
    /// <param name="property">The <see cref="Property"/> to set.</param>
    /// <param name="value">Value to set for <paramref name="property"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="property"/> has been set to <paramref name="value"/> and that changed <c>this</c> node
    /// (i.e. <paramref name="property"/> is known to <c>this</c> and didn't have <paramref name="value"/> before);
    /// <c>false</c> otherwise.
    /// </returns>
    bool SetPropertyRaw(Property property, object? value);

    #endregion

    #region Containment

    /// <summary>
    /// Sets the given <paramref name="containment"/> on <c>this</c> node to the given <paramref name="node"/>.
    /// </summary>
    /// <param name="containment">The <see cref="Link.Multiple">single</see> <see cref="Containment"/> to set.</param>
    /// <param name="node">Value to set for <paramref name="containment"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="containment"/> has been set to <paramref name="node"/> and that changed <c>this</c> node
    /// (i.e. <paramref name="containment"/> is known to <c>this</c>,
    /// <paramref name="node"/> is a valid value,
    /// and <paramref name="node"/> is not yet the value of <paramref name="containment"/> in <c>this</c> node);
    /// <c>false</c> otherwise.
    /// </returns>
    protected internal bool SetContainmentRaw(Containment containment, IWritableNode? node);

    /// <summary>
    /// Adds <paramref name="node"/> to the given <paramref name="containment"/> of <c>this</c> node.
    /// </summary>
    /// <param name="containment">The <see cref="Link.Multiple">multiple</see> <see cref="Containment"/> to add to.</param>
    /// <param name="node">Value to add to <paramref name="containment"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="node"/> has been added to <paramref name="containment"/> and that changed <c>this</c> node
    /// (i.e. <paramref name="containment"/> is known to <c>this</c>,
    /// <paramref name="node"/> is a valid value,
    /// and <paramref name="node"/> is not yet the last value of <paramref name="containment"/> in <c>this</c> node);
    /// <c>false</c> otherwise.
    /// </returns>
    protected internal bool AddContainmentsRaw(Containment containment, IWritableNode node);

    /// <summary>
    /// Inserts <paramref name="node"/> into <c>this</c> node's <paramref name="containment"/> at <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The position at which to insert <paramref name="node"/>.</param>
    /// <param name="containment">The <see cref="Link.Multiple">multiple</see> <see cref="Containment"/> to insert into.</param>
    /// <param name="node">Value to insert into <paramref name="containment"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="node"/> has been inserted into <paramref name="containment"/> and that changed <c>this</c> node
    /// (i.e. <paramref name="containment"/> is known to <c>this</c>,
    /// <paramref name="node"/> is a valid value,
    /// <paramref name="index"/> is within <paramref name="containment"/>'s bounds,
    /// and <paramref name="node"/> is not yet the <paramref name="containment"/> of <c>this</c> at <paramref name="index"/>);
    /// <c>false</c> otherwise.
    /// </returns>
    protected internal bool InsertContainmentsRaw(Containment containment, Index index, IWritableNode node);

    /// <summary>
    /// Removes <paramref name="node"/> from the given <paramref name="containment"/> of <c>this</c> node.
    /// </summary>
    /// <param name="containment">The <see cref="Link.Multiple">multiple</see> <see cref="Containment"/> to remove from.</param>
    /// <param name="node">Value to remove to <paramref name="containment"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="node"/> has been removed from <paramref name="containment"/> and that changed <c>this</c> node
    /// (i.e. <paramref name="node"/> was a value of <paramref name="containment"/> in <c>this</c> node);
    /// <c>false</c> otherwise.
    /// </returns>
    protected internal bool RemoveContainmentsRaw(Containment containment, IWritableNode node);

    #endregion

    #region Reference

    /// <summary>
    /// Sets the given <paramref name="reference"/> on <c>this</c> node to the given <paramref name="target"/>.
    /// </summary>
    /// <param name="reference">The <see cref="Link.Multiple">single</see> <see cref="Reference"/> to set.</param>
    /// <param name="target">Value to set for <paramref name="reference"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="reference"/> has been set to <paramref name="target"/> and that changed <c>this</c> node
    /// (i.e. <paramref name="reference"/> is known to <c>this</c>,
    /// <paramref name="target"/> is a valid value,
    /// and <paramref name="target"/> is not yet the value of <paramref name="reference"/> in <c>this</c> node);
    /// <c>false</c> otherwise.
    /// </returns>
    protected internal bool SetReferenceRaw(Reference reference, ReferenceTarget? target);

    /// <summary>
    /// Adds <paramref name="target"/> to the given <paramref name="reference"/> of <c>this</c> node.
    /// </summary>
    /// <param name="reference">The <see cref="Link.Multiple">multiple</see> <see cref="Reference"/> to add to.</param>
    /// <param name="target">Value to add to <paramref name="reference"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="target"/> has been added to <paramref name="reference"/> and that changed <c>this</c> node
    /// (i.e. <paramref name="reference"/> is known to <c>this</c>
    /// and <paramref name="target"/> is a valid value);
    /// <c>false</c> otherwise.
    /// </returns>
    protected internal bool AddReferencesRaw(Reference reference, ReferenceTarget target);

    /// <summary>
    /// Inserts <paramref name="target"/> into <c>this</c> node's <paramref name="reference"/> at <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The position at which to insert <paramref name="target"/>.</param>
    /// <param name="reference">The <see cref="Link.Multiple">multiple</see> <see cref="Reference"/> to insert into.</param>
    /// <param name="target">Value to insert into <paramref name="reference"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="target"/> has been inserted into <paramref name="reference"/> and that changed <c>this</c> node
    /// (i.e. <paramref name="reference"/> is known to <c>this</c>,
    /// <paramref name="target"/> is a valid value,
    /// and <paramref name="index"/> is within <paramref name="reference"/>'s bounds,
    /// and <paramref name="target"/> is not yet the <paramref name="reference"/> of <c>this</c> at <paramref name="index"/>);
    /// <c>false</c> otherwise.
    /// </returns>
    protected internal bool InsertReferencesRaw(Reference reference, Index index, ReferenceTarget target);

    /// <summary>
    /// Removes the first instance of <paramref name="target"/> from the given <paramref name="reference"/> of <c>this</c> node.
    /// </summary>
    /// <param name="reference">The <see cref="Link.Multiple">multiple</see> <see cref="Reference"/> to remove from.</param>
    /// <param name="target">Value to remove to <paramref name="reference"/>.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="target"/> has been removed from <paramref name="reference"/> and that changed <c>this</c> node
    /// (i.e. <paramref name="target"/> was a value of <paramref name="reference"/> in <c>this</c> node);
    /// <c>false</c> otherwise.
    /// </returns>
    protected internal bool RemoveReferencesRaw(Reference reference, ReferenceTarget target);

    #endregion

    #endregion
}

/// The type-parametrized twin of the non-generic <see cref="IWritableNode"/> interface.
public interface IWritableNode<T> : IReadableNode<T>, IWritableNode where T : class, IWritableNode
{
    /// <inheritdoc/>
    void IWritableNode.SetParent(IWritableNode? parent)
    {
        switch (parent)
        {
            case null:
                SetParent((T?)null);
                return;
            case T t:
                SetParent(t);
                return;
            default:
                throw new UnsupportedNodeTypeException(parent, nameof(parent));
        }
    }

    /// <inheritdoc cref="IWritableNode.SetParent"/>
    protected internal void SetParent(T? parent);

    /// <inheritdoc/>
    bool IWritableNode.DetachChild(IWritableNode child)
    {
        if (child is T t)
        {
            return DetachChild(t);
        }

        throw new UnsupportedNodeTypeException(child, nameof(child));
    }

    /// <inheritdoc cref="IWritableNode.DetachChild"/>
    protected internal bool DetachChild(T child);

    /// <inheritdoc/>
    Containment? IWritableNode.GetContainmentOf(IWritableNode child)
    {
        if (child is T t)
        {
            return GetContainmentOf(t);
        }

        throw new UnsupportedNodeTypeException(child, nameof(child));
    }

    /// <inheritdoc cref="IWritableNode.GetContainmentOf"/>
    public Containment? GetContainmentOf(T child);

    /// <inheritdoc/>
    void IWritableNode.AddAnnotations(IEnumerable<IWritableNode> annotations) =>
        AddAnnotations(M2Extensions.AsNodes<T>(annotations, null));

    /// <inheritdoc cref="IWritableNode.AddAnnotations"/>
    public void AddAnnotations(IEnumerable<T> annotations);

    /// <inheritdoc/>
    void IWritableNode.InsertAnnotations(Index index, IEnumerable<IWritableNode> annotations) =>
        InsertAnnotations(index, M2Extensions.AsNodes<T>(annotations, null));

    /// <inheritdoc cref="IWritableNode.InsertAnnotations"/>
    public void InsertAnnotations(Index index, IEnumerable<T> annotations);

    /// <inheritdoc/>
    bool IWritableNode.RemoveAnnotations(IEnumerable<IWritableNode> annotations) =>
        RemoveAnnotations(M2Extensions.AsNodes<T>(annotations, null));

    /// <inheritdoc cref="IWritableNode.RemoveAnnotations"/>
    public bool RemoveAnnotations(IEnumerable<T> annotations);
}