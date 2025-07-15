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

namespace LionWeb.Protocol.Delta.Repository.Forest;

using Core;
using Core.M1;
using Core.M1.Event;
using Core.M1.Event.Forest;
using Message.Command;

public class DeltaCommandToForestEventMapper(
    SharedNodeMap sharedNodeMap,
    SharedKeyedMap sharedKeyedMap,
    DeserializerBuilder deserializerBuilder)
    : DeltaCommandToEventMapperBase(sharedNodeMap, sharedKeyedMap, deserializerBuilder)
{
    public IForestEvent Map(IDeltaCommand command) =>
        command switch
        {
            AddPartition a => OnAddPartition(a),
            DeletePartition a => OnDeletePartition(a),
            _ => throw new NotImplementedException(command.GetType().Name)
        };

    private PartitionAddedEvent OnAddPartition(AddPartition addPartition) =>
        new(
            (IPartitionInstance)Deserialize(addPartition.NewPartition),
            ToEventId(addPartition)
        );

    private PartitionDeletedEvent OnDeletePartition(DeletePartition deletePartition) =>
        new(
            (IPartitionInstance)ToNode(deletePartition.DeletedPartition),
            ToEventId(deletePartition)
        );
}