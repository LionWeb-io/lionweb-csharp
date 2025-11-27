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

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class TwowayTests : TwowayReplicatorTestsBase
{
    #region ChildAdded

    [TestMethod]
    public void ChildAdded_Multiple_Only()
    {
        var node = new Geometry("a");

        var clone = new Geometry("a");

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);

        var added = new Circle("added");
        node.AddShapes([added]);
        var added2 = new Circle("added2");
        clone.AddShapes([added2]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
        Assert.AreNotSame(added2, node.Shapes[1]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_First()
    {
        var node = new Geometry("a") { Shapes = [new Line("l")] };

        var clone = new Geometry("a") { Shapes = [new Line("l")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new Circle("added");
        node.InsertShapes(0, [added]);
        var added2 = new Circle("added2");
        clone.InsertShapes(1, [added2]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
        Assert.AreNotSame(added2, node.Shapes[2]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_Last()
    {
        var node = new Geometry("a") { Shapes = [new Line("l")] };

        var clone = new Geometry("a") { Shapes = [new Line("l")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new Circle("added");
        node.InsertShapes(1, [added]);
        var added2 = new Circle("added2");
        clone.InsertShapes(0, [added2]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[2]);
        Assert.AreNotSame(added2, node.Shapes[0]);
    }

    [TestMethod]
    [Ignore("issue with replaceWith")]
    public void ChildAdded_Single()
    {
        var node = new Geometry("a");

        var clone = new Geometry("a");

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new Documentation("added");
        node.Documentation = added;
        var added2 = new Documentation("added2");
        clone.Documentation = added2;

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Documentation);
        Assert.AreNotSame(added2, node.Documentation);
    }

    [TestMethod]
    public void ChildAdded_Deep()
    {
        var node = new Geometry("a");

        var clone = new Geometry("a");

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new Circle("added") { Center = new Coord("coord") { X = 1, Y = 2, Z = 3 } };
        node.AddShapes([added]);
        var added2 = new Circle("added2") { Center = new Coord("coord2") { X = 11, Y = 12, Z = 13 } };
        clone.AddShapes([added2]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
        Assert.AreNotSame(added2, node.Shapes[1]);
    }

    [TestMethod]
    public void ChildAdded_Deep_DuplicateNodeId()
    {
        var node = new Geometry("a");

        var clone = new Geometry("a");

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new Circle("added") { Center = new Coord("coord") { X = 1, Y = 2, Z = 3 } };
        node.AddShapes([added]);
        var added2 = new Circle("added2") { Center = new Coord("coord") { X = 1, Y = 2, Z = 3 } };

        Assert.ThrowsException<DuplicateNodeIdException>(() => node.AddShapes([added2]));
    }

    #endregion

    #region ChildDeleted

    [TestMethod]
    public void ChildDeleted_Multiple_Only()
    {
        var deleted = new Circle("deleted");
        var node = new Geometry("a") { Shapes = [deleted] };

        var clone = new Geometry("a") { Shapes = [new Circle("deleted")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.RemoveShapes([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_First()
    {
        var deleted = new Circle("deleted");
        var node = new Geometry("a") { Shapes = [deleted, new Line("l")] };

        var clone = new Geometry("a") { Shapes = [new Circle("deleted"), new Line("l")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.RemoveShapes([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_Last()
    {
        var deleted = new Circle("deleted");
        var node = new Geometry("a") { Shapes = [new Line("l"), deleted] };

        var clone = new Geometry("a") { Shapes = [new Line("l"), new Circle("deleted")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.RemoveShapes([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Single()
    {
        var deleted = new Documentation("deleted");
        var node = new Geometry("a") { Documentation = deleted };

        var clone = new Geometry("a") { Documentation = new Documentation("deleted") };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.Documentation = null;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ChildReplaced

    [TestMethod]
    [Ignore("issue with replaceWith")]
    public void ChildReplaced_Single()
    {
        var node = new Geometry("a") { Documentation = new Documentation("replaced") { Text = "a" } };

        var clone = new Geometry("a") { Documentation = new Documentation("replaced") { Text = "a" } };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        var added = new Documentation("added") { Text = "added" };
        node.Documentation = added;

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    [Ignore("issue with replaceWith")]
    public void ChildReplaced_Deep()
    {
        var node = new Geometry("a");
        var bof = new BillOfMaterials("bof")
        {
            DefaultGroup = new MaterialGroup("mg") { MatterState = MatterState.liquid }
        };
        node.AddAnnotations([bof]);

        var clone = new Geometry("a");
        clone.AddAnnotations([
            new BillOfMaterials("bof") { DefaultGroup = new MaterialGroup("mg") { MatterState = MatterState.liquid } }
        ]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.DefaultGroup = new MaterialGroup("replaced") { MatterState = MatterState.gas };

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ChildMovedFromOtherContainment

    [TestMethod]
    public void ChildMovedFromOtherContainment_Multiple()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var node = new Geometry("a") { Shapes = [origin] };

        var clone = new Geometry("a") { Shapes = [new CompositeShape("origin") { Parts = [new Circle("moved")] }] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.AddShapes([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Single()
    {
        var moved = new Documentation("moved");
        var node = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var clone = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = new Documentation("moved") }] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.Documentation = moved;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ChildMovedFromOtherContainmentInSameParent

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Multiple()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var node = new Geometry("a") { Shapes = [origin] };

        var clone = new Geometry("a") { Shapes = [new CompositeShape("origin") { Parts = [new Circle("moved")] }] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        origin.AddDisabledParts([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Single()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var node = new Geometry("a") { Shapes = [origin] };

        var clone = new Geometry("a") { Shapes = [new CompositeShape("origin") { Parts = [new Circle("moved")] }] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        origin.EvilPart = moved;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ChildMovedInSameContainment

    [TestMethod]
    public void ChildMovedInSameContainment_Forward()
    {
        var moved = new Circle("moved");
        var node = new Geometry("a") { Shapes = [moved, new Line("l")] };

        var clone = new Geometry("a") { Shapes = [new Circle("moved"), new Line("l")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.AddShapes([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_Backward()
    {
        var moved = new Circle("moved");
        var node = new Geometry("a") { Shapes = [new Line("l"), moved] };

        var clone = new Geometry("a") { Shapes = [new Line("l"), new Circle("moved")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        node.InsertShapes(0, [moved]);

        AssertEquals([node], [clone]);
    }

    #endregion
}