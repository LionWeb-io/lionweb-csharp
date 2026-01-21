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
using Notification;
using Notification.Partition;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

public abstract partial class NodeBase
{
    #region Parent handling

    /// <inheritdoc />
    void IWritableNode<INode>.SetParent(INode? parent) =>
        _parent = parent;

    /// <inheritdoc />
    bool IWritableNode<INode>.DetachChild(INode child) =>
        DetachChild(child);

    /// <inheritdoc cref="IWritableNode.DetachChild"/>
    protected virtual bool DetachChild(INode child) =>
        _annotations.Remove(child);

    /// <summary>
    /// Unsets <paramref name="child">child's</paramref> parent, if applicable. 
    /// Does <i>not</i> update parent's containments.
    /// </summary>
    /// <param name="child">Node to unset parent of.</param>
    protected void SetParentNull(INode? child)
    {
        if (child != null)
            SetParentInternal(child, null);
    }
    
    /// <summary>
    /// <see cref="DetachChildInternal">Detaches</see> <paramref name="child"/> from its current parent,
    /// and adds it to <c>this</c> node's containments.
    /// Does <i>not</i> update parent's containments.
    /// </summary>
    /// <param name="child">Node to become a child of <c>this</c> node.</param>
    protected void AttachChild(IReadableNode? child)
    {
        if (child == null)
            return;

        DetachChildInternal((INode)child);
        SetParentInternal((INode)child, this);
    }

    /// <summary>
    /// Sets <paramref name="child">child's</paramref> parent to <paramref name="parent"/>.
    /// For some visibility reason, we cannot call <c>child.SetParent(parent)</c> directly at all places.
    /// Does <i>not</i> update parent's containments.
    /// </summary>
    /// <param name="child">Child to set new parent of.</param>
    /// <param name="parent">New parent to <paramref name="child"/>.</param>
    /// <seealso cref="IWritableNode.SetParent"/>
    protected void SetParentInternal(INode child, INode? parent) =>
        child.SetParent(parent);

    /// <summary>
    /// Detaches <paramref name="child"/> from its parent, if applicable.
    /// Updates old parent's containments.
    /// </summary>
    /// <param name="child">Child to detach from its parent.</param>
    /// <seealso cref="IWritableNode.DetachChild"/>
    protected void DetachChildInternal(INode child)
    {
        var parent = child.GetParent();
        if (parent != null)
            parent.DetachChild(child);
    }

    /// <summary>
    /// Removes all members of <paramref name="list"/> from their old parent, and sets <c>this</c> node as their new parent.
    /// Updates old parents' containments.
    /// Does <i>not</i> update new parent's (aka <c>this</c>) containments.
    /// </summary>
    /// <param name="list">Nodes that should have <c>this</c> node as parent.</param>
    /// <param name="link">Origin of <paramref name="list"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="list"/>.</typeparam>
    /// <returns><paramref name="list"/> as new list.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="list"/> is <c>null</c> or contains any <c>null</c> members.</exception>
    protected List<T> SetSelfParent<T>([NotNull] List<T>? list, Link? link) where T : IReadableNode
    {
        AssureNotNull(list, link);
        AssureNotNullMembers(list, link);

        return list.Select(n =>
        {
            if (n is INode iNode)
            {
                DetachChildInternal(iNode);
                SetParentInternal(iNode, this);
            }

            return n;
        }).ToList();
    }

    /// <summary>
    /// Removes <paramref name="node"/> from <paramref name="storage"/>, and sets its parent to <c>null</c>.
    /// Does <i>not</i> update old parent's containment.
    /// </summary>
    /// <param name="node">Node to remove from <paramref name="storage"/>.</param>
    /// <param name="storage">Storage potentially containing <paramref name="node"/>.</param>
    /// <param name="link">Origin of <paramref name="storage"/>.</param>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    /// <typeparam name="T">Type of members of <paramref name="storage"/>.</typeparam>
    /// <returns><c>true</c> if <paramref name="node"/> has been removed from <paramref name="storage"/>; <c>false</c> otherwise.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="node"/> is <c>null</c> or not an instance of <typeparamref name="T"/>.</exception>
    protected bool RemoveSelfParent<T>(INode node, List<T> storage, Link? link, INotificationId? notificationId = null)
        where T : class, INode =>
        RemoveSelfParent(AsList<T>(node, link), storage, link, notificationId: notificationId);

