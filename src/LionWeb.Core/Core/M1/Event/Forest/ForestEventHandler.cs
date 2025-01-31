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
/// <param name="sender">Optional sender of the events.</param>
public class ForestEventHandler(object? sender) : EventHandlerBase<IForestEvent>(sender), IForestPublisher, IForestCommander
{
    /// <inheritdoc />
    public void AddPartition(IPartitionInstance newPartition, EventId? eventId = null) =>
        Raise(new IForestPublisher.NewPartitionArgs(newPartition, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseAddPartition => CanRaise(typeof(IForestPublisher.NewPartitionArgs));

    /// <inheritdoc />
    public void DeletePartition(IPartitionInstance deletedPartition, EventId? eventId = null) =>
        Raise(new IForestPublisher.PartitionDeletedArgs(deletedPartition, eventId ?? CreateEventId()));

    /// <inheritdoc />
    public bool CanRaiseDeletePartition => CanRaise(typeof(IForestPublisher.PartitionDeletedArgs));
}