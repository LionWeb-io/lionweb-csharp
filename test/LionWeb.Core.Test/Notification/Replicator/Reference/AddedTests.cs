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

using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class AddedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
    {
        var bof = new BillOfMaterials("bof");
        var line = new Line("line");
        var originalPartition = new Geometry("a") { Shapes = [line] };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.AddMaterials([line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_First()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.InsertMaterials(0, [line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        bof.InsertMaterials(1, [line]);

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od");
        var originalPartition = new Geometry("a") { Shapes = [od, circle] };

        var clonedPartition = ClonePartition(originalPartition);
        CreatePartitionReplicator(clonedPartition, originalPartition);

        od.Source = circle;

        AssertEquals([originalPartition], [clonedPartition]);

        var clonedOffsetDuplicate = (OffsetDuplicate)clonedPartition.Shapes[0];
        var clonedCircle = (Circle)clonedPartition.Shapes[1];
        Assert.AreSame(clonedCircle, clonedOffsetDuplicate.Source);
    }
}