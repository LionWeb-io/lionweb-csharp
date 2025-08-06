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

namespace LionWeb.Protocol.Delta.Client.Partition;

using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Partition;
using Message.Event;

public class DeltaProtocolPartitionEventReceiver : PublisherBase<IPartitionEvent>, IPartitionPublisher
{
    private readonly DeltaEventToPartitionEventMapper _mapper;

    public DeltaProtocolPartitionEventReceiver(
        SharedNodeMap sharedNodeMap,
        SharedKeyedMap sharedKeyedMap,
        DeserializerBuilder deserializerBuilder
    )
    {
        _mapper = new DeltaEventToPartitionEventMapper(sharedNodeMap, sharedKeyedMap, deserializerBuilder);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    public void Receive(IDeltaEvent deltaEvent)
    {
        IPartitionEvent partitionEvent = _mapper.Map(deltaEvent);

        RaiseInternal(this, partitionEvent);
    }
}