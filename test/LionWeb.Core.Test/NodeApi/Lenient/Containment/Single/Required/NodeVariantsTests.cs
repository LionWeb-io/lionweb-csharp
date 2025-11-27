// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Test.NodeApi.Lenient.Containment.Single.Required;

using Languages;
using Languages.Generated.V2024_1.Shapes.M2;
using M2;
using M3;
using System.Collections;

[TestClass]
public class NodeVariantsTests : LenientNodeTestsBase
{
    [TestMethod]
    public void INode_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var valueA = new Line("sA");
        var valueB = new Line("sB");
        var values = new List<INode> { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_offset) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_offset) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void DynamicNode_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        Classifier line = ShapesDynamic.Language.ClassifierByKey("key-Line");
        var valueA = new DynamicNode("sA", line);
        var valueB = new DynamicNode("sA", line);
        var values = new List<INode> { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_offset) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_offset) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void LenientNode_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        Classifier line = ShapesDynamic.Language.ClassifierByKey("key-Line");
        var valueA = new LenientNode("sA", line);
        var valueB = new LenientNode("sA", line);
        var values = new List<INode> { valueA, valueB };
        parent.Set(OffsetDuplicate_offset, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_offset) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(OffsetDuplicate_offset) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    [Ignore("fails after adding generic api")]
    public void IReadableNode_Reflective()
    {
        var parent = newOffsetDuplicate("g");
        var valueA = new ReadOnlyLine("sA", null) {Name = "nameA", Uuid = "uuidA", Start = new Coord("startA"), End = new Coord("endA")};
        var valueB = new ReadOnlyLine("sB", null) {Name = "nameB", Uuid = "uuidB", Start = new Coord("startB"), End = new Coord("endB")};
        var values = new ArrayList { valueA, valueB };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Set(OffsetDuplicate_offset, values));
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Get(OffsetDuplicate_offset));
    }
}