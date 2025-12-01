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

#region Annotation

/// Encapsulates notification-related logic and data for <i>adding</i> or <i>inserting</i> of <see cref="Annotation"/>s.
public class AnnotationAddMultipleNotificationEmitter : AnnotationNotificationEmitterBase
{
    private Index _newIndex;

    /// <param name="destinationParent"> Owner of the represented <see cref="Annotation"/>.</param>
    /// <param name="addedValues">Newly added values.</param>
    /// <param name="existingValues">Values already present in <see cref="IReadableNode.GetAnnotations"/>.</param>
    /// <param name="startIndex">Optional index where we add <paramref name="addedValues"/> to <see cref="Annotation"/>s.</param>
    /// <param name="notificationId">The notification ID of the notification emitted by this notification emitter.</param>
    public AnnotationAddMultipleNotificationEmitter(INotifiableNode destinationParent,
        List<INode>? addedValues,
        List<INode> existingValues, Index? startIndex = null, INotificationId? notificationId = null) : base(destinationParent, addedValues, notificationId)
    {
        _newIndex = startIndex ?? Math.Max(existingValues.Count - 1, 0);
    }

    public AnnotationAddMultipleNotificationEmitter(INotifiableNode destinationParent,
        List<IAnnotationInstance>? addedValues,
        List<INode> existingValues, Index? startIndex = null, INotificationId? notificationId = null) : base(destinationParent, addedValues, notificationId)
    {
        _newIndex = startIndex ?? Math.Max(existingValues.Count - 1, 0);
    }

    /// <inheritdoc />
    public override void Notify()
    {
        if (!IsActive())
            return;

        foreach ((INode? added, OldAnnotationInfo? old) in NewValues)
        {
            switch (old)
            {
                case null:
                    ProduceNotification(new AnnotationAddedNotification(DestinationParent, added, _newIndex,
                        GetNotificationId()));
                    break;

                case not null when old.Parent != DestinationParent:
                    var notificationId = GetNotificationId();
                    var notification = new AnnotationMovedFromOtherParentNotification(DestinationParent, _newIndex, added, old.Parent,
                        old.Index, notificationId);
                    ProduceOriginMoveNotification(old, notification);
                    ProduceNotification(notification);
                    break;


                case not null when old.Parent == DestinationParent && old.Index == _newIndex:
                    // no-op
                    break;

                case not null when old.Parent == DestinationParent:
                    ProduceNotification(new AnnotationMovedInSameParentNotification(_newIndex, added, DestinationParent,
                        old.Index, GetNotificationId()));
                    break;

                default:
                    throw new ArgumentException("Unknown state");
            }

            _newIndex++;
        }
    }

    /// <inheritdoc />
    protected override bool IsActive() =>
        Handles(
            typeof(AnnotationAddedNotification),
            typeof(AnnotationMovedFromOtherParentNotification),
            typeof(AnnotationMovedInSameParentNotification)
        );
}

#endregion