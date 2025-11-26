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

namespace LionWeb.Core.Test.Notification.Replicator.Containment;

using Languages.Generated.V2025_1.Shapes.M2;
using Languages.Generated.V2025_1.TestLanguage;

[TestClass]
public class MovedFromOtherContainmentInSameParentTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        origin.AddDisabledParts([moved]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Single()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        origin.EvilPart = moved;

        AssertEquals([originalPartition], [clonedPartition]);
    }


    [TestMethod]
    public void Reverse_first_moves_required_containment()
    {
        var moved1 = new LinkTestConcept("moved1");
        var moved2 = new LinkTestConcept("moved2");
        var originalPartition = new LinkTestConcept("a")
        {
            Containment_0_1 = moved1,
            Containment_1 = moved2,
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.Containment_0_1 = moved2; // emits ChildMovedAndReplacedFromOtherContainmentInSameParentNotification
        originalPartition.Containment_1 = moved1; // emits ChildAddedNotification

        AssertEquals([originalPartition], [clonedPartition]);
    }


    [TestMethod]
    public void Reverse_first_moves_optional_containment()
    {
        var moved1 = new LinkTestConcept("moved1");
        var moved2 = new LinkTestConcept("moved2");
        var originalPartition = new LinkTestConcept("a")
        {
            Containment_0_1 = moved1,
            Containment_1 = moved2,
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        originalPartition.Containment_1 = moved1; // emits ChildMovedAndReplacedFromOtherContainmentInSameParentNotification
        originalPartition.Containment_0_1 = moved2; // emits ChildAddedNotification

        AssertEquals([originalPartition], [clonedPartition]);
    }
}