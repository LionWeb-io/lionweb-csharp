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

namespace LionWeb.Core.M2.Generated.Test;

using Examples.Shapes.M2;

[TestClass]
public class ParentHandlingTests
{
    #region single

    #region optional

    #region SameInOtherInstance

    [TestMethod]
    public void SingleOptional_SameInOtherInstance()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new Geometry("tgt");

        target.Documentation = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(source.Documentation);
    }

    [TestMethod]
    public void SingleOptional_SameInOtherInstance_detach()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var orphan = new Documentation("o");
        var target = new Geometry("tgt") { Documentation = orphan };

        target.Documentation = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(source.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void SingleOptional_SameInOtherInstance_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(source.Documentation);
    }

    [TestMethod]
    public void SingleOptional_SameInOtherInstance_detach_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var orphan = new Documentation("o");
        var target = new Geometry("tgt") { Documentation = orphan };

        target.Set(ShapesLanguage.Instance.Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(source.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region other

    [TestMethod]
    public void SingleOptional_Other()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new OffsetDuplicate("tgt");

        target.Docs = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Docs);
        Assert.IsNull(source.Documentation);
    }

    [TestMethod]
    public void SingleOptional_Other_detach()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var orphan = new Documentation("o");
        var target = new OffsetDuplicate("tgt") { Docs = orphan };

        target.Docs = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Docs);
        Assert.IsNull(source.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void SingleOptional_Other_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new OffsetDuplicate("tgt");

        target.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Docs);
        Assert.IsNull(source.Documentation);
    }

    [TestMethod]
    public void SingleOptional_Other_detach_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var orphan = new Documentation("o");
        var target = new OffsetDuplicate("tgt") { Docs = orphan };

        target.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Docs);
        Assert.IsNull(source.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region otherInSameInstance

    [TestMethod]
    public void SingleOptional_OtherInSameInstance()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.SecretDocs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void SingleOptional_OtherInSameInstance_detach()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var parent = new OffsetDuplicate("src") { Docs = child, SecretDocs = orphan };

        parent.SecretDocs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void SingleOptional_OtherInSameInstance_Reflective()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_secretDocs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void SingleOptional_OtherInSameInstance_detach_Reflective()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var parent = new OffsetDuplicate("src") { Docs = child, SecretDocs = orphan };

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_secretDocs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region sameInSameInstance

    [TestMethod]
    public void SingleOptional_SameInSameInstance()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.SecretDocs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.SecretDocs);
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void SingleOptional_SameInSameInstance_Reflective()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Docs);
    }

    #endregion

    #region annotation

    [TestMethod]
    public void SingleOptional_ToAnnotation()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new Line("tgt");

        target.AddAnnotations([child]);

        Assert.AreSame(target, child.GetParent());
        Assert.IsTrue(target.GetAnnotations().Contains(child));
        Assert.IsFalse(source.GetAnnotations().Contains(child));
    }

    [TestMethod]
    public void SingleOptional_ToAnnotation_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Geometry("src") { Documentation = child };
        var target = new Line("tgt");

        target.Set(null, new List<INode> { child });

        Assert.AreSame(target, child.GetParent());
        Assert.IsTrue(target.GetAnnotations().Contains(child));
        Assert.IsFalse(source.GetAnnotations().Contains(child));
    }

    [TestMethod]
    public void Annotation_ToSingleOptional()
    {
        var child = new Documentation("myId");
        var source = new Line("src");
        source.AddAnnotations([child]);
        var target = new Geometry("tgt");

        target.Documentation = child;

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Documentation);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_detach()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var source = new Line("src");
        source.AddAnnotations([child]);
        var target = new Geometry("tgt") { Documentation = orphan };

        target.Documentation = child;

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_Reflective()
    {
        var child = new Documentation("myId");
        var source = new Line("src");
        source.AddAnnotations([child]);
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Documentation);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_detach_Reflective()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var source = new Line("src");
        source.AddAnnotations([child]);
        var target = new Geometry("tgt") { Documentation = orphan };

        target.Set(ShapesLanguage.Instance.Geometry_documentation, child);

        Assert.AreSame(target, child.GetParent());
        Assert.IsFalse(source.GetAnnotations().Contains(child));
        Assert.AreSame(child, target.Documentation);
        Assert.IsNull(orphan.GetParent());
    }

    #region sameInstance

    [TestMethod]
    public void SingleOptional_ToAnnotation_SameInstance()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.AddAnnotations([child]);

        Assert.AreSame(parent, child.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(child));
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void SingleOptional_ToAnnotation_SameInstance_Reflective()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src") { Docs = child };

        parent.Set(null, new List<INode> { child });

        Assert.AreSame(parent, child.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(child));
        Assert.IsNull(parent.Docs);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src");
        parent.AddAnnotations([child]);

        parent.Docs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Docs);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance_detach()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var parent = new OffsetDuplicate("src") { Docs = orphan };
        parent.AddAnnotations([child]);

        parent.Docs = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Docs);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance_Reflective()
    {
        var child = new Documentation("myId");
        var parent = new OffsetDuplicate("src");
        parent.AddAnnotations([child]);

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Docs);
    }

    [TestMethod]
    public void Annotation_ToSingleOptional_SameInstance_detach_Reflective()
    {
        var child = new Documentation("myId");
        var orphan = new Documentation("o");
        var parent = new OffsetDuplicate("src") { Docs = orphan };
        parent.AddAnnotations([child]);

        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_docs, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(child));
        Assert.AreSame(child, parent.Docs);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #endregion

    #endregion

    #region required

    #region SameInOtherInstance

    [TestMethod]
    public void SingleRequired_SameInOtherInstance()
    {
        var child = new Coord("myId");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new OffsetDuplicate("tgt");

        target.Offset = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Offset);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
    }

    [TestMethod]
    public void SingleRequired_SameInOtherInstance_detach()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new OffsetDuplicate("tgt") { Offset = orphan };

        target.Offset = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Offset);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void SingleRequired_SameInOtherInstance_Reflective()
    {
        var child = new Coord("myId");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new OffsetDuplicate("tgt");

        target.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Offset);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
    }

    [TestMethod]
    public void SingleRequired_SameInOtherInstance_detach_Reflective()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new OffsetDuplicate("tgt") { Offset = orphan };

        target.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Offset);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region other

    [TestMethod]
    public void SingleRequired_Other()
    {
        var child = new Coord("myId");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new Circle("tgt");

        target.Center = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Center);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
    }

    [TestMethod]
    public void SingleRequired_Other_detach()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new Circle("tgt") { Center = orphan };

        target.Center = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Center);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void SingleRequired_Other_Reflective()
    {
        var child = new Coord("myId");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new Circle("tgt");

        target.Set(ShapesLanguage.Instance.Circle_center, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Center);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
    }

    [TestMethod]
    public void SingleRequired_Other_detach_Reflective()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new Circle("tgt") { Center = orphan };

        target.Set(ShapesLanguage.Instance.Circle_center, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Center);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void SingleRequired_SameInSameInstance()
    {
        var child = new Coord("myId");
        var parent = new Line("src") { Start = child };

        parent.Start = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Start);
    }

    [TestMethod]
    public void SingleRequired_SameInSameInstance_Reflective()
    {
        var child = new Coord("myId");
        var parent = new Line("src") { Start = child };

        parent.Set(ShapesLanguage.Instance.Line_start, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Start);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void SingleRequired_OtherInSameInstance()
    {
        var child = new Coord("myId");
        var parent = new Line("src") { Start = child };

        parent.End = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.End);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Start);
    }

    [TestMethod]
    public void SingleRequired_OtherInSameInstance_detach()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var parent = new Line("src") { Start = child, End = orphan };

        parent.End = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.End);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Start);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void SingleRequired_OtherInSameInstance_Reflective()
    {
        var child = new Coord("myId");
        var parent = new Line("src") { Start = child };

        parent.Set(ShapesLanguage.Instance.Line_end, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.End);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Start);
    }

    [TestMethod]
    public void SingleRequired_OtherInSameInstance_detach_Reflective()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var parent = new Line("src") { Start = child, End = orphan };

        parent.Set(ShapesLanguage.Instance.Line_end, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.End);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Start);
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
    public void MultipleOptional_Single_SameInOtherInstance_Add()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new Geometry("tgt");

        target.AddShapes([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void MultipleOptional_Single_SameInOtherInstance_Insert()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void MultipleOptional_Single_SameInOtherInstance_Reflective()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    #endregion

    #region Other

    [TestMethod]
    public void MultipleOptional_Single_Other_Add()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.AddParts([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Parts.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void MultipleOptional_Single_Other_Insert()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.InsertParts(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Parts.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void MultipleOptional_Single_Other_Reflective()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Parts.ToList());
        Assert.AreEqual(0, source.Shapes.Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleOptional_Single_OtherInSameInstance_Add()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.AddAltGroups([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.AreEqual(0, parent.Groups.Count);
    }

    [TestMethod]
    public void MultipleOptional_Single_OtherInSameInstance_Insert()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.InsertAltGroups(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.AreEqual(0, parent.Groups.Count);
    }

    [TestMethod]
    public void MultipleOptional_Single_OtherInSameInstance_Reflective()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_altGroups, new List<MaterialGroup> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.AreEqual(0, parent.Groups.Count);
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void MultipleOptional_Single_SameInSameInstance_Add()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.AddGroups([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.Groups.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Single_SameInSameInstance_Insert_Start()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.InsertGroups(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.Groups.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Single_SameInSameInstance_Insert_End()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertGroups(1, [child]));

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.Groups.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Single_SameInSameInstance_Reflective()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { Groups = [child] };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_groups, new List<MaterialGroup> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.Groups.ToList());
    }

    #endregion

    #endregion

    #region someEntries

    #region SameInOtherInstance

    [TestMethod]
    public void MultipleOptional_Partial_SameInOtherInstance_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new Geometry("tgt");

        target.AddShapes([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_SameInOtherInstance_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_SameInOtherInstance_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    #endregion

    #region Other

    [TestMethod]
    public void MultipleOptional_Partial_Other_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.AddParts([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_Other_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.InsertParts(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_Other_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new Geometry("src") { Shapes = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Shapes.ToList());
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleOptional_Partial_OtherInSameInstance_Add()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.AddAltGroups([childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA }, parent.AltGroups.ToList());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB }, parent.Groups.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_OtherInSameInstance_Insert()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.InsertAltGroups(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA }, parent.AltGroups.ToList());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB }, parent.Groups.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_OtherInSameInstance_Reflective()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_altGroups, new List<MaterialGroup> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA }, parent.AltGroups.ToList());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB }, parent.Groups.ToList());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void MultipleOptional_Partial_SameInSameInstance_Add()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.AddGroups([childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB, childA }, parent.Groups.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_SameInSameInstance_Insert_Start()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.InsertGroups(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA, childB }, parent.Groups.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_SameInSameInstance_Insert_Middle()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.InsertGroups(1, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<MaterialGroup> { childB, childA }, parent.Groups.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_SameInSameInstance_Insert_End()
    {
        var childA = new MaterialGroup("a");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertGroups(2, [childA]));

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA, childB }, parent.Groups.ToList());
    }

    [TestMethod]
    public void MultipleOptional_Partial_SameInSameInstance_Reflective()
    {
        var childA = new MaterialGroup("myId");
        var childB = new MaterialGroup("b");
        var parent = new BillOfMaterials("src") { Groups = [childA, childB] };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_groups, new List<MaterialGroup> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { childA }, parent.Groups.ToList());
    }

    #endregion

    #endregion

    #region FromSingle

    #region Other

    [TestMethod]
    public void MultipleOptional_FromSingle_Other_Add()
    {
        var child = new Line("myId");
        var source = new MaterialGroup("src") { DefaultShape = child };
        var target = new Geometry("tgt");

        target.AddShapes([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.IsNull(source.DefaultShape);
    }

    [TestMethod]
    public void MultipleOptional_FromSingle_Other_Insert()
    {
        var child = new Line("myId");
        var source = new MaterialGroup("src") { DefaultShape = child };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.IsNull(source.DefaultShape);
    }

    [TestMethod]
    public void MultipleOptional_FromSingle_Other_Reflective()
    {
        var child = new Line("myId");
        var source = new MaterialGroup("src") { DefaultShape = child };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.IsNull(source.DefaultShape);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleOptional_FromSingle_OtherInSameInstance_Add()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { DefaultGroup = child };

        parent.AddAltGroups([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.IsNull(parent.DefaultGroup);
    }

    [TestMethod]
    public void MultipleOptional_FromSingle_OtherInSameInstance_Insert()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { DefaultGroup = child };

        parent.InsertAltGroups(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.IsNull(parent.DefaultGroup);
    }

    [TestMethod]
    public void MultipleOptional_FromSingle_OtherInSameInstance_Reflective()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { DefaultGroup = child };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_altGroups, new List<MaterialGroup> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<MaterialGroup> { child }, parent.AltGroups.ToList());
        Assert.IsNull(parent.DefaultGroup);
    }

    #endregion

    #endregion

    #region ToSingle

    #region Other

    [TestMethod]
    public void MultipleOptional_ToSingle_Other()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new MaterialGroup("tgt");

        target.DefaultShape = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.DefaultShape);
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void MultipleOptional_ToSingle_Other_Reflective()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new MaterialGroup("tgt");

        target.Set(ShapesLanguage.Instance.MaterialGroup_defaultShape, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.DefaultShape);
        Assert.AreEqual(0, source.Shapes.Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleOptional_ToSingle_OtherInSameInstance()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { AltGroups = [child] };

        parent.DefaultGroup = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.DefaultGroup);
        Assert.AreEqual(0, parent.AltGroups.Count);
    }

    [TestMethod]
    public void MultipleOptional_ToSingle_OtherInSameInstance_Reflective()
    {
        var child = new MaterialGroup("myId");
        var parent = new BillOfMaterials("src") { AltGroups = [child] };

        parent.Set(ShapesLanguage.Instance.BillOfMaterials_defaultGroup, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.DefaultGroup);
        Assert.AreEqual(0, parent.AltGroups.Count);
    }

    #endregion

    #endregion

    #endregion

    #region required

    #region singleEntry

    #region SameInOtherInstance

    [TestMethod]
    public void MultipleRequired_Single_SameInOtherInstance_Add()
    {
        var child = new Line("myId");
        var source = new CompositeShape("src") { Parts = [child] };
        var target = new CompositeShape("tgt");

        target.AddParts([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Parts.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => source.Parts.Count);
    }

    [TestMethod]
    public void MultipleRequired_Single_SameInOtherInstance_Insert()
    {
        var child = new Line("myId");
        var source = new CompositeShape("src") { Parts = [child] };
        var target = new CompositeShape("tgt");

        target.InsertParts(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Parts.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => source.Parts.Count);
    }

    [TestMethod]
    public void MultipleRequired_Single_SameInOtherInstance_Reflective()
    {
        var child = new Line("myId");
        var source = new CompositeShape("src") { Parts = [child] };
        var target = new CompositeShape("tgt");

        target.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Parts.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => source.Parts.Count);
    }

    #endregion

    #region Other

    [TestMethod]
    public void MultipleRequired_Single_Other_Add()
    {
        var child = new Line("myId");
        var source = new CompositeShape("src") { Parts = [child] };
        var target = new Geometry("tgt");

        target.AddShapes([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => source.Parts.Count);
    }

    [TestMethod]
    public void MultipleRequired_Single_Other_Insert()
    {
        var child = new Line("myId");
        var source = new CompositeShape("src") { Parts = [child] };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => source.Parts.Count);
    }

    [TestMethod]
    public void MultipleRequired_Single_Other_Reflective()
    {
        var child = new Line("myId");
        var source = new CompositeShape("src") { Parts = [child] };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => source.Parts.Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleRequired_Single_OtherInSameInstance_Add()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.AddDisabledParts([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.DisabledParts.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count);
    }

    [TestMethod]
    public void MultipleRequired_Single_OtherInSameInstance_Insert()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.InsertDisabledParts(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.DisabledParts.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count);
    }

    [TestMethod]
    public void MultipleRequired_Single_OtherInSameInstance_Reflective()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.Set(ShapesLanguage.Instance.CompositeShape_disabledParts, new List<IShape> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.DisabledParts.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts.Count);
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void MultipleRequired_Single_SameInSameInstance_Add()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.AddParts([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Single_SameInSameInstance_Insert_Start()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.InsertParts(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Single_SameInSameInstance_Insert_End()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertParts(1, [child]));

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Single_SameInSameInstance_Reflective()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.Parts.ToList());
    }

    #endregion

    #endregion

    #region someEntries

    #region SameInOtherInstance

    [TestMethod]
    public void MultipleRequired_Partial_SameInOtherInstance_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.AddParts([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_SameInOtherInstance_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.InsertParts(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_SameInOtherInstance_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new CompositeShape("tgt");

        target.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Parts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    #endregion

    #region Other

    [TestMethod]
    public void MultipleRequired_Partial_Other_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new Geometry("tgt");

        target.AddShapes([childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_Other_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [childA]);

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_Other_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var source = new CompositeShape("src") { Parts = [childA, childB] };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { childA });

        Assert.AreSame(target, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, target.Shapes.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, source.Parts.ToList());
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleRequired_Partial_OtherInSameInstance_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.AddDisabledParts([childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, parent.DisabledParts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_OtherInSameInstance_Insert()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.InsertDisabledParts(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, parent.DisabledParts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_OtherInSameInstance_Reflective()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.Set(ShapesLanguage.Instance.CompositeShape_disabledParts, new List<IShape> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, parent.DisabledParts.ToList());
        CollectionAssert.AreEqual(new List<IShape> { childB }, parent.Parts.ToList());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void MultipleRequired_Partial_SameInSameInstance_Add()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.AddParts([childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<IShape> { childB, childA }, parent.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_SameInSameInstance_Insert_Start()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.InsertParts(0, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA, childB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_SameInSameInstance_Insert_Middle()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.InsertParts(1, [childA]);

        Assert.AreSame(parent, childA.GetParent());
        // changed order
        CollectionAssert.AreEqual(new List<IShape> { childB, childA }, parent.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_SameInSameInstance_Insert_End()
    {
        var childA = new Line("a");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => parent.InsertParts(2, [childA]));

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA, childB }, parent.Parts.ToList());
    }

    [TestMethod]
    public void MultipleRequired_Partial_SameInSameInstance_Reflective()
    {
        var childA = new Line("myId");
        var childB = new Line("b");
        var parent = new CompositeShape("src") { Parts = [childA, childB] };

        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { childA });

        Assert.AreSame(parent, childA.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { childA }, parent.Parts.ToList());
    }

    #endregion

    #endregion

    #region FromSingle

    #region Other

    [TestMethod]
    public void MultipleRequired_FromSingle_Other_Add()
    {
        var child = new Line("myId");
        var source = new CompositeShape("src") { EvilPart = child };
        var target = new Geometry("tgt");

        target.AddShapes([child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => source.EvilPart);
    }

    [TestMethod]
    public void MultipleRequired_FromSingle_Other_Insert()
    {
        var child = new Line("myId");
        var source = new CompositeShape("src") { EvilPart = child };
        var target = new Geometry("tgt");

        target.InsertShapes(0, [child]);

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => source.EvilPart);
    }

    [TestMethod]
    public void MultipleRequired_FromSingle_Other_Reflective()
    {
        var child = new Line("myId");
        var source = new CompositeShape("src") { EvilPart = child };
        var target = new Geometry("tgt");

        target.Set(ShapesLanguage.Instance.Geometry_shapes, new List<IShape> { child });

        Assert.AreSame(target, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, target.Shapes.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => source.EvilPart);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleRequired_FromSingle_OtherInSameInstance_Add()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { EvilPart = child };

        parent.AddParts([child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.Parts.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.EvilPart);
    }

    [TestMethod]
    public void MultipleRequired_FromSingle_OtherInSameInstance_Insert()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { EvilPart = child };

        parent.InsertParts(0, [child]);

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.Parts.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.EvilPart);
    }

    [TestMethod]
    public void MultipleRequired_FromSingle_OtherInSameInstance_Reflective()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { EvilPart = child };

        parent.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<IShape> { child });

        Assert.AreSame(parent, child.GetParent());
        CollectionAssert.AreEqual(new List<IShape> { child }, parent.Parts.ToList());
        Assert.ThrowsException<UnsetFeatureException>(() => parent.EvilPart);
    }

    #endregion

    #endregion

    #region ToSingle

    #region Other

    [TestMethod]
    public void MultipleRequired_ToSingle_Other()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.EvilPart = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.EvilPart);
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void MultipleRequired_ToSingle_Other_Reflective()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.Set(ShapesLanguage.Instance.CompositeShape_evilPart, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.EvilPart);
        Assert.AreEqual(0, source.Shapes.Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void MultipleRequired_ToSingle_OtherInSameInstance()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.EvilPart = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.EvilPart);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts);
    }

    [TestMethod]
    public void MultipleRequired_ToSingle_OtherInSameInstance_Reflective()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.Set(ShapesLanguage.Instance.CompositeShape_evilPart, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.EvilPart);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts);
    }

    #endregion

    #endregion

    #endregion

    #endregion
}