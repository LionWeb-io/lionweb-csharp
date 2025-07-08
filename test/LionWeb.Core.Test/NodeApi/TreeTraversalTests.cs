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

namespace LionWeb.Core.Test.NodeApi;

using Languages.Generated.V2024_1.TestLanguage;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M3;

[TestClass]
public class TreeTraversalTests
{
    #region ancestors

    #region noParent

    [TestMethod]
    public void NoParent()
    {
        var node = new Coord("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void NoParent_Self()
    {
        var node = new Coord("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Ancestors(true).ToList());
    }

    [TestMethod]
    public void NoParent_Annotation()
    {
        var node = new Documentation("n");
        CollectionAssert.AreEqual(new List<INode> { }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void NoParent_Annotation_Self()
    {
        var node = new Documentation("n");
        CollectionAssert.AreEqual(new List<INode> { node }, node.Ancestors(true).ToList());
    }

    #endregion

    #region oneParent

    [TestMethod]
    public void OneParent()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        CollectionAssert.AreEqual(new List<INode> { parent }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void OneParent_Self()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        CollectionAssert.AreEqual(new List<INode> { node, parent }, node.Ancestors(true).ToList());
    }

    [TestMethod]
    public void OneParent_Annotation()
    {
        var node = new Documentation("n");
        var parent = new Line("p");
        parent.AddAnnotations([node]);
        CollectionAssert.AreEqual(new List<INode> { parent }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void OneParent_Annotation_Self()
    {
        var node = new Documentation("n");
        var parent = new Line("p");
        parent.AddAnnotations([node]);
        CollectionAssert.AreEqual(new List<INode> { node, parent }, node.Ancestors(true).ToList());
    }

    #endregion

    #region manyParents

    [TestMethod]
    public void ManyParents()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        var ancestor = new Geometry("a") { Shapes = [parent] };
        CollectionAssert.AreEqual(new List<INode> { parent, ancestor }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void ManyParents_Self()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        var ancestor = new Geometry("a") { Shapes = [parent] };
        CollectionAssert.AreEqual(new List<INode> { node, parent, ancestor }, node.Ancestors(true).ToList());
    }

    [TestMethod]
    public void ManyParents_Annotation()
    {
        var node = new Documentation("n");
        var parent = new Line("p");
        parent.AddAnnotations([node]);
        var ancestor = new Geometry("a") { Shapes = [parent] };
        CollectionAssert.AreEqual(new List<INode> { parent, ancestor }, node.Ancestors(false).ToList());
    }

    [TestMethod]
    public void ManyParents_Annotation_Self()
    {
        var node = new Documentation("n");
        var parent = new Line("p");
        parent.AddAnnotations([node]);
        var ancestor = new Geometry("a") { Shapes = [parent] };
        CollectionAssert.AreEqual(new List<INode> { node, parent, ancestor }, node.Ancestors(true).ToList());
    }

    #endregion

    #endregion

    #region ancestor

    [TestMethod]
    public void Ancestor_NoParent()
    {
        var node = new Circle("n");
        Assert.AreEqual(null, node.Ancestor<Shape>(false));
        Assert.IsNull(node.Ancestor<Shape>(false));
    }

    [TestMethod]
    public void Ancestor_NoParent_Self()
    {
        var node = new Circle("n");
        Assert.AreEqual(node, node.Ancestor<Shape>(true));
        Assert.IsNotNull(node.Ancestor<Shape>(true));
    }

    [TestMethod]
    public void Ancestor_ManyParents()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        var ancestor = new Geometry("a") { Shapes = [parent] };

        Assert.IsNotNull(node.Ancestor<Shape>());
        Assert.AreEqual(parent, node.Ancestor<Shape>());

        Assert.IsNotNull(parent.Ancestor<Geometry>());
        Assert.AreEqual(ancestor, parent.Ancestor<Geometry>());
    }

    [TestMethod]
    public void Ancestor_ManyParents_Self()
    {
        var node = new Coord("n");
        var parent = new Line("p") { Start = node };
        var ancestor = new Geometry("a") { Shapes = [parent] };

        Assert.IsNotNull(node.Ancestor<Shape>(true));
        Assert.AreEqual(parent, node.Ancestor<Shape>(true));

        Assert.IsNotNull(parent.Ancestor<Geometry>(true));
        Assert.AreEqual(ancestor, parent.Ancestor<Geometry>(true));
    }

    #endregion

    #region precedingSibling

    [TestMethod]
    public void PrecedingSibling()
    {
        var circleA = new Circle("a");
        var circleB = new Circle("b");
        var circleC = new Circle("c");
        var circleD = new Circle("d");
        var ancestor = new Geometry("a") { Shapes = [circleA, circleB, circleC, circleD] };
        CollectionAssert.AreEqual(new List<INode> { circleA, circleB }, circleC.PrecedingSiblings().ToList());
    }

    [TestMethod]
    public void PrecedingSibling_Self()
    {
        var circleA = new Circle("a");
        var circleB = new Circle("b");
        var circleC = new Circle("c");
        var circleD = new Circle("d");
        var ancestor = new Geometry("a") { Shapes = [circleA, circleB, circleC, circleD] };
        CollectionAssert.AreEqual(new List<INode> { circleA, circleB, circleC },
            circleC.PrecedingSiblings(true).ToList());
    }

    [TestMethod]
    public void PrecedingSibling_NoParent()
    {
        var circleA = new Circle("a");
        Assert.ThrowsException<TreeShapeException>(() => circleA.PrecedingSiblings());
    }

    [TestMethod]
    public void PrecedingSibling_SingleContainment()
    {
        var coord = new Coord("a");
        var circle = new Circle("b") { Center = coord };
        Assert.ThrowsException<TreeShapeException>(() => coord.PrecedingSiblings());
    }

    [TestMethod]
    public void PrecedingSibling_NoPrecedingSibling()
    {
        var circleA = new Circle("a");
        var circleB = new Circle("b");
        var ancestor = new Geometry("a") { Shapes = [circleA, circleB] };
        CollectionAssert.AreEqual(new List<INode> { }, circleA.PrecedingSiblings().ToList());
    }

    [TestMethod]
    public void PrecedingSibling_NoPrecedingSibling_Self()
    {
        var circleA = new Circle("a");
        var circleB = new Circle("b");
        var ancestor = new Geometry("a") { Shapes = [circleA, circleB] };
        CollectionAssert.AreEqual(new List<INode> { circleA }, circleA.PrecedingSiblings(true).ToList());
    }

    #endregion

    #region followingSibling

    [TestMethod]
    public void FollowingSibling()
    {
        var circleA = new Circle("a");
        var circleB = new Circle("b");
        var circleC = new Circle("c");
        var circleD = new Circle("d");
        var ancestor = new Geometry("a") { Shapes = [circleA, circleB, circleC, circleD] };
        CollectionAssert.AreEqual(new List<INode> { circleD }, circleC.FollowingSiblings().ToList());
    }

    [TestMethod]
    public void FollowingSibling_Self()
    {
        var circleA = new Circle("a");
        var circleB = new Circle("b");
        var circleC = new Circle("c");
        var circleD = new Circle("d");
        var ancestor = new Geometry("a") { Shapes = [circleA, circleB, circleC, circleD] };
        CollectionAssert.AreEqual(new List<INode> { circleC, circleD }, circleC.FollowingSiblings(true).ToList());
    }

    [TestMethod]
    public void FollowingSibling_NoParent()
    {
        var circleA = new Circle("a");
        Assert.ThrowsException<TreeShapeException>(() => circleA.FollowingSiblings());
    }

    [TestMethod]
    public void FollowingSibling_SingleContainment()
    {
        var coord = new Coord("a");
        var circle = new Circle("b") { Center = coord };
        Assert.ThrowsException<TreeShapeException>(() => coord.FollowingSiblings());
    }

    [TestMethod]
    public void FollowingSibling_NoFollowingSibling()
    {
        var circleA = new Circle("a");
        var circleB = new Circle("b");
        var ancestor = new Geometry("a") { Shapes = [circleA, circleB] };
        CollectionAssert.AreEqual(new List<INode> { }, circleB.FollowingSiblings().ToList());
    }

    [TestMethod]
    public void FollowingSibling_NoFollowingSibling_Self()
    {
        var circleA = new Circle("a");
        var circleB = new Circle("b");
        var ancestor = new Geometry("a") { Shapes = [circleA, circleB] };
        CollectionAssert.AreEqual(new List<INode> { circleB }, circleB.FollowingSiblings(true).ToList());
    }

    #endregion

    #region children

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

    #endregion

    #region descendants

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

    #endregion

    #region partition

    [TestMethod]
    public void Partition_NoParent_Self()
    {
        var node = new Geometry("p");

        Assert.AreSame(node, node.GetPartition());
    }

    [TestMethod]
    public void Partition_NoParent_None()
    {
        var node = new Circle("p");

        Assert.IsNull(node.GetPartition());
    }

    [TestMethod]
    public void Partition_Parent_Parent()
    {
        var node = new LinkTestConcept("p");
        var partition = new LinkTestConcept("parent") { Containment_0_1 = node };

        Assert.AreSame(partition, node.GetPartition());
    }

    [TestMethod]
    public void Partition_Parent_None()
    {
        var node = new Coord("c");
        _ = new Circle("p") { Center = node };

        Assert.IsNull(node.GetPartition());
    }

    [TestMethod]
    public void Partition_NotSelf_Parent_Parent()
    {
        var node = new Circle("p");
        var partition = new Geometry("partition") { Shapes = [node] };

        Assert.AreSame(partition, node.GetPartition());
    }

    [TestMethod]
    public void Partition_Ancestor()
    {
        var node = new LinkTestConcept("p");
        var partition =
            new LinkTestConcept("parent") { Containment_0_1 = new LinkTestConcept("middle") { Containment_1 = node } };

        Assert.AreSame(partition, node.GetPartition());
    }

    [TestMethod]
    public void Partition_NotRoot()
    {
        var lang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Name = "lang", Key = "lang", Version = "lang"
        };
        var iface = lang.Interface("iface", "iface", "iface");
        var cont = iface.Containment("cont", "cont", "cont").IsMultiple(false).IsOptional(true).OfType(iface);
        var root = lang.Concept("root", "root", "root").IsPartition(false).Implementing(iface);
        var partition = lang.Concept("partition", "partition", "partition").IsPartition(true).Implementing(iface);

        var r = lang.GetFactory().CreateNode("rootInstance", root);
        DynamicPartitionInstance p =
            (DynamicPartitionInstance)lang.GetFactory().CreateNode("partitionInstance", partition);
        var c = lang.GetFactory().CreateNode("childPartitionInstance", partition);
        var l = lang.GetFactory().CreateNode("leafInstance", partition);

        r.Set(cont, p);
        p.Set(cont, c);
        c.Set(cont, l);

        Assert.AreSame(p, l.GetPartition());
    }

    #endregion
}