// Copyright 2026 TRUMPF Laser SE and other contributors
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

/// <summary>
/// Provides unique, consecutive, positive event sequence numbers.
/// </summary>
public interface IEventSequenceNumberProvider
{
    /// <summary>
    /// The next unique, consecutive, positive event sequence number.
    /// </summary>
    EventSequenceNumber Next();
}

/// <inheritdoc />
public class EventSequenceNumberProvider : IEventSequenceNumberProvider
{
    private EventSequenceNumber _nextEventSequenceNumber = 0;

    /// <inheritdoc />
    public EventSequenceNumber Next() => ++_nextEventSequenceNumber;
}

/// <summary>
/// Dummy no-op implementation of <see cref="IEventSequenceNumberProvider"/>.
/// </summary>
public class NoOpEventSequenceNumberProvider : IEventSequenceNumberProvider
{
    /// <inheritdoc />
    public EventSequenceNumber Next() => -1;
}