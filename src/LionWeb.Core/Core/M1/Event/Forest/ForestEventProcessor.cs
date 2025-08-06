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

public interface IForestProcessor : IEventProcessor<IForestEvent>, IForestCommander, IForestPublisher;

/// Provides events for adding and deleting <see cref="IPartitionInstance">partitions</see>.
/// <seealso cref="IForestCommander"/>
/// <seealso cref="ForestEventProcessor"/>
public interface IForestPublisher : IPublisher<IForestEvent>;

/// Raises events for adding and deleting <see cref="IPartitionInstance">partitions</see>.
/// <seealso cref="IForestPublisher"/>
/// <seealso cref="ForestEventProcessor"/>
public interface IForestCommander : ICommander<IForestEvent>;

/// Forwards <see cref="IForestCommander"/> commands to <see cref="IForestPublisher"/> events.
/// <param name="sender">Optional sender of the events.</param>
public class ForestEventProcessor(object? sender)
    : CommanderPublisherEventProcessorBase<IForestEvent>(sender), IForestProcessor
;