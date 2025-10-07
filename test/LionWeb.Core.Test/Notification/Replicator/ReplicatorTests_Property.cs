namespace LionWeb.Core.Test.Notification;

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

    #endregion
}