    /// <summary>
    /// Removes all members of <paramref name="list"/> from <paramref name="storage"/>, and sets their parent to <c>null</c>.
    /// Does <i>not</i> update old parents' containments.
    /// </summary>
    /// <param name="list">Nodes to remove from <paramref name="storage"/>.</param>
    /// <param name="storage">Storage potentially containing members of <paramref name="list"/>.</param>
    /// <param name="link">Origin of <paramref name="storage"/>.</param>
    /// <param name="remover">
    ///     Optional Action to call for each removed element of <paramref name="list"/>.
    ///     Only called if <see cref="GetPartitionNotificationProducer"/> is available.
    /// </param>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    /// <typeparam name="T">Type of members of <paramref name="list"/> and <paramref name="storage"/>.</typeparam>
    /// <returns><c>true</c> if at least one member of <paramref name="list"/> has been removed from <paramref name="storage"/>; <c>false</c> otherwise.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="list"/> is <c>null</c> or contains any <c>null</c> members.</exception>
    protected bool RemoveSelfParent<T>([NotNull] List<T>? list, List<T> storage, Link? link,
        Action<IPartitionNotificationProducer, Index, T, INotificationId?>? remover = null,
        INotificationId? notificationId = null)
        where T : IReadableNode
    {
        AssureNotNull(list, link);
        AssureNotNullMembers(list, link);

        var partitionProducer = GetPartitionNotificationProducer();

        bool result = false;
        foreach (T node in list)
        {
            var index = storage.IndexOf(node);
            if (index < 0)
                continue;

            storage.RemoveAt(index);
            result = true;
            if (node is INode iNode)
                SetParentInternal(iNode, null);
            if (partitionProducer != null && remover != null)
                remover(partitionProducer, index, node, notificationId ?? partitionProducer.CreateNotificationId());
        }

        return result;
    }

    #endregion

    /// <summary>
    /// Returns <paramref name="storage"/> as non-empty read-only list.
    /// </summary>
    /// <param name="storage">list to return.</param>
    /// <param name="link">Origin of <paramref name="storage"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="storage"/>.</typeparam>
    /// <returns>Non-empty, read-only view of <paramref name="storage"/>.</returns>
    /// <exception cref="UnsetFeatureException">If <paramref name="storage"/> is empty.</exception>
    protected IReadOnlyList<T> AsNonEmptyReadOnly<T>(List<T> storage, Link link) where T : IReadableNode =>
        storage.Count != 0
            ? storage.AsReadOnly()
            : throw new UnsetFeatureException(link);

    /// <inheritdoc cref="AsNonEmptyReadOnly{T}(List{T},Link)"/>
    protected IReadOnlyList<T> AsNonEmptyReadOnly<T>(IReadOnlyList<T> storage, Link link) where T : IReadableNode =>
        storage.Count != 0
            ? storage
            : throw new UnsetFeatureException(link);

    /// <summary>
    /// Assures <paramref name="index"/> is in range of <paramref name="storage"/>.
    /// We need to check this independently of <see cref="List{T}.InsertRange"/>
    /// to assure we're throwing the correct exception in case multiple exceptions might occur.
    /// </summary>
    /// <param name="index">Index to check for.</param>
    /// <param name="storage">List to check for.</param>
    /// <typeparam name="T">Type of members of <paramref name="storage"/>.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is in out of range of <paramref name="storage"/>.</exception>
    protected void AssureInRange<T>(Index index, IList<T> storage)
    {
        if (!IsInRange(index, storage))
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }

    /// <summary>
    /// Assures <paramref name="value"/>'s is not <c>null</c> and its <see cref="ReferenceTarget.Target"/> is <c>null</c> or <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">Value to guard against <c>null</c>.</param>
    /// <param name="feature">Feature <paramref name="value"/> originates from.</param>
    /// <exception cref="InvalidValueException">If <paramref name="value"/> is null or its <see cref="ReferenceTarget.Target"/> is not <typeparamref name="T"/>.</exception>
    protected void AssureNotNullInstance<T>([NotNull] ReferenceTarget? value, Feature? feature)
    {
        if (value is { Target: null or T })
            return;

        throw new InvalidValueException(feature, value);
    }

