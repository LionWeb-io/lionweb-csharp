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

namespace LionWeb.Core.Test.Utilities.Hasher;

using Core.Notification;
using Core.Serialization;
using Core.Utilities;
using Languages.Generated.V2024_1.SDTLang;
using Languages.Generated.V2024_1.Shapes.M2;
using M3;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[TestClass]
public class HasherTests
{
    [TestMethod]
    public void EmptyList()
    {
        var hash = new Hasher([]).Hash();
        Assert.IsNotNull(hash);

        Assert.AreEqual(
            "SHA256_09-CE-E9-A6-FF-28-72-80-CC-5A-33-B6-1C-13-46-AE-F8-43-13-29-DD-58-FC-D1-78-91-67-23-AC-2F-7C-D6",
            hash.ToString()
        );
    }

    [TestMethod]
    public void IdIndependent()
    {
        var hashA = new Hasher([new Geometry("A")]).Hash();
        var hashB = new Hasher([new Geometry("B")]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Classifier()
    {
        var hashA = new Hasher([new Geometry("A")]).Hash();
        var hashB = new Hasher([new ReferenceGeometry("A")]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    #region Property

    [TestMethod]
    public void Property_Equal()
    {
        var hashA = new Hasher([new Circle("A") { Name = "X" }]).Hash();
        var hashB = new Hasher([new Circle("B") { Name = "X" }]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Property_DifferentValue_String()
    {
        var hashA = new Hasher([new Circle("A") { Name = "X" }]).Hash();
        var hashB = new Hasher([new Circle("A") { Name = "Y" }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Property_DifferentValue_Integer()
    {
        var hashA = new Hasher([new Coord("A") { X = 1 }]).Hash();
        var hashB = new Hasher([new Coord("A") { X = 2 }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Property_DifferentValue_Boolean()
    {
        var hashA = new Hasher([new Documentation("A") { Technical = true }]).Hash();
        var hashB = new Hasher([new Documentation("A") { Technical = false }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Property_DifferentValue_Enum()
    {
        var hashA = new Hasher([new MaterialGroup("A") { MatterState = MatterState.gas }]).Hash();
        var hashB = new Hasher([new MaterialGroup("A") { MatterState = MatterState.liquid }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Property_DifferentValue_Sdt()
    {
        var hashA = new Hasher([new SDTConcept("A") { Decimal = new Decimal(1, 0) }]).Hash();
        var hashB = new Hasher([new SDTConcept("A") { Decimal = new Decimal(0, 1) }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Property_DifferentValue_Null()
    {
        var hashA = new Hasher([new Circle("A") { Name = "X" }]).Hash();
        var hashB = new Hasher([new Circle("A")]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Property_DifferentProperties()
    {
        var hashA = new Hasher([new Circle("A") { Name = "X" }]).Hash();
        var hashB = new Hasher([new Circle("A") { Uuid = "X" }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Property_DifferentProperties_AmountToSameString()
    {
        var hashA = new Hasher([new Circle("A") { Name = "X", Uuid = "XX" }]).Hash();
        var hashB = new Hasher([new Circle("A") { Name = "XX", Uuid = "X" }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    #endregion

    #region Annotation

    [TestMethod]
    public void Annotation_Equal()
    {
        var a = new Circle("A");
        a.AddAnnotations([new Documentation("A")]);
        var hashA = new Hasher([a]).Hash();
        var b = new Circle("B");
        b.AddAnnotations([new Documentation("B")]);
        var hashB = new Hasher([b]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Annotation_DifferentElements_Empty()
    {
        var a = new Circle("A");
        a.AddAnnotations([new Documentation("A") { Text = "X" }]);
        var hashA = new Hasher([a]).Hash();
        var b = new Circle("B");
        var hashB = new Hasher([b]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Annotation_DifferentElements()
    {
        var a = new Circle("A");
        a.AddAnnotations([new Documentation("A") { Text = "X" }]);
        var hashA = new Hasher([a]).Hash();
        var b = new Circle("B");
        b.AddAnnotations([new Documentation("B") { Text = "Y" }]);
        var hashB = new Hasher([b]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Annotation_DifferentOrder()
    {
        var a = new Circle("A");
        a.AddAnnotations([new Documentation("A"), new BillOfMaterials("A")]);
        var hashA = new Hasher([a]).Hash();
        var b = new Circle("A");
        b.AddAnnotations([new BillOfMaterials("A"), new Documentation("A")]);
        var hashB = new Hasher([b]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    #endregion

    #region Containment

    [TestMethod]
    public void Containment_Equal()
    {
        var hashA = new Hasher([new Circle("A") { Center = new Coord("X") }]).Hash();
        var hashB = new Hasher([new Circle("B") { Center = new Coord("X") }]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Containment_DifferentElement()
    {
        var hashA = new Hasher([new Circle("A") { Center = new Coord("X") { X = 1 } }]).Hash();
        var hashB = new Hasher([new Circle("B") { Center = new Coord("Y") { X = 2 } }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Containment_DifferentElements_Empty()
    {
        var hashA = new Hasher([new Geometry("A") { Shapes = [new Line("A")] }]).Hash();
        var hashB = new Hasher([new Geometry("B") { }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Containment_DifferentElements()
    {
        var hashA = new Hasher([new Geometry("A") { Shapes = [new Line("A")] }]).Hash();
        var hashB = new Hasher([new Geometry("B") { Shapes = [new Circle("A")] }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Containment_DifferentOrder()
    {
        var hashA = new Hasher([new Geometry("A") { Shapes = [new Line("A"), new Circle("A")] }]).Hash();
        var hashB = new Hasher([new Geometry("B") { Shapes = [new Circle("A"), new Line("A")] }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Containment_DifferentOrder_AmountToSameContent()
    {
        var hashA = new Hasher(
            [new Geometry("A") { Shapes = [new Line("A") { Name = "X" }, new Line("A") { Name = "XX" }] }]).Hash();
        var hashB = new Hasher(
            [new Geometry("B") { Shapes = [new Line("A") { Name = "XX" }, new Line("A") { Name = "X" }] }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Containment_DifferentContainments()
    {
        var hashA = new Hasher([new CompositeShape("A") { Parts = [new Line("A")] }]).Hash();
        var hashB = new Hasher([new CompositeShape("B") { DisabledParts = [new Line("A")] }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    #endregion

    #region Reference

    #region Internal

    [TestMethod]
    public void Reference_Internal_Equal_Before()
    {
        var targetA = new Line("T");
        var hashA = new Hasher([targetA, new OffsetDuplicate("A") { Source = targetA }]).Hash();
        var targetB = new Line("T");
        var hashB = new Hasher([targetB, new OffsetDuplicate("B") { Source = targetB }]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_Internal_Equal_After()
    {
        var targetA = new Line("T");
        var hashA = new Hasher([new OffsetDuplicate("A") { Source = targetA }, targetA]).Hash();
        var targetB = new Line("T");
        var hashB = new Hasher([new OffsetDuplicate("B") { Source = targetB }, targetB]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_Internal_Equal_After_OtherId()
    {
        var targetA = new Line("Ta");
        var hashA = new Hasher([new OffsetDuplicate("A") { Source = targetA }, targetA]).Hash();
        var targetB = new Line("Tb");
        var hashB = new Hasher([new OffsetDuplicate("B") { Source = targetB }, targetB]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_Internal_Different_Before()
    {
        var targetA = new Line("T");
        var hashA = new Hasher([targetA, new OffsetDuplicate("A") { Source = targetA }]).Hash();
        var targetB = new Circle("T");
        var hashB = new Hasher([targetB, new OffsetDuplicate("B") { Source = targetB }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_Internal_Different_After()
    {
        var targetA = new Line("T");
        var hashA = new Hasher([new OffsetDuplicate("A") { Source = targetA }, targetA]).Hash();
        var targetB = new Circle("T");
        var hashB = new Hasher([new OffsetDuplicate("B") { Source = targetB }, targetB]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    #endregion

    #region External

    [TestMethod]
    public void Reference_External_Same()
    {
        var target = new Line("T");
        var hashA = new Hasher([new OffsetDuplicate("A") { Source = target }]).Hash();
        var hashB = new Hasher([new OffsetDuplicate("B") { Source = target }]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_External_Equal()
    {
        var targetA = new Line("T");
        var hashA = new Hasher([new OffsetDuplicate("A") { Source = targetA }]).Hash();
        var targetB = new Line("T");
        var hashB = new Hasher([new OffsetDuplicate("B") { Source = targetB }]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_External_SameId()
    {
        var targetA = new Line("T");
        var hashA = new Hasher([new OffsetDuplicate("A") { Source = targetA }]).Hash();
        var targetB = new Circle("T");
        var hashB = new Hasher([new OffsetDuplicate("B") { Source = targetB }]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_External_DifferentId()
    {
        var targetA = new Line("T");
        var hashA = new Hasher([new OffsetDuplicate("A") { Source = targetA }]).Hash();
        var targetB = new Circle("TT");
        var hashB = new Hasher([new OffsetDuplicate("B") { Source = targetB }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_External_MultipleReferences()
    {
        var externalTarget = new Line("T");
        var internalTarget = new Circle("TT");

        var hashA = new Hasher([
            new OffsetDuplicate("A") { Source = internalTarget },
            new OffsetDuplicate("A") { Source = externalTarget },
            internalTarget,
            new OffsetDuplicate("A") { Source = externalTarget },
        ]).Hash();

        var hashB = new Hasher([
            new OffsetDuplicate("A") { Source = internalTarget },
            new OffsetDuplicate("A") { Source = externalTarget },
            internalTarget,
            new OffsetDuplicate("A") { Source = externalTarget },
        ]).Hash();

        Assert.AreEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_External_LookupCount()
    {
        var externalTarget = new Line("T");
        var internalTarget = new Circle("TT");

        var hasherBefore = new Hasher([
            internalTarget,
            new OffsetDuplicate("A") { Source = internalTarget },
            new OffsetDuplicate("A") { Source = externalTarget },
            new OffsetDuplicate("A") { Source = externalTarget },
        ]);
        hasherBefore.Hash();

        var hasherAfter = new Hasher([
            new OffsetDuplicate("A") { Source = internalTarget },
            new OffsetDuplicate("A") { Source = externalTarget },
            internalTarget,
            new OffsetDuplicate("A") { Source = externalTarget },
        ]);
        hasherAfter.Hash();

        // check that we added only one external reference
        Assert.AreEqual(2, GetNextReferenceIndex(hasherBefore));
        Assert.AreEqual(2, GetNextReferenceIndex(hasherAfter));
    }

    private static int GetNextReferenceIndex(Hasher hasher) =>
        (int)typeof(Hasher)
            .GetRuntimeFields()
            .First(f => f.Name == "_nextReferenceIndex")
            .GetValue(hasher)!;

    #endregion

    [TestMethod]
    public void Reference_DifferentElements_Empty()
    {
        var targetA = new Line("T");
        var hashA = new Hasher([new ReferenceGeometry("A") { Shapes = [targetA] }]).Hash();
        var hashB = new Hasher([new ReferenceGeometry("B") { }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_DifferentElements()
    {
        var targetA = new Line("T");
        var hashA = new Hasher([new ReferenceGeometry("A") { Shapes = [targetA] }]).Hash();
        var targetB = new Circle("TT");
        var hashB = new Hasher([new ReferenceGeometry("B") { Shapes = [targetB] }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_DifferentOrder()
    {
        var target1 = new Line("X");
        var target2 = new Line("Y");
        var hashA = new Hasher([new ReferenceGeometry("A") { Shapes = [target1, target2] }]).Hash();
        var hashB = new Hasher([new ReferenceGeometry("B") { Shapes = [target2, target1] }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_DifferentOrder_AmountsToSameConcatenatedTargetNodeIds()
    {
        var target1 = new Line("T");
        var target2 = new Line("TT");
        var hashA = new Hasher([new ReferenceGeometry("A") { Shapes = [target1, target2] }]).Hash();
        var hashB = new Hasher([new ReferenceGeometry("B") { Shapes = [target2, target1] }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    [TestMethod]
    public void Reference_External_SpoofSeparatorId()
    {
        var hashA = new Hasher([new ReferenceGeometry("A") { Shapes = [new SpoofNode("T\0TT")] }]).Hash();
        var hashB =
            new Hasher([new ReferenceGeometry("B") { Shapes = [new SpoofNode("T"), new SpoofNode("TT")] }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    class SpoofNode(string id) : IShape
    {
        public string GetId() => id;

        public INode? GetParent() => null;

        public IReadOnlyList<INode> GetAnnotations() => [];

        public Classifier GetClassifier() => ShapesLanguage.Instance.IShape;

        public IEnumerable<Feature> CollectAllSetFeatures() => [];

        public object? Get(Feature feature) => throw new UnknownFeatureException(GetClassifier(), feature);
        public bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
        {
            value = null;
            return false;
        }

        public void DetachFromParent() { }

        public void Set(Feature feature, object? value, INotificationId? notificationId = null) { }

        public void SetParent(INode? parent) { }

        public bool DetachChild(INode child) => false;

        public Containment? GetContainmentOf(INode child) => null;

        public void AddAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null) { }

        public void InsertAnnotations(Int32 index, IEnumerable<INode> annotations, INotificationId? notificationId = null) { }

        public bool RemoveAnnotations(IEnumerable<INode> annotations, INotificationId? notificationId = null) => false;

        public IReadOnlyList<Coord> Fixpoints { get => []; init { } }
        public IShape AddFixpoints(IEnumerable<Coord> nodes, INotificationId? notificationId = null) => this;

        public IShape InsertFixpoints(int index, IEnumerable<Coord> nodes, INotificationId? notificationId = null) => this;

        public IShape RemoveFixpoints(IEnumerable<Coord> nodes, INotificationId? notificationId = null) => this;
        
        public string Uuid { get => null; set { } }
        public IShape SetUuid(string value, INotificationId? notificationId = null) => this;
    }

    [TestMethod]
    public void Reference_DifferentReferences()
    {
        var target = new Line("T");
        var hashA = new Hasher([new OffsetDuplicate("A") { Source = target }]).Hash();
        var hashB = new Hasher([new OffsetDuplicate("B") { AltSource = target }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }

    #endregion

    #region MetaPointer

    [TestMethod]
    public void MetaPointer_Same()
    {
        var hasherA = new TestHasher([]);
        var hasherB = new TestHasher([]);

        hasherA.HashMetaPointer(new MetaPointer("A", "B", "C"));
        hasherB.HashMetaPointer(new MetaPointer("A", "B", "C"));

        Assert.AreEqual(hasherA.Hash(), hasherB.Hash());
    }

    [TestMethod]
    public void MetaPointer_Language()
    {
        var hasherA = new TestHasher([]);
        var hasherB = new TestHasher([]);

        hasherA.HashMetaPointer(new MetaPointer("A", "B", "C"));
        hasherB.HashMetaPointer(new MetaPointer("B", "B", "C"));

        Assert.AreNotEqual(hasherA.Hash(), hasherB.Hash());
    }

    [TestMethod]
    public void MetaPointer_Key()
    {
        var hasherA = new TestHasher([]);
        var hasherB = new TestHasher([]);

        hasherA.HashMetaPointer(new MetaPointer("A", "B", "C"));
        hasherB.HashMetaPointer(new MetaPointer("A", "B", "A"));

        Assert.AreNotEqual(hasherA.Hash(), hasherB.Hash());
    }

    [TestMethod]
    public void MetaPointer_Version()
    {
        var hasherA = new TestHasher([]);
        var hasherB = new TestHasher([]);

        hasherA.HashMetaPointer(new MetaPointer("A", "B", "C"));
        hasherB.HashMetaPointer(new MetaPointer("A", "C", "C"));

        Assert.AreNotEqual(hasherA.Hash(), hasherB.Hash());
    }

    #endregion

    [TestMethod]
    public void ByteArrayHash_Short()
    {
        Assert.ThrowsException<ArgumentException>(() => new ByteArrayHash("a", [1]));
    }

    [TestMethod]
    public void ByteArrayHash_Same()
    {
        var a = new ByteArrayHash("aaa", [1, 2, 3, 4]);
        var b = new ByteArrayHash("aaa", [1, 2, 3, 4]);

        Assert.AreEqual(a, b);
        Assert.AreEqual(a.ToString(), b.ToString());
    }

    [TestMethod]
    public void ByteArrayHash_DifferentAlgorithm()
    {
        var a = new ByteArrayHash("aaa", [1, 2, 3, 4]);
        var b = new ByteArrayHash("bbb", [1, 2, 3, 4]);

        Assert.AreNotEqual(a, b);
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        Assert.AreNotEqual(a.ToString(), b.ToString());
    }

    [TestMethod]
    public void ByteArrayHash_DifferentHash()
    {
        var a = new ByteArrayHash("aaa", [1, 2, 3, 4]);
        var b = new ByteArrayHash("aaa", [4, 3, 2, 1]);

        Assert.AreNotEqual(a, b);
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        Assert.AreNotEqual(a.ToString(), b.ToString());
    }
}

internal class TestHasher(IList<IReadableNode> nodes, LionWebVersions? lionWebVersion = null)
    : Hasher(nodes, lionWebVersion)
{
    public void HashMetaPointer(MetaPointer metaPointer) => base.MetaPointer(metaPointer);
}