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

namespace LionWeb.Core.M1.Event.Partition.Emitter;

using M3;

/// Encapsulates event-related logic and data for <i>multiple</i> <see cref="Annotation"/>s.
public abstract class AnnotationEventEmitterBase : PartitionEventEmitterBase<INode>
{
    /// Newly set values and their previous context.
    protected readonly Dictionary<INode, OldAnnotationInfo?> NewValues;

    /// <param name="newParent"> Owner of the represented <see cref="Annotation"/>s.</param>
    /// <param name="newValues">Newly set values.</param>
    protected AnnotationEventEmitterBase(NodeBase newParent, List<INode>? newValues) : base(newParent)
    {
        NewValues = newValues?.ToDictionary<INode, INode, OldAnnotationInfo?>(k => k, _ => null) ?? [];
    }

    /// <inheritdoc />
    public override void CollectOldData()
    {
        if (!IsActive())
            return;

        foreach (var newValue in NewValues.Keys.ToList())
        {
            var oldParent = newValue.GetParent();
            if (oldParent == null)
                continue;

            var oldIndex = oldParent.GetAnnotations().ToList().IndexOf(newValue);

            var oldPartition = newValue.GetPartition();

            NewValues[newValue] = new(oldParent, oldIndex, oldPartition);
        }
    }

    /// Context of an annotation instance before it has been removed from its previous <paramref name="Parent"/>.
    /// <param name="Parent"></param>
    /// <param name="Index"></param>
    protected record OldAnnotationInfo(INode Parent, Index Index, IPartitionInstance? Partition);


    protected void RaiseOldDeletionEvent(OldAnnotationInfo old, EventId eventId, IWritableNode moved)
    {
        if (old.Partition != null && old.Partition != newPartition)
        {
            old.Partition.GetCommander()
                ?.DeleteAnnotation(moved, old.Parent, old.Index, eventId);
        }
    }
}