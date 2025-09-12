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

using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class NotificationTests_Forest : NotificationTestsBase
{
    #region Partition

    [TestMethod]
    public void PartitionAdded()
    {
        var node = new Geometry("a");
        var originalForest = new Forest();
        var cloneForest = new Forest();
        
        CreateForestReplicator(cloneForest, originalForest);

        originalForest.AddPartitions([node]);

        AssertEquals([node], cloneForest.Partitions);
    }
    
    [TestMethod]
    public void PartitionDeleted()
    {
        var node = new Geometry("a");
        var originalForest = new Forest();
        originalForest.AddPartitions([node]);
        
        var cloneForest = new Forest();
        cloneForest.AddPartitions([node]);
        
        CreateForestReplicator(cloneForest, originalForest);

        originalForest.RemovePartitions([node]);

        Assert.IsEmpty(cloneForest.Partitions);
        Assert.IsEmpty(originalForest.Partitions);
        
    }

    #endregion
    
    #region Children
    
    #region ChildMovedFromOtherContainment
    
    [TestMethod]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_Works()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var node = new Geometry("a") { Shapes = [origin] };

        var originalForest = new Forest();
        var cloneForest = new Forest();
        
        CreateForestReplicator(cloneForest, originalForest);

        originalForest.AddPartitions([node]);
        node.AddShapes([moved]);

        AssertEquals([node], cloneForest.Partitions);
    }
    
    [TestMethod]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_Destination_Works()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originPartition = new Geometry("g") { Shapes = [origin] };

        var node = new Geometry("a") { Shapes = [] };

        var originalForest = new Forest();
        originalForest.AddPartitions([originPartition]);

        var cloneForest = new Forest();
        
        CreateForestReplicator(cloneForest, originalForest);

        originalForest.AddPartitions([node]);
        node.AddShapes([moved]);

        AssertEquals([node], cloneForest.Partitions);
    }
   
   
    [TestMethod]
    public void ChildMovedFromOtherContainment_AddBeforeSubscribe_CloneExists_NotReplicated()
    {
        var moved = new Documentation("moved");
        var node = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var originalForest = new Forest();
        originalForest.AddPartitions([node]);

        var clone = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = new Documentation("moved") }] };

        var cloneForest = new Forest();
        cloneForest.AddPartitions([clone]);
        
        CreateForestReplicator(cloneForest, originalForest);
        
        node.Documentation = moved;

        Assert.AreEqual(node, moved.GetParent());
        Assert.IsNotNull(clone.Shapes.OfType<Shape>().First().ShapeDocs);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_AddAfterSubscribe_DifferentPartitions_Works()
    {
        var moved = new Documentation("moved");
        var originPartition = new Geometry("g") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var node = new Geometry("a") { };

        var originalForest = new Forest();
        var cloneForest = new Forest();
        
        CreateForestReplicator(cloneForest, originalForest);

        originalForest.AddPartitions([node, originPartition]);
        node.Documentation = moved;

        AssertEquals([node, originPartition], cloneForest.Partitions.OrderBy(p => p.GetId()).ToList());
    }

    #endregion

    #endregion
    
}