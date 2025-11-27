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

namespace LionWeb.Core.Test.NodeApi.Lenient.Containment.Multiple.Optional;

using System.Collections;

[TestClass]
public class MultipleCollectionTests : LenientNodeTestsBase
{
    [TestMethod]
    public void Array_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new LenientNode[] { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void UntypedArray_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new object[] { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void UntypedList_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void ListMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void ListSubtype_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void Set_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new HashSet<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void SingleEnumerable_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new SingleEnumerable<LenientNode> { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void ListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new List<LenientNode>() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void UntypedListNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new ArrayList() { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void UntypedArrayNonMatchingType_Reflective()
    {
        var parent = newGeometry("g");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var values = new object[] { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes) as IEnumerable<IReadableNode>).Contains(valueB));
    }

    [TestMethod]
    public void ResultUnmodifiable_Set()
    {
        var parent = newGeometry("g");
        var valueA = newLine("sA");
        var valueB = newLine("sB");
        var values = new LenientNode[] { valueA, valueB };
        parent.Set(Geometry_shapes, values);
        var result = parent.Get(Geometry_shapes);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }

    [TestMethod]
    public void ResultUnmodifiable_Unset()
    {
        var parent = newGeometry("g");
        var result = parent.Get(Geometry_shapes);
        Assert.IsInstanceOfType<IReadOnlyList<INode>>(result);
    }
}