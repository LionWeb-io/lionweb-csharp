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

/// <summary>
/// Provides low-level write access to the contents of a <see cref="IWritableNode"/>.
///
/// <para/>
/// Does <i>neither</i> trigger any notifications <i>nor</i> validates constraints,
/// but <i>does</i> maintain tree shape and <i>may</i> check type compatibility
/// (e.g. maybe we cannot add a node of type <c>A</c> to a containment of type <c>B</c>).
/// 
/// <para/>
/// Should be time and memory efficient.
/// </summary>
public interface IWritableNodeRaw : IReadableNodeRaw, IWritableNode
{
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
    protected internal bool SetPropertyRaw(Property property, object? value);

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
}

/// <inheritdoc cref="IWritableNodeRaw" />
public interface IWritableNodeRaw<T> : IWritableNodeRaw, IWritableNode<T> where T : class, IWritableNode;