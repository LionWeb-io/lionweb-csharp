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

namespace LionWeb.Core.Test.Listener;

using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using Comparer = Core.Utilities.Comparer;

[TestClass]
public class EventTests
{
    #region Properties

    [TestMethod]
    public void PropertyAdded()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        var cloneCircle = new Circle("c");
        var clone = new Geometry("a") { Shapes = [cloneCircle] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        circle.Name = "Hello";

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var circle = new Circle("c") { Name = "Hello" };
        var node = new Geometry("a") { Shapes = [circle] };

        var cloneCircle = new Circle("c") { Name = "Hello" };
        var clone = new Geometry("a") { Shapes = [cloneCircle] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        circle.Name = "Bye";

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var docs = new Documentation("c") { Text = "Hello" };
        var node = new Geometry("a") { Documentation = docs };

        var cloneDocs = new Documentation("c") { Text = "Hello" };
        var clone = new Geometry("a") { Documentation = cloneDocs };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        docs.Text = null;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region Children

    #region ChildAdded

    [TestMethod]
    public void ChildAdded_Multiple_Only()
    {
        var node = new Geometry("a");

        var clone = new Geometry("a");

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Circle("added");
        node.AddShapes([added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_First()
    {
        var node = new Geometry("a") { Shapes = [new Line("l")] };

        var clone = new Geometry("a") { Shapes = [new Line("l")] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Circle("added");
        node.InsertShapes(0, [added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_Last()
    {
        var node = new Geometry("a") { Shapes = [new Line("l")] };

        var clone = new Geometry("a") { Shapes = [new Line("l")] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Circle("added");
        node.InsertShapes(1, [added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[1]);
    }

    [TestMethod]
    public void ChildAdded_Single()
    {
        var node = new Geometry("a");

        var clone = new Geometry("a");

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Documentation("added");
        node.Documentation = added;

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Documentation);
    }

    [TestMethod]
    public void ChildAdded_Deep()
    {
        var node = new Geometry("a");

        var clone = new Geometry("a");

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Circle("added") { Center = new Coord("coord") { X = 1, Y = 2, Z = 3 } };
        node.AddShapes([added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
    }

    #endregion

    #region ChildDeleted

    [TestMethod]
    public void ChildDeleted_Multiple_Only()
    {
        var deleted = new Circle("deleted");
        var node = new Geometry("a") { Shapes = [deleted] };

        var clone = new Geometry("a") { Shapes = [new Circle("deleted")] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.RemoveShapes([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_First()
    {
        var deleted = new Circle("deleted");
        var node = new Geometry("a") { Shapes = [deleted, new Line("l")] };

        var clone = new Geometry("a") { Shapes = [new Circle("deleted"), new Line("l")] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.RemoveShapes([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_Last()
    {
        var deleted = new Circle("deleted");
        var node = new Geometry("a") { Shapes = [new Line("l"), deleted] };

        var clone = new Geometry("a") { Shapes = [new Line("l"), new Circle("deleted")] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.RemoveShapes([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Single()
    {
        var deleted = new Documentation("deleted");
        var node = new Geometry("a") { Documentation = deleted };

        var clone = new Geometry("a") { Documentation = new Documentation("deleted") };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.Documentation = null;

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

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.AddShapes([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Single()
    {
        var moved = new Documentation("moved");
        var node = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var clone = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = new Documentation("moved") }] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

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

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

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

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

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

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.AddShapes([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_Backward()
    {
        var moved = new Circle("moved");
        var node = new Geometry("a") { Shapes = [new Line("l"), moved] };

        var clone = new Geometry("a") { Shapes = [new Line("l"), new Circle("moved")] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.InsertShapes(0, [moved]);

        AssertEquals([node], [clone]);
    }

    #endregion

    #endregion

    #region Annotations

    #region AnnotationAdded

    [TestMethod]
    public void AnnotationAdded_Multiple_Only()
    {
        var node = new Geometry("a");

        var clone = new Geometry("a");

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new BillOfMaterials("added");
        node.AddAnnotations([added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.GetAnnotations()[0]);
    }

    [TestMethod]
    public void AnnotationAdded_Multiple_First()
    {
        var node = new Geometry("a");
        node.AddAnnotations([new BillOfMaterials("bof")]);

        var clone = new Geometry("a");
        clone.AddAnnotations([new BillOfMaterials("bof")]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new BillOfMaterials("added");
        node.InsertAnnotations(0, [added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.GetAnnotations()[0]);
    }

    [TestMethod]
    public void AnnotationAdded_Multiple_Last()
    {
        var node = new Geometry("a");
        node.AddAnnotations([new BillOfMaterials("bof")]);

        var clone = new Geometry("a");
        clone.AddAnnotations([new BillOfMaterials("bof")]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new BillOfMaterials("added");
        node.InsertAnnotations(1, [added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.GetAnnotations()[1]);
    }

    [TestMethod]
    public void AnnotationAdded_Deep()
    {
        var node = new Geometry("a");

        var clone = new Geometry("a");

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new BillOfMaterials("added")
        {
            AltGroups = [new MaterialGroup("mg") { MatterState = MatterState.gas }]
        };
        node.AddAnnotations([added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.GetAnnotations()[0]);
    }

    #endregion

    #region AnnotationDeleted

    [TestMethod]
    public void AnnotationDeleted_Multiple_Only()
    {
        var deleted = new BillOfMaterials("deleted");
        var node = new Geometry("a");
        node.AddAnnotations([deleted]);

        var clone = new Geometry("a");
        clone.AddAnnotations([new BillOfMaterials("deleted")]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.RemoveAnnotations([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_First()
    {
        var deleted = new BillOfMaterials("deleted");
        var node = new Geometry("a");
        node.AddAnnotations([deleted, new BillOfMaterials("bof")]);

        var clone = new Geometry("a");
        clone.AddAnnotations([new BillOfMaterials("deleted"), new BillOfMaterials("bof")]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.RemoveAnnotations([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_Last()
    {
        var deleted = new BillOfMaterials("deleted");
        var node = new Geometry("a");
        node.AddAnnotations([new BillOfMaterials("bof"), deleted]);

        var clone = new Geometry("a");
        clone.AddAnnotations([new BillOfMaterials("bof"), new BillOfMaterials("deleted")]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.RemoveAnnotations([deleted]);

        AssertEquals([node], [clone]);
    }

    #endregion

    #region AnnotationMovedFromOtherParent

    [TestMethod]
    public void AnnotationMovedFromOtherParent_Multiple()
    {
        var moved = new BillOfMaterials("moved");
        var origin = new CompositeShape("origin");
        origin.AddAnnotations([moved]);
        var node = new Geometry("a") { Shapes = [origin] };

        var cloneOrigin = new CompositeShape("origin");
        cloneOrigin.AddAnnotations([new BillOfMaterials("moved")]);
        var clone = new Geometry("a") { Shapes = [cloneOrigin] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.AddAnnotations([moved]);

        AssertEquals([node], [clone]);
    }

    #endregion

    #region AnnotationMovedInSameParent

    [TestMethod]
    public void AnnotationMovedInSameParent_Forward()
    {
        var moved = new BillOfMaterials("moved");
        var node = new Geometry("a");
        node.AddAnnotations([moved, new BillOfMaterials("bof")]);

        var clone = new Geometry("a");
        clone.AddAnnotations([new BillOfMaterials("moved"), new BillOfMaterials("bof")]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.AddAnnotations([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void AnnotationMovedInSameParent_Backward()
    {
        var moved = new BillOfMaterials("moved");
        var node = new Geometry("a");
        node.AddAnnotations([new BillOfMaterials("bof"), moved]);

        var clone = new Geometry("a");
        clone.AddAnnotations([new BillOfMaterials("bof"), new BillOfMaterials("moved")]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        node.InsertAnnotations(0, [moved]);

        AssertEquals([node], [clone]);
    }

    #endregion

    #endregion

    #region References

    #region ReferenceAdded

    [TestMethod]
    public void ReferenceAdded_Multiple_Only()
    {
        var bof = new BillOfMaterials("bof");
        var line = new Line("line");
        var node = new Geometry("a") { Shapes = [line] };
        node.AddAnnotations([bof]);

        var clone = new Geometry("a") { Shapes = [new Line("line")] };
        clone.AddAnnotations([new BillOfMaterials("bof")]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        bof.AddMaterials([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_First()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var node = new Geometry("a") { Shapes = [line, circle] };
        node.AddAnnotations([bof]);

        var cloneCircle = new Circle("circle");
        var clone = new Geometry("a") { Shapes = [new Line("line"), cloneCircle] };
        clone.AddAnnotations([new BillOfMaterials("bof") { Materials = [cloneCircle] }]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        bof.InsertMaterials(0, [line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_Last()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var node = new Geometry("a") { Shapes = [line, circle] };
        node.AddAnnotations([bof]);

        var cloneCircle = new Circle("circle");
        var clone = new Geometry("a") { Shapes = [new Line("line"), cloneCircle] };
        clone.AddAnnotations([new BillOfMaterials("bof") { Materials = [cloneCircle] }]);

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        bof.InsertMaterials(1, [line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od");
        var node = new Geometry("a") { Shapes = [od, circle] };

        var clone = new Geometry("a") { Shapes = [new OffsetDuplicate("od"), new Circle("circle")] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        od.Source = circle;

        AssertEquals([node], [clone]);
    }

    #endregion

    #endregion

    private void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
}