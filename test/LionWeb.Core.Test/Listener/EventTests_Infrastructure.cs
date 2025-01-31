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

namespace LionWeb.Core.Test.Listener;

using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M1.Event;
using M1.Event.Partition;
using System.Threading.Channels;
using Comparer = Core.Utilities.Comparer;

[TestClass]
public class EventTests_Infrastructure
{
    [TestMethod]
    public void MultiListeners_NoRead()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        var added = node.Publisher.Subscribe<IPartitionPublisher.PropertyAddedArgs>();
        var changed = node.Publisher.Subscribe<IPartitionPublisher.PropertyChangedArgs>();
        var all = node.Publisher.Subscribe<IPartitionEvent>();

        circle.Name = "Hello";
        circle.Name = "World";

        Assert.AreEqual("World", circle.Name);
    }

    [TestMethod]
    public void MultiListeners_SomeRead()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        var added = node.Publisher.Subscribe<IPartitionPublisher.PropertyAddedArgs>();
        int addedCount = 0;
        added.WaitForNextEvent(b =>
        {
            added.TryRead(out var item);
            Console.WriteLine($"added: {item?.ToString() ?? "null"}");
            addedCount++;
        });
        
        var changed = node.Publisher.Subscribe<IPartitionPublisher.PropertyChangedArgs>();
        int changedCount = 0;
        changed.WaitForNextEvent(b =>
        {
            changed.TryRead(out var item);
            Console.WriteLine($"changed: {item?.ToString() ?? "null"}");
            changedCount++;
        });

        var all = node.Publisher.Subscribe<IPartitionEvent>();
        int allCount = 0;
        all.WaitForNextEvent(b =>
        {
            all.TryRead(out var item);
            Console.WriteLine($"all: {item?.ToString() ?? "null"}");
            allCount++;
        });

        circle.Name = "Hello";
        circle.Name = "World";
        
        Assert.AreEqual("World", circle.Name);
        Assert.AreEqual(1, addedCount);
        Assert.AreEqual(1, changedCount);
        Assert.AreEqual(2, allCount);
    }

    private void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
}