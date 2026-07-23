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

using M1;
using M3;

/// Encapsulates notification-related logic and data for <i>multiple</i> <see cref="Annotation"/>s.
public abstract class AnnotationNotificationEmitterBase : PartitionNotificationEmitterBase<IWritableNode>
{
    /// Newly set values and their previous context.
    protected readonly Dictionary<IWritableNode, OldAnnotationInfo?> OldAnnotationInfos;

    /// <param name="destinationParent"> Owner of the represented <see cref="Annotation"/>s.</param>
    /// <param name="newValues">Newly set values.</param>
    protected AnnotationNotificationEmitterBase(INotifiableNode destinationParent, List<IWritableNode>? newValues) : base(destinationParent)
    {
        OldAnnotationInfos = newValues?.ToDictionary<IWritableNode, IWritableNode, OldAnnotationInfo?>(k => k, _ => null) ?? [];
    }

    /// <inheritdoc />
    protected override bool IsActive() =>
        base.IsActive() ||
        OldAnnotationInfos.Values.Any(v => v?.Partition?.GetNotificationProducer()?.Handles() ?? false) ||
        OldAnnotationInfos.Keys.Any(k => k.GetPartition()?.GetNotificationProducer()?.Handles() ?? false);

    /// <inheritdoc />
    public override void CollectOldData()
    {
        if (!IsActive())
            return;

        foreach (var newValue in OldAnnotationInfos.Keys.ToList())
        {
            OldAnnotationInfos[newValue] = Collect(newValue);
        }
    }

    /// Collects <see cref="OldAnnotationInfo"/> from <paramref name="value"/>, to be used in <see cref="PartitionNotificationEmitterBase{T}.CollectOldData"/>
    protected OldAnnotationInfo? Collect(IWritableNode value)
    {
        var oldParent = value.GetParent();
        if (oldParent == null)
            return null;

        var oldPartition = value.GetPartition();
        if (oldPartition == null)
            return null;
        
        var oldIndex = oldParent.GetAnnotations().ToList().IndexOf(value);

        return new((IWritableNode)oldParent, oldIndex, oldPartition);
    }

    /// Context of an annotation instance before it has been removed from its previous <paramref name="Parent"/>.
    /// <param name="Parent"></param>
    /// <param name="Index"></param>
    protected record OldAnnotationInfo(IWritableNode Parent, Index Index, IPartitionInstance? Partition);

    protected void ProduceOriginNotification(OldAnnotationInfo old, INotification notification)
    {
        if (old.Partition != null && old.Partition != DestinationPartition)
            old.Partition.GetNotificationProducer()?.ProduceNotification(notification);
    }
}