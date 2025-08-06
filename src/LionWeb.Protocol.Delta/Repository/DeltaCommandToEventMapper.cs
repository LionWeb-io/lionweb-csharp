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

namespace LionWeb.Protocol.Delta.Repository;

using Core.M1.Event;
using Message.Command;

public class DeltaCommandToEventMapper
{
    private readonly DeltaCommandToPartitionEventMapper _partitionMapper;

    public DeltaCommandToEventMapper(DeltaCommandToPartitionEventMapper partitionMapper)
    {
        _partitionMapper = partitionMapper;
    }

    public IDelta Map(ICommand command) =>
        command switch
        {
            IForestDeltaCommand c => _partitionMapper.Map(c),
            
            _ => throw new NotImplementedException(command.GetType().Name)
        };
}