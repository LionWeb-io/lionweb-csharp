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

namespace LionWeb.Core.Test.NodeApi.TreeTraversal;

using Languages.Generated.V2024_1.Shapes.M2;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;

[TestClass]
public class DescendantsTests
{
    #region noDescendants

    [TestMethod]
    public void NoDescendants()
    {
        var node = new Geometry("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Descendants(false, false).ToList());
    }

    [TestMethod]
    public void NoDescendants_Self()
    {
        var node = new Geometry("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Descendants(true, false).ToList());
    }

    [TestMethod]
    public void NoDescendants_Annotations()
    {
        var node = new Geometry("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Descendants(false, true).ToList());
    }

    [TestMethod]
    public void NoDescendants_Self_Annotations()
    {
        var node = new Geometry("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Descendants(true, true).ToList());
    }

    [TestMethod]
    public void NoDescendants_Annotation()
    {
        var node = new Geometry("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Descendants(false, false).ToList());
    }

    [TestMethod]
    public void NoDescendants_Annotation_Self()
    {
        var node = new Geometry("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Descendants(true, false).ToList());
    }

    [TestMethod]
    public void NoDescendants_Annotation_Annotations()
    {
        var node = new Geometry("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Descendants(false, true).ToList());
    }

    [TestMethod]
    public void NoDescendants_Annotation_Self_Annotations()
    {
        var node = new Geometry("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Descendants(true, true).ToList());
    }

    #endregion

    #region oneLevelDescendants

    [TestMethod]
    public void OneLevelDescendants()
    {
        var childA = new Line("cA");
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        CollectionAssert.AreEquivalent(new List<INode> { childA, childB }, node.Descendants(false, false).ToList());
    }

    [TestMethod]
    public void OneLevelDescendants_Self()
    {
        var childA = new Line("cA");
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        CollectionAssert.AreEquivalent(new List<INode> { node, childA, childB }, node.Descendants(true, false).ToList());
    }

    [TestMethod]
    public void OneLevelDescendants_Annotations()
    {
        var childA = new Line("cA");
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        CollectionAssert.AreEquivalent(new List<INode> { childA, childB }, node.Descendants(false, true).ToList());
    }

    [TestMethod]
    public void OneLevelDescendants_Self_Annotations()
    {
        var childA = new Line("cA");
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        CollectionAssert.AreEquivalent(new List<INode> { node, childA, childB }, node.Descendants(true, true).ToList());
    }

    [TestMethod]
    public void OneLevelDescendants_Annotation()
    {
        var childA = new Line("cA");
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode> { childA, childB }, node.Descendants(false, false).ToList());
    }

    [TestMethod]
    public void OneLevelDescendants_Annotation_Self()
    {
        var childA = new Line("cA");
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode> { node, childA, childB }, node.Descendants(true, false).ToList());
    }

    [TestMethod]
    public void OneLevelDescendants_Annotation_Annotations()
    {
        var childA = new Line("cA");
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode> { childA, childB, ann }, node.Descendants(false, true).ToList());
    }

    [TestMethod]
    public void OneLevelDescendants_Annotation_Self_Annotations()
    {
        var childA = new Line("cA");
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode> { node, childA, childB, ann }, node.Descendants(true, true).ToList());
    }

    #endregion

    #region manyLevelDescendants

    [TestMethod]
    public void ManyLevelDescendants()
    {
        var grandchildAA = new Coord("gAA");
        var grandchildAB = new Coord("gAB");
        var childA = new Line("cA") { Start = grandchildAA, End = grandchildAB };
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        CollectionAssert.AreEquivalent(new List<INode> { childA, grandchildAA, grandchildAB, childB },
            node.Descendants(false, false).ToList());
    }

    [TestMethod]
    public void ManyLevelDescendants_Self()
    {
        var grandchildAA = new Coord("gAA");
        var grandchildAB = new Coord("gAB");
        var childA = new Line("cA") { Start = grandchildAA, End = grandchildAB };
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        CollectionAssert.AreEquivalent(new List<INode>
        {
            node,
            childA,
            grandchildAA,
            grandchildAB,
            childB
        }, node.Descendants(true, false).ToList());
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotations()
    {
        var grandchildAA = new Coord("gAA");
        var grandchildAB = new Coord("gAB");
        var childA = new Line("cA") { Start = grandchildAA, End = grandchildAB };
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        CollectionAssert.AreEquivalent(new List<INode> { childA, grandchildAA, grandchildAB, childB },
            node.Descendants(false, true).ToList());
    }

    [TestMethod]
    public void ManyLevelDescendants_Self_Annotations()
    {
        var grandchildAA = new Coord("gAA");
        var grandchildAB = new Coord("gAB");
        var childA = new Line("cA") { Start = grandchildAA, End = grandchildAB };
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };
        CollectionAssert.AreEquivalent(new List<INode>
        {
            node,
            childA,
            grandchildAA,
            grandchildAB,
            childB
        }, node.Descendants(true, true).ToList());
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation()
    {
        var grandchildAA = new Coord("gAA");
        var grandchildAAAnn = new BillOfMaterials("gAAa");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new Coord("gAB");
        var childA = new Line("cA") { Start = grandchildAA, End = grandchildAB };
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };

        var grandchild = new MaterialGroup("aG");
        var grandchildAnn = new BillOfMaterials("aGa");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new BillOfMaterials("a") { Groups = [grandchild] };
        var annAnn = new BillOfMaterials("aa");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode> { childA, grandchildAA, grandchildAB, childB },
            node.Descendants(false, false).ToList());
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation_Self()
    {
        var grandchildAA = new Coord("gAA");
        var grandchildAAAnn = new BillOfMaterials("gAAa");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new Coord("gAB");
        var childA = new Line("cA") { Start = grandchildAA, End = grandchildAB };
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };

        var grandchild = new MaterialGroup("aG");
        var grandchildAnn = new BillOfMaterials("aGa");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new BillOfMaterials("a") { Groups = [grandchild] };
        var annAnn = new BillOfMaterials("aa");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode>
        {
            node,
            childA,
            grandchildAA,
            grandchildAB,
            childB
        }, node.Descendants(true, false).ToList());
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation_Annotations()
    {
        var grandchildAA = new Coord("gAA");
        var grandchildAAAnn = new BillOfMaterials("gAAa");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new Coord("gAB");
        var childA = new Line("cA") { Start = grandchildAA, End = grandchildAB };
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };

        var grandchild = new MaterialGroup("aG");
        var grandchildAnn = new BillOfMaterials("aGa");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new BillOfMaterials("a") { Groups = [grandchild] };
        var annAnn = new BillOfMaterials("aa");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode>
        {
            childA,
            grandchildAA,
            grandchildAAAnn,
            grandchildAB,
            childB,
            ann,
            grandchild,
            grandchildAnn,
            annAnn
        }, node.Descendants(false, true).ToList());
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation_Self_Annotations()
    {
        var grandchildAA = new Coord("gAA");
        var grandchildAAAnn = new BillOfMaterials("gAAa");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new Coord("gAB");
        var childA = new Line("cA") { Start = grandchildAA, End = grandchildAB };
        var childB = new Documentation("cB");
        var node = new Geometry("n") { Shapes = [childA], Documentation = childB };

        var grandchild = new MaterialGroup("aG");
        var grandchildAnn = new BillOfMaterials("aGa");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new BillOfMaterials("a") { Groups = [grandchild] };
        var annAnn = new BillOfMaterials("aa");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode>
        {
            node,
            childA,
            grandchildAA,
            grandchildAAAnn,
            grandchildAB,
            childB,
            ann,
            grandchild,
            grandchildAnn,
            annAnn
        }, node.Descendants(true, true).ToList());
    }

    #endregion

    #region noReferences

    [TestMethod]
    public void DescendantsNoReferences()
    {
        var child = new Coord("c");
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Offset = child, Source = reference };
        CollectionAssert.AreEqual(new List<INode> { child }, node.Descendants(false, false).ToList());
    }

