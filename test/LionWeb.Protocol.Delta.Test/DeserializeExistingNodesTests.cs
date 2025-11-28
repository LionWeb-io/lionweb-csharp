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

namespace LionWeb.Protocol.Delta.Test;

using Core;
using Core.M1;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class DeserializeExistingNodesTests: DeltaTestsBase
{
    #region attempt to add an existing child

    /// <summary>
    /// According to spec, all descendants in an added node MUST be new (i.e. not present in the model).
    /// This test case is legal in api but results in delta error because the existingChild is attempted
    /// to be added (as a new node) again to the model 
    /// </summary>
    [TestMethod]
    public void AddChildWithNewChildContainingExistingNode()
    {
        // Arrange
        var existingChild = new LinkTestConcept("existingChild");
        
        var existingParent = new LinkTestConcept("existingParent")
        {
            Containment_0_1 = existingChild 
        };
        
        var originalPartition = new LinkTestConcept("originalPartition")
        {
            Containment_0_1 = existingParent
        };

        var clonedPartition = CreateDeltaReplicator(originalPartition);

        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        // Act
        // This emits a ChildAddedNotification, "replacement" is a new node but its child "existingChild" is not.
        // That's what makes the ChildAddedNotification "invalid".
        var applyReplacement = () =>
        {
            originalPartition.Containment_1 = replacement;
        };
        
        // Assert
        Assert.ThrowsExactly<InvalidNotificationException>(applyReplacement);
        
        // changes applied to original partition
        // (node api maintains tree shape: moves existingChild from Containment_0_1 to Containment_1)
        Assert.AreSame(replacement, originalPartition.Containment_1);
        Assert.IsNull(existingParent.Containment_0_1);
        
        // change has not replicated to the clone
        Assert.IsFalse(clonedPartition.TryGetContainment_1(out LinkTestConcept? _));
        Assert.AreEqual("existingChild", clonedPartition.Containment_0_1!.Containment_0_1!.GetId());
    }
    
    [TestMethod]
    public void AddChildWithNewChildContainingExistingNode_with_PartitionAdded()
    {
        // Arrange
        var existingChild = new LinkTestConcept("existingChild");
        
        var existingParent = new LinkTestConcept("existingParent")
        {
            Containment_0_1 = existingChild 
        };
        
        var originalPartition = new LinkTestConcept("originalPartition")
        {
            Containment_0_1 = existingParent
        };

        var originalForest = new Forest();
        
        var clonedPartition = CreateDeltaReplicator(originalPartition);
        
        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        // Act
        originalForest.AddPartitions([originalPartition]);
        
        var applyReplacement = () =>
        {
            originalPartition.Containment_1 = replacement;
        };
        
        // Assert
        Assert.ThrowsExactly<InvalidNotificationException>(applyReplacement);
        
        // Assert
        // changes applied to original partition
        Assert.AreSame(replacement, originalPartition.Containment_1);
        Assert.IsNull(existingParent.Containment_0_1);
        
        // change has not replicated to the clone
        Assert.IsFalse(clonedPartition.TryGetContainment_1(out LinkTestConcept? _));
        Assert.AreEqual("existingChild", clonedPartition.Containment_0_1!.Containment_0_1!.GetId());
    }
    
    [TestMethod]
    public void RemoveExistingChild_and_AddExistingChildAgain()
    {
        // Arrange
        var existingChild = new LinkTestConcept("existingChild");
        
        var existingParent = new LinkTestConcept("existingParent")
        {
            Containment_0_1 = existingChild 
        };
        
        var originalPartition = new LinkTestConcept("originalPartition")
        {
            Containment_0_1 = existingParent
        };

        var originalForest = new Forest();
        
        var clonedForest = CreateDeltaReplicator(originalForest);
        
        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };

        // Act
        // add partition to the forest
        originalForest.AddPartitions([originalPartition]);

        // remove existing child
        existingParent.Containment_0_1 = null;

        // add removed existing child back to partition
        originalPartition.Containment_1 = replacement;
        
        // Assert 
        AssertEquals(clonedForest.Partitions, originalForest.Partitions);
    }
    
    /// <summary>
    /// existingChild is part of another partition in the same forest.
    /// Therefore, it is considered as a known node (not a new node).
    /// </summary>
    [TestMethod]
    public void AddChildWithNewChildContainingExistingNode_FromOtherPartition()
    {
        // Arrange
        var existingChild = new LinkTestConcept("existingChild");
        
        var existingParent = new LinkTestConcept("existingParent")
        {
            Containment_0_1 = existingChild 
        };
        
        var otherPartition = new LinkTestConcept("otherPartition")
        {
            Containment_0_1 = existingParent
        };

        var changedPartition = new LinkTestConcept("changedPartition");

        var originalForest = new Forest();
        originalForest.AddPartitions([otherPartition, changedPartition]);
        
        var clonedForest = CreateDeltaReplicator(originalForest);
        
        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };

        // Act
        var applyReplacement = () =>
        {
            changedPartition.Containment_1 = replacement;
        };
        
        // Assert
        Assert.ThrowsExactly<InvalidNotificationException>(applyReplacement);
        
        // changes applied to original partition
        Assert.AreSame(replacement, changedPartition.Containment_1);
        Assert.IsNull(existingParent.Containment_0_1);
        
        // change has not replicated to the clone
        Assert.IsFalse(((LinkTestConcept)clonedForest.Partitions.ToList()[1]).TryGetContainment_1(out LinkTestConcept? _));
    }

    /// <summary>
    /// existingChild is part of another (free floating) partition which is not in the same forest.
    /// Therefore, it is considered as a new node.
    /// </summary>
    [TestMethod]
    public void AddChildWithNewChildContainingExistingNode_FromFreeFloatingPartition()
    {
        // Arrange
        var existingChild = new LinkTestConcept("existingChild");
        
        var existingParent = new LinkTestConcept("existingParent")
        {
            Containment_0_1 = existingChild 
        };
        
        var otherPartition = new LinkTestConcept("otherPartition")
        {
            Containment_0_1 = existingParent
        };

        var changedPartition = new LinkTestConcept("changedPartition");

        var originalForest = new Forest();
        originalForest.AddPartitions([changedPartition]);
        
        var clonedForest = CreateDeltaReplicator(originalForest);

        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        // Act
        changedPartition.Containment_1 = replacement;
        
        // Assert
        AssertEquals(clonedForest.Partitions, originalForest.Partitions);
    }
    
    #endregion
    
    #region attempt to replace an existing child   
    
    /// <summary>
    /// Model is valid after replacement
    /// </summary>
    [TestMethod]
    public void ReplacedChildWithReplacementContainingChildOfReplacedNode()
    {
        // Arrange
        var existingChild = new LinkTestConcept("existingChild");
        
        var replaced = new LinkTestConcept("replaced")
        {
            Containment_0_1 = existingChild 
        };
        
        var originalPartition = new LinkTestConcept("originalPartition")
        {
            Containment_0_1 = replaced
        };
        
        var clonedPartition = CreateDeltaReplicator(originalPartition);

        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        // Act
        replaced.ReplaceWith(replacement);
        
        // Assert
        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    
    /// <summary>
    /// Model is valid after replacement
    /// </summary>
    [TestMethod]
    public void ReplacedChildWithReplacementContainingChildOfReplacedNode_with_PartitionAdded()
    {
        // Arrange
        var existingChild = new LinkTestConcept("existingChild");
        
        var replaced = new LinkTestConcept("replaced")
        {
            Containment_0_1 = existingChild 
        };
        
        var originalPartition = new LinkTestConcept("originalPartition")
        {
            Containment_0_1 = replaced
        };
        
        var originalForest = new Forest();
        
        var clonedForest = CreateDeltaReplicator(originalForest);

        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        // Act
        originalForest.AddPartitions([originalPartition]);
        replaced.ReplaceWith(replacement);
        
        // Assert
        AssertEquals(originalForest.Partitions, clonedForest.Partitions);
    }
    
    
    /// <summary>
    /// existingChild is part of another partition in the same forest.
    /// Model is valid after replacement.
    /// </summary>
    [TestMethod]
    public void ReplacedChildWithReplacementContainingChildOfReplacedNode_FromOtherPartition()
    {
        // Arrange
        var existingChild = new LinkTestConcept("existingChild");
        
        var replaced = new LinkTestConcept("replaced")
        {
            Containment_0_1 = existingChild 
        };
        
        var otherPartition = new LinkTestConcept("originalPartition")
        {
            Containment_0_1 = replaced
        };
        
        var changedPartition = new LinkTestConcept("changedPartition");
        
        var originalForest = new Forest();
        originalForest.AddPartitions([otherPartition, changedPartition]);
        
        var clonedForest = CreateDeltaReplicator(originalForest);

        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        // Act
        replaced.ReplaceWith(replacement);
        
        // Assert
        AssertEquals(originalForest.Partitions, clonedForest.Partitions);
    }
    
    /// <summary>
    /// existingChild is part of another (free floating) partition which is not in the same forest.
    /// Model is valid after replacement.
    /// </summary>
    [TestMethod]
    public void ReplacedChildWithReplacementContainingChildOfReplacedNode_FromFreeFloatingPartition()
    {
        // Arrange
        var existingChild = new LinkTestConcept("existingChild");
        
        var replaced = new LinkTestConcept("replaced")
        {
            Containment_0_1 = existingChild 
        };
        
        var otherPartition = new LinkTestConcept("originalPartition")
        {
            Containment_0_1 = replaced
        };
        
        var changedPartition = new LinkTestConcept("changedPartition");
        
        var originalForest = new Forest();
        originalForest.AddPartitions([changedPartition]);
        
        var clonedForest = CreateDeltaReplicator(originalForest);

        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        // Act
        replaced.ReplaceWith(replacement);
        
        // Assert
        AssertEquals(originalForest.Partitions, clonedForest.Partitions);
    }
    
    
    #endregion

    #region attempt to add/replace an existing annotation

    [TestMethod]
    public void AddNewAnnotationAnnotatedWithExistingAnnotation()
    {
        // Arrange
        var existingParentAnnotation = new TestAnnotation("existingParentAnnotation");
        
        var existingChildAnnotation = new TestAnnotation("existingChildAnnotation");
        existingParentAnnotation.AddAnnotations([existingChildAnnotation]);
        
        var child0 = new LinkTestConcept("child0");
        child0.AddAnnotations([existingParentAnnotation]);

        var child1 = new LinkTestConcept("child1");
        var originalPartition = new LinkTestConcept("originalPartition")
        {
            Containment_0_1 = child0,
            Containment_1 = child1
        };

        var clonedPartition = CreateDeltaReplicator(originalPartition);

        var newParentAnnotation = new TestAnnotation("newAnnotation");
        newParentAnnotation.AddAnnotations([existingChildAnnotation]);
        
        // Act
        var addAnnotation = () =>
        {
            child1.AddAnnotations([newParentAnnotation]);
        };
        
        // Assert
        Assert.ThrowsExactly<InvalidNotificationException>(addAnnotation);
        
        // changes applied to original partition
        Assert.HasCount(1,originalPartition.Containment_0_1.GetAnnotations());
        Assert.AreEqual("existingParentAnnotation", originalPartition.Containment_0_1.GetAnnotations()[0].GetId());
        Assert.HasCount(1, originalPartition.Containment_1.GetAnnotations());
        Assert.HasCount(1, originalPartition.Containment_1.GetAnnotations()[0].GetAnnotations());
        Assert.AreEqual("existingChildAnnotation", originalPartition.Containment_1.GetAnnotations()[0].GetAnnotations()[0].GetId());
        
        // change has not replicated to the clone
        Assert.HasCount(1,clonedPartition.Containment_0_1!.GetAnnotations());
        Assert.IsEmpty(clonedPartition.Containment_1.GetAnnotations());
    }

    
    [TestMethod]
    public void ReplaceAnnotationWithAnnotationContainingExistingAnnotation()
    {
        // Arrange
        var existingParentAnnotation = new TestAnnotation("existingParentAnnotation");
        
        var existingChildAnnotation = new TestAnnotation("existingChildAnnotation");
        existingParentAnnotation.AddAnnotations([existingChildAnnotation]);
        
        var child = new LinkTestConcept("child");
        child.AddAnnotations([existingParentAnnotation]);
        
        var originalPartition = new LinkTestConcept("originalPartition")
        {
            Containment_0_1 = child
        };
        
        var newParentAnnotation = new TestAnnotation("newAnnotation");
        newParentAnnotation.AddAnnotations([existingChildAnnotation]);
        
        var clonedPartition = CreateDeltaReplicator(originalPartition);
        
        // Act
        child.GetAnnotations()[0].ReplaceWith(newParentAnnotation);
        
        // Assert
        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    #endregion
    
    [TestMethod]
    [Ignore("All nodes must have unique ids, not checked (yet) for partitions")]
    public void AddExistingPartition()
    {
        // Arrange
        var partition = new LinkTestConcept("partition");
        
        var originalForest = new Forest();
        
        originalForest.AddPartitions([partition]);
        
        CreateDeltaReplicator(originalForest);

        // Act
        var addPartition = () =>
        {
            originalForest.AddPartitions([partition]);
        };
        
        // Assert 
        Assert.ThrowsExactly<InvalidNotificationException>(addPartition);
    }
}