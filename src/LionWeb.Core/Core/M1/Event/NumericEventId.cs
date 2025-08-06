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

namespace LionWeb.Core.M1.Event;

using Utilities;

/// Internal event id based on a string and an increasing number.
public record NumericEventId(string Base, int Id) : IEventId
{
    /// <inheritdoc />
    public virtual bool Equals(NumericEventId? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Base, other.Base, StringComparison.InvariantCulture) && Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Base, StringComparer.InvariantCulture);
        hashCode.Add(Id);
        return hashCode.ToHashCode();
    }
}

public interface IEventIdProvider
{
    IEventId CreateEventId();
}

public class EventIdProvider : IEventIdProvider
{
    // Unique per instance
    private readonly EventId _eventIdBase;

    private int _nextId = 0;

    public EventIdProvider(object? sender)
    {
        _eventIdBase = sender as string ?? IdUtils.NewId();
    }

    /// <inheritdoc />
    public virtual IEventId CreateEventId() =>
        new NumericEventId(_eventIdBase, _nextId++);
}