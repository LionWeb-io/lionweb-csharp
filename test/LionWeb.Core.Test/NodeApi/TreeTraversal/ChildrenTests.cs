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
using M1;

[TestClass]
public class ChildrenTests
{
    #region noChildren

    [TestMethod]
    public void NoChildren()
    {
        var node = new CompositeShape("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Children(false, false).ToList());
    }

    [TestMethod]
    public void NoChildren_Self()
    {
        var node = new CompositeShape("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Children(true, false).ToList());
    }

    [TestMethod]
    public void NoChildren_Annotations()
    {
        var node = new CompositeShape("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Children(false, true).ToList());
    }

    [TestMethod]
    public void NoChildren_Self_Annotations()
    {
        var node = new CompositeShape("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Children(true, true).ToList());
    }

    [TestMethod]
    public void NoChildren_Annotation()
    {
        var node = new BillOfMaterials("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Children(false, false).ToList());
    }

    [TestMethod]
    public void NoChildren_Annotation_Self()
    {
        var node = new BillOfMaterials("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Children(true, false).ToList());
    }

    [TestMethod]
    public void NoChildren_Annotation_Annotations()
    {
        var node = new BillOfMaterials("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Children(false, true).ToList());
    }

    [TestMethod]
    public void NoChildren_Annotation_Self_Annotations()
    {
        var node = new BillOfMaterials("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Children(true, true).ToList());
    }

    #endregion

    #region oneChild

    [TestMethod]
    public void OneChild()
    {
        var child = new Line("c");
        var node = new CompositeShape("n") { Parts = [child] };
        CollectionAssert.AreEqual(new List<INode> { child }, node.Children(false, false).ToList());
    }

    [TestMethod]
    public void OneChild_Self()
    {
        var child = new Line("c");
        var node = new CompositeShape("n") { Parts = [child] };
        CollectionAssert.AreEqual(new List<INode> { node, child }, node.Children(true, false).ToList());
    }

    [TestMethod]
    public void OneChild_Annotations()
    {
        var child = new Line("c");
        var node = new CompositeShape("n") { Parts = [child] };
        CollectionAssert.AreEqual(new List<INode> { child }, node.Children(false, true).ToList());
    }

    [TestMethod]
    public void OneChild_Self_Annotations()
    {
        var child = new Line("c");
        var node = new CompositeShape("n") { Parts = [child] };
        CollectionAssert.AreEqual(new List<INode> { node, child }, node.Children(true, true).ToList());
    }

    [TestMethod]
    public void OneChild_Annotation()
    {
        var node = new CompositeShape("n");
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { }, node.Children(false, false).ToList());
    }

    [TestMethod]
    public void OneChild_Annotation_Self()
    {
        var node = new CompositeShape("n");
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { node }, node.Children(true, false).ToList());
    }

    [TestMethod]
    public void OneChild_Annotation_Annotations()
    {
        var node = new CompositeShape("n");
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { ann }, node.Children(false, true).ToList());
    }

    [TestMethod]
    public void OneChild_Annotation_Self_Annotations()
    {
        var node = new CompositeShape("n");
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { node, ann }, node.Children(true, true).ToList());
    }

    #endregion

    #region manyChildren

    [TestMethod]
    public void ManyChildren()
    {
        var childA = new Line("cA");
        var childB = new Circle("cB");
        var node = new CompositeShape("n") { Parts = [childA], EvilPart = childB };
        CollectionAssert.AreEquivalent(new List<INode> { childA, childB }, node.Children(false, false).ToList());
    }

    [TestMethod]
    public void ManyChildren_Self()
    {
        var childA = new Line("cA");
        var childB = new Circle("cB");
        var node = new CompositeShape("n") { Parts = [childA], EvilPart = childB };
        CollectionAssert.AreEquivalent(new List<INode> { node, childA, childB }, node.Children(true, false).ToList());
    }

    [TestMethod]
    public void ManyChildren_Annotations()
    {
        var childA = new Line("cA");
        var childB = new Circle("cB");
        var node = new CompositeShape("n") { Parts = [childA], EvilPart = childB };
        CollectionAssert.AreEquivalent(new List<INode> { childA, childB }, node.Children(false, true).ToList());
    }

    [TestMethod]
    public void ManyChildren_Self_Annotations()
    {
        var childA = new Line("cA");
        var childB = new Circle("cB");
        var node = new CompositeShape("n") { Parts = [childA], EvilPart = childB };
        CollectionAssert.AreEquivalent(new List<INode> { node, childA, childB }, node.Children(true, true).ToList());
    }

    [TestMethod]
    public void ManyChildren_Annotation()
    {
        var childA = new Line("cA");
        var childB = new Circle("cB");
        var node = new CompositeShape("n") { Parts = [childA], EvilPart = childB };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode> { childA, childB }, node.Children(false, false).ToList());
    }

    [TestMethod]
    public void ManyChildren_Annotation_Self()
    {
        var childA = new Line("cA");
        var childB = new Circle("cB");
        var node = new CompositeShape("n") { Parts = [childA], EvilPart = childB };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode> { node, childA, childB }, node.Children(true, false).ToList());
    }

    [TestMethod]
    public void ManyChildren_Annotation_Annotations()
    {
        var childA = new Line("cA");
        var childB = new Circle("cB");
        var node = new CompositeShape("n") { Parts = [childA], EvilPart = childB };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode> { childA, childB, ann }, node.Children(false, true).ToList());
    }

    [TestMethod]
    public void ManyChildren_Annotation_Self_Annotations()
    {
        var childA = new Line("cA");
        var childB = new Circle("cB");
        var node = new CompositeShape("n") { Parts = [childA], EvilPart = childB };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEquivalent(new List<INode> { node, childA, childB, ann }, node.Children(true, true).ToList());
    }

    #endregion

    #region noReferences

    [TestMethod]
    public void NoReferences()
    {
        var child = new Coord("c");
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Offset = child, Source = reference };
        CollectionAssert.AreEqual(new List<INode> { child }, node.Children(false, false).ToList());
    }

    [TestMethod]
    public void NoReferences_Self()
    {
        var child = new Coord("c");
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Offset = child, Source = reference };
        CollectionAssert.AreEqual(new List<INode> { node, child }, node.Children(true, false).ToList());
    }

    [TestMethod]
    public void NoReferences_Annotations()
    {
        var child = new Coord("c");
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Offset = child, Source = reference };
        CollectionAssert.AreEqual(new List<INode> { child }, node.Children(false, true).ToList());
    }

    [TestMethod]
    public void NoReferences_Self_Annotations()
    {
        var child = new Coord("c");
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Offset = child, Source = reference };
        CollectionAssert.AreEqual(new List<INode> { node, child }, node.Children(true, true).ToList());
    }

    [TestMethod]
    public void NoReferences_Annotation()
    {
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Source = reference };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { }, node.Children(false, false).ToList());
    }

    [TestMethod]
    public void NoReferences_Annotation_Self()
    {
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Source = reference };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { node }, node.Children(true, false).ToList());
    }

    [TestMethod]
    public void NoReferences_Annotation_Annotations()
    {
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Source = reference };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { ann }, node.Children(false, true).ToList());
    }

    [TestMethod]
    public void NoReferences_Annotation_Self_Annotations()
    {
        var reference = new Line("r");
        var node = new OffsetDuplicate("n") { Source = reference };
        var ann = new BillOfMaterials("a");
        node.AddAnnotations([ann]);
        CollectionAssert.AreEqual(new List<INode> { node, ann }, node.Children(true, true).ToList());
    }

    #endregion
}