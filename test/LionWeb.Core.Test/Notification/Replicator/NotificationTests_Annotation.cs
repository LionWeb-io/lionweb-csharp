﻿// Copyright 2025 TRUMPF Laser SE and other contributors
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
using Languages.Generated.V2025_1.Shapes.M2;
using M1;

[TestClass]
public class NotificationTests_Annotation: ReplicatorTestsBase
{
    //TODO: misses tests for MoveAndReplaceAnnotationFromOtherParent
    // Requires implementation of emitter logic and support in replicator.
    
    #region Annotations

    #region AnnotationAdded

    [TestMethod]
    public void AnnotationAdded_Multiple_Only()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new BillOfMaterials("added");
        originalPartition.AddAnnotations([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.GetAnnotations()[0]);
    }

    [TestMethod]
    public void AnnotationAdded_Multiple_First()
    {
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof")]);
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new BillOfMaterials("added");
        originalPartition.InsertAnnotations(0, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.GetAnnotations()[0]);
    }

    [TestMethod]
    public void AnnotationAdded_Multiple_Last()
    {
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof")]);
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new BillOfMaterials("added");
        originalPartition.InsertAnnotations(1, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.GetAnnotations()[1]);
    }

    [TestMethod]
    public void AnnotationAdded_Deep()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new BillOfMaterials("added") { AltGroups = [new MaterialGroup("mg") { MatterState = MatterState.gas }] };
        originalPartition.AddAnnotations([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.GetAnnotations()[0]);
    }

    #endregion

    #region AnnotationDeleted

