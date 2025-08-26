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
using Core.Notification.Handler;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class CompositorTests
{
    [TestMethod]
    public void ComposePartition_none()
    {
        var partition = new Geometry("partition");

        var compositor = new NotificationCompositor("compositor");
        partition.GetNotificationHandler()?.ConnectTo(compositor);

        var counter = new PartitionEventCounter();
        compositor.ConnectTo(counter);
        
        partition.Documentation = new Documentation("documentation");
        partition.Documentation.Text = "hello";
        partition.AddShapes([new Circle("c")]);
        
        Assert.AreEqual(3, counter.Count);
    }
    
    [TestMethod]
    public void ComposePartition_one_send()
    {
        var partition = new Geometry("partition");

        var compositor = new NotificationCompositor("compositor");
        partition.GetNotificationHandler()?.ConnectTo(compositor);

        var counter = new PartitionEventCounter();
        compositor.ConnectTo(counter);

        var composite = compositor.Push();
        
        partition.Documentation = new Documentation("documentation");
        partition.Documentation.Text = "hello";
        partition.AddShapes([new Circle("c")]);
        
        Assert.AreEqual(3, composite.Parts.Count);
        Assert.AreEqual(0, counter.Count);

        compositor.Pop(true);
        Assert.AreEqual(1, counter.Count);
    }
    
    [TestMethod]
    public void ComposePartition_one_suppress()
    {
        var partition = new Geometry("partition");

        var compositor = new NotificationCompositor("compositor");
        partition.GetNotificationHandler()?.ConnectTo(compositor);

        var counter = new PartitionEventCounter();
        compositor.ConnectTo(counter);

        var composite = compositor.Push();
        
        partition.Documentation = new Documentation("documentation");
        partition.Documentation.Text = "hello";
        partition.AddShapes([new Circle("c")]);
        
        Assert.AreEqual(3, composite.Parts.Count);
        Assert.AreEqual(0, counter.Count);

        compositor.Pop(false);
        Assert.AreEqual(0, counter.Count);
    }
    
    [TestMethod]
    public void ComposePartition_two()
    {
        var partition = new Geometry("partition");

        var compositor = new NotificationCompositor("compositor");
        partition.GetNotificationHandler()?.ConnectTo(compositor);

        var counter = new PartitionEventCounter();
        compositor.ConnectTo(counter);

        var compositeA = compositor.Push();
        
        partition.Documentation = new Documentation("documentation");
        partition.Documentation.Text = "hello";

        var compositeB = new CompositeNotification(new NumericNotificationId("manual", 0));
        var compositeBPushed = compositor.Push(compositeB);
        Assert.AreSame(compositeB, compositeBPushed);
        
        partition.AddShapes([new Circle("c")]);
        
        Assert.AreEqual(3, compositeA.Parts.Count);
        Assert.AreEqual(1, compositeB.Parts.Count);
        Assert.AreEqual(0, counter.Count);

        var compositeBPopped = compositor.Pop(true);
        Assert.AreSame(compositeB, compositeBPopped);
        Assert.AreEqual(1, counter.Count);
    }
    
    [TestMethod]
    public void ComposeForest_onePartition_oneComposite()
    {
        var forest = new Forest();
        var partition = new Geometry("partition");

        var compositor = new NotificationCompositor("compositor");
        forest.GetNotificationHandler().ConnectTo(compositor);
        
        forest.AddPartitions([partition]);

        var counter = new ForestEventCounter();
        compositor.ConnectTo(counter);

        var composite = compositor.Push();
        
        partition.Documentation = new Documentation("documentation");
        partition.Documentation.Text = "hello";
        partition.AddShapes([new Circle("c")]);
        
        Assert.AreEqual(3, composite.Parts.Count);
        Assert.AreEqual(0, counter.Count);

        compositor.Pop(true);
        Assert.AreEqual(1, counter.Count);
    }

    [TestMethod]
    public void ComposeForest_deletePartition_oneComposite()
    {
        var forest = new Forest();
        var partition = new Geometry("partition");

        var compositor = new NotificationCompositor("compositor");
        forest.GetNotificationHandler().ConnectTo(compositor);
        
        forest.AddPartitions([partition]);

        var counter = new ForestEventCounter();
        compositor.ConnectTo(counter);

        var composite = compositor.Push();
        
        partition.Documentation = new Documentation("documentation");
        partition.Documentation.Text = "hello";
        partition.AddShapes([new Circle("c")]);
        
        forest.RemovePartitions([partition]);
        
        partition.Documentation.Text = "goodbye";
        
        Assert.AreEqual(4, composite.Parts.Count);
        Assert.AreEqual(0, counter.Count);

        compositor.Pop(true);
        Assert.AreEqual(1, counter.Count);
    }

    [TestMethod]
    public void ComposeForest_twoPartitions_oneComposite()
    {
        var forest = new Forest();

        var compositor = new NotificationCompositor("compositor");
        forest.GetNotificationHandler().ConnectTo(compositor);
        
        var partitionA = new Geometry("partitionA");
        // outside counter
        forest.AddPartitions([partitionA]);

        var counter = new ForestEventCounter();
        compositor.ConnectTo(counter);
        
        var partitionB = new Geometry("partitionB");
        // inside counter, outside composite
        forest.AddPartitions([partitionB]);

        var composite = compositor.Push();
        
        partitionA.Documentation = new Documentation("documentation");
        partitionA.Documentation.Text = "hello";
        partitionB.AddShapes([new Circle("c")]);
        
        Assert.AreEqual(3, composite.Parts.Count);
        Assert.AreEqual(1, counter.Count);

        compositor.Pop(true);
        Assert.AreEqual(2, counter.Count);
    }
    
    [TestMethod]
    public void ComposeForest_twoPartitions_twoComposites()
    {
        var forest = new Forest();

        var compositor = new NotificationCompositor("compositor");
        forest.GetNotificationHandler().ConnectTo(compositor);
        
        var counter = new ForestEventCounter();
        compositor.ConnectTo(counter);
        
        var compositeA = compositor.Push();
        
        var partitionA = new Geometry("partitionA");
        // compositeA[0]
        forest.AddPartitions([partitionA]);

        var partitionB = new Geometry("partitionB");
        // compositeA[1]
        forest.AddPartitions([partitionB]);

        // compositeA[2]
        partitionA.Documentation = new Documentation("documentation");
        
        var compositeB = new CompositeNotification(new NumericNotificationId("manual", 0));
        // compositeA[3]
        var compositeBPushed = compositor.Push(compositeB);
        Assert.AreSame(compositeB, compositeBPushed);
        
        // compositeB[0]
        forest.RemovePartitions([partitionA]);
        
        // NOT in forest anymore 
        partitionA.Documentation.Text = "hello";
        
        // compositeB[1]
        partitionB.AddShapes([new Circle("cA")]);
        
        var compositeBPopped = compositor.Pop(true);
        Assert.AreSame(compositeB, compositeBPopped);
       
        // compositeA[4]
        partitionB.AddShapes([new Circle("cB")]);
        
        Assert.AreEqual(5, compositeA.Parts.Count);
        Assert.AreEqual(1, counter.Count);

        Assert.AreEqual(2, compositeB.Parts.Count);

        compositor.Pop(true);
        
        // no composite
        forest.AddPartitions([partitionA]);
        Assert.AreEqual(3, counter.Count);
    }
}

internal class ForestEventCounter() : NotificationHandlerBase(null), IReceivingNotificationHandler
{
    public int Count { get; private set; }
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification) => 
        Count++;
}

internal class PartitionEventCounter() : NotificationHandlerBase(null), IReceivingNotificationHandler
{
    public int Count { get; private set; }
    public void Receive(ISendingNotificationHandler correspondingHandler, INotification notification) => 
        Count++;
}