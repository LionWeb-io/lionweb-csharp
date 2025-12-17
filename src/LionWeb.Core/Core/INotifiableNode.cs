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
using Notification;

/// <summary>
/// Represents an interface for nodes that support notifications with optional notification identifiers.
/// </summary>
/// <seealso cref="INotification"/>
public interface INotifiableNode : IWritableNode
{
    /// <inheritdoc cref="IWritableNode.AddAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    [Obsolete("Use AddAnnotations(IEnumerable<IWritableNode>) instead")]
    void AddAnnotations(IEnumerable<IWritableNode> annotations, INotificationId? notificationId) =>
        AddAnnotations(annotations);

    /// <inheritdoc cref="IWritableNode.InsertAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    [Obsolete("Use InsertAnnotations(Index, IEnumerable<IWritableNode>) instead")]
    void InsertAnnotations(Index index, IEnumerable<IWritableNode> annotations, INotificationId? notificationId) =>
        InsertAnnotations(index, annotations);

    /// <inheritdoc cref="IWritableNode.RemoveAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    [Obsolete("Use RemoveAnnotations(IEnumerable<IWritableNode>) instead")]
    bool RemoveAnnotations(IEnumerable<IWritableNode> annotations, INotificationId? notificationId) =>
        RemoveAnnotations(annotations); 

    /// <inheritdoc cref="IWritableNode.Set"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    [Obsolete("Use Set(Feature, object?) instead")]
    void Set(Feature feature, object? value, INotificationId? notificationId)
        => Set(feature, value);
}

/// The type-parametrized twin of the non-generic <see cref="INotifiableNode"/> interface.
public interface INotifiableNode<T> : INotifiableNode, IWritableNode<T> where T : class, INotifiableNode
{
    #region AddAnnotations

    /// <inheritdoc cref="IWritableNode.AddAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    [Obsolete("Use AddAnnotations(IEnumerable<T>) instead")]
    public void AddAnnotations(IEnumerable<T> annotations, INotificationId? notificationId) =>
        AddAnnotations(annotations);

    #endregion

    #region InsertAnnotations

    /// <inheritdoc cref="IWritableNode.InsertAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param> 
    [Obsolete("Use InsertAnnotations(Index, IEnumerable<IWritableNode>) instead")]
    public void InsertAnnotations(Index index, IEnumerable<T> annotations, INotificationId? notificationId) =>
        InsertAnnotations(index, annotations);


    #endregion

    #region RemoveAnnotations

    /// <inheritdoc cref="IWritableNode.RemoveAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    [Obsolete("Use RemoveAnnotations(IEnumerable<T>) instead")]
    public bool RemoveAnnotations(IEnumerable<T> annotations, INotificationId? notificationId) =>
        RemoveAnnotations(annotations);

    #endregion
}