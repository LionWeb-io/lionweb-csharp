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

namespace LionWeb.Core.Test;

using M1;

[TestClass]
public class ShortCircuitEventHandlerTests
{
    EventHandler<int> a = (_, _) => { };
    EventHandler<int> b = (_, _) => { };
    
    [TestMethod]
    public void NeverSubscribed()
    {
        var handler = new TestEventHost();
        Assert.IsFalse(handler.ShortCircuit.HasSubscribers);
    }
    
    [TestMethod]
    public void Subscribed()
    {
        var handler = new TestEventHost();

        handler.E += a;
        
        Assert.IsTrue(handler.ShortCircuit.HasSubscribers);
    }

    [TestMethod]
    public void MultiSubscribed()
    {
        var handler = new TestEventHost();

        handler.E += a;
        handler.E += b;
        
        Assert.IsTrue(handler.ShortCircuit.HasSubscribers);
    }

    [TestMethod]
    public void DoubleSubscribedUnsubscribed()
    {
        var handler = new TestEventHost();

        handler.E += a;
        handler.E += a;
        handler.E -= a;
        
        Assert.IsTrue(handler.ShortCircuit.HasSubscribers);
    }

    [TestMethod]
    public void DoubleSubscribed()
    {
        var handler = new TestEventHost();

        handler.E += a;
        handler.E += a;
        
        Assert.IsTrue(handler.ShortCircuit.HasSubscribers);
    }

    [TestMethod]
    public void AllUnsubscribed()
    {
        var handler = new TestEventHost();

        handler.E += a;
        handler.E += b;
        handler.E -= a;
        handler.E -= b;
        
        Assert.IsFalse(handler.ShortCircuit.HasSubscribers);
    }

    class TestEventHost
    {
        public event EventHandler<int> E{
            add => ShortCircuit.Add(value);
            remove => ShortCircuit.Remove(value);
        }

        public readonly ShortCircuitEventHandler<int> ShortCircuit = new();
    }
}