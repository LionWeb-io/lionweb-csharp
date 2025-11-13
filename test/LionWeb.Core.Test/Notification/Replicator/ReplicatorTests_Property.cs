namespace LionWeb.Core.Test.Notification;

using Core.Notification;
using Core.Notification.Partition;
using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class ReplicatorTests_Property : ReplicatorTestsBase
{
    #region Properties

    [TestMethod]
    public void PropertyAdded()
    {
        var circle = new Circle("c");
        var originalPartition = new Geometry("a") { Shapes = [circle] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        circle.Name = "Hello";

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var circle = new Circle("c") { Name = "Hello" };
        var originalPartition = new Geometry("a") { Shapes = [circle] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        circle.Name = "Bye";

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var docs = new Documentation("c") { Text = "Hello" };
        var originalPartition = new Geometry("a") { Documentation = docs };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        docs.Text = null;

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void PropertyDeleted_required_single_containment()
    {
        const int x = 1;
        var coord = new Coord("coord")
        {
            X = x, Y = 2, Z = 3
        };
        var circle = new Circle("circle")
        {
            Center = coord
        };
        var originalPartition = new Geometry("a")
        {
            Shapes = [circle]
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new PropertyDeletedNotification(coord, ShapesLanguage.Instance.Coord_x, x, 
            new NumericNotificationId("propertyDeletedNotification", 0));

        Assert.ThrowsExactly<InvalidValueException>(() => CreatePartitionReplicator(clonedPartition, notification));

    }

    #endregion
}