    [TestMethod]
    public void DescendantsNoReferences_Self()
    {
        var child = new Coord("c");
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Offset = child, Source = reference };
        CollectionAssert.AreEqual(new List<INode> { node, child }, node.Descendants(true, false).ToList());
    }

    [TestMethod]
    public void DescendantsNoReferences_Annotations()
    {
        var child = new Coord("c");
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Offset = child, Source = reference };
        CollectionAssert.AreEqual(new List<INode> { child }, node.Descendants(false, true).ToList());
    }

    [TestMethod]
    public void DescendantsNoReferences_Self_Annotations()
    {
        var child = new Coord("c");
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Offset = child, Source = reference };
        CollectionAssert.AreEqual(new List<INode> { node, child }, node.Descendants(true, true).ToList());
    }

    [TestMethod]
    public void DescendantsNoReferences_Annotation()
    {
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Source = reference };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { }, node.Descendants(false, false).ToList());
    }

    [TestMethod]
    public void DescendantsNoReferences_Annotation_Self()
    {
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Source = reference };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { node }, node.Descendants(true, false).ToList());
    }

    [TestMethod]
    public void DescendantsNoReferences_Annotation_Annotations()
    {
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Source = reference };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { ann }, node.Descendants(false, true).ToList());
    }

    [TestMethod]
    public void DescendantsNoReferences_Annotation_Self_Annotations()
    {
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Source = reference };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { node, ann }, node.Descendants(true, true).ToList());
    }

    #endregion
}