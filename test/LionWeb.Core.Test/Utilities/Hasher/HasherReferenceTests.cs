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

using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using System.Reflection;

[TestClass]
public class HasherReferenceTests
{
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

    [TestMethod]
    public void Reference_DifferentReferences()
    {
        var target = new Line("T");
        var hashA = new Hasher([new OffsetDuplicate("A") { Source = target }]).Hash();
        var hashB = new Hasher([new OffsetDuplicate("B") { AltSource = target }]).Hash();

        Assert.AreNotEqual(hashA, hashB);
    }
}