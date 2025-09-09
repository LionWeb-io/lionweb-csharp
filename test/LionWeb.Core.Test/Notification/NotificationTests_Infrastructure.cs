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

namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Partition;
using Core.Notification.Pipe;
using Languages.Generated.V2024_1.Shapes.M2;
using System.Reflection;

[TestClass]
public class NotificationTests_Infrastructure: NotificationTestsBase
{
    [TestMethod]
    public void NotificationProducer()
    {
        var node = new Geometry("a");
        
        Assert.IsNotNull(node.GetNotificationSender());
        Assert.AreSame(node.GetNotificationSender(), ((IPartitionInstance)node).GetNotificationProducer());
        Assert.AreSame(node.GetNotificationSender(), ((IPartitionInstance)node).GetNotificationSender());
    }  
    
    [TestMethod]
    public void MultiListeners_NoRead()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        node.GetNotificationSender()!.Subscribe<PropertyAddedNotification>((sender, args) => { } );
        node.GetNotificationSender()!.Subscribe<PropertyChangedNotification>((sender, args) => { });
        node.GetNotificationSender()!.Subscribe<IPartitionNotification>((sender, args) => { });

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
        node.GetNotificationSender()!.Subscribe<PropertyAddedNotification>((sender, args) => addedCount++);
        node.GetNotificationSender()!.Subscribe<PropertyChangedNotification>((sender, args) => {});

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
        node.GetNotificationSender()!.Subscribe<PropertyAddedNotification>((sender, args) => addedCount++);
        
        int changedCount = 0;
        node.GetNotificationSender()!.Subscribe<PropertyChangedNotification>((sender, args) => changedCount++);

        int allCount = 0;
        node.GetNotificationSender()!.Subscribe<IPartitionNotification>((sender, args) => allCount++);

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

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);

        int nodeCount = 0;
        node.GetNotificationSender()!.Subscribe<IPartitionNotification>((sender, args) => nodeCount++);
        
        int cloneCount = 0;
        clone.GetNotificationSender()!.Subscribe<IPartitionNotification>((sender, args) => cloneCount++);
        
        circle.Name = "Hello";
        cloneCircle.Name = "World";

        AssertEquals([node], [clone]);
        Assert.AreEqual(2, nodeCount);
        Assert.AreEqual(2, cloneCount);
    }
    
    [TestMethod]
    public void Twoway_NoLingeringNotificiationIds()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        var cloneCircle = new Circle("c");
        var clone = new Geometry("a") { Shapes = [cloneCircle] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);
        
        circle.Name = "Hello";
        cloneCircle.Name = "World";

        AssertEquals([node], [clone]);
        
        Assert.AreEqual(0, ReplicatorNotificationIds(replicator).Count);
        Assert.AreEqual(0, ReplicatorNotificationIds(cloneReplicator).Count);
    }

    private static HashSet<INotificationId> ReplicatorNotificationIds(INotificationPipe replicator)
    {
        var fieldInfoFilter = typeof(MultipartNotificationHandler).GetRuntimeFields().First(f => f.Name == "_lastHandler");
        var filter = (IdFilteringNotificationFilter) fieldInfoFilter.GetValue(replicator);
     
        var fieldInfoNotificationIds = typeof(IdFilteringNotificationFilter).GetRuntimeFields().First(f => f.Name == "_notificationIds");
        var notificationIds = fieldInfoNotificationIds.GetValue(filter);
        
        return (HashSet<INotificationId>)notificationIds!;
    }

    private Tuple<INotificationHandler, INotificationHandler>
        CreateReplicators(IPartitionInstance node, IPartitionInstance clone)
    {
        var replicator = PartitionReplicator.Create(clone, new(),"cloneReplicator");
        var cloneReplicator = PartitionReplicator.Create(node, new(), "nodeReplicator");
        
        cloneReplicator.ConnectTo(replicator);
        replicator.ConnectTo(cloneReplicator);
        
        return Tuple.Create(replicator, cloneReplicator);
    }
}