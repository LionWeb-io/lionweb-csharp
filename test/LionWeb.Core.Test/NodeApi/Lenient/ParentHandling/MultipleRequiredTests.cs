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

namespace LionWeb.Core.Test.NodeApi.Lenient.ParentHandling;

using System.Collections;

[TestClass]
public class MultipleRequiredTests : LenientNodeTestsBase
{
    #region singleEntry

    #region SameInOtherInstance

    [TestMethod]
    public void MultipleRequired_Single_SameInOtherInstance_Reflective()
    {
        var child = newLine("myId");
        var source = newCompositeShape("src");
        source.Set(CompositeShape_parts, new List<INode> { child });
        var target = newCompositeShape("tgt");

        target.Set(CompositeShape_parts, new List<LenientNode> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { child }, target.Get(CompositeShape_parts) as IList);
        Assert.ThrowsException<UnsetFeatureException>(() => (source.Get(CompositeShape_parts) as IList).Count);
    }

    #endregion

    #region Other

    [TestMethod]
    public void MultipleRequired_Single_Other_Reflective()
    {
        var child = newLine("myId");
        var source = newCompositeShape("src");
        source.Set(CompositeShape_parts, new List<INode> { child });
        var target = newGeometry("tgt");

        target.Set(Geometry_shapes, new List<LenientNode> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { child }, target.Get(Geometry_shapes) as IList);
        Assert.ThrowsException<UnsetFeatureException>(() => (source.Get(CompositeShape_parts) as IList).Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleRequired_Single_OtherInSameInstance_Reflective()
    {
        var child = newLine("myId");
        var parent = newCompositeShape("src");
        parent.Set(CompositeShape_parts, new List<INode> { child });

        parent.Set(CompositeShape_disabledParts, new List<LenientNode> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { child }, parent.Get(CompositeShape_disabledParts) as IList);
        Assert.ThrowsException<UnsetFeatureException>(() => (parent.Get(CompositeShape_parts) as IList).Count);
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void MultipleRequired_Single_SameInSameInstance_Reflective()
    {
        var child = newLine("myId");
        var parent = newCompositeShape("src");
        parent.Set(CompositeShape_parts, new List<INode> { child });

        parent.Set(CompositeShape_parts, new List<LenientNode> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { child }, parent.Get(CompositeShape_parts) as IList);
    }

    #endregion

    #endregion

    #region someEntries

    #region SameInOtherInstance

    [TestMethod]
    public void MultipleRequired_Partial_SameInOtherInstance_Reflective()
    {
        var childA = newLine("a");
        var childB = newLine("b");
        var source = newCompositeShape("src");
        source.Set(CompositeShape_parts, new List<INode> { childA, childB });
        var target = newCompositeShape("tgt");

        target.Set(CompositeShape_parts, new List<LenientNode> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { childA }, target.Get(CompositeShape_parts) as IList);
        CollectionAssert.AreEqual(new List<LenientNode> { childB }, source.Get(CompositeShape_parts) as IList);
    }

    #endregion

    #region Other

    [TestMethod]
    public void MultipleRequired_Partial_Other_Reflective()
    {
        var childA = newLine("a");
        var childB = newLine("b");
        var source = newCompositeShape("src");
        source.Set(CompositeShape_parts, new List<INode> { childA, childB });
        var target = newGeometry("tgt");

        target.Set(Geometry_shapes, new List<LenientNode> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { childA }, target.Get(Geometry_shapes) as IList);
        CollectionAssert.AreEqual(new List<LenientNode> { childB }, source.Get(CompositeShape_parts) as IList);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleRequired_Partial_OtherInSameInstance_Reflective()
    {
        var childA = newLine("a");
        var childB = newLine("b");
        var parent = newCompositeShape("src");
        parent.Set(CompositeShape_parts, new List<INode> { childA, childB });

        parent.Set(CompositeShape_disabledParts, new List<LenientNode> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { childA }, parent.Get(CompositeShape_disabledParts) as IList);
        CollectionAssert.AreEqual(new List<LenientNode> { childB }, parent.Get(CompositeShape_parts) as IList);
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void MultipleRequired_Partial_SameInSameInstance_Reflective()
    {
        var childA = newLine("myId");
        var childB = newLine("b");
        var parent = newCompositeShape("src");
        parent.Set(CompositeShape_parts, new List<INode> { childA, childB });

        parent.Set(CompositeShape_parts, new List<LenientNode> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { childA }, parent.Get(CompositeShape_parts) as IList);
    }

    #endregion

    #endregion

    #region FromSingle

    #region Other

    [TestMethod]
    public void MultipleRequired_FromSingle_Other_Reflective()
    {
        var child = newLine("myId");
        var source = newCompositeShape("src");
        source.Set(CompositeShape_evilPart, child);
        var target = newGeometry("tgt");

        target.Set(Geometry_shapes, new List<LenientNode> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { child }, target.Get(Geometry_shapes) as IList);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Get(CompositeShape_evilPart));
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleRequired_FromSingle_OtherInSameInstance_Reflective()
    {
        var child = newLine("myId");
        var parent = newCompositeShape("src");
        parent.Set(CompositeShape_evilPart, child);

        parent.Set(CompositeShape_parts, new List<LenientNode> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<LenientNode> { child }, parent.Get(CompositeShape_parts) as IList);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(CompositeShape_evilPart));
    }

    #endregion

    #endregion

    #region ToSingle

    #region Other

    [TestMethod]
    public void MultipleRequired_ToSingle_Other_Reflective()
    {
        var child = newLine("myId");
        var source = newGeometry("src");
        source.Set(Geometry_shapes, new List<INode> { child });
        var target = newCompositeShape("tgt");

        target.Set(CompositeShape_evilPart, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(CompositeShape_evilPart));
        Assert.AreEqual(0, (source.Get(Geometry_shapes) as IList).Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleRequired_ToSingle_OtherInSameInstance_Reflective()
    {
        var child = newLine("myId");
        var parent = newCompositeShape("src");
        parent.Set(CompositeShape_parts, new List<INode> { child });

        parent.Set(CompositeShape_evilPart, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(CompositeShape_evilPart));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(CompositeShape_parts) as IList);
    }

    #endregion

    #endregion
}