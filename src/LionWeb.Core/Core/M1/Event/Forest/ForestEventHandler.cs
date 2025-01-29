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

/// Forwards <see cref="IForestCommander"/> commands to <see cref="IForestListener"/> events.
public class ForestEventHandler : IForestListener, IForestCommander
{
    private readonly object _sender;

    /// <inheritdoc cref="ForestEventHandler"/>
    /// <param name="sender">Optional sender of the events.</param>
    public ForestEventHandler(object? sender)
    {
        _sender = sender ?? this;
    }

    /// <inheritdoc />
    public event EventHandler<IForestListener.NewPartitionArgs> NewPartition
    {
        add => _newPartition.Add(value);
        remove => _newPartition.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IForestListener.NewPartitionArgs> _newPartition = new();

    /// <inheritdoc />
    public void AddPartition(IPartitionInstance newPartition) =>
        _newPartition.Invoke(_sender, new(newPartition));

    /// <inheritdoc />
    public bool CanRaiseAddPartition => _newPartition.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IForestListener.PartitionDeletedArgs> PartitionDeleted
    {
        add => _partitionDeleted.Add(value);
        remove => _partitionDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IForestListener.PartitionDeletedArgs> _partitionDeleted = new();

    /// <inheritdoc />
    public void DeletePartition(IPartitionInstance deletedPartition) =>
        _partitionDeleted.Invoke(_sender, new(deletedPartition));

    /// <inheritdoc />
    public bool CanRaiseDeletePartition => _partitionDeleted.HasSubscribers;
}