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
    /// <summary>
    /// According to spec, all descendants in an added node MUST be new (i.e. not present in the model).
    /// This test case is legal in api but results in delta error because the existingChild is attempted to be added again
    /// to the model 
    /// </summary>
    [TestMethod]
    public void AddChildWithNewChildContainingExistingNode()
    {
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
        
        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            originalPartition.Containment_1 = replacement;
        });
        
        // changes applied to original partition
        Assert.AreSame(replacement, originalPartition.Containment_1);
        Assert.IsNull(existingParent.Containment_0_1);
        
        // change has not replicated to the clone
        Assert.ThrowsExactly<UnsetFeatureException>(() => clonedPartition.Containment_1);
        Assert.AreEqual("existingChild", clonedPartition.Containment_0_1!.Containment_0_1!.GetId());
    }
    
    
    [TestMethod]
    public void AddChildWithNewChildContainingExistingNode_with_PartitionAdded()
    {
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
        
        originalForest.AddPartitions([originalPartition]);

        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            originalPartition.Containment_1 = replacement;
        });
        
        // changes applied to original partition
        Assert.AreSame(replacement, originalPartition.Containment_1);
        Assert.IsNull(existingParent.Containment_0_1);
        
        // change has not replicated to the clone
        Assert.ThrowsExactly<UnsetFeatureException>(() => clonedPartition.Containment_1);
        Assert.AreEqual("existingChild", clonedPartition.Containment_0_1!.Containment_0_1!.GetId());
    }
    
    [TestMethod]
    [Ignore("This test fails because child deletion in remote replicator uses DetachFromParent")]
    public void RemoveExistingChild_and_AddExistingChildAgain()
    {
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
        
        // add partition to the forest
        originalForest.AddPartitions([originalPartition]);

        // remove existing Child
        existingParent.Containment_0_1 = null;
        
        // add removed existing child back to partition
        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        originalPartition.Containment_1 = replacement;
        
        AssertEquals(clonedForest.Partitions, originalForest.Partitions);
    }
    
    /// <summary>
    /// Model is valid after replacement
    /// </summary>
    [TestMethod]
    public void ReplacedChildWithReplacementContainingChildOfReplacedNode()
    {
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
        
        replaced.ReplaceWith(replacement);
        
        AssertEquals([originalPartition], [clonedPartition]);
    }
    
    
    /// <summary>
    /// Model is valid after replacement
    /// </summary>
    [TestMethod]
    public void ReplacedChildWithReplacementContainingChildOfReplacedNode_with_PartitionAdded()
    {
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
        
        originalForest.AddPartitions([originalPartition]);

        var replacement = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        replaced.ReplaceWith(replacement);
        
        AssertEquals(originalForest.Partitions, clonedForest.Partitions);
    }

    /// <summary>
    /// existingChild is part of another partition in the same forest.
    /// Therefore, it is considered as a known node (not a new node).
    /// </summary>
    [TestMethod]
    public void AddChildWithNewChildContainingExistingNode_FromOtherPartition()
    {
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
        
        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            changedPartition.Containment_1 = replacement;
        });
        
        // changes applied to original partition
        Assert.AreSame(replacement, changedPartition.Containment_1);
        Assert.IsNull(existingParent.Containment_0_1);
        
        // change has not replicated to the clone
        Assert.ThrowsExactly<UnsetFeatureException>(() => ((LinkTestConcept)clonedForest.Partitions.ToList()[1]).Containment_1);
    }

    /// <summary>
    /// existingChild is part of another (free floating) partition which is not in the same forest.
    /// Therefore, it is considered as a new node.
    /// </summary>
    [TestMethod]
    public void AddChildWithNewChildContainingExistingNode_FromFreeFloatingPartition()
    {
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
        
        changedPartition.Containment_1 = replacement;
        
        AssertEquals(clonedForest.Partitions, originalForest.Partitions);
    }

    [TestMethod]
    [Ignore("All nodes must have unique ids, not checked (yet) for partitions")]
    public void AddExistingPartition()
    {
        var partition = new LinkTestConcept("partition");
        
        var originalForest = new Forest();
        
        originalForest.AddPartitions([partition]);
        
        CreateDeltaReplicator(originalForest);
        
        Assert.ThrowsExactly<InvalidNotificationException>(() =>
        {
            originalForest.AddPartitions([partition]);
        });
    }
    
    //TODO: add tests with forest for replaced child case
    
}