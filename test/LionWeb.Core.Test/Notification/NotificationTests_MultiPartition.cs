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
using Notification.Partition;
using Notification.Processor;
using Comparer = Core.Utilities.Comparer;
using NotificationId = string;

[TestClass]
public class NotificationTests_MultiPartition
{
    #region Children

    #region ChildMovedFromOtherContainment

    [TestMethod]
    public void ChildMovedFromOtherContainment_Multiple_SamePartition()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var node = new Geometry("a") { Shapes = [origin] };

        var clone = new Geometry("a") { Shapes = [new CompositeShape("origin") { Parts = [new Circle("moved")] }] };

        var replicator = CreateReplicator(clone, node);

        List<(NotificationId, IReadableNode)> deletions = [];
        node.GetProcessor().Subscribe<ChildDeletedNotification>((o, e) => deletions.Add((e.NotificationId.ToString(), e.DeletedChild)));
        List<(NotificationId, IReadableNode)> moves = [];
        node.GetProcessor()
            .Subscribe<ChildMovedFromOtherContainmentNotification>((o, e) => moves.Add((e.NotificationId.ToString(), e.MovedChild)));

        node.AddShapes([moved]);

        AssertEquals([node], [clone]);
        Assert.IsFalse(deletions.Any());
        CollectionAssert.Contains(moves.Select(it => it.Item2).ToList(), moved);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Multiple_DifferentPartition_Add()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originPartition = new Geometry("g") { Shapes = [origin] };

        var node = new Geometry("a") { Shapes = [] };

        var clone = new Geometry("a") { Shapes = [] };

        var replicator = CreateReplicator(clone, node);

        List<(NotificationId, IReadableNode)> originMoves = [];
        originPartition.GetProcessor()
            .Subscribe<ChildMovedFromOtherContainmentNotification>((o, e) => originMoves.Add((e.NotificationId.ToString(), e.MovedChild)));
        List<(NotificationId, IReadableNode)> destinationMoves = [];
        node.GetProcessor()
            .Subscribe<ChildMovedFromOtherContainmentNotification>((o, e) => destinationMoves.Add((e.NotificationId.ToString(), e.MovedChild)));