    [TestMethod]
    public void AnnotationDeleted_Multiple_Only()
    {
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([deleted]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_First()
    {
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([deleted, new BillOfMaterials("bof")]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_Last()
    {
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof"), deleted]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    #endregion

    #region AnnotationReplaced
    
    #region tests uses ReplaceWith method

    [TestMethod]
    public void AnnotationReplaced_Multiple_Only_uses_ReplaceWith()
    {
        var replacement = new BillOfMaterials("replacement");
        var replaced = new BillOfMaterials("replaced");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([replaced]);
        
        var clonedPartition = ClonePartition(originalPartition);
        
        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(replacement);

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void AnnotationReplaced_Multiple_First_uses_ReplaceWith()
    {
        var replacement = new BillOfMaterials("replacement");
        var replaced = new BillOfMaterials("replaced");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([replaced, new BillOfMaterials("bof")]);
        
        var clonedPartition = ClonePartition(originalPartition);
        
        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(replacement);

        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void AnnotationReplaced_Multiple_Last_uses_ReplaceWith()
    {
        var replacement = new BillOfMaterials("replacement");
        var replaced = new BillOfMaterials("replaced");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof"), replaced]);
        
        var clonedPartition = ClonePartition(originalPartition);
        
        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(replacement);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    #endregion
    
    [TestMethod]
    public void AnnotationReplaced_Multiple_Only()
    {
        var replaced = new BillOfMaterials("replaced");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([replaced]);
        
        var clonedPartition = ClonePartition(originalPartition);
        
        var newAnnotation = new BillOfMaterials("new");
        var annotationReplacedNotification = new AnnotationReplacedNotification(newAnnotation, replaced, originalPartition, 
            0, new NumericNotificationId("annotationReplaced", 0));
        
        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(1, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(newAnnotation.GetId(), clonedPartition.GetAnnotations()[0].GetId());
    }

    [TestMethod]
    public void AnnotationReplaced_Multiple_First()
    {
        var replaced = new BillOfMaterials("replaced");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([replaced, new BillOfMaterials("bof")]);
        
        var clonedPartition = ClonePartition(originalPartition);
        
        var newAnnotation = new BillOfMaterials("new");
        var annotationReplacedNotification = new AnnotationReplacedNotification(newAnnotation, replaced, originalPartition, 
            0, new NumericNotificationId("annotationReplaced", 0));
        
        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(2, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(newAnnotation.GetId(), clonedPartition.GetAnnotations()[0].GetId());
    }

    [TestMethod]
    public void AnnotationReplaced_Multiple_Last()
    {
        var replaced = new BillOfMaterials("replaced");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof"), replaced]);
        
        var clonedPartition = ClonePartition(originalPartition);
        
        var index = 1;
        var newAnnotation = new BillOfMaterials("new");
        var annotationReplacedNotification = new AnnotationReplacedNotification(newAnnotation, replaced, originalPartition, 
            index, new NumericNotificationId("annotationReplaced", 0));
        
        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(2, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(newAnnotation.GetId(), clonedPartition.GetAnnotations()[index].GetId());
    }
    
    #endregion

    #region AnnotationMovedFromOtherParent

    [TestMethod]
    public void AnnotationMovedFromOtherParent_Multiple()
    {
        var moved = new BillOfMaterials("moved");
        var origin = new CompositeShape("origin");
        origin.AddAnnotations([moved]);
        var originalPartition = new Geometry("a") { Shapes = [origin] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.AddAnnotations([moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    #endregion

    #region AnnotationMovedInSameParent

    [TestMethod]
    public void AnnotationMovedInSameParent_Forward()
    {
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([moved, new BillOfMaterials("bof")]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.AddAnnotations([moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void AnnotationMovedInSameParent_Backward()
    {
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof"), moved]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.InsertAnnotations(0, [moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    #endregion
    
    #region AnnotationMovedAndReplacedInSameParent

    #region tests uses ReplaceWith method

    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent_Forward_uses_ReplaceWith()
    {
        var replaced = new BillOfMaterials("replaced");
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([moved, replaced]);
        
        var clonedPartition = ClonePartition(originalPartition);
        
        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(moved);
        
        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent_Backward_uses_ReplaceWith()
    {
        var replaced = new BillOfMaterials("replaced");
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([replaced, moved]);

        var clonedPartition = ClonePartition(originalPartition);
        
        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(moved);
        
        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    
     [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent_Backward_MoreThanThreeNodes_uses_ReplaceWith()
    {
        var replaced = new BillOfMaterials("replaced");
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("A"), replaced, new BillOfMaterials("B"), moved, new BillOfMaterials("C")]);
        
        var clonedPartition = ClonePartition(originalPartition);
        
        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(moved);
        
        AssertEquals([originalPartition], [clonedPartition]);
    }
      
    
    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent_Forward_MoreThanThreeNodes_uses_ReplaceWith()
    {
        var replaced = new BillOfMaterials("replaced");
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("A"), moved, new BillOfMaterials("B"), replaced, new BillOfMaterials("C")]);
        
        var clonedPartition = ClonePartition(originalPartition);
        
        CreatePartitionReplicator(clonedPartition, originalPartition);

        replaced.ReplaceWith(moved);
        
        AssertEquals([originalPartition], [clonedPartition]);
    }
    

    #endregion

    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent_Forward()
    {
        var replaced = new BillOfMaterials("replaced");
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([moved, replaced]);
        
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 0;
        var annotationReplacedNotification = new AnnotationMovedAndReplacedInSameParentNotification(newIndex, moved, originalPartition,oldIndex, 
            replaced, new NumericNotificationId("annotationMovedAndReplaced", 0));
        
        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(1, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.GetAnnotations()[0].GetId());
    }
    
    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent_Backward()
    {
        var replaced = new BillOfMaterials("replaced");
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([replaced, moved]);
        
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 0;
        var oldIndex = 1;
        var annotationReplacedNotification = new AnnotationMovedAndReplacedInSameParentNotification(newIndex, moved, originalPartition,oldIndex, 
            replaced, new NumericNotificationId("annotationMovedAndReplaced", 0));
        
        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(1, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.GetAnnotations()[0].GetId());
    }
    
    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent_Backward_MoreThanThreeNodes()
    {
        var replaced = new BillOfMaterials("replaced");
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("A"), replaced, new BillOfMaterials("B"), moved, new BillOfMaterials("C")]);
        
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 1;
        var oldIndex = 3;
        var annotationReplacedNotification = new AnnotationMovedAndReplacedInSameParentNotification(newIndex, moved, originalPartition,oldIndex, 
            replaced, new NumericNotificationId("annotationMovedAndReplaced", 0));
        
        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(4, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.GetAnnotations()[1].GetId());
    }
      
    
    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent_Forward_MoreThanThreeNodes()
    {
        var replaced = new BillOfMaterials("replaced");
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("A"), moved, new BillOfMaterials("B"), replaced, new BillOfMaterials("C")]);
        
        var clonedPartition = ClonePartition(originalPartition);

        var newIndex = 3;
        var oldIndex = 1;
        var annotationReplacedNotification = new AnnotationMovedAndReplacedInSameParentNotification(newIndex, moved, originalPartition,oldIndex, 
            replaced, new NumericNotificationId("annotationMovedAndReplaced", 0));
        
        CreatePartitionReplicator(clonedPartition, annotationReplacedNotification);

        Assert.AreEqual(4, clonedPartition.GetAnnotations().Count);
        Assert.AreEqual(moved.GetId(), clonedPartition.GetAnnotations()[2].GetId());
    }
    

    #endregion

    #endregion
}