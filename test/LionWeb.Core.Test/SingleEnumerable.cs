// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Test;

using System.Collections;

/// <summary>
/// Helper class that can enumerate exactly once.
/// </summary>
class SingleEnumerable<T> : IEnumerable<T>
{
    private readonly List<T> _wrapped;
    private bool firstTime = true;

    public SingleEnumerable()
    {
        _wrapped = new();
    }
    
    public SingleEnumerable(IEnumerable<T> wrapped)
    {
        _wrapped = wrapped.ToList();
    }

    public void Add(T entry) => _wrapped.Add(entry);
    
    public IEnumerator<T> GetEnumerator()
    {
        if (firstTime)
        {
            firstTime = false;
            return _wrapped.GetEnumerator();
        }

        throw new AssertFailedException("Double enumeration");
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}