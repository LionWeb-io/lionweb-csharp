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

[TestClass]
public class SingleTests : DynamicNodeTestsBase
{
    #region optional

    #region SameInOtherInstance

    [TestMethod]
    public void Optional_SameInOtherInstance_Reflective()
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
    public void Optional_SameInOtherInstance_detach_Reflective()
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
    public void Optional_Other_Reflective()
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
    public void Optional_Other_detach_Reflective()
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
    public void Optional_OtherInSameInstance_Reflective()
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
    public void Optional_OtherInSameInstance_detach_Reflective()
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
    public void Optional_SameInSameInstance_Reflective()
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
    public void Optional_ToAnnotation()
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
    public void Optional_ToAnnotation_Reflective()
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
    public void Optional_ToAnnotation_SameInstance()
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
    public void Optional_ToAnnotation_SameInstance_Reflective()
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
    public void Required_SameInOtherInstance_Reflective()
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
    public void Required_SameInOtherInstance_detach_Reflective()
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
    public void Required_Other_Reflective()
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
    public void Required_Other_detach_Reflective()
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
    public void Required_SameInSameInstance_Reflective()
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
    public void Required_OtherInSameInstance_Reflective()
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
    public void Required_OtherInSameInstance_detach_Reflective()
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
}