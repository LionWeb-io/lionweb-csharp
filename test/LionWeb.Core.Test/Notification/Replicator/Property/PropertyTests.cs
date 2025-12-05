namespace LionWeb.Core.Test.Notification.Replicator;

using Languages.Generated.V2024_1.TestLanguage;
using LionWeb.Core.Notification;
using LionWeb.Core.Notification.Partition;

[TestClass]
public class PropertyTests : ReplicatorTestsBase
{
    [TestMethod]
    public void PropertyAdded()
    {
        var circle = new LinkTestConcept("c");
        var originalPartition = new TestPartition("a") { Links =  [circle] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        circle.Name = "Hello";

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var circle = new LinkTestConcept("c") { Name = "Hello" };
        var originalPartition = new TestPartition("a") { Links =  [circle] };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        circle.Name = "Bye";

        AssertEquals([originalPartition], [clonedPartition]);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var docs = new DataTypeTestConcept("c") { StringValue_0_1 = "Hello" };
        var originalPartition = new TestPartition("a") { DataType = docs };
        var clonedPartition = ClonePartition(originalPartition);

        CreatePartitionReplicator(clonedPartition, originalPartition);

        docs.StringValue_0_1 = null;

        AssertEquals([originalPartition], [clonedPartition]);
    }

    /// <summary>
    /// TODO:
    /// This one is open to debate whether it's a bug:
    /// If we get a notification that would not be possible in our implementation, do we want to execute it?
    /// In general the answer is yes, because otherwise our model is out-of-sync.
    /// In this specific case, we had a slight chance to get away with it,
    /// as messing with properties has little side effects.
    /// </summary>
    [TestMethod]
    public void PropertyDeleted_required_single_containment()
    {
        const int x = 1;
        var coord = new DataTypeTestConcept("coord")
        {
            IntegerValue_1 = x
        };
        var originalPartition = new TestPartition("a")
        {
            DataType =  coord
        };
        
        var clonedPartition = ClonePartition(originalPartition);

        var notification = new PropertyDeletedNotification(coord, TestLanguageLanguage.Instance.DataTypeTestConcept_integerValue_1, x, 
            new NumericNotificationId("propertyDeletedNotification", 0));

        Assert.ThrowsExactly<InvalidValueException>(() =>
        {
            CreatePartitionReplicator(clonedPartition, notification);
        });

    }
}