    /// <summary>
    /// Assures <paramref name="value"/>'s is <c>null</c> or its <see cref="ReferenceTarget.Target"/> is <c>null</c> or <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">Value to guard against <c>null</c>.</param>
    /// <param name="feature">Feature <paramref name="value"/> originates from.</param>
    /// <exception cref="InvalidValueException">If <paramref name="value"/> is not <typeparamref name="T"/>.</exception>
    protected void AssureNullableInstance<T>(ReferenceTarget? value, Feature? feature)
    {
        if (value is null or { Target: null or T })
            return;

        throw new InvalidValueException(feature, value);
    }

    /// <summary>
    /// Assures <paramref name="list"/> is not <c>null</c>, none of its members are <c>null</c>,
    /// and neither <paramref name="list"/> nor <paramref name="storage"/> are empty.
    /// </summary>
    /// <param name="list">Nodes that should be added to <paramref name="storage"/>.</param>
    /// <param name="storage">Where nodes of <paramref name="list"/> are planned to be stored.</param>
    /// <param name="link">Link containing <paramref name="storage"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="list"/> and <paramref name="storage"/>.</typeparam>
    /// <exception cref="InvalidValueException">If <paramref name="list"/> is <c>null</c>, contains any <c>null</c> members,
    /// or both <paramref name="list"/> and <paramref name="storage"/> are empty.</exception>
    protected void AssureNonEmpty<T>([NotNull] List<T>? list, IList storage, Link link)
    {
        if (list == null || (list.Count == 0 && storage.Count == 0))
            throw new InvalidValueException(link, list);
        AssureNotNullMembers(list, link);
    }

    /// <summary>
    /// Assures <paramref name="safeNodes"/> is not <c>null</c>, not emmpty, and none of its members are <c>null</c>.
    /// </summary>
    /// <param name="safeNodes">Nodes that should be added to <c>this</c>.</param>
    /// <param name="link">Link that should contain <paramref name="safeNodes"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="safeNodes"/>.</typeparam>
    /// <exception cref="InvalidValueException">If <paramref name="safeNodes"/> is <c>null</c>, empty or contains any <c>null</c> members.</exception>
    protected void AssureNonEmpty<T>(List<T> safeNodes, Link link)
    {
        if (safeNodes.Count == 0)
            throw new InvalidValueException(link, safeNodes);
        AssureNotNullMembers(safeNodes, link);
    }

    protected bool IsInRange<T>(Index index, IList<T> storage) =>
        (uint)index <= (uint)storage.Count;

    /// <summary>
    /// Usually, we <i>can</i> insert nodes at the end of <param name="storage"></param>
    /// by supplying <paramref name="index"/> = <paramref name="storage"/>.Count.
    /// However, if any of <paramref name="safeNodes"/> is already contained in <paramref name="storage"/>,
    /// we just move the node around -- without changing the length of <paramref name="safeNodes"/>.
    /// This method assures against that case.
    /// </summary>
    /// <param name="index">Index to check for.</param>
    /// <param name="safeNodes">Elements to be inserted.</param>
    /// <param name="storage">Currently stored elements.</param>
    /// <typeparam name="T">Type of members of <paramref name="safeNodes"/> and <paramref name="storage"/>.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If trying to insert nodes at the end of <paramref name="storage"/>,
    /// and any of <paramref name="safeNodes"/> is already contained in <paramref name="storage"/>.
    /// </exception>
    protected void AssureNoSelfMove<T>(Index index, List<T> safeNodes, List<T> storage)
    {
        if (index == storage.Count && safeNodes.Any(storage.Contains))
            throw new ArgumentOutOfRangeException(nameof(index), index, null);
    }

    /// <summary>
    /// Assures <paramref name="storage"/> would contain at least one member after removing all of <paramref name="safeNodes"/>.
    /// Does <i>not</i> modify <paramref name="storage"/>.
    /// </summary>
    /// <param name="safeNodes">Candidates to be removed.</param>
    /// <param name="storage">Currently stored elements.</param>
    /// <param name="link">Link of <paramref name="storage"/>.</param>
    /// <typeparam name="T">Type of members of <paramref name="safeNodes"/> and <paramref name="storage"/>.</typeparam>
    /// <exception cref="InvalidValueException">If <paramref name="storage"/> were empty after removing all of <paramref name="safeNodes"/>.</exception>
    protected void AssureNotClearing<T>(List<T?> safeNodes, IEnumerable<T> storage, Link link) where T : IReadableNode?
    {
        var copy = new List<T>(storage);
        RemoveAll(safeNodes, copy, null);
        if (copy.Count == 0)
            throw new InvalidValueException(link, safeNodes);
    }

