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

/// Forwards <see cref="IForestCommander"/> commands to <see cref="IForestPublisher"/> events.
public class ForestEventHandler : EventHandlerBase<IForestEvent>, IForestPublisher, IForestCommander
{
    /// <inheritdoc cref="ForestEventHandler"/>
    /// <param name="sender">Optional sender of the events.</param>
    public ForestEventHandler(object? sender) : base(sender)
    {
    }

    /// <inheritdoc />
    public event EventHandler<IForestPublisher.NewPartitionArgs> NewPartition
    {
        add => _newPartition.Add(value);
        remove => _newPartition.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IForestPublisher.NewPartitionArgs> _newPartition = new();

    /// <inheritdoc />
    public void AddPartition(IPartitionInstance newPartition, EventId? eventId = null)
    {
        var args = new IForestPublisher.NewPartitionArgs(newPartition, eventId ?? CreateEventId());
        Raise(args);
        _newPartition.Invoke(_sender, args);
    }

    /// <inheritdoc />
    public bool CanRaiseAddPartition => _newPartition.HasSubscribers;

    /// <inheritdoc />
    public event EventHandler<IForestPublisher.PartitionDeletedArgs> PartitionDeleted
    {
        add => _partitionDeleted.Add(value);
        remove => _partitionDeleted.Remove(value);
    }

    private readonly ShortCircuitEventHandler<IForestPublisher.PartitionDeletedArgs> _partitionDeleted = new();

    /// <inheritdoc />
    public void DeletePartition(IPartitionInstance deletedPartition, EventId? eventId = null)
    {
        var args = new IForestPublisher.PartitionDeletedArgs(deletedPartition, eventId ?? CreateEventId());
        _partitionDeleted.Invoke(_sender, args);
        Raise(args);
    }

    /// <inheritdoc />
    public bool CanRaiseDeletePartition => _partitionDeleted.HasSubscribers;
}