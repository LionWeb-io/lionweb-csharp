﻿// Copyright 2025 TRUMPF Laser SE and other contributors
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

using Event.Forest;
using Utilities;

public interface IForest
{
    public IReadOnlySet<IPartitionInstance> Partitions { get; }

    public void AddPartitions(IEnumerable<IPartitionInstance> partitions);

    public void RemovePartitions(IEnumerable<IPartitionInstance> partitions);

    IForestPublisher? GetPublisher();
    IForestCommander? GetCommander();
}

public class Forest : IForest
{
    private readonly HashSet<IPartitionInstance> _partitions;
    private readonly ForestEventHandler _eventHandler;

    public Forest()
    {
        _partitions = new HashSet<IPartitionInstance>(new NodeIdComparer());
        _eventHandler = new ForestEventHandler(this);
    }

    /// <inheritdoc />
    public IReadOnlySet<IPartitionInstance> Partitions => _partitions;

    /// <inheritdoc />
    public void AddPartitions(IEnumerable<IPartitionInstance> partitions)
    {
        foreach (var partition in partitions)
        {
            if (_partitions.Add(partition))
                _eventHandler.Raise(new NewPartitionEvent(partition, _eventHandler.CreateEventId()));
        }
    }

    /// <inheritdoc />
    public void RemovePartitions(IEnumerable<IPartitionInstance> partitions)
    {
        foreach (var partition in partitions)
        {
            if (_partitions.Remove(partition))
                _eventHandler.Raise(new PartitionDeletedEvent(partition, _eventHandler.CreateEventId()));
        }
    }

    /// <inheritdoc />
    public IForestPublisher GetPublisher() => _eventHandler;

    /// <inheritdoc />
    public IForestCommander GetCommander() => _eventHandler;
}