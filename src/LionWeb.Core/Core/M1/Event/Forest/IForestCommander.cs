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

namespace LionWeb.Core.M1.Event.Forest;

/// Raises events for adding and deleting <see cref="IPartitionInstance">partitions</see>.
/// <seealso cref="IForestPublisher"/>
/// <seealso cref="ForestEventHandler"/>
public interface IForestCommander
{
    /// A new partition is being added to this forrest.
    /// <param name="newPartition">The newly added partition.</param>
    /// <param name="eventId"></param>
    /// <seealso cref="IForestPublisher.NewPartition"/>
    void AddPartition(IPartitionInstance newPartition, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="AddPartition"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="AddPartition"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseAddPartition { get; }

    /// A partition has been deleted from this forest.
    /// <param name="deletedPartition">The deleted partition.</param>
    /// <param name="eventId"></param>
    /// <seealso cref="IForestPublisher.PartitionDeleted"/>
    void DeletePartition(IPartitionInstance deletedPartition, EventId? eventId = null);

    /// Whether anybody would receive the <see cref="DeletePartition"/> event.
    /// <value>
    ///     <c>true</c> if someone would receive the <see cref="DeletePartition"/> event; <c>false</c> otherwise.
    /// </value>
    bool CanRaiseDeletePartition { get; }
}