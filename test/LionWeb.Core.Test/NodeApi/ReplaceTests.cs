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

using Core.Utilities;
using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class ReplaceTests_Containment
{

    #region Child replaced in same containment

    [TestMethod]
    public void ChildReplaced_Multiple_InSameContainment_Forward_WithTwoChildren()
    {
        var replacement = new LinkTestConcept("replacement");
        var replaced = NewLtc("replaced");
        
        var actual = new TestPartition("a")
        {
            Contents = [replacement, replaced]
        };
        
        replaced.ReplaceWith(replacement);
        
        var expected = new TestPartition("a")
        {
            Contents = [new LinkTestConcept("replacement")]
        };
        
        AssertEquals([expected], [actual]);
    }
    
    [TestMethod]
    public void ChildReplaced_Multiple_InSameContainment_Backward_WithTwoChildren()
    {
        var replacement = new LinkTestConcept("replacement");
        var replaced = NewLtc("replaced");
        
        var actual = new TestPartition("a")
        {
            Contents = [replaced, replacement]
        };
        
        replaced.ReplaceWith(replacement);
        
        var expected = new TestPartition("a")
        {
            Contents = [new LinkTestConcept("replacement")]
        };
        
        AssertEquals([expected], [actual]);
    } 
    
    [TestMethod]
    public void ChildReplaced_Multiple_InSameContainment_Forward_ReplaceLast()
    {
        var replacement = new LinkTestConcept("replacement");
        var replaced = NewLtc("child");
        
        var actual = new TestPartition("a")
        {
            Contents = [NewLtc("child"), replacement, replaced]
        };
        
        replaced.ReplaceWith(replacement);
        
        var expected = new TestPartition("a")
        {
            Contents = [NewLtc("child"), new LinkTestConcept("replacement")]
        };
        
        AssertEquals([expected], [actual]);
    } 
    
    
    [TestMethod]
    public void ChildReplaced_Multiple_InSameContainment_Backward_ReplaceMiddle()
    {
        var replacement = new LinkTestConcept("replacement");
        var replaced = NewLtc("replaced");
        
        var actual = new TestPartition("a")
        {
            Contents = [NewLtc("child"), replaced, replacement]
        };
        
        replaced.ReplaceWith(replacement);
        
        var expected = new TestPartition("a")
        {
            Contents = [NewLtc("child"), new LinkTestConcept("replacement")]
        };
        
        AssertEquals([expected], [actual]);
    } 
    
    [TestMethod]
    public void ChildReplaced_Multiple_InSameContainment_Backward_ReplaceFirst()
    {
        var replacement = new LinkTestConcept("replacement");
        var replaced = NewLtc("replaced");
        
        var actual = new TestPartition("a")
        {
            Contents = [replaced, NewLtc("child"), replacement]
        };
        
        replaced.ReplaceWith(replacement);
        
        var expected = new TestPartition("a")
        {
            Contents = [new LinkTestConcept("replacement"), NewLtc("child")]
        };
        
        AssertEquals([expected], [actual]);
    }
    
    [TestMethod]
    public void ChildReplaced_Multiple_InSameContainment_Backward_MoreThanThreeChildren()
    {
        var replacement = new LinkTestConcept("E");
        var replaced = NewLtc("B");
        
        var actual = new TestPartition("container")
        {
            Contents = [NewLtc("A"), replaced, NewLtc("C"), NewLtc("D"), replacement, NewLtc("F")]
        };
        
        replaced.ReplaceWith(replacement);
        
        var expected = new TestPartition("container")
        {
            Contents = [NewLtc("A"), new LinkTestConcept("E"), NewLtc("C"), NewLtc("D"), NewLtc("F")]
        };
        
        AssertEquals([expected], [actual]);
    }

    private LinkTestConcept NewLtc(string id) => new(id) { Name = id };

    private static void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
    
    #endregion

    [TestMethod]
    public void Beginning()
    {
        var circle = new LinkTestConcept("circ0");
        var offsetDuplicate = new LinkTestConcept("off0");

        var geometry = new TestPartition("geom")
        {
            Contents =
            [
                circle,
                offsetDuplicate
            ]
        };
        var line = new LinkTestConcept("line");
        circle.ReplaceWith(line);

        Assert.AreEqual(geometry, line.GetParent());
        Assert.IsNull(circle.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept> { line, offsetDuplicate }, geometry.Contents.ToList());
    }

    [TestMethod]
    public void Middle()
    {
        var circle = new LinkTestConcept("circ0");
        var offsetDuplicate = new LinkTestConcept("off0");
        var composite = new LinkTestConcept("comp0");

        var geometry = new TestPartition("geom")
        {
            Contents =
            [
                circle,
                offsetDuplicate,
                composite
            ]
        };
        var line = new LinkTestConcept("line");
        offsetDuplicate.ReplaceWith(line);

        Assert.AreEqual(geometry, line.GetParent());
        Assert.IsNull(offsetDuplicate.GetParent());

        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle, line, composite }, geometry.Contents.ToList());
    }

    [TestMethod]
    public void NoParent()
    {
        var circle = new LinkTestConcept("circ0");
        var line = new LinkTestConcept("line");

        Assert.ThrowsExactly<TreeShapeException>(() => circle.ReplaceWith(line));
    }

    [TestMethod]
    public void SingleContainment()
    {
        var coord = new LinkTestConcept("coord0");
        var circle = new LinkTestConcept("circ0") { Containment_0_1 = coord };

        var newCoord = new LinkTestConcept("coord1");
        coord.ReplaceWith(newCoord);

        Assert.AreEqual(circle, newCoord.GetParent());
        Assert.IsNull(coord.GetParent());
        Assert.AreEqual(newCoord, circle.Containment_0_1);
    }

    [TestMethod]
    public void NonFittingType()
    {
        var circle = new LinkTestConcept("circ0");

        var geometry = new TestPartition("geom")
        {
            Contents =
            [
                circle
            ]
        };
        var coord = new DataTypeTestConcept("coord");
        Assert.ThrowsExactly<InvalidValueException>(() => circle.ReplaceWith(coord));
    }

    [TestMethod]
    public void Null()
    {
        var circle = new LinkTestConcept("circ0");

        var geometry = new TestPartition("geom")
        {
            Contents =
            [
                circle
            ]
        };
        Assert.ThrowsExactly<UnsupportedNodeTypeException>(() => circle.ReplaceWith((INode)null));
    }
}

