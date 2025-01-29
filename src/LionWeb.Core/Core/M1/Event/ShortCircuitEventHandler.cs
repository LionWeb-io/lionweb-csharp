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

/// Event handler that allows to check for existing subscribers.
/// Used to avoid expensive event argument preparation if nobody is listening.
internal class ShortCircuitEventHandler<T>
{
    public event EventHandler<T>? Event;

    public void Invoke(object sender, T args)
    {
        if (HasSubscribers && Event != null)
            Event.Invoke(sender, args);
    }

    public void Add(EventHandler<T> handler)
    {
        lock (this)
        {
            Event += handler;
        }
    }

    public void Remove(EventHandler<T> handler)
    {
        lock (this)
        {
            Event -= handler;
        }
    }

    public bool HasSubscribers
    {
        get
        {
            lock (this)
            {
                return Event != null;
            }
        }
    }
}