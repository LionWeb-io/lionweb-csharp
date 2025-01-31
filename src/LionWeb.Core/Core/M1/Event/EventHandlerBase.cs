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

using Forest;
using Partition;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Channels;

public abstract class EventHandlerBase
{
    protected static readonly ILookup<Type, Type> _allSubtypes = InitAllSubtypes();

    private static ILookup<Type, Type> InitAllSubtypes()
    {
        Type[] allTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes();

        List<Type> baseTypes = [typeof(IEvent), typeof(IForestEvent), typeof(IPartitionEvent)];

        return baseTypes
            .SelectMany(baseType => allTypes
                .Where(subType => subType.IsAssignableTo(baseType))
                .Select(subType => (baseType, subType))
            )
            .ToLookup(k => k.baseType, e => e.subType);
    }
}

public abstract class EventHandlerBase<TWrite> : EventHandlerBase, ICommander<TWrite>, IPublisher<TWrite>
    where TWrite : IEvent
{
    private readonly Dictionary<object, Channel<TWrite>> _channels = [];
    private readonly Dictionary<Type, int> _subscribedEvents = [];

    private int nextId = 0;
    
    /// <inheritdoc />
    public virtual EventId CreateEventId() =>
        nextId++.ToString();

    /// <inheritdoc />
    public ChannelReader<TRead> Subscribe<TRead>() where TRead : TWrite
    {
        var eventType = typeof(TRead);
        foreach (var subtype in _allSubtypes[eventType])
        {
            if (_subscribedEvents.TryGetValue(subtype, out var count))
            {
                _subscribedEvents[subtype] = count + 1;
            } else
            {
                _subscribedEvents[subtype] = 1;
            }
        }

        Channel<TWrite> channel = Channel.CreateUnbounded<TWrite>(new UnboundedChannelOptions
        {
            SingleReader = true, SingleWriter = true, AllowSynchronousContinuations = true
        });
        var filteredChannelReader =
            new FilteredChannelReader<TWrite, TRead>(channel, f => _allSubtypes[eventType].Contains(f.GetType()));
        _channels.Add(filteredChannelReader, channel);
        return filteredChannelReader;
    }

    /// <inheritdoc />
    public void Unsubscribe<TRead>(ChannelReader<TRead> reader) where TRead : TWrite
    {
        if (!_channels.Remove(reader))
            return;

        var eventType = typeof(TRead);
        var allSubtypes = _allSubtypes[eventType];
        foreach (var subtype in allSubtypes)
        {
            _subscribedEvents[subtype]--;
        }
    }

    /// <inheritdoc />
    public void Raise(TWrite @event)
    {
        foreach (var channel in _channels.Values)
        {
            channel.Writer.TryWrite(@event);
        }
    }

    /// <inheritdoc />
    public bool CanRaise(Type eventType) =>
        _subscribedEvents.TryGetValue(eventType, out var count) && count > 0;
}

internal class FilteredChannelReader<TWrite, TRead>(Channel<TWrite> input, Func<TWrite, bool> filter)
    : ChannelReader<TRead>
{
    public override bool TryRead([MaybeNullWhen(false)] out TRead item)
    {
        while (input.Reader.TryRead(out TWrite? inputItem))
        {
            if (inputItem is TRead r && filter(inputItem))
            {
                item = r;
                return true;
            }
        }

        item = default;

        return false;
    }


    public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var x = input.Reader
            .WaitToReadAsync(cancellationToken)
            .AsTask()
            .ContinueWith(ContinuationFunction, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        
        return new ValueTask<bool>(x);

        bool ContinuationFunction(Task<bool> b)
        {
            if (!b.Result) return false;

            if (input.Reader.TryPeek(out TWrite? inputItem))
            {
                if (inputItem is TRead && filter(inputItem))
                    return true;
                input.Reader.TryRead(out _);
            }

            return false;
        }
    }
}