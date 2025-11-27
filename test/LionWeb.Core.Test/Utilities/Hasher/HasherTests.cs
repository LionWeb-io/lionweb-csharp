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

using Core.Serialization;
using Core.Utilities;
using Languages.Generated.V2024_1.SDTLang;
using Languages.Generated.V2024_1.Shapes.M2;

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
        Assert.ThrowsExactly<ArgumentException>(() => new ByteArrayHash("a", [1]));
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