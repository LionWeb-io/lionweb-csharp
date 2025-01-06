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

/// Provides events for adding and deleting <see cref="IPartitionInstance">partitions</see>.
/// <seealso cref="IForestCommander"/>
/// <seealso cref="ForestEventHandler"/>
public interface IForestListener
{
    /// <inheritdoc cref="IForestListener.NewPartition"/>
    /// <param name="NewPartition">The newly added partition.</param>
    record NewPartitionArgs(IPartitionInstance NewPartition);

    /// A new partition has been added to this forest.
    /// <seealso cref="IForestCommander.AddPartition"/>
    event EventHandler<NewPartitionArgs> NewPartition;

    /// <inheritdoc cref="IForestListener.PartitionDeleted"/>
    /// <param name="DeletedPartition">The deleted partition.</param>
    record PartitionDeletedArgs(IReadableNode DeletedPartition);

    /// A partition has been deleted from this forest.
    /// <seealso cref="IForestCommander.DeletePartition"/>
    event EventHandler<PartitionDeletedArgs> PartitionDeleted;
}

/// Raises events for adding and deleting <see cref="IPartitionInstance">partitions</see>.
/// <seealso cref="IForestListener"/>
/// <seealso cref="ForestEventHandler"/>
public interface IForestCommander
{
    /// A new partition is being added to this forrest.  
    /// <param name="newPartition">The newly added partition.</param>
    /// <seealso cref="IForestListener.NewPartition"/>
    void AddPartition(IPartitionInstance newPartition);

    /// Whether anybody would receive the <see cref="AddPartition"/> event.
    /// <returns><c>true</c> if someone would receive the <see cref="AddPartition"/> event; <c>false</c> otherwise.</returns>
    bool CanRaiseAddPartition();

    /// A partition has been deleted from this forest.
    /// <param name="deletedPartition">The deleted partition.</param>
    /// <seealso cref="IForestListener.PartitionDeleted"/>
    void DeletePartition(IPartitionInstance deletedPartition);

    /// Whether anybody would receive the <see cref="DeletePartition"/> event.
    /// <returns><c>true</c> if someone would receive the <see cref="DeletePartition"/> event; <c>false</c> otherwise.</returns>
    bool CanRaiseDeletePartition();
}