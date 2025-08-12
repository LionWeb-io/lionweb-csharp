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

/// <summary>
/// Represents an interface for nodes that support notifications with optional notification identifiers.
/// </summary>
/// <seealso cref="INotification"/>
public interface INotifiableNode : IWritableNode
{
    void IWritableNode.AddAnnotations(IEnumerable<IWritableNode> annotations) => AddAnnotations(annotations, null);

    /// <inheritdoc cref="IWritableNode.AddAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    public void AddAnnotations(IEnumerable<IWritableNode> annotations, INotificationId? notificationId = null);

    void IWritableNode.InsertAnnotations(Index index, IEnumerable<IWritableNode> annotations) => InsertAnnotations(index, annotations, null);

    /// <inheritdoc cref="IWritableNode.InsertAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    public void InsertAnnotations(Index index, IEnumerable<IWritableNode> annotations, INotificationId? notificationId = null);

    bool IWritableNode.RemoveAnnotations(IEnumerable<IWritableNode> annotations) => RemoveAnnotations(annotations, null);

    /// <inheritdoc cref="IWritableNode.RemoveAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    public bool RemoveAnnotations(IEnumerable<IWritableNode> annotations, INotificationId? notificationId = null);

    void IWritableNode.Set(Feature feature, object? value) => Set(feature, value, null);

    /// <inheritdoc cref="IWritableNode.Set"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    public void Set(Feature feature, object? value, INotificationId? notificationId = null);
}

/// The type-parametrized twin of the non-generic <see cref="INotifiableNode"/> interface.
public interface INotifiableNode<T> : INotifiableNode, IWritableNode<T> where T : class, INotifiableNode
{
    #region AddAnnotations

    void IWritableNode.AddAnnotations(IEnumerable<IWritableNode> annotations) => 
        AddAnnotations(M2Extensions.AsNodes<T>(annotations), null);
    
    void INotifiableNode.AddAnnotations(IEnumerable<IWritableNode> annotations, INotificationId? notificationId) => 
        AddAnnotations(M2Extensions.AsNodes<T>(annotations), notificationId);
    
    void IWritableNode<T>.AddAnnotations(IEnumerable<T> annotations) => 
        AddAnnotations(M2Extensions.AsNodes<T>(annotations));

    /// <inheritdoc cref="IWritableNode.AddAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    public void AddAnnotations(IEnumerable<T> annotations, INotificationId? notificationId = null);

    #endregion

    #region InsertAnnotations

    void IWritableNode.InsertAnnotations(Index index, IEnumerable<IWritableNode> annotations) =>
        InsertAnnotations(index, M2Extensions.AsNodes<T>(annotations), null);
    
    void INotifiableNode.InsertAnnotations(Index index, IEnumerable<IWritableNode> annotations, INotificationId? notificationId) =>
        InsertAnnotations(index, M2Extensions.AsNodes<T>(annotations), notificationId);
    
    void IWritableNode<T>.InsertAnnotations(Index index, IEnumerable<T> annotations) =>
        InsertAnnotations(index, M2Extensions.AsNodes<T>(annotations));

    /// <inheritdoc cref="IWritableNode.InsertAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param> 
    public void InsertAnnotations(Index index, IEnumerable<T> annotations, INotificationId? notificationId = null);

    #endregion

    #region RemoveAnnotations

    bool IWritableNode.RemoveAnnotations(IEnumerable<IWritableNode> annotations) => 
        RemoveAnnotations(M2Extensions.AsNodes<T>(annotations), null);

    bool INotifiableNode.RemoveAnnotations(IEnumerable<IWritableNode> annotations, INotificationId? notificationId) => 
        RemoveAnnotations(M2Extensions.AsNodes<T>(annotations), notificationId);

    bool IWritableNode<T>.RemoveAnnotations(IEnumerable<T> annotations) => 
        RemoveAnnotations(M2Extensions.AsNodes<T>(annotations));

    /// <inheritdoc cref="IWritableNode.RemoveAnnotations"/>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    public bool RemoveAnnotations(IEnumerable<T> annotations, INotificationId? notificationId = null);

    #endregion
}