    /// <summary>
    /// Returns singleton list of <paramref name="node"/>.
    /// </summary>
    /// <param name="node">Node to return in a singleton list.</param>
    /// <param name="link">Origin of <paramref name="node"/>.</param>
    /// <typeparam name="T">Desired type of returned singleton list.</typeparam>
    /// <returns>Singleton list of <paramref name="node"/>.</returns>
    /// <exception cref="InvalidValueException">If <paramref name="node"/> is not an instance of <typeparamref name="T"/>.</exception>
    protected List<T> AsList<T>(IReadableNode node, Link? link)
    {
        if (node is not T t)
            throw new InvalidValueException(link, node);

        return [t];
    }

    /// <summary>
    /// Removes all members of <paramref name="safeNodes"/> from <paramref name="storage"/>.
    /// Silently ignores members of <paramref name="safeNodes"/> that aren't part of <paramref name="storage"/>.
    /// </summary>
    /// <param name="safeNodes">Nodes to remove.</param>
    /// <param name="storage">Storage of nodes.</param>
    /// <param name="remover">
    ///     Optional Action to call for each removed element of <paramref name="safeNodes"/>.
    ///     Only called if <see cref="GetPartitionNotificationProducer"/> is available.
    /// </param>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    /// <typeparam name="T">Type of members of <paramref name="safeNodes"/> and <paramref name="storage"/>.</typeparam>
    protected void RemoveAll<T>(List<T?> safeNodes, List<T> storage,
        Action<IPartitionNotificationProducer, Index, T, INotificationId?>? remover,
        INotificationId? notificationId = null)
        where T : IReadableNode
    {
        var partitionProducer = GetPartitionNotificationProducer();

        foreach (var node in safeNodes)
        {
            if (node is null)
                continue;
            var index = storage.IndexOf(node);
            if (index < 0)
                continue;

            storage.RemoveAt(index);
            if (partitionProducer != null && remover != null)
                remover(partitionProducer, index, node, notificationId ?? partitionProducer.CreateNotificationId());
        }
    }

    /// <inheritdoc cref="RemoveAll{T}(List{T}, List{T}, Action{IPartitionNotificationProducer, Index, T, INotificationId?}?, INotificationId?)"/>
    protected void RemoveAll<T>(List<T> safeNodes, List<ReferenceTarget> storage,
        Action<IPartitionNotificationProducer, Index, T, INotificationId?>? remover,
        INotificationId? notificationId = null)
        where T : IReadableNode
    {
        var partitionProducer = GetPartitionNotificationProducer();

        foreach (var node in safeNodes)
        {
            var index = storage.FindIndex(r => Equals(node, r.Target));
            if (index < 0)
                continue;

            storage.RemoveAt(index);
            if (partitionProducer != null && remover != null)
                remover(partitionProducer, index, node, notificationId ?? partitionProducer.CreateNotificationId());
        }
    }

    /// Raises <see cref="ReferenceDeletedNotification"/> for <paramref name="reference"/>.
    protected Action<IPartitionNotificationProducer, Index, T, INotificationId?>
        ReferenceRemover<T>(Reference reference) where T : IReadableNode =>
        (producer, index, node, notificationId) =>
        {
            IReferenceTarget deletedTarget = ReferenceTarget.FromNode(node);
            producer.ProduceNotification(new ReferenceDeletedNotification(this, reference, index, deletedTarget,
                notificationId ?? producer.CreateNotificationId()));
        };

    /// Raises <see cref="ChildDeletedNotification"/> for <paramref name="containment"/>.
    protected Action<IPartitionNotificationProducer, Index, T, INotificationId?>
        ContainmentRemover<T>(Containment containment) where T : INode =>
        (producer, index, node, notificationId) =>
            producer.ProduceNotification(new ChildDeletedNotification(node, this, containment, index,
                notificationId ?? producer.CreateNotificationId()));

    /// Raises <see cref="AnnotationDeletedNotification"/>.
    private void AnnotationRemover(IPartitionNotificationProducer producer, Index index, INode node,
        INotificationId? notificationId = null) =>
        producer.ProduceNotification(new AnnotationDeletedNotification(node, this, index,
            notificationId ?? producer.CreateNotificationId()));
}