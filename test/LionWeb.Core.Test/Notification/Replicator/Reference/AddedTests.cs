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

namespace LionWeb.Core.Test.Notification.Replicator.Reference;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class AddedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
    {
        var bof = new LinkTestConcept("bof");
        var line = new LinkTestConcept("line");
        var originalPartition = new TestPartition("a") { Links =  [line] };
        originalPartition.AddLinks([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.AddReference_0_n([line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_First()
    {
        var circle = new LinkTestConcept("circle");
        var bof = new LinkTestConcept("bof") { Reference_0_n =  [circle] };
        var line = new LinkTestConcept("line");
        var originalPartition = new TestPartition("a") { Links =  [line, circle] };
        originalPartition.AddLinks([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.InsertReference_0_n(0, [line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var circle = new LinkTestConcept("circle");
        var bof = new LinkTestConcept("bof") { Reference_0_n =  [circle] };
        var line = new LinkTestConcept("line");
        var originalPartition = new TestPartition("a") { Links =  [line, circle] };
        originalPartition.AddLinks([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.InsertReference_0_n(1, [line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Single()
    {
        var circle = new LinkTestConcept("circle");
        var od = new LinkTestConcept("od");
        var originalPartition = new TestPartition("a") { Links =  [od, circle] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        od.Reference_0_1 = circle;

        AssertEquals([originalPartition], [clonedPartition]);

        var clonedOffsetDuplicate = clonedPartition.Links[0];
        var clonedCircle = clonedPartition.Links[1];
        Assert.AreSame(clonedCircle, clonedOffsetDuplicate.Reference_0_1);
    }
}