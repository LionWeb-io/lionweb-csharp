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

/// Encapsulates notification-related logic and data for <i>adding</i> or <i>inserting</i> of <see cref="Annotation"/>s.
public class AnnotationAddSingleNotificationEmitter : AnnotationNotificationEmitterBase
{
    private readonly List<INode> _existingValues;
    private Index _newIndex;

    /// <param name="destinationParent"> Owner of the represented <see cref="Annotation"/>.</param>
    /// <param name="addedValue">Newly added value.</param>
    /// <param name="startIndex">Optional index where we add <paramref name="addedValue"/> to <see cref="Annotation"/>s.</param>
    public AnnotationAddSingleNotificationEmitter(
        INotifiableNode destinationParent,
        IWritableNode addedValue,
        List<INode> existingValues,
        Index? startIndex = null
    ) : base(destinationParent, [addedValue])
    {
        _existingValues = existingValues;
        _newIndex = startIndex ?? Math.Max(existingValues.Count - 1, 0);
    }

    /// <inheritdoc />
    public override void Notify()
    {
        if (!IsActive())
            return;

        foreach ((IWritableNode added, OldAnnotationInfo? old) in OldAnnotationInfos)
        {
            switch (old)
            {
                case null or { Partition: null }:
                    ProduceNotification(new AnnotationAddedNotification(DestinationParent, added, _newIndex,
                        GetNotificationId()));
                    break;

                case not null when old.Parent != DestinationParent && DestinationPartition is null:
                    if (old.Partition?.GetNotificationProducer() is { } prod)
                    {
                        var deletedNotification = new AnnotationDeletedNotification(added, old.Parent, old.Index, GetNotificationId(old.Partition));
                        prod.ProduceNotification(deletedNotification);
                    }

                    break;

                case not null when old.Parent != DestinationParent:
                    var notificationId = GetNotificationId();
                    var notification = new AnnotationMovedFromOtherParentNotification(DestinationParent, _newIndex, added, old.Parent,
                        old.Index, notificationId);
                    ProduceOriginNotification(old, notification);
                    ProduceNotification(notification);
                    break;


                case not null when old.Parent == DestinationParent && old.Index == _newIndex:
                    // no-op
                    break;

                case not null when old.Parent == DestinationParent:
                    if (_newIndex == _existingValues.Count)
                        _newIndex--;
                    if (old.Index == _newIndex)
                        break;

                    ProduceNotification(new AnnotationMovedInSameParentNotification(_newIndex, added, DestinationParent,
                        old.Index, _newIndex - old.Index, GetNotificationId()));
                    break;

                default:
                    throw new ArgumentException("Unknown state");
            }

            _newIndex++;
        }
    }
}