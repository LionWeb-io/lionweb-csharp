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

namespace LionWeb.Core.Notification.Partition.Emitter;

using M3;
using System.Diagnostics.CodeAnalysis;
using Utilities.ListComparer;

/// Encapsulates notification-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Annotation"/>s.
public class AnnotationSetNotificationEmitter : AnnotationNotificationEmitterBase
{
    private readonly List<IListChange<INode>> _changes = [];

    /// <param name="destinationParent"> Owner of the represented <see cref="Annotation"/>s.</param>
    /// <param name="setValues">Newly set values.</param>
    /// <param name="existingValues">Values previously present in <see cref="IReadableNode.GetAnnotations"/>.</param>
    public AnnotationSetNotificationEmitter(INotifiableNode destinationParent,
        List<INode>? setValues,
        List<INode> existingValues) : base(destinationParent, setValues)
    {
        if (!IsActive() || setValues == null)
            return;

        var listComparer = IListComparer.CreateForNodes(existingValues, setValues);
        _changes = listComparer.Compare();
    }

    [Obsolete]
    public AnnotationSetNotificationEmitter(INotifiableNode destinationParent,
        List<INode>? setValues,
        List<INode> existingValues,
        INotificationId? notificationId = null) : this(destinationParent, setValues, existingValues)
    {}

    /// <inheritdoc />
    public override void Notify()
    {
        if (!IsActive())
            return;

        foreach (var change in _changes)
        {
            switch (change)
            {
                case ListAdded<INode> added:
                    switch (NewValues[added.Element])
                    {
                        case null:
                            ProduceNotification(new AnnotationAddedNotification(DestinationParent, added.Element,
                                added.RightIndex, GetNotificationId()));
                            break;

                        case { } old when old.Parent != DestinationParent:
                            var notificationId = GetNotificationId();
                            var notification = new AnnotationMovedFromOtherParentNotification(DestinationParent, added.RightIndex,
                                added.Element, old.Parent, old.Index, notificationId);
                            ProduceOriginMoveNotification(old, notification);
                            ProduceNotification(notification);
                            break;


                        default:
                            throw new ArgumentException("Unknown state");
                    }

                    break;

                case ListMoved<INode> moved:
                    ProduceNotification(new AnnotationMovedInSameParentNotification(moved.RightIndex, moved.LeftElement,
                        DestinationParent, moved.LeftIndex, GetNotificationId()));
                    break;

                case ListDeleted<INode> deleted:
                    ProduceNotification(new AnnotationDeletedNotification(deleted.Element, DestinationParent, deleted.LeftIndex,
                        GetNotificationId()));
                    break;
            }
        }
    }

    /// <inheritdoc />
    protected override bool IsActive() =>
        Handles(
            typeof(AnnotationAddedNotification),
            typeof(AnnotationDeletedNotification),
            typeof(AnnotationMovedFromOtherParentNotification),
            typeof(AnnotationMovedInSameParentNotification)
        );
}