        node.AddShapes([moved]);

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Multiple_DifferentPartition_Insert()
    {
        var moved = new Circle("moved");
        var origin = new CompositeShape("origin") { Parts = [moved] };
        var originPartition = new Geometry("g") { Shapes = [origin] };

        var node = new Geometry("a") { Shapes = [] };

        var clone = new Geometry("a") { Shapes = [] };

        var replicator = CreateReplicator(clone, node);

        List<(NotificationId, IReadableNode)> originMoves = [];
        originPartition.GetProcessor()
            .Subscribe<ChildMovedFromOtherContainmentNotification>((o, e) => originMoves.Add((e.NotificationId.ToString(), e.MovedChild)));
        List<(NotificationId, IReadableNode)> destinationMoves = [];
        node.GetProcessor()
            .Subscribe<ChildMovedFromOtherContainmentNotification>((o, e) => destinationMoves.Add((e.NotificationId.ToString(), e.MovedChild)));

        node.InsertShapes(0, [moved]);

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Single_SamePartition()
    {
        var moved = new Documentation("moved");
        var node = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var clone = new Geometry("a") { Shapes = [new Line("l") { ShapeDocs = new Documentation("moved") }] };

        var replicator = CreateReplicator(clone, node);

        List<(NotificationId, IReadableNode)> deletions = [];
        node.GetProcessor().Subscribe<ChildDeletedNotification>((o, e) => deletions.Add((e.NotificationId.ToString(), e.DeletedChild)));
        List<(NotificationId, IReadableNode)> moves = [];
        node.GetProcessor()
            .Subscribe<ChildMovedFromOtherContainmentNotification>((o, e) => moves.Add((e.NotificationId.ToString(), e.MovedChild)));

        node.Documentation = moved;

        AssertEquals([node], [clone]);
        Assert.IsFalse(deletions.Any());
        CollectionAssert.Contains(moves.Select(it => it.Item2).ToList(), moved);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment_Single_DifferentPartition()
    {
        var moved = new Documentation("moved");
        var originPartition = new Geometry("g") { Shapes = [new Line("l") { ShapeDocs = moved }] };

        var node = new Geometry("a") { };

        var clone = new Geometry("a") { };

        var replicator = CreateReplicator(clone, node);

        List<(NotificationId, IReadableNode)> originMoves = [];
        originPartition.GetProcessor()
            .Subscribe<ChildMovedFromOtherContainmentNotification>((o, e) => originMoves.Add((e.NotificationId.ToString(), e.MovedChild)));
        List<(NotificationId, IReadableNode)> destinationMoves = [];
        node.GetProcessor()
            .Subscribe<ChildMovedFromOtherContainmentNotification>((o, e) => destinationMoves.Add((e.NotificationId.ToString(), e.MovedChild)));

        node.Documentation = moved;

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    #endregion

    #endregion

    #region Annotations

    #region AnnotationMovedFromOtherParent

    [TestMethod]
    public void AnnotationMovedFromOtherParent_Multiple_SamePartition()
    {
        var moved = new BillOfMaterials("moved");
        var origin = new CompositeShape("origin");
        origin.AddAnnotations([moved]);
        var node = new Geometry("a") { Shapes = [origin] };

        var cloneOrigin = new CompositeShape("origin");
        cloneOrigin.AddAnnotations([new BillOfMaterials("moved")]);
        var clone = new Geometry("a") { Shapes = [cloneOrigin] };

        var replicator = CreateReplicator(clone, node);

        List<(NotificationId, IReadableNode)> deletions = [];
        node.GetProcessor()
            .Subscribe<AnnotationDeletedNotification>((o, e) => deletions.Add((e.NotificationId.ToString(), e.DeletedAnnotation)));
        List<(NotificationId, IReadableNode)> moves = [];
        node.GetProcessor()
            .Subscribe<AnnotationMovedFromOtherParentNotification>((o, e) => moves.Add((e.NotificationId.ToString(), e.MovedAnnotation)));

        node.AddAnnotations([moved]);

        AssertEquals([node], [clone]);
        Assert.IsFalse(deletions.Any());
        CollectionAssert.Contains(moves.Select(it => it.Item2).ToList(), moved);
    }

    [TestMethod]
    public void AnnotationMovedFromOtherParent_Multiple_DifferentPartition_Add()
    {
        var moved = new BillOfMaterials("moved");
        var origin = new CompositeShape("origin");
        origin.AddAnnotations([moved]);
        var originPartition = new Geometry("g") { Shapes = [origin] };

        var node = new Geometry("a") { };

        var clone = new Geometry("a") { };

        var replicator = CreateReplicator(clone, node);

        List<(NotificationId, IReadableNode)> originMoves = [];
        originPartition.GetProcessor()
            .Subscribe<AnnotationMovedFromOtherParentNotification>((o, e) => originMoves.Add((e.NotificationId.ToString(), e.MovedAnnotation)));
        List<(NotificationId, IReadableNode)> destinationMoves = [];
        node.GetProcessor()
            .Subscribe<AnnotationMovedFromOtherParentNotification>((o, e) =>
                destinationMoves.Add((e.NotificationId.ToString(), e.MovedAnnotation)));

        node.AddAnnotations([moved]);

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    [TestMethod]
    public void AnnotationMovedFromOtherParent_Multiple_DifferentPartition_Insert()
    {
        var moved = new BillOfMaterials("moved");
        var origin = new CompositeShape("origin");
        origin.AddAnnotations([moved]);
        var originPartition = new Geometry("g") { Shapes = [origin] };

        var node = new Geometry("a") { };

        var clone = new Geometry("a") { };

        var replicator = CreateReplicator(clone, node);

        List<(NotificationId, IReadableNode)> originMoves = [];
        originPartition.GetProcessor()
            .Subscribe<AnnotationMovedFromOtherParentNotification>((o, e) => originMoves.Add((e.NotificationId.ToString(), e.MovedAnnotation)));
        List<(NotificationId, IReadableNode)> destinationMoves = [];
        node.GetProcessor()
            .Subscribe<AnnotationMovedFromOtherParentNotification>((o, e) =>
                destinationMoves.Add((e.NotificationId.ToString(), e.MovedAnnotation)));

        node.InsertAnnotations(0, [moved]);

        AssertEquals([node], [clone]);
        CollectionAssert.Contains(originMoves.Select(it => it.Item2).ToList(), moved);
        CollectionAssert.Contains(destinationMoves.Select(it => it.Item2).ToList(), moved);
        Assert.AreEqual(originMoves[0].Item1, destinationMoves[0].Item1);
    }

    #endregion

    #endregion

    private static INotificationProcessor<IPartitionNotification> CreateReplicator(Geometry clone, Geometry node)
    {
        var replicator = PartitionNotificationReplicator.Create(clone, new(), "cloneReplicator");
        var cloneProcessor = new NodeCloneProcessor<IPartitionNotification>(node.GetId());
        IProcessor.Connect((IPartitionProcessor)node.GetProcessor(), cloneProcessor);
        IProcessor.Connect(cloneProcessor, replicator);
        
        return replicator;
    }

    private void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
}