[TestClass]
public class ReplaceTests_Annotation
{
    [TestMethod]
    public void Beginning()
    {
        var doc = new TestAnnotation("circ0");
        var bom = new TestAnnotation("off0");

        var shape = new LinkTestConcept("geom");
        shape.AddAnnotations([doc, bom]);
        var ann = new TestAnnotation("line");
        doc.ReplaceWith(ann);

        Assert.AreEqual(shape, ann.GetParent());
        Assert.IsNull(doc.GetParent());

        CollectionAssert.AreEqual(new List<INode> { ann, bom }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Middle()
    {
        var doc = new TestAnnotation("circ0");
        var bom = new TestAnnotation("off0");
        var bom2 = new TestAnnotation("comp0");

        var shape = new LinkTestConcept("geom");
        shape.AddAnnotations([doc, bom, bom2]);
        var ann = new TestAnnotation("line");
        bom.ReplaceWith(ann);

        Assert.AreEqual(shape, ann.GetParent());
        Assert.IsNull(bom.GetParent());

        CollectionAssert.AreEqual(new List<INode> { doc, ann, bom2 }, shape.GetAnnotations().ToList());
    }

    [TestMethod]
    public void NoParent()
    {
        var doc = new TestAnnotation("circ0");
        var bom = new TestAnnotation("line");

        Assert.ThrowsExactly<TreeShapeException>(() => doc.ReplaceWith(bom));
    }

    [TestMethod]
    public void NonFittingType()
    {
        var doc = new TestAnnotation("circ0");

        var shape = new LinkTestConcept("geom");
        shape.AddAnnotations([doc]);
        var coord = new DataTypeTestConcept("coord");
        Assert.ThrowsExactly<InvalidValueException>(() => doc.ReplaceWith(coord));
    }

    [TestMethod]
    public void Null()
    {
        var doc = new TestAnnotation("circ0");

        var shape = new LinkTestConcept("geom");
        shape.AddAnnotations([doc]);
        Assert.ThrowsExactly<UnsupportedNodeTypeException>(() => doc.ReplaceWith((INode)null));
    }
}