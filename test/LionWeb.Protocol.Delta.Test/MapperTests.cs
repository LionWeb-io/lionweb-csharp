namespace LionWeb.Protocol.Delta.Test;

using Core;
using Core.M1;
using Core.M3;
using Core.Notification;
using Core.Notification.Forest;
using Core.Notification.Partition;
using Core.Notification.Pipe;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;
using Core.Test.Notification;
using Core.Utilities;
using Delta.Client;
using Delta.Repository;
using Message.Command;
using Message.Event;
using System.Collections;

[TestClass]
public class MapperTests : DeltaTestsBase
{
    private static readonly IVersion2024_1 LionWebVersion = LionWebVersions.v2024_1;
    private static readonly TestLanguageLanguage Language = TestLanguageLanguage.Instance;
    private static readonly Containment Containment = Language.LinkTestConcept_containment_0_n;
    private static readonly Containment OldContainment = Containment;
    private static readonly Containment NewContainment = Language.LinkTestConcept_containment_1_n;
    private static readonly Reference Reference = Language.LinkTestConcept_reference_0_n;
    private static readonly Property Property = Language.DataTypeTestConcept_integerValue_0_1;

    private readonly INotificationIdProvider _notificationIdProvider = new TestParticipationNotificationIdProvider();

    #region Annotation

    [TestMethod]
    public void AnnotationAdded()
    {
        var parent = new LinkTestConcept("parent");
        List<IReadableNode> nodes = [parent];

        Test<AddAnnotation, AnnotationAdded>(nodes,
            new AnnotationAddedNotification(parent, new TestAnnotation("ann"), 0, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void AnnotationDeleted()
    {
        var ann = new TestAnnotation("ann");
        var parent = new LinkTestConcept("parent").WithAnnotation(ann);
        List<IReadableNode> nodes = [parent];

        Test<DeleteAnnotation, AnnotationDeleted>(nodes,
            new AnnotationDeletedNotification(ann, parent, 0, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedFromOtherParent()
    {
        var other = new TestAnnotation("other");
        var moved = new TestAnnotation("moved");
        var oldParent = new LinkTestConcept("oldParent").WithAnnotation(other).WithAnnotation(moved);
        var replaced = new TestAnnotation("replaced");
        var newParent = new LinkTestConcept("newParent").WithAnnotation(replaced);
        List<IReadableNode> nodes = [oldParent, newParent];

        Test<MoveAndReplaceAnnotationFromOtherParent, AnnotationMovedAndReplacedFromOtherParent>(nodes,
            new AnnotationMovedAndReplacedFromOtherParentNotification(newParent, 0, moved, oldParent, 1, replaced, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent()
    {
        var other = new TestAnnotation("other");
        var replaced = new TestAnnotation("replaced");
        var moved = new TestAnnotation("moved");
        var parent = new LinkTestConcept("parent").WithAnnotation(moved).WithAnnotation(other).WithAnnotation(replaced);
        List<IReadableNode> nodes = [parent];

        Test<MoveAndReplaceAnnotationInSameParent, AnnotationMovedAndReplacedInSameParent>(nodes,
            new AnnotationMovedAndReplacedInSameParentNotification(1, moved, parent, 0, replaced, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void AnnotationMovedFromOtherParent()
    {
        var other = new TestAnnotation("other");
        var moved = new TestAnnotation("moved");
        var oldParent = new LinkTestConcept("oldParent").WithAnnotation(other).WithAnnotation(moved);
        var newParent = new LinkTestConcept("newParent");
        List<IReadableNode> nodes = [oldParent, newParent];

        Test<MoveAnnotationFromOtherParent, AnnotationMovedFromOtherParent>(nodes,
            new AnnotationMovedFromOtherParentNotification(newParent, 0, moved, oldParent, 1, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void AnnotationMovedInSameParent()
    {
        var other = new TestAnnotation("other");
        var moved = new TestAnnotation("moved");
        var parent = new LinkTestConcept("parent").WithAnnotation(moved).WithAnnotation(other);
        List<IReadableNode> nodes = [parent];

        Test<MoveAnnotationInSameParent, AnnotationMovedInSameParent>(nodes,
            new AnnotationMovedInSameParentNotification(1, moved, parent, 0, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void AnnotationReplaced()
    {
        var newAnn = new TestAnnotation("newAnn");
        var replaced = new TestAnnotation("replaced");
        var parent = new LinkTestConcept("parent").WithAnnotation(replaced);
        List<IReadableNode> nodes = [parent];

        Test<ReplaceAnnotation, AnnotationReplaced>(nodes,
            new AnnotationReplacedNotification(newAnn, replaced, parent, 0, _notificationIdProvider.Create())
        );
    }

    #endregion

    #region Child

    [TestMethod]
    public void ChildAdded()
    {
        var parent = new LinkTestConcept("parent");
        List<IReadableNode> nodes = [parent];

        Test<AddChild, ChildAdded>(nodes,
            new ChildAddedNotification(parent, new LinkTestConcept("child"), Containment, 0, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ChildDeleted()
    {
        var child = new LinkTestConcept("child");
        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n = [child]
        };
        List<IReadableNode> nodes = [parent];

        Test<DeleteChild, ChildDeleted>(nodes,
            new ChildDeletedNotification(child, parent, Containment, 0, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherParent()
    {
        var other = new LinkTestConcept("other");
        var moved = new LinkTestConcept("moved");
        var oldParent = new LinkTestConcept("oldParent")
        {
            Containment_0_n = [other, moved]
        };
        var replaced = new LinkTestConcept("replaced");
        var newParent = new LinkTestConcept("newParent") { Containment_1_n = [replaced] };
        List<IReadableNode> nodes = [oldParent, newParent];

        Test<MoveAndReplaceChildFromOtherContainment, ChildMovedAndReplacedFromOtherContainment>(nodes,
            new ChildMovedAndReplacedFromOtherContainmentNotification(newParent, NewContainment, 0, moved, oldParent, OldContainment, 1, replaced, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent()
    {
        var other = new LinkTestConcept("other");
        var replaced = new LinkTestConcept("replaced");
        var moved = new LinkTestConcept("moved");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [other, moved], Containment_1_n = [replaced] };
        List<IReadableNode> nodes = [parent];

        Test<MoveAndReplaceChildFromOtherContainmentInSameParent, ChildMovedAndReplacedFromOtherContainmentInSameParent>(nodes,
            new ChildMovedAndReplacedFromOtherContainmentInSameParentNotification(NewContainment, 0, moved, parent, OldContainment, 1, replaced, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameParent()
    {
        var other = new LinkTestConcept("other");
        var replaced = new LinkTestConcept("replaced");
        var moved = new LinkTestConcept("moved");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [moved, other, replaced] };
        List<IReadableNode> nodes = [parent];

        Test<MoveAndReplaceChildInSameContainment, ChildMovedAndReplacedInSameContainment>(nodes,
            new ChildMovedAndReplacedInSameContainmentNotification(1, moved, parent, Containment, replaced, 0, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ChildMovedFromOtherParent()
    {
        var other = new LinkTestConcept("other");
        var moved = new LinkTestConcept("moved");
        var oldParent = new LinkTestConcept("oldParent")
        {
            Containment_0_n = [other, moved]
        };
        var newParent = new LinkTestConcept("newParent");
        List<IReadableNode> nodes = [oldParent, newParent];

        Test<MoveChildFromOtherContainment, ChildMovedFromOtherContainment>(nodes,
            new ChildMovedFromOtherContainmentNotification(newParent, NewContainment, 0, moved, oldParent, OldContainment, 1, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent()
    {
        var other = new LinkTestConcept("other");
        var moved = new LinkTestConcept("moved");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [other, moved] };
        List<IReadableNode> nodes = [parent];

        Test<MoveChildFromOtherContainmentInSameParent, ChildMovedFromOtherContainmentInSameParent>(nodes,
            new ChildMovedFromOtherContainmentInSameParentNotification(NewContainment, 0, moved, parent, OldContainment, 1, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ChildMovedInSameParent()
    {
        var other = new LinkTestConcept("other");
        var moved = new LinkTestConcept("moved");
        var parent = new LinkTestConcept("parent") { Containment_0_n = [moved, other] };
        List<IReadableNode> nodes = [parent];

        Test<MoveChildInSameContainment, ChildMovedInSameContainment>(nodes,
            new ChildMovedInSameContainmentNotification(1, moved, parent, Containment, 0, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ChildReplaced()
    {
        var newChild = new LinkTestConcept("newChild");
        var replacedChild = new LinkTestConcept("replacedChild");
        var parent = new LinkTestConcept("parent")
        {
            Containment_0_n = [replacedChild]
        };
        List<IReadableNode> nodes = [parent];

        Test<ReplaceChild, ChildReplaced>(nodes,
            new ChildReplacedNotification(newChild, replacedChild, parent, Containment, 0, _notificationIdProvider.Create())
        );
    }

    #endregion

    #region Reference

    [TestMethod]
    public void ReferenceAdded()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent");
        List<IReadableNode> nodes = [parent, target];

        Test<AddReference, ReferenceAdded>(nodes,
            new ReferenceAddedNotification(parent, Reference, 0, ReferenceTarget.FromNode(target)with { ResolveInfo = "resolveInfo" }, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ReferenceDeleted()
    {
        var target = new LinkTestConcept("target");
        var parent = new LinkTestConcept("parent")
        {
            Reference_0_n = [target]
        };
        List<IReadableNode> nodes = [parent, target];

        Test<DeleteReference, ReferenceDeleted>(nodes,
            new ReferenceDeletedNotification(parent, Reference, 0, ReferenceTarget.FromNode(target)with { ResolveInfo = "resolveInfo" }, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void ReferenceChanged()
    {
        var oldTarget = new LinkTestConcept("oldTarget");
        var newTarget = new LinkTestConcept("newTarget");
        var parent = new LinkTestConcept("parent")
        {
            Reference_0_n = [oldTarget]
        };
        List<IReadableNode> nodes = [parent, oldTarget, newTarget];

        Test<ChangeReference, ReferenceChanged>(nodes,
            new ReferenceChangedNotification(parent, Reference, 0, ReferenceTarget.FromNode(newTarget)with { ResolveInfo = "resolveInfo" }, ReferenceTarget.FromNode(oldTarget),
                _notificationIdProvider.Create())
        );
    }

    #endregion

    #region Property

    [TestMethod]
    public void PropertyAdded()
    {
        var parent = new DataTypeTestConcept("parent");
        List<IReadableNode> nodes = [parent];

        Test<AddProperty, PropertyAdded>(nodes,
            new PropertyAddedNotification(parent, Property, 23, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var parent = new DataTypeTestConcept("parent")
        {
            IntegerValue_0_1 = 42
        };
        List<IReadableNode> nodes = [parent];

        Test<DeleteProperty, PropertyDeleted>(nodes,
            new PropertyDeletedNotification(parent, Property, 42, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var parent = new DataTypeTestConcept("parent")
        {
            IntegerValue_0_1 = 42
        };
        List<IReadableNode> nodes = [parent];

        Test<ChangeProperty, PropertyChanged>(nodes,
            new PropertyChangedNotification(parent, Property, 23, 42, _notificationIdProvider.Create())
        );
    }

    #endregion

    #region Partition

    [TestMethod]
    public void PartitionAdded()
    {
        var partition = new TestPartition("partition");
        List<IReadableNode> nodes = [];

        Test<AddPartition, PartitionAdded>(nodes,
            new PartitionAddedNotification(partition, _notificationIdProvider.Create())
        );
    }

    [TestMethod]
    public void PartitionDeleted()
    {
        var partition = new TestPartition("partition");
        List<IReadableNode> nodes = [partition];

        Test<DeletePartition, PartitionDeleted>(nodes,
            new PartitionDeletedNotification(partition, _notificationIdProvider.Create())
        );
    }

    #endregion

    #region Misc

    [TestMethod]
    public void Composite()
    {
        var partition = new TestPartition("partition");
        List<IReadableNode> nodes = [];

        Test<CompositeCommand, CompositeEvent>(nodes,
            new CompositeNotification([
                new PartitionAddedNotification(partition, _notificationIdProvider.Create()),
                new PropertyAddedNotification(partition, Language.LionWebVersion.BuiltIns.INamed_name, "newName", _notificationIdProvider.Create())
            ], _notificationIdProvider.Create())
        );
    }
    
    [TestMethod]
    [Ignore("changing classifier not supported yet")]
    public void ClassifierChanged()
    {
        var node = new LinkTestConcept("node");
        List<IReadableNode> nodes = [node];

        Test<ChangeClassifier, ClassifierChanged>(nodes,
            new ClassifierChangedNotification(node, Language.DataTypeTestConcept, Language.LinkTestConcept, _notificationIdProvider.Create())
        );
    }

    #endregion


    private void Test<TCommand, TEvent>(List<IReadableNode> nodes, INotification notification)
        where TCommand : IDeltaCommand
        where TEvent : IDeltaEvent
    {
        var command = CreateNotificationToDeltaCommandMapper().Map(notification);
        Assert.IsInstanceOfType<TCommand>(command);
        var commandNotification = CreateDeltaCommandToNotificationMapper(nodes).Map(command);
        AssertEquals(notification, commandNotification);

        var @event = CreateNotificationToDeltaEventMapper().Map(notification);
        Assert.IsInstanceOfType<TEvent>(@event);
        var eventNotification = CreateDeltaEventToNotificationMapper(nodes).Map(@event);
        AssertEquals(notification, eventNotification);

        var mappedNotification = CreateNotificationToNotificationMapper(nodes).Map(notification);
        Assert.IsInstanceOfType(mappedNotification, notification.GetType());
    }

    private void AssertEquals<T>(T expected, INotification actual) where T : INotification
    {
        if (actual is not T actualT)
            Assert.Fail($"different types: expected: {expected.GetType()} actual: {actual.GetType()}");

        foreach (var propertyInfo in expected.GetType().GetProperties())
        {
            var ex = propertyInfo.GetValue(expected, null);
            var act = propertyInfo.GetValue(actual, null);

            switch (ex, act)
            {
                case (IReadableNode e, IReadableNode a):
                    AssertSurfaceEquals(e, a);
                    break;
                case (string, string):
                    Assert.AreEqual(ex, act);
                    break;
                case (IEnumerable<INotification> ln, IEnumerable<INotification> rn):
                    var lList = ln.ToList();
                    var rList = rn.ToList();
                    Assert.AreEqual(lList.Count, rList.Count, propertyInfo.ToString());

                    for (var i = 0; i < lList.Count; i++)
                    {
                        AssertEquals(lList[i], rList[i]);
                    }

                    break;
                    
                case (IEnumerable l, IEnumerable r):
                    var expList = l.Cast<IReadableNode>().ToList();
                    var actList = r.Cast<IReadableNode>().ToList();

                    Assert.AreEqual(expList.Count, actList.Count, propertyInfo.ToString());

                    for (var i = 0; i < expList.Count; i++)
                    {
                        AssertSurfaceEquals(expList[i], actList[i]);
                    }

                    break;
                default:
                    Assert.AreEqual(ex, act);
                    break;
            }
        }
    }

    private void AssertSurfaceEquals(IReadableNode expected, IReadableNode actual) =>
        Assert.AreEqual(expected.GetClassifier(), actual.GetClassifier(), new LanguageEntityIdentityComparer());

    private static NotificationToDeltaCommandMapper CreateNotificationToDeltaCommandMapper() => 
        new(new CommandIdProvider(), LionWebVersion);

    private static DeltaCommandToNotificationMapper CreateDeltaCommandToNotificationMapper(List<IReadableNode> nodes) =>
        new(
            CreateSharedNodeMap(nodes),
            CreateSharedKeyedMap(),
            CreateDeserializationBuilder()
        );

    private static NotificationToDeltaEventMapper CreateNotificationToDeltaEventMapper() => 
        new(new ParticipationIdProvider(), LionWebVersion);

    private static DeltaEventToNotificationMapper CreateDeltaEventToNotificationMapper(List<IReadableNode> nodes) =>
        new(
            CreateSharedNodeMap(nodes),
            SharedKeyedMapBuilder.BuildSharedKeyMap([Language]),
            CreateDeserializationBuilder()
        );

    private static NotificationToNotificationMapper CreateNotificationToNotificationMapper(List<IReadableNode> nodes) => 
        new(CreateSharedNodeMap(SameIdCloner.Clone(nodes.Cast<INode>()).Cast<IReadableNode>().ToList()));

    private static SharedNodeMap CreateSharedNodeMap(List<IReadableNode> nodes)
    {
        var result = new SharedNodeMap();
        foreach (var node in nodes)
        {
            result.RegisterNode(node);
        }

        return result;
    }

    private static SharedKeyedMap CreateSharedKeyedMap() => 
        SharedKeyedMapBuilder.BuildSharedKeyMap([Language]);

    private static DeserializerBuilder CreateDeserializationBuilder() =>
        new DeserializerBuilder()
            .WithLionWebVersion(LionWebVersion)
            .WithLanguage(Language);
}

internal class TestParticipationNotificationIdProvider : INotificationIdProvider
{
    private int _nextId = 0;

    /// <inheritdoc />
    public virtual INotificationId Create() =>
        new ParticipationNotificationId(null, _nextId++.ToString());
}