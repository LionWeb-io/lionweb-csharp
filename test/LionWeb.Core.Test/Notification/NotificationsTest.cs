namespace LionWeb.Core.Test.Notification;

using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class NotificationsTest : NotificationTestsBase
{
    #region Properties

    [TestMethod]
    public void PropertyAdded()
    {
        var circle = new Circle("c");
        var originalPartition = new Geometry("a") { Shapes = [circle] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        circle.Name = "Hello";

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var circle = new Circle("c") { Name = "Hello" };
        var originalPartition = new Geometry("a") { Shapes = [circle] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        circle.Name = "Bye";

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var docs = new Documentation("c") { Text = "Hello" };
        var originalPartition = new Geometry("a") { Documentation = docs };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        docs.Text = null;

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region Children

    #region ChildAdded

    [TestMethod]
    public void ChildAdded_Multiple_Only()
    {
        var originalPartition = new Geometry("a");
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new Circle("added");
        originalPartition.AddShapes([added]);

        AssertEquals([originalPartition], [clonePartition]);
        Assert.AreNotSame(added, clonePartition.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_First()
    {
        var originalPartition = new Geometry("a") { Shapes = [new Line("l")] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new Circle("added");
        originalPartition.InsertShapes(0, [added]);

        AssertEquals([originalPartition], [clonePartition]);
        Assert.AreNotSame(added, clonePartition.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_Last()
    {
        var originalPartition = new Geometry("a") { Shapes = [new Line("l")] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new Circle("added");
        originalPartition.InsertShapes(1, [added]);

        AssertEquals([originalPartition], [clonePartition]);
        Assert.AreNotSame(added, clonePartition.Shapes[1]);
    }

    [TestMethod]
    public void ChildAdded_Single()
    {
        var originalPartition = new Geometry("a");
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new Documentation("added");
        originalPartition.Documentation = added;

        AssertEquals([originalPartition], [clonePartition]);
        Assert.AreNotSame(added, clonePartition.Documentation);
    }

    [TestMethod]
    public void ChildAdded_Deep()
    {
        var originalPartition = new Geometry("a");
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new Circle("added") { Center = new Coord("coord") { X = 1, Y = 2, Z = 3 } };
        originalPartition.AddShapes([added]);

        AssertEquals([originalPartition], [clonePartition]);
        Assert.AreNotSame(added, clonePartition.Shapes[0]);
    }

    #endregion

    #region ChildDeleted

    [TestMethod]
    public void ChildDeleted_Multiple_Only()
    {
        var deleted = new Circle("deleted");
        var originalPartition = new Geometry("a") { Shapes = [deleted] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.RemoveShapes([deleted]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_First()
    {
        var deleted = new Circle("deleted");
        var originalPartition = new Geometry("a") { Shapes = [deleted, new Line("l")] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.RemoveShapes([deleted]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_Last()
    {
        var deleted = new Circle("deleted");
        var originalPartition = new Geometry("a") { Shapes = [new Line("l"), deleted] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.RemoveShapes([deleted]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildDeleted_Single()
    {
        var deleted = new Documentation("deleted");
        var originalPartition = new Geometry("a") { Documentation = deleted };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.Documentation = null;

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region ChildReplaced

    [TestMethod]
    public void ChildReplaced_Single()
    {
        var originalPartition = new Geometry("a") { Documentation = new Documentation("replaced") { Text = "a" } };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new Documentation("added") { Text = "added" };
        originalPartition.Documentation = added;

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildReplaced_Deep()
    {
        var originalPartition = new Geometry("a");
        var bof = new BillOfMaterials("bof") { DefaultGroup = new MaterialGroup("mg") { MatterState = MatterState.liquid } };
        originalPartition.AddAnnotations([bof]);

        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        bof.DefaultGroup = new MaterialGroup("replaced") { MatterState = MatterState.gas };

        AssertEquals([originalPartition], [clonePartition]);
    }

    /// <summary>
    /// This test triggers two notifications: ChildDeletedNotification, ChildAddedNotification
    /// but it should trigger ChildReplacedNotification //TODO: requires fix !
    /// </summary>
    [TestMethod]
    public void ChildReplaced_Multiple_First()
    {
        var originalPartition = new Geometry("a") { Shapes = [new Circle("replaced"), new Circle("child")] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var substituteNode = new Line("substituteNode");
        originalPartition.Shapes[0].ReplaceWith(substituteNode);

        AssertEquals([originalPartition], [clonePartition]);
    }

    /// <summary>
    /// This test triggers two notifications: ChildDeletedNotification, ChildAddedNotification
    /// but it should trigger ChildReplacedNotification //TODO: requires fix !
    /// </summary>
    [TestMethod]
    public void ChildReplaced_Multiple_Last()
    {
        var originalPartition = new Geometry("a") { Shapes = [new Circle("child"), new Circle("replaced")] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var substituteNode = new Line("substituteNode");
        originalPartition.Shapes[^1].ReplaceWith(substituteNode);

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region ChildMovedFromOtherContainment

    [TestMethod]
    public void ChildMovedFromOtherContainment_Multiple()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.AddShapes([moved]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Single()
    {
        var moved = new Documentation("moved");
        var originalPartition = new Geometry("a")
        {
            Shapes = [new Line("l") { ShapeDocs = moved }]
        };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.Documentation = moved;

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region ChildMovedAndReplacedFromOtherContainment

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment_Single()
    {
        var moved = new Documentation("moved");
        var replaced = new Documentation("replaced");
        var originalPartition = new Geometry("a")
        {
            Documentation = replaced, Shapes = [new Line("l") { ShapeDocs = moved }]
        };
        var clonePartition = PartitionCloner(originalPartition);
        
        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.Documentation = moved;

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment_Single_ReplaceWith()
    {
        var moved = new Documentation("moved");
        var replaced = new Documentation("replaced");
        var originalPartition = new Geometry("a")
        {
            Documentation = replaced, Shapes = [new Line("l") { ShapeDocs = moved }]
        };
        var clonePartition = PartitionCloner(originalPartition);
        
        CreatePartitionReplicator(clonePartition, originalPartition);

        replaced.ReplaceWith(moved);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment_Multiple()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var replaced = new Circle("replaced");
        var originalPartition = new Geometry("a") { Shapes = [origin, replaced] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        replaced.ReplaceWith(moved);

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region ChildMovedAndReplacedFromOtherContainmentInSameParent

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent_Single()
    {
        var line = new Line("l")
        {
            Start = new Coord("moved"), End = new Coord("replaced")
        };
        var originalPartition = new Geometry("a") { Shapes = [line] };
        var clonePartition = PartitionCloner(originalPartition);
        
        CreatePartitionReplicator(clonePartition, originalPartition);

        line.End = line.Start;

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent_Single_ReplaceWith()
    {
        var line = new Line("l")
        {
            Start = new Coord("moved"), End = new Coord("replaced")
        };
        var originalPartition = new Geometry("a") { Shapes = [line] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        line.End.ReplaceWith(line.Start);

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region ChildMovedFromOtherContainmentInSameParent

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Multiple()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        origin.AddDisabledParts([moved]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Single()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originalPartition = new Geometry("a") { Shapes = [origin] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        origin.EvilPart = moved;

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region ChildMovedInSameContainment

    [TestMethod]
    public void ChildMovedInSameContainment_Forward()
    {
        var moved = new Circle("moved");
        var originalPartition = new Geometry("a") { Shapes = [moved, new Line("l")] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.AddShapes([moved]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_Backward()
    {
        var moved = new Circle("moved");
        var originalPartition = new Geometry("a") { Shapes = [new Line("l"), moved] };
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.InsertShapes(0, [moved]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #endregion

    #region Annotations

    #region AnnotationAdded

    [TestMethod]
    public void AnnotationAdded_Multiple_Only()
    {
        var originalPartition = new Geometry("a");
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new BillOfMaterials("added");
        originalPartition.AddAnnotations([added]);

        AssertEquals([originalPartition], [clonePartition]);
        Assert.AreNotSame(added, clonePartition.GetAnnotations()[0]);
    }

    [TestMethod]
    public void AnnotationAdded_Multiple_First()
    {
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof")]);
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new BillOfMaterials("added");
        originalPartition.InsertAnnotations(0, [added]);

        AssertEquals([originalPartition], [clonePartition]);
        Assert.AreNotSame(added, clonePartition.GetAnnotations()[0]);
    }

    [TestMethod]
    public void AnnotationAdded_Multiple_Last()
    {
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof")]);
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new BillOfMaterials("added");
        originalPartition.InsertAnnotations(1, [added]);

        AssertEquals([originalPartition], [clonePartition]);
        Assert.AreNotSame(added, clonePartition.GetAnnotations()[1]);
    }

    [TestMethod]
    public void AnnotationAdded_Deep()
    {
        var originalPartition = new Geometry("a");
        var clonePartition = PartitionCloner(originalPartition);

        CreatePartitionReplicator(clonePartition, originalPartition);

        var added = new BillOfMaterials("added") { AltGroups = [new MaterialGroup("mg") { MatterState = MatterState.gas }] };
        originalPartition.AddAnnotations([added]);

        AssertEquals([originalPartition], [clonePartition]);
        Assert.AreNotSame(added, clonePartition.GetAnnotations()[0]);
    }

    #endregion

    #region AnnotationDeleted

    [TestMethod]
    public void AnnotationDeleted_Multiple_Only()
    {
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([deleted]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_First()
    {
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([deleted, new BillOfMaterials("bof")]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_Last()
    {
        var deleted = new BillOfMaterials("deleted");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof"), deleted]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.RemoveAnnotations([deleted]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region AnnotationMovedFromOtherParent

    [TestMethod]
    public void AnnotationMovedFromOtherParent_Multiple()
    {
        var moved = new BillOfMaterials("moved");
        var origin = new CompositeShape("origin");
        origin.AddAnnotations([moved]);
        var originalPartition = new Geometry("a") { Shapes = [origin] };

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.AddAnnotations([moved]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region AnnotationMovedInSameParent

    [TestMethod]
    public void AnnotationMovedInSameParent_Forward()
    {
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([moved, new BillOfMaterials("bof")]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.AddAnnotations([moved]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void AnnotationMovedInSameParent_Backward()
    {
        var moved = new BillOfMaterials("moved");
        var originalPartition = new Geometry("a");
        originalPartition.AddAnnotations([new BillOfMaterials("bof"), moved]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        originalPartition.InsertAnnotations(0, [moved]);

        AssertEquals([originalPartition], [clonePartition]);
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
        var originalPartition = new Geometry("a") { Shapes = [line] };
        originalPartition.AddAnnotations([bof]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        bof.AddMaterials([line]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_First()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        bof.InsertMaterials(0, [line]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_Last()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        bof.InsertMaterials(1, [line]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ReferenceAdded_Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od");
        var originalPartition = new Geometry("a") { Shapes = [od, circle] };

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        od.Source = circle;

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region ReferenceDeleted

    [TestMethod]
    public void ReferenceDeleted_Multiple_Only()
    {
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [line] };
        var originalPartition = new Geometry("a") { Shapes = [line] };
        originalPartition.AddAnnotations([bof]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        bof.RemoveMaterials([line]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_First()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [line, circle] };
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        bof.RemoveMaterials([line]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_Last()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [circle, line] };
        var originalPartition = new Geometry("a") { Shapes = [line, circle] };
        originalPartition.AddAnnotations([bof]);

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        bof.RemoveMaterials([line]);

        AssertEquals([originalPartition], [clonePartition]);
    }

    [TestMethod]
    public void ReferenceDeleted_Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var originalPartition = new Geometry("a") { Shapes = [od, circle] };

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        od.AltSource = null;

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #region ReferenceChanged

    [TestMethod]
    public void ReferenceChanged_Single()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var originalPartition = new Geometry("a") { Shapes = [od, circle, line] };

        var clonePartition = PartitionCloner(originalPartition);
        CreatePartitionReplicator(clonePartition, originalPartition);

        od.AltSource = line;

        AssertEquals([originalPartition], [clonePartition]);
    }

    #endregion

    #endregion
}