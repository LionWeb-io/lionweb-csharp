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

namespace LionWeb.Core.Test.NodeApi.Dynamic.ParentHandling;

using System.Collections;

[TestClass]
public class MultipleOptionalTests : DynamicNodeTestsBase
{
    #region singleEntry

    #region SameInOtherInstance

    [TestMethod]
    public void Single_SameInOtherInstance_Reflective()
    {
        var child = newLine("myId");
        var source = newGeometry("src");
        source.Set(Geometry_shapes, new List<INode> { child });
        var target = newGeometry("tgt");

        target.Set(Geometry_shapes, new List<DynamicNode> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, target.Get(Geometry_shapes) as IList);
        Assert.AreEqual(0, (source.Get(Geometry_shapes) as IList).Count);
    }

    #endregion

    #region Other

    [TestMethod]
    public void Single_Other_Reflective()
    {
        var child = newLine("myId");
        var source = newGeometry("src");
        source.Set(Geometry_shapes, new List<INode> { child });
        var target = newCompositeShape("tgt");

        target.Set(CompositeShape_parts, new List<DynamicNode> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, target.Get(CompositeShape_parts) as IList);
        Assert.AreEqual(0, (source.Get(Geometry_shapes) as IList).Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void Single_OtherInSameInstance_Reflective()
    {
        var child = newMaterialGroup("myId");
        var parent = newBillOfMaterials("src");
        parent.Set(BillOfMaterials_groups, new List<INode> { child });

        parent.Set(BillOfMaterials_altGroups, new List<DynamicNode> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, parent.Get(BillOfMaterials_altGroups) as IList);
        Assert.AreEqual(0, (parent.Get(BillOfMaterials_groups) as IList).Count);
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void Single_SameInSameInstance_Reflective()
    {
        var child = newMaterialGroup("myId");
        var parent = newBillOfMaterials("src");
        parent.Set(BillOfMaterials_groups, new List<INode> { child });

        parent.Set(BillOfMaterials_groups, new List<DynamicNode> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, parent.Get(BillOfMaterials_groups) as IList);
    }

    #endregion

    #endregion

    #region someEntries

    #region SameInOtherInstance

    [TestMethod]
    public void Partial_SameInOtherInstance_Reflective()
    {
        var childA = newLine("a");
        var childB = newLine("b");
        var source = newGeometry("src");
        source.Set(Geometry_shapes, new List<INode> { childA,childB });
        var target = newGeometry("tgt");

        target.Set(Geometry_shapes, new List<DynamicNode> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { childA }, target.Get(Geometry_shapes) as IList);
        CollectionAssert.AreEqual(new List<DynamicNode> { childB }, source.Get(Geometry_shapes) as IList);
    }

    #endregion

    #region Other

    [TestMethod]
    public void Partial_Other_Reflective()
    {
        var childA = newLine("a");
        var childB = newLine("b");
        var source = newGeometry("src");
        source.Set(Geometry_shapes, new List<INode> { childA,childB });
        var target = newCompositeShape("tgt");

        target.Set(CompositeShape_parts, new List<DynamicNode> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { childA }, target.Get(CompositeShape_parts) as IList);
        CollectionAssert.AreEqual(new List<DynamicNode> { childB }, source.Get(Geometry_shapes) as IList);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void Partial_OtherInSameInstance_Reflective()
    {
        var childA = newMaterialGroup("a");
        var childB = newMaterialGroup("b");
        var parent = newBillOfMaterials("src");
        parent.Set(BillOfMaterials_groups, new List<INode> { childA, childB });

        parent.Set(BillOfMaterials_altGroups, new List<DynamicNode> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { childA }, parent.Get(BillOfMaterials_altGroups) as IList);
        CollectionAssert.AreEqual(new List<DynamicNode> { childB }, parent.Get(BillOfMaterials_groups) as IList);
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void Partial_SameInSameInstance_Reflective()
    {
        var childA = newMaterialGroup("myId");
        var childB = newMaterialGroup("b");
        var parent = newBillOfMaterials("src");
        parent.Set(BillOfMaterials_groups, new List<INode> { childA, childA });

        parent.Set(BillOfMaterials_groups, new List<DynamicNode> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { childA }, parent.Get(BillOfMaterials_groups) as IList);
    }

    #endregion

    #endregion

    #region FromSingle

    #region Other

    [TestMethod]
    public void FromSingle_Other_Reflective()
    {
        var child = newLine("myId");
        var source = newMaterialGroup("src");
        source.Set(MaterialGroup_defaultShape, child);
        var target = newGeometry("tgt");

        target.Set(Geometry_shapes, new List<DynamicNode> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, target.Get(Geometry_shapes) as IList);
        Assert.IsNull(source.Get(MaterialGroup_defaultShape));
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void FromSingle_OtherInSameInstance_Reflective()
    {
        var child = newMaterialGroup("myId");
        var parent = newBillOfMaterials("src");
        parent.Set(BillOfMaterials_defaultGroup, child);

        parent.Set(BillOfMaterials_altGroups, new List<DynamicNode> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, parent.Get(BillOfMaterials_altGroups) as IList);
        Assert.IsNull(parent.Get(BillOfMaterials_defaultGroup));
    }

    #endregion

    #endregion

    #region ToSingle

    #region Other

    [TestMethod]
    public void ToSingle_Other_Reflective()
    {
        var child = newLine("myId");
        var source = newGeometry("src");
        source.Set(Geometry_shapes, new List<INode> { child });
        var target = newMaterialGroup("tgt");

        target.Set(MaterialGroup_defaultShape, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(MaterialGroup_defaultShape));
        Assert.AreEqual(0, (source.Get(Geometry_shapes) as IList).Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void ToSingle_OtherInSameInstance_Reflective()
    {
        var child = newMaterialGroup("myId");
        var parent = newBillOfMaterials("src");
        parent.Set(BillOfMaterials_altGroups, new List<INode> { child });

        parent.Set(BillOfMaterials_defaultGroup, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(BillOfMaterials_defaultGroup));
        Assert.AreEqual(0, (parent.Get(BillOfMaterials_altGroups) as IList).Count);
    }

    #endregion

    #endregion
}