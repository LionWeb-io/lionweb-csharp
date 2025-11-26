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

namespace LionWeb.Core.Test.Notification.Replicator.Annotation;

using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class AddedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
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
    public void Multiple_First()
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
    public void Multiple_Last()
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
    public void Deep()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new BillOfMaterials("added") { AltGroups = [new MaterialGroup("mg") { MatterState = MatterState.gas }] };
        originalPartition.AddAnnotations([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.GetAnnotations()[0]);
    }
}