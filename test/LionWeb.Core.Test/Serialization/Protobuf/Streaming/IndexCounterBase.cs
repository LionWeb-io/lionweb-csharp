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

namespace LionWeb.Core.Test.Serialization.Protobuf.Streaming;

abstract class IndexCounterBase<T> where T : notnull
{
    private readonly Dictionary<T, ulong> _entries;
    private readonly Action<T, ulong> _adder;

    private ulong _nextIndex;

    public IndexCounterBase(ulong nextIndex, Action<T, ulong> adder, IEqualityComparer<T>? comparer = null)
    {
        _nextIndex = nextIndex;
        _adder = adder;
        _entries = new Dictionary<T, ulong>(comparer ?? EqualityComparer<T>.Default);
    }

    public ulong GetOrCreate(T? candidate)
    {
        ulong result;

        if (TryGet(candidate, out result))
            return result;

        if (_entries.TryGetValue(candidate, out result))
            return result;

        result = _nextIndex++;
        _entries[candidate] = result;
        _adder(candidate, result);

        return result;
    }

    protected abstract bool TryGet(T? candidate, out ulong result);
}

abstract class IndexLookupBase<T>
{
    private readonly Dictionary<ulong, T> _entries;

    public IndexLookupBase()
    {
        _entries = [];
    }

    public T Get(ulong idx)
    {
        if (TryGet(idx, out var candidate))
            return candidate;

        return _entries[idx];
    }
    
    public T Register(T element, ulong idx)
    {
        if (TryGet(idx, out var candidate) && Equals(element, candidate))
            return candidate;

        _entries[idx] = element;
        return element;
    }

    protected abstract bool TryGet(ulong idx, out T? candidate);
}