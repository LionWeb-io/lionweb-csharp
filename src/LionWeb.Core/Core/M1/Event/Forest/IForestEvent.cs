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

namespace LionWeb.Core.M1.Event.Forest;

public interface IForestEvent : IEvent;

/// A partition has been deleted from this forest.
/// <param name="DeletedPartition">The deleted partition.</param>
public record PartitionDeletedEvent(IPartitionInstance DeletedPartition, EventId EventId) : IForestEvent;

/// A new partition has been added to this forrest.
/// <param name="NewPartition">The newly added partition.</param>
public record NewPartitionEvent(IPartitionInstance NewPartition, EventId EventId) : IForestEvent;