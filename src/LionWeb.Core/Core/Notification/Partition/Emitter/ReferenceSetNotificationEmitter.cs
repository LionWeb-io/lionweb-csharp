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
using Utilities.ListComparer;

/// Encapsulates notification-related logic and data for <see cref="IWritableNode.Set">reflective</see> change of <see cref="Reference"/>s.
/// <typeparam name="T">Type of nodes of the represented <see cref="Reference"/>.</typeparam>
public class ReferenceSetNotificationEmitter<T> : ReferenceMultipleNotificationEmitterBase<T> where T : IReadableNode
{
    private readonly List<IListChange<ReferenceTarget>> _changes = [];

    /// <param name="reference">Represented <see cref="Reference"/>.</param>
    /// <param name="destinationParent"> Owner of the represented <paramref name="reference"/>.</param>
    /// <param name="safeNodes">Newly added values.</param>
    /// <param name="storage">Values already present in <paramref name="reference"/>.</param>
    public ReferenceSetNotificationEmitter(Reference reference, INotifiableNode destinationParent, List<ReferenceTarget> safeNodes, IList<ReferenceTarget> storage) :
        base(reference, destinationParent, safeNodes)
    {
        if (!IsActive())
            return;

        var listComparer = IListComparer.CreateForReferenceTarget(storage, safeNodes);
        _changes = listComparer.Compare();
    }

    [Obsolete]
    public ReferenceSetNotificationEmitter(Reference reference, INotifiableNode destinationParent,
        List<ReferenceTarget> safeNodes, IList<ReferenceTarget> storage, INotificationId? notificationId = null) :
        this(reference, destinationParent, safeNodes, storage)
    {
    }

    /// <inheritdoc />
    public override void CollectOldData() { }

    /// <inheritdoc />
    public override void Notify()
    {
        if (!IsActive())
            return;

        foreach (var change in _changes)
        {
            switch (change)
            {
                case ListAdded<ReferenceTarget> added:
                    ProduceNotification(new ReferenceAddedNotification(DestinationParent, Reference, added.RightIndex, added.Element,
                        GetNotificationId()));
                    break;
                case ListMoved<ReferenceTarget> moved:
                    ProduceNotification(new EntryMovedInSameReferenceNotification(DestinationParent, Reference, moved.RightIndex,
                        moved.LeftIndex, moved.LeftElement,
                        GetNotificationId()));
                    break;
                case ListDeleted<ReferenceTarget> deleted:
                    ProduceNotification(new ReferenceDeletedNotification(DestinationParent, Reference, deleted.LeftIndex,
                        deleted.Element, GetNotificationId()));
                    break;
            }
        }
    }

    /// <inheritdoc />
    protected override bool IsActive() =>
        Handles(
            typeof(ReferenceAddedNotification),
            typeof(EntryMovedInSameReferenceNotification),
            typeof(ReferenceDeletedNotification)
        );
}