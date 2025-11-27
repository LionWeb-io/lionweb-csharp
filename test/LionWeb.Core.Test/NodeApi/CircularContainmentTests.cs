// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using M3;

[TestClass]
public class CircularContainmentTests
{

    #region Ancestor

    [TestMethod]
    public void SelfCircularAncestor()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(a);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestor<INode>(false));
    }
    
    [TestMethod]
    public void SelfCircularAncestor_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(a);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestor<INode>(true));
    }
    
    [TestMethod]
    public void DirectCircularAncestor()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(b);
        b.SetParent(a);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestor<INode>(false));
    }
    
    [TestMethod]
    public void DirectCircularAncestor_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(b);
        b.SetParent(a);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestor<INode>(true));
    }
    
    [TestMethod]
    public void IndirectCircularAncestor()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("C", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(c);
        b.SetParent(a);
        c.SetParent(b);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestor<INode>(false));
    }
    
    [TestMethod]
    public void IndirectCircularAncestor_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("C", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(c);
        b.SetParent(a);
        c.SetParent(b);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestor<INode>(true));
    }

    #endregion
    
    #region Ancestors

    [TestMethod]
    public void NonCircularMultipleEnumerationAncestors()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("c", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { c });

        IEnumerable<INode> ancestors = c.Ancestors(true);
        CollectionAssert.AreEqual(new List<INode> { c, b, a }, ancestors.ToList());
        CollectionAssert.AreEqual(new List<INode> { c, b, a }, ancestors.ToList());
    }
    
    [TestMethod]
    public void SelfCircularAncestors()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(a);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestors(false));
    }
    
    [TestMethod]
    public void SelfCircularAncestors_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(a);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestors(true));
    }
    
    [TestMethod]
    public void DirectCircularAncestors()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(b);
        b.SetParent(a);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestors(false));
    }
    
    [TestMethod]
    public void DirectCircularAncestors_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(b);
        b.SetParent(a);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestors(true));
    }
    
    [TestMethod]
    public void IndirectCircularAncestors()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("C", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(c);
        b.SetParent(a);
        c.SetParent(b);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestors(false));
    }
    
    [TestMethod]
    public void IndirectCircularAncestors_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("C", ShapesLanguage.Instance.CompositeShape);

        a.SetParent(c);
        b.SetParent(a);
        c.SetParent(b);

        Assert.ThrowsExactly<TreeShapeException>(() => a.Ancestors(true));
    }

    #endregion
    
    #region Children
    [TestMethod]
    public void NonCircularMultipleEnumerationChildren()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });

        IEnumerable<INode> children = a.Children();
        CollectionAssert.AreEqual(new List<INode> { b }, children.ToList());
        CollectionAssert.AreEqual(new List<INode> { b }, children.ToList());
    }

    [TestMethod]
    public void SelfCircularChildren()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        Assert.ThrowsExactly<TreeShapeException>(() => a.Children(false).ToList());
    }
    
    [TestMethod]
    public void SelfCircularChildren_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        Assert.ThrowsExactly<TreeShapeException>(() => a.Children(true).ToList());
    }
    
    [TestMethod]
    public void DirectCircularChildren()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        CollectionAssert.AreEqual(new List<INode> { b }, a.Children(false).ToList());
    }
    
    [TestMethod]
    public void DirectCircularChildren_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        CollectionAssert.AreEqual(new List<INode> { a, b }, a.Children(true).ToList());
    }
    
    [TestMethod]
    public void DirectCircularMultipleChildren()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("c", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b,c });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });
        c.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        CollectionAssert.AreEqual(new List<INode> { b,c }, a.Children(false).ToList());
    }
    
    [TestMethod]
    public void DirectCircularMultipleChildren_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("c", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b,c });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });
        c.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        CollectionAssert.AreEqual(new List<INode> { a, b, c }, a.Children(true).ToList());
    }
    
    [TestMethod]
    public void IndirectCircularChildren()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("C", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { c });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });
        c.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });

        CollectionAssert.AreEqual(new List<INode> { c }, a.Children(false).ToList());
    }
    
    [TestMethod]
    public void IndirectCircularChildren_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("C", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { c });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });
        c.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });

        CollectionAssert.AreEqual(new List<INode> { a, c }, a.Children(true).ToList());
    }

    #endregion
    
    #region Descendants

    [TestMethod]
    public void NonCircularMultipleEnumerationDescendants()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });

        IEnumerable<INode> descendants = a.Descendants();
        CollectionAssert.AreEqual(new List<INode> { b }, descendants.ToList());
        CollectionAssert.AreEqual(new List<INode> { b }, descendants.ToList());
    }
    
    [TestMethod]
    public void SelfCircularDescendants()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        Assert.ThrowsExactly<TreeShapeException>(() => a.Descendants(false).ToList());
    }
    
    [TestMethod]
    public void SelfCircularDescendants_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        Assert.ThrowsExactly<TreeShapeException>(() => a.Descendants(true).ToList());
    }
    
    [TestMethod]
    public void DirectCircularDescendants()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        Assert.ThrowsExactly<TreeShapeException>(() => a.Descendants(false).ToList());
    }
    
    [TestMethod]
    public void DirectCircularDescendants_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        Assert.ThrowsExactly<TreeShapeException>(() => a.Descendants(true).ToList());
    }
    
    [TestMethod]
    public void DirectCircularMultipleDescendants()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("C", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b, c });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });
        c.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });

        Assert.ThrowsExactly<TreeShapeException>(() => a.Descendants(false).ToList());
    }
    
    [TestMethod]
    public void IndirectCircularDescendants()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("C", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { c });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });
        c.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });

        Assert.ThrowsExactly<TreeShapeException>(() => a.Descendants(false).ToList());
    }
    
    [TestMethod]
    public void IndirectCircularDescendants_Self()
    {
        var a = new TestDynamicNode("A", ShapesLanguage.Instance.CompositeShape);
        var b = new TestDynamicNode("B", ShapesLanguage.Instance.CompositeShape);
        var c = new TestDynamicNode("C", ShapesLanguage.Instance.CompositeShape);

        a.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { c });
        b.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { a });
        c.Set(ShapesLanguage.Instance.CompositeShape_parts, new List<INode> { b });

        Assert.ThrowsExactly<TreeShapeException>(() => a.Descendants(true).ToList());
    }

    #endregion
}

internal class TestDynamicNode(string id, Classifier classifier) : DynamicNode(id, classifier)
{
    public void SetParent(INode parent) =>
        SetParentInternal(this, parent);
}