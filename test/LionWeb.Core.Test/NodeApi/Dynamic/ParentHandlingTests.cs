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

namespace LionWeb.Core.Test.NodeApi.Dynamic;

using System.Collections;

[TestClass]
public class ParentHandlingTests : DynamicNodeTestsBase
{
    #region single

    #region optional

    #region SameInOtherInstance

    [TestMethod]
    public void SingleOptional_SameInOtherInstance_Reflective()
    {
        var child = newDocumentation("myId");
        var source = newGeometry("src");
        source.Set(Geometry_documentation, child);
        var target = newGeometry("tgt");

        target.Set(Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(Geometry_documentation));
        Assert.IsNull(source.Get(Geometry_documentation));
    }

    [TestMethod]
    public void SingleOptional_SameInOtherInstance_detach_Reflective()
    {
        var child = newDocumentation("myId");
        var source = newGeometry("src");
        source.Set(Geometry_documentation, child);
        var orphan = newDocumentation("o");
        var target = newGeometry("tgt");
        target.Set(Geometry_documentation, orphan);

        target.Set(Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(Geometry_documentation));
        Assert.IsNull(source.Get(Geometry_documentation));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region other

    [TestMethod]
    public void SingleOptional_Other_Reflective()
    {
        var child = newDocumentation("myId");
        var source = newGeometry("src");
        source.Set(Geometry_documentation, child);
        var target = newOffsetDuplicate("tgt");

        target.Set(OffsetDuplicate_docs, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(OffsetDuplicate_docs));
        Assert.IsNull(source.Get(Geometry_documentation));
    }

    [TestMethod]
    public void SingleOptional_Other_detach_Reflective()
    {
        var child = newDocumentation("myId");
        var source = newGeometry("src");
        source.Set(Geometry_documentation, child);
        var orphan = newDocumentation("o");
        var target = newOffsetDuplicate("tgt");
        target.Set(OffsetDuplicate_docs, orphan);

        target.Set(OffsetDuplicate_docs, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(OffsetDuplicate_docs));
        Assert.IsNull(source.Get(Geometry_documentation));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region otherInSameInstance

    [TestMethod]
    public void SingleOptional_OtherInSameInstance_Reflective()
    {
        var child = newDocumentation("myId");
        var parent = newOffsetDuplicate("src");
        parent.Set(OffsetDuplicate_docs, child);

        parent.Set(OffsetDuplicate_secretDocs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(OffsetDuplicate_secretDocs));
        Assert.IsNull(parent.Get(OffsetDuplicate_docs));
    }

    [TestMethod]
    public void SingleOptional_OtherInSameInstance_detach_Reflective()
    {
        var child = newDocumentation("myId");
        var orphan = newDocumentation("o");
        var parent = newOffsetDuplicate("src");
        parent.Set(OffsetDuplicate_docs, child);
        parent.Set(OffsetDuplicate_secretDocs, orphan);

        parent.Set(OffsetDuplicate_secretDocs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(OffsetDuplicate_secretDocs));
        Assert.IsNull(parent.Get(OffsetDuplicate_docs));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region sameInSameInstance

    [TestMethod]
    public void SingleOptional_SameInSameInstance_Reflective()
    {
        var child = newDocumentation("myId");
        var parent = newOffsetDuplicate("src");
        parent.Set(OffsetDuplicate_docs, child);

        parent.Set(OffsetDuplicate_docs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(OffsetDuplicate_docs));
    }

    #endregion

    #region annotation

    [TestMethod]
    public void SingleOptional_ToAnnotation()
    {
        var child = newDocumentation("myId");
        var source = newGeometry("src");
        source.Set(Geometry_documentation, child);
        var target = newLine("tgt");

        target.AddAnnotations([child]);

        Assert.AreSame(target, child.GetParent());
        Assert.IsTrue(target.GetAnnotations().Contains(child));
        Assert.IsFalse(source.GetAnnotations().Contains(child));
    }

    [TestMethod]
    public void SingleOptional_ToAnnotation_Reflective()
    {
        var child = newDocumentation("myId");
        var source = newGeometry("src");
        source.Set(Geometry_documentation, child);
        var target = newLine("tgt");

        target.Set(null, new List<INode> { child });

        Assert.AreSame(target, child.GetParent());
        Assert.IsTrue(target.GetAnnotations().Contains(child));
        Assert.IsFalse(source.GetAnnotations().Contains(child));
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_Reflective()
    {
        var child = newDocumentation("myId");
        var source = newLine("src");
        source.AddAnnotations([child]);
        var target = newGeometry("tgt");

        target.Set(Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Get(Geometry_documentation));
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_detach_Reflective()
    {
        var child = newDocumentation("myId");
        var orphan = newDocumentation("o");
        var source = newLine("src");
        source.AddAnnotations([child]);
        var target = newGeometry("tgt");
        target.Set(Geometry_documentation, orphan);

        target.Set(Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Get(Geometry_documentation));
        Assert.IsNull(orphan.GetParent());
    }

    #region sameInstance

    [TestMethod]
    public void SingleOptional_ToAnnotation_SameInstance()
    {
        var child = newDocumentation("myId");
        var parent = newOffsetDuplicate("src");
        parent.Set(OffsetDuplicate_docs, child);

        parent.AddAnnotations([child]);

        Assert.AreSame(parent, child.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(child));
        Assert.IsNull(parent.Get(OffsetDuplicate_docs));
    }

    [TestMethod]
    public void SingleOptional_ToAnnotation_SameInstance_Reflective()
    {
        var child = newDocumentation("myId");
        var parent = newOffsetDuplicate("src");
        parent.Set(OffsetDuplicate_docs, child);

        parent.Set(null, new List<INode> { child });

        Assert.AreSame(parent, child.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(child));
        Assert.IsNull(parent.Get(OffsetDuplicate_docs));
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance_Reflective()
    {
        var child = newDocumentation("myId");
        var parent = newOffsetDuplicate("src");
        parent.AddAnnotations([child]);

        parent.Set(OffsetDuplicate_docs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Get(OffsetDuplicate_docs));
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance_detach_Reflective()
    {
        var child = newDocumentation("myId");
        var orphan = newDocumentation("o");
        var parent = newOffsetDuplicate("src");
        parent.Set(OffsetDuplicate_docs, orphan);
        parent.AddAnnotations([child]);

        parent.Set(OffsetDuplicate_docs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Get(OffsetDuplicate_docs));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #endregion

    #endregion

    #region required

    #region SameInOtherInstance

    [TestMethod]
    public void SingleRequired_SameInOtherInstance_Reflective()
    {
        var child = newCoord("myId");
        var source = newOffsetDuplicate("src");
        source.Set(OffsetDuplicate_offset, child);
        var target = newOffsetDuplicate("tgt");

        target.Set(OffsetDuplicate_offset, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(OffsetDuplicate_offset));
        Assert.ThrowsException<UnsetFeatureException>(() => source.Get(OffsetDuplicate_offset));
    }

    [TestMethod]
    public void SingleRequired_SameInOtherInstance_detach_Reflective()
    {
        var child = newCoord("myId");
        var orphan = newCoord("o");
        var source = newOffsetDuplicate("src");
        source.Set(OffsetDuplicate_offset, child);
        var target = newOffsetDuplicate("tgt");
        target.Set(OffsetDuplicate_offset, orphan);

        target.Set(OffsetDuplicate_offset, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(OffsetDuplicate_offset));
        Assert.ThrowsException<UnsetFeatureException>(() => source.Get(OffsetDuplicate_offset));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region other

    [TestMethod]
    public void SingleRequired_Other_Reflective()
    {
        var child = newCoord("myId");
        var source = newOffsetDuplicate("src");
        source.Set(OffsetDuplicate_offset, child);
        var target = newCircle("tgt");

        target.Set(Circle_center, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(Circle_center));
        Assert.ThrowsException<UnsetFeatureException>(() => source.Get(OffsetDuplicate_offset));
    }

    [TestMethod]
    public void SingleRequired_Other_detach_Reflective()
    {
        var child = newCoord("myId");
        var orphan = newCoord("o");
        var source = newOffsetDuplicate("src");
        source.Set(OffsetDuplicate_offset, child);
        var target = newCircle("tgt");
        target.Set(Circle_center, orphan);

        target.Set(Circle_center, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(Circle_center));
        Assert.ThrowsException<UnsetFeatureException>(() => source.Get(OffsetDuplicate_offset));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void SingleRequired_SameInSameInstance_Reflective()
    {
        var child = newCoord("myId");
        var parent = newLine("src");
        parent.Set(Line_start, child);

        parent.Set(Line_start, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(Line_start));
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void SingleRequired_OtherInSameInstance_Reflective()
    {
        var child = newCoord("myId");
        var parent = newLine("src");
        parent.Set(Line_start, child);

        parent.Set(Line_end, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(Line_end));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Line_start));
    }

    [TestMethod]
    public void SingleRequired_OtherInSameInstance_detach_Reflective()
    {
        var child = newCoord("myId");
        var orphan = newCoord("o");
        var parent = newLine("src");
        parent.Set(Line_start, child);
        parent.Set(Line_end, orphan);

        parent.Set(Line_end, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(Line_end));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Line_start));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #endregion

    #endregion

    #region multiple

    #region optional

    #region singleEntry

    #region SameInOtherInstance

    [TestMethod]
    public void MultipleOptional_Single_SameInOtherInstance_Reflective()
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
    public void MultipleOptional_Single_Other_Reflective()
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
    public void MultipleOptional_Single_OtherInSameInstance_Reflective()
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
    public void MultipleOptional_Single_SameInSameInstance_Reflective()
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
    public void MultipleOptional_Partial_SameInOtherInstance_Reflective()
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
    public void MultipleOptional_Partial_Other_Reflective()
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
    public void MultipleOptional_Partial_OtherInSameInstance_Reflective()
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
    public void MultipleOptional_Partial_SameInSameInstance_Reflective()
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
    public void MultipleOptional_FromSingle_Other_Reflective()
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
    public void MultipleOptional_FromSingle_OtherInSameInstance_Reflective()
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
    public void MultipleOptional_ToSingle_Other_Reflective()
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
    public void MultipleOptional_ToSingle_OtherInSameInstance_Reflective()
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

    #endregion

    #region required

    #region singleEntry

    #region SameInOtherInstance

    [TestMethod]
    public void MultipleRequired_Single_SameInOtherInstance_Reflective()
    {
        var child = newLine("myId");
        var source = newCompositeShape("src");
        source.Set(CompositeShape_parts, new List<INode> { child });
        var target = newCompositeShape("tgt");

        target.Set(CompositeShape_parts, new List<DynamicNode> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, target.Get(CompositeShape_parts) as IList);
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

        target.Set(Geometry_shapes, new List<DynamicNode> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, target.Get(Geometry_shapes) as IList);
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

        parent.Set(CompositeShape_disabledParts, new List<DynamicNode> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, parent.Get(CompositeShape_disabledParts) as IList);
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

        parent.Set(CompositeShape_parts, new List<DynamicNode> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, parent.Get(CompositeShape_parts) as IList);
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

        target.Set(CompositeShape_parts, new List<DynamicNode> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { childA }, target.Get(CompositeShape_parts) as IList);
        CollectionAssert.AreEqual(new List<DynamicNode> { childB }, source.Get(CompositeShape_parts) as IList);
    }

    #endregion

    #region Other

    [TestMethod]
    public void MultipleRequired_Partial_Other_Reflective()
    {
        var childA = newLine("a");
        var childB = newLine("b");
        var source = newCompositeShape("src");
        source.Set(CompositeShape_parts, new List<INode> { childA,childB });
        var target = newGeometry("tgt");

        target.Set(Geometry_shapes, new List<DynamicNode> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { childA }, target.Get(Geometry_shapes) as IList);
        CollectionAssert.AreEqual(new List<DynamicNode> { childB }, source.Get(CompositeShape_parts) as IList);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleRequired_Partial_OtherInSameInstance_Reflective()
    {
        var childA = newLine("a");
        var childB = newLine("b");
        var parent = newCompositeShape("src");
        parent.Set(CompositeShape_parts, new List<INode> { childA,childB });

        parent.Set(CompositeShape_disabledParts, new List<DynamicNode> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { childA }, parent.Get(CompositeShape_disabledParts) as IList);
        CollectionAssert.AreEqual(new List<DynamicNode> { childB }, parent.Get(CompositeShape_parts) as IList);
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

        parent.Set(CompositeShape_parts, new List<DynamicNode> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { childA }, parent.Get(CompositeShape_parts) as IList);
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

        target.Set(Geometry_shapes, new List<DynamicNode> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, target.Get(Geometry_shapes) as IList);
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

        parent.Set(CompositeShape_parts, new List<DynamicNode> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<DynamicNode> { child }, parent.Get(CompositeShape_parts) as IList);
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

    #endregion

    #endregion
}