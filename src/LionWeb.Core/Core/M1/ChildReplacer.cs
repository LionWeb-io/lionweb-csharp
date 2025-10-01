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

namespace LionWeb.Core.M1;

using M3;
using Notification;
using Notification.Partition;
using System.Collections;

public class ChildReplacer<T>(INode self, T replacement) where T : INode
{
    /// <summary>
    /// Replaces <paramref name="self"/> in its parent with <paramref name="replacement"/>.
    /// 
    /// Does <i>not</i> change references to <paramref name="self"/>.
    /// </summary>
    /// <param name="self">Base node, must have a parent.</param>
    /// <param name="replacement">Node that will replace <paramref name="self"/> in <paramref name="self"/>'s parent.</param>
    /// <param name="notificationId">The notification ID of the notification that triggers this action.</param>
    /// <typeparam name="T">Type of <paramref name="replacement"/>.</typeparam>
    /// <returns><paramref name="replacement"/></returns>
    /// <exception cref="TreeShapeException">If <paramref name="self"/> has no parent.</exception>
    public T ReplaceWith()
    {
        if (ReferenceEquals(self, replacement))
            return replacement;

        INode? parent = self.GetParent();
        if (parent == null)
            throw new TreeShapeException(self, "Cannot replace a node with no parent");

        if (replacement is null)
            throw new UnsupportedNodeTypeException(replacement, nameof(replacement));

        Containment? containment = parent.GetContainmentOf(self);

        if (containment == null)
        {
            var index = parent.GetAnnotations().ToList().IndexOf(self);
            if (index < 0)
                // should not happen
                throw new TreeShapeException(self, "Node not contained in its parent");
            ReplaceAnnotation(parent, index, self);

            return replacement;
        }

        if (containment.Multiple)
        {
            var value = parent.Get(containment);
            if (value is not IEnumerable enumerable)
                // should not happen
                throw new TreeShapeException(self, "Multiple containment does not yield enumerable");

            var nodes = enumerable.Cast<INode>().ToList();
            var index = nodes.IndexOf(self);
            if (index < 0)
                // should not happen
                throw new TreeShapeException(self, "Node not contained in its parent");

            var replacementParent = replacement.GetParent();
            var replacementContainment = replacementParent?.GetContainmentOf(replacement);

            if (containment.Equals(replacementContainment))
            {
                var replacementIndex = nodes.IndexOf(replacement);
                nodes.Insert(index, replacement);
                nodes.RemoveAt(index + 1);
                nodes.RemoveAt(index < replacementIndex ? nodes.LastIndexOf(replacement) : nodes.IndexOf(replacement));
            } else
            {
                nodes.Insert(index, replacement);
                nodes.Remove(self);
            }

            SetMany(parent, containment, nodes);
        } else
        {
            SetSingle(parent, containment, self);
        }

        return replacement;
    }

    protected virtual void SetSingle(INode parent, Containment containment, IWritableNode replacedChild) =>
        parent.Set(containment, replacement);

    protected virtual void SetMany(INode parent, Containment containment, List<INode> nodes,
        IWritableNode replacedChild, Index index) =>
        parent.Set(containment, nodes);

    protected virtual void ReplaceAnnotation(INode parent, int index, IWritableNode replacedAnnotation)
    {
        parent.InsertAnnotations(index, [replacement]);
        parent.RemoveAnnotations([self]);
    }
}

public class NotificationChildReplacer<T>(
    INode self,
    T replacement,
    IPartitionNotificationProducer producer,
    INotificationId? notificationId = null) : ChildReplacer<T>(self, replacement) where T : INode
{
    private readonly INotificationIdProvider _notificationIdProvider = new NotificationIdProvider(null);

    protected override void SetSingle(INode parent, Containment containment, IWritableNode replacedChild)
    {
        var setId = _notificationIdProvider.CreateNotificationId();
        producer.Memorize([setId],
            new ChildReplacedNotification(replacement, replacedChild, parent, containment, 0, NotificationId));

        parent.Set(containment, replacement);
    }

    protected override void SetMany(INode parent, Containment containment, IWritableNode replacedChild, Index index)
    {
        var insertId = _notificationIdProvider.CreateNotificationId();
        var removeId = _notificationIdProvider.CreateNotificationId();

        producer.Memorize(
            [insertId, removeId],
            new ChildReplacedNotification(replacement, replacedChild, parent, index, NotificationId)
        );

        parent.Insert(containment, index, [replacement], insertId);
        parent.Remove(containment, [self], removeId);
    }

    protected override void ReplaceAnnotation(INode parent, int index, IWritableNode replacedAnnotation)
    {
        var insertId = _notificationIdProvider.CreateNotificationId();
        var removeId = _notificationIdProvider.CreateNotificationId();

        producer.Memorize(
            [insertId, removeId],
            new AnnotationReplacedNotification(replacement, replacedAnnotation, parent, index, NotificationId)
        );

        parent.InsertAnnotations(index, [replacement]);
        parent.RemoveAnnotations([self]);
    }

    private INotificationId NotificationId => notificationId ?? _notificationIdProvider.CreateNotificationId();
}