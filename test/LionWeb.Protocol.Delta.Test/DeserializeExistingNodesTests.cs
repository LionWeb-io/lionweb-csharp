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
    /// legal in api, error in delta: new child already exists
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

        Assert.ThrowsExactly<DuplicateNodeIdException>(() =>
        {
            originalPartition.Containment_1 = new LinkTestConcept("replacement")
            {
                Containment_0_1 = existingChild
            };
        });
        
        // AssertEquals([originalPartition], [clonedPartition]);
    }
    
    /// <summary>
    /// should work: model is valid after replacement
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
        
        replaced.ReplaceWith(new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        });
        
        AssertEquals([originalPartition], [clonedPartition]);
    }

    /// <summary>
    /// should fail: child is not new
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
        
        Assert.ThrowsExactly<DuplicateNodeIdException>(() =>
        {
            changedPartition.Containment_1 = new LinkTestConcept("replacement")
            {
                Containment_0_1 = existingChild
            };
        });
    }

    /// <summary>
    /// should succeed: child from free floating partitions are new
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
        
        changedPartition.Containment_1 = new LinkTestConcept("replacement")
        {
            Containment_0_1 = existingChild
        };
        
        AssertEquals(clonedForest.Partitions, originalForest.Partitions);
    }

    
    //TODO: I can add tests with forest with replaced child
    
}