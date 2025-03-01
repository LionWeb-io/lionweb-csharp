﻿// Copyright 2025 TRUMPF Laser SE and other contributors
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

using Core.Serialization;
using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M1.Event;
using M1.Event.Partition;
using M2;
using M3;
using Comparer = Core.Utilities.Comparer;
using NodeId = string;

public abstract class EventTestsBase
{
    protected abstract Geometry CreateReplicator(Geometry node);

    protected Geometry Clone(Geometry node) => (Geometry)new SameIdCloner([node]) { IncludingReferences = true }.Clone()[node];

    #region Properties

    [TestMethod]
    public void PropertyAdded()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        var clone = CreateReplicator(node);

        circle.Name = "Hello";

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var circle = new Circle("c") { Name = "Hello" };
        var node = new Geometry("a") { Shapes = [circle] };

        var clone = CreateReplicator(node);

        circle.Name = "Bye";

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var docs = new Documentation("c") { Text = "Hello" };
        var node = new Geometry("a") { Documentation = docs };

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

        var added = new Circle("added");
        node.AddShapes([added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_First()
    {
        var node = new Geometry("a") { Shapes = [new Line("l")] };

        var clone = CreateReplicator(node);

        var added = new Circle("added");
        node.InsertShapes(0, [added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_Last()
    {
        var node = new Geometry("a") { Shapes = [new Line("l")] };

        var clone = CreateReplicator(node);

        var added = new Circle("added");
        node.InsertShapes(1, [added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[1]);
    }

    [TestMethod]
    public void ChildAdded_Single()
    {
        var node = new Geometry("a");

        var clone = CreateReplicator(node);

        var added = new Documentation("added");
        node.Documentation = added;

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Documentation);
    }

    [TestMethod]
    public void ChildAdded_Deep()
    {
        var node = new Geometry("a");

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

        node.RemoveShapes([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_First()
    {
        var deleted = new Circle("deleted");
        var node = new Geometry("a") { Shapes = [deleted, new Line("l")] };

        var clone = CreateReplicator(node);

        node.RemoveShapes([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Multiple_Last()
    {
        var deleted = new Circle("deleted");
        var node = new Geometry("a") { Shapes = [new Line("l"), deleted] };

        var clone = CreateReplicator(node);

        node.RemoveShapes([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildDeleted_Single()
    {
        var deleted = new Documentation("deleted");
        var node = new Geometry("a") { Documentation = deleted };

        var clone = CreateReplicator(node);

        node.Documentation = null;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ChildReplaced

    [TestMethod]
    public void ChildReplaced_Single()
    {
        var node = new Geometry("a") { Documentation = new Documentation("replaced") { Text = "a" } };

        var clone = CreateReplicator(node);

        var added = new Documentation("added") { Text = "added" };
        node.Documentation = added;

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildReplaced_Deep()
    {
        var node = new Geometry("a");
        var bof = new BillOfMaterials("bof")
        {
            DefaultGroup = new MaterialGroup("mg") { MatterState = MatterState.liquid }
        };
        node.AddAnnotations([bof]);

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

        node.AddShapes([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Single()
    {
        var moved = new Documentation("moved");
        var node = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

        origin.AddDisabledParts([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent_Single()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var node = new Geometry("a") { Shapes = [origin] };

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

        node.AddShapes([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ChildMovedInSameContainment_Backward()
    {
        var moved = new Circle("moved");
        var node = new Geometry("a") { Shapes = [new Line("l"), moved] };

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

        var added = new BillOfMaterials("added");
        node.InsertAnnotations(1, [added]);

        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.GetAnnotations()[1]);
    }

    [TestMethod]
    public void AnnotationAdded_Deep()
    {
        var node = new Geometry("a");

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

        node.RemoveAnnotations([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_First()
    {
        var deleted = new BillOfMaterials("deleted");
        var node = new Geometry("a");
        node.AddAnnotations([deleted, new BillOfMaterials("bof")]);

        var clone = CreateReplicator(node);

        node.RemoveAnnotations([deleted]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void AnnotationDeleted_Multiple_Last()
    {
        var deleted = new BillOfMaterials("deleted");
        var node = new Geometry("a");
        node.AddAnnotations([new BillOfMaterials("bof"), deleted]);

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

        node.AddAnnotations([moved]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void AnnotationMovedInSameParent_Backward()
    {
        var moved = new BillOfMaterials("moved");
        var node = new Geometry("a");
        node.AddAnnotations([new BillOfMaterials("bof"), moved]);

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

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

        var clone = CreateReplicator(node);

        bof.InsertMaterials(1, [line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od");
        var node = new Geometry("a") { Shapes = [od, circle] };

        var clone = CreateReplicator(node);

        od.Source = circle;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ReferenceDeleted

    [TestMethod]
    public void ReferenceDeleted_Multiple_Only()
    {
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [line] };
        var node = new Geometry("a") { Shapes = [line] };
        node.AddAnnotations([bof]);

        var clone = CreateReplicator(node);

        bof.RemoveMaterials([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_First()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [line, circle] };
        var node = new Geometry("a") { Shapes = [line, circle] };
        node.AddAnnotations([bof]);

        var clone = CreateReplicator(node);

        bof.RemoveMaterials([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_Last()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [circle, line] };
        var node = new Geometry("a") { Shapes = [line, circle] };
        node.AddAnnotations([bof]);

        var clone = CreateReplicator(node);

        bof.RemoveMaterials([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceDeleted_Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var node = new Geometry("a") { Shapes = [od, circle] };

        var clone = CreateReplicator(node);

        od.AltSource = null;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ReferenceChanged

    [TestMethod]
    public void ReferenceChanged_Single()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var node = new Geometry("a") { Shapes = [od, circle, line] };

        var clone = CreateReplicator(node);

        od.AltSource = line;

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

[TestClass]
public class EventsTest : EventTestsBase
{
    protected override Geometry CreateReplicator(Geometry node)
    {
        var clone = Clone(node);
        
        var replicator = new PartitionEventReplicator(clone);
        replicator.ReplicateFrom(node.GetPublisher());

        return clone;
    }
}

[TestClass]
public class EventsTestSerialized : EventTestsBase
{
    protected override Geometry CreateReplicator(Geometry node)
    {
        var clone = Clone(node);
        
        var lionWebVersion = LionWebVersions.v2024_1;
        List<Language> languages = [ShapesLanguage.Instance, lionWebVersion.BuiltIns, lionWebVersion.LionCore];

        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap = CommandToEventMapper.BuildSharedKeyMap(languages);

        Dictionary<NodeId, IReadableNode> sharedNodeMap = [];

        var commandSender =
            new DeltaProtocolPartitionCommandSender(node.GetPublisher(), new CommandIdProvider(), lionWebVersion);

        var partitionEventHandler = new PartitionEventHandler(null);

        var deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)
            ;

        var eventReceiver = new DeltaProtocolPartitionEventReceiver(
            partitionEventHandler,
            sharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder
        );

        var commandToEventMapper = new CommandToEventMapper(sharedNodeMap);
        commandSender.DeltaCommand += (sender, command) => eventReceiver.Receive(commandToEventMapper.Map(command));

        var replicator = new PartitionEventReplicator(clone, sharedNodeMap);
        replicator.ReplicateFrom(partitionEventHandler);
        return clone;
    }
}

[TestClass]
public class EventsTestJson : EventTestsBase
{
    protected override Geometry CreateReplicator(Geometry node)
    {
        var clone = Clone(node);
        
        var lionWebVersion = LionWebVersions.v2024_1;
        List<Language> languages = [ShapesLanguage.Instance, lionWebVersion.BuiltIns, lionWebVersion.LionCore];

        Dictionary<CompressedMetaPointer, IKeyed> sharedKeyedMap = CommandToEventMapper.BuildSharedKeyMap(languages);

        Dictionary<NodeId, IReadableNode> sharedNodeMap = [];

        var commandSender =
            new DeltaProtocolPartitionCommandSender(node.GetPublisher(), new CommandIdProvider(), lionWebVersion);

        var partitionEventHandler = new PartitionEventHandler(null);

        var deserializerBuilder = new DeserializerBuilder()
                .WithLionWebVersion(lionWebVersion)
                .WithLanguages(languages)
            ;

        var eventReceiver = new DeltaProtocolPartitionEventReceiver(
            partitionEventHandler,
            sharedNodeMap,
            sharedKeyedMap,
            deserializerBuilder
        );

        var deltaSerializer = new DeltaSerializer();

        var commandToEventMapper = new CommandToEventMapper(sharedNodeMap);
        commandSender.DeltaCommand += (sender, command) =>
        {
            var json = deltaSerializer.Serialize(command);
            var deserialized = deltaSerializer.Deserialize<IDeltaCommand>(json);
            eventReceiver.Receive(commandToEventMapper.Map(deserialized));
        };

        var replicator = new PartitionEventReplicator(clone, sharedNodeMap);
        replicator.ReplicateFrom(partitionEventHandler);
        return clone;
    }
}

internal class CommandToEventMapper
{
    private readonly Dictionary<NodeId, IReadableNode> _sharedNodeMap;

    public CommandToEventMapper(Dictionary<NodeId, IReadableNode> sharedNodeMap)
    {
        _sharedNodeMap = sharedNodeMap;
    }

    public static Dictionary<CompressedMetaPointer, IKeyed> BuildSharedKeyMap(IEnumerable<Language> languages)
    {
        Dictionary<CompressedMetaPointer, IKeyed>  sharedKeyedMap = [];
        
        foreach (IKeyed keyed in languages.SelectMany(l => M1Extensions.Descendants<IKeyed>(l)))
        {
            var metaPointer = keyed switch
            {
                LanguageEntity l => l.ToMetaPointer(),
                Feature feat => feat.ToMetaPointer(),
                EnumerationLiteral l => l.GetEnumeration().ToMetaPointer(),
                _ => throw new NotImplementedException(keyed.GetType().Name)
            };

            sharedKeyedMap[CompressedMetaPointer.Create(metaPointer, true)] = keyed;
        }
        
        return sharedKeyedMap;
    }
    
    public IDeltaEvent Map(IDeltaCommand deltaCommand) =>
        deltaCommand switch
        {
            AddProperty a => new PropertyAdded(a.Property, a.NewValue, OriginCommands(a), null),
            DeleteProperty a => new PropertyDeleted(a.Property, null, OriginCommands(a), null),
            ChangeProperty a => new PropertyChanged(a.Property, a.NewValue, null, OriginCommands(a), null),
            AddChild a => new ChildAdded(a.Containment, a.NewChild, OriginCommands(a), null),
            DeleteChild a => new ChildDeleted(a.Containment, new([]), OriginCommands(a), null),
            ReplaceChild a => new ChildReplaced(a.Containment, a.NewChild, new([]), OriginCommands(a), null),
            MoveChildFromOtherContainment a => new ChildMovedFromOtherContainment(a.NewContainment, a.MovedChild, ContainmentParent(a.MovedChild), OriginCommands(a), null),
            MoveChildFromOtherContainmentInSameParent a => new ChildMovedFromOtherContainmentInSameParent(a.NewContainment, a.NewIndex, a.MovedChild, ContainmentParent(a.MovedChild).Parent, ContainmentParent(a.MovedChild).Containment, ContainmentParent(a.MovedChild).Index, OriginCommands(a), null),
            MoveChildInSameContainment a => new ChildMovedInSameContainment(a.NewIndex, a.MovedChild, ContainmentParent(a.MovedChild).Parent, ContainmentParent(a.MovedChild).Containment, ContainmentParent(a.MovedChild).Index, OriginCommands(a), null),
            AddAnnotation a => new AnnotationAdded(a.Parent, a.NewAnnotation, OriginCommands(a), null),
            DeleteAnnotation a => new AnnotationDeleted(a.Parent, new([]), OriginCommands(a), null),
            ReplaceAnnotation a => new AnnotationReplaced(a.Parent, a.NewAnnotation, new([]), OriginCommands(a), null),
            MoveAnnotationFromOtherParent a => new AnnotationMovedFromOtherParent(a.NewParent, a.MovedAnnotation, AnnotationParent(a.MovedAnnotation), OriginCommands(a), null),
            MoveAnnotationInSameParent a => new AnnotationMovedInSameParent(a.NewIndex, a.MovedAnnotation, AnnotationParent(a.MovedAnnotation).Parent, AnnotationParent(a.MovedAnnotation).Index, OriginCommands(a), null),
            AddReference a => new ReferenceAdded(a.Reference, a.NewTarget, OriginCommands(a), null),
            DeleteReference a => new ReferenceDeleted(a.Reference, new(), OriginCommands(a), null),
            ChangeReference a => new ReferenceChanged(a.Reference, a.NewTarget, new(), OriginCommands(a), null),
        };

    private DeltaContainment ContainmentParent(NodeId childId)
    {
        var child = (IWritableNode)_sharedNodeMap[childId];
        var parent = (IWritableNode)child.GetParent();
        var containment = parent.GetContainmentOf(child);
        return new DeltaContainment(parent.GetId(), containment.ToMetaPointer(),
            M2Extensions.AsNodes<IWritableNode>(parent.Get(containment)).ToList().IndexOf(child));
    }

    private DeltaAnnotation AnnotationParent(NodeId annotationId)
    {
        var annotation = _sharedNodeMap[annotationId];
        var parent = annotation.GetParent();
        return new DeltaAnnotation(parent.GetId(), parent.GetAnnotations().ToList().IndexOf(annotation));
    }

    private CommandSource[] OriginCommands(ISingleDeltaCommand a) => [new CommandSource(a.CommandId)];
}

internal class CommandIdProvider : ICommandIdProvider
{
    private int nextId = 0;
    public string Create() => (++nextId).ToString();
}