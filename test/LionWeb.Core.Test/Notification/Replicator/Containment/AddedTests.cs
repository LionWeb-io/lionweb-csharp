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

[TestClass]
public class AddedTests : ReplicatorTestsBase
{
    [TestMethod]
    public void Multiple_Only()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added");
        originalPartition.AddShapes([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[0]);
    }

    [TestMethod]
    public void Multiple_adds_one_node_using_generic_api()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added");
        originalPartition.Add(ShapesLanguage.Instance.Geometry_shapes, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[0]);
    }

    [TestMethod]
    public void Multiple_adds_multiple_nodes_using_generic_api()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added1 = new Circle("added1");
        var added2 = new Circle("added2");
        originalPartition.Add(ShapesLanguage.Instance.Geometry_shapes, [added1, added2]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added2, clonedPartition.Shapes[0]);
    }

    [TestMethod]
    public void Multiple_sets_one_node_using_set()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added");
        var values = new IShape[] { added };

        originalPartition.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[0]);
    }

    [TestMethod]
    public void Multiple_sets_multiple_nodes_using_set()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added1 = new Circle("added1");
        var added2 = new Circle("added2");
        var values = new IShape[] { added1, added2 };

        originalPartition.Set(ShapesLanguage.Instance.Geometry_shapes, values);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added2, clonedPartition.Shapes[0]);
    }

    [TestMethod]
    public void Multiple_First()
    {
        var originalPartition = new Geometry("a") { Shapes = [new Line("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added");
        originalPartition.InsertShapes(0, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[0]);
    }

    [TestMethod]
    public void Multiple_Last()
    {
        var originalPartition = new Geometry("a") { Shapes = [new Line("l")] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added");
        originalPartition.InsertShapes(1, [added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[1]);
    }

    [TestMethod]
    public void Single()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Documentation("added");
        originalPartition.Documentation = added;

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Documentation);
    }

    [TestMethod]
    public void Deep()
    {
        var originalPartition = new Geometry("a");
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        var added = new Circle("added") { Center = new Coord("coord") { X = 1, Y = 2, Z = 3 } };
        originalPartition.AddShapes([added]);

        AssertEquals([originalPartition], [clonedPartition]);
        Assert.AreNotSame(added, clonedPartition.Shapes[0]);
    }
}