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

namespace LionWeb.Protocol.Delta.Repository;

using Core.M1.Event;
using Core.M1.Event.Partition;
using Message.Event;

public class EventToDeltaEventMapper
{
    private readonly PartitionEventToDeltaEventMapper _partitionMapper;

    public EventToDeltaEventMapper(PartitionEventToDeltaEventMapper partitionMapper)
    {
        _partitionMapper = partitionMapper;
    }

    public IEvent Map(IDelta @event) =>
        @event switch
        {
            IPartitionEvent e => _partitionMapper.Map(e),
            _ => throw new NotImplementedException(@event.GetType().Name)
        };
}