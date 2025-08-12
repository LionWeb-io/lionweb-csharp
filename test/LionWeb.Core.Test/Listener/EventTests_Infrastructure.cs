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
using Notification;
using Notification.Partition;
using System.Reflection;
using Comparer = Core.Utilities.Comparer;

[TestClass]
public class EventTests_Infrastructure
{
    [TestMethod]
    public void MultiListeners_NoRead()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        node.GetPublisher().Subscribe<PropertyAddedNotification>((sender, args) => { } );
        node.GetPublisher().Subscribe<PropertyChangedNotification>((sender, args) => { });
        node.GetPublisher().Subscribe<IPartitionNotification>((sender, args) => { });

        circle.Name = "Hello";
        circle.Name = "World";

        Assert.AreEqual("World", circle.Name);
    }

    [TestMethod]
    public void MultiListeners_SomeRead()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        int addedCount = 0;
        node.GetPublisher().Subscribe<PropertyAddedNotification>((sender, args) => addedCount++);
        
        node.GetPublisher().Subscribe<PropertyChangedNotification>((sender, args) => {});

        circle.Name = "Hello";
        circle.Name = "World";
        
        Assert.AreEqual("World", circle.Name);
        Assert.AreEqual(1, addedCount);
    }

    [TestMethod]
    public void MultiListeners_AllRead()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        int addedCount = 0;
        node.GetPublisher().Subscribe<PropertyAddedNotification>((sender, args) => addedCount++);
        
        int changedCount = 0;
        node.GetPublisher().Subscribe<PropertyChangedNotification>((sender, args) => changedCount++);

        int allCount = 0;
        node.GetPublisher().Subscribe<IPartitionNotification>((sender, args) => allCount++);

        circle.Name = "Hello";
        circle.Name = "World";
        
        Assert.AreEqual("World", circle.Name);
        Assert.AreEqual(1, addedCount);
        Assert.AreEqual(1, changedCount);
        Assert.AreEqual(2, allCount);
    }

    [TestMethod]
    public void Twoway_OtherListeners()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        var cloneCircle = new Circle("c");
        var clone = new Geometry("a") { Shapes = [cloneCircle] };

        var replicator = new PartitionNotificationReplicator(clone);
        var cloneReplicator = new PartitionNotificationReplicator(node);
        
        replicator.ReplicateFrom(cloneReplicator);
        cloneReplicator.ReplicateFrom(replicator);

        int nodeCount = 0;
        node.GetPublisher().Subscribe<IPartitionNotification>((sender, args) => nodeCount++);
        
        int cloneCount = 0;
        clone.GetPublisher().Subscribe<IPartitionNotification>((sender, args) => cloneCount++);
        
        circle.Name = "Hello";
        cloneCircle.Name = "World";

        AssertEquals([node], [clone]);
        Assert.AreEqual(2, nodeCount);
        Assert.AreEqual(2, cloneCount);
    }
    
    [TestMethod]
    public void Twoway_NoLingeringEventIds()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        var cloneCircle = new Circle("c");
        var clone = new Geometry("a") { Shapes = [cloneCircle] };

        var replicator = new PartitionNotificationReplicator(clone);
        var cloneReplicator = new PartitionNotificationReplicator(node);
        
        replicator.ReplicateFrom(cloneReplicator);
        cloneReplicator.ReplicateFrom(replicator);

        circle.Name = "Hello";
        cloneCircle.Name = "World";

        AssertEquals([node], [clone]);
        
        Assert.AreEqual(0, ReplicatorEventIds(replicator).Count);
        Assert.AreEqual(0, ReplicatorEventIds(cloneReplicator).Count);
    }

    private static HashSet<INotificationId> ReplicatorEventIds(PartitionNotificationReplicator replicator)
    {
        var type = typeof(NotificationIdFilteringNotificationForwarder<IPartitionNotification, IPartitionPublisher>);
        var fieldInfo = type.GetRuntimeFields().First(f => f.Name == "_notificationIds");
        var value = fieldInfo.GetValue(replicator);
        return (HashSet<INotificationId>)value!;
    }

    private void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
}