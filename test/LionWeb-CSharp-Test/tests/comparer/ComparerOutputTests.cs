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

// ReSharper disable InconsistentNaming

namespace LionWeb.Utils.Tests.Comparer;

using Core;
using Core.Utilities;
using Examples.Shapes.M2;

[TestClass]
public class ComparerOutputTests : ComparerTestsBase
{
    [TestMethod]
    public void Single_Null_Different_Left()
    {
        var right = rF.NewLine("b");
        AssertOutput([null], [right],
            """
            left null: (b)<Line>

            """
        );
    }

    [TestMethod]
    public void Single_Null_Different_Right()
    {
        var left = lF.NewLine("a");
        AssertOutput([left], [null],
            """
            right null: (a)<Line>

            """
        );
    }

    [TestMethod]
    public void Multi_Null_Different_Left()
    {
        var left = lF.NewLine("a");
        var right0 = rF.NewCircle("b0");
        var right1 = rF.NewLine("b1");
        AssertOutput([null, left], [right0, right1],
            """
            left null: (b0)<Circle>

            """
        );
    }

    [TestMethod]
    public void Multi_Null_Different_Right()
    {
        var left0 = lF.NewLine("a");
        var left1 = lF.NewCircle("a0");
        var right = rF.NewCircle("b0");
        AssertOutput([left0, left1], [null, right],
            """
            right null: (a)<Line>

            """
        );
    }

    [TestMethod]
    public void Multi_Different_Count_Left()
    {
        var left0 = lF.NewLine("a");
        var left1 = lF.NewCircle("a0");
        var right = rF.NewLine("b0");
        AssertOutput([left0, left1], [right],
            """
            Number of nodes: left: 2 vs. right: 1
            Surplus: left: (a0)<Circle>

            """
        );
    }

    [TestMethod]
    public void Different_Annotation_Types()
    {
        var left = lF.NewCircle("a");
        var left0 = lF.NewDocumentation("aa0");
        var left1 = lF.NewBillOfMaterials("aa1");
        left.AddAnnotations([left0, left1]);
        var right = rF.NewCircle("b");
        var right0 = rF.NewBillOfMaterials("bb0");
        var right1 = rF.NewDocumentation("bb1");
        right.AddAnnotations([right0, right1]);

        AssertOutput([left], [right],
            """
            Node: left: (a)<Circle> vs. right: (b)<Circle>
              Annotations
                Node: left: (aa0)<Documentation> vs. right: (bb0)<BillOfMaterials>
                  Classifiers: left: <Documentation> vs. right: <BillOfMaterials>
                Node: left: (aa1)<BillOfMaterials> vs. right: (bb1)<Documentation>
                  Classifiers: left: <BillOfMaterials> vs. right: <Documentation>

            """
        );
    }

    [TestMethod]
    public void Single_Containment()
    {
        var left = lF.NewCircle("a");
        Coord leftCenter = lF.NewCoord("aa");
        leftCenter.X = -10;
        left.Center = leftCenter;
        var right = rF.NewCircle("b");
        var rightCenter = rF.NewCoord("bb");
        rightCenter.SetX(+10);
        right.SetCenter(rightCenter);

        AssertOutput([left], [right],
            """
            Node: left: (a)<Circle> vs. right: (b)<Circle>
              Containment <center>
                Node: left: (aa)<Coord> vs. right: (bb)<Coord>
                  Value of <x>: left: '-10' vs. right: '10'

            """
        );
    }

    [TestMethod]
    public void Single_Containment_LeftUnset()
    {
        var left = lF.NewCircle("a");
        var right = rF.NewCircle("b");
        var rightCenter = rF.NewCoord("bb");
        rightCenter.SetX(+10);
        right.SetCenter(rightCenter);

        AssertOutput([left], [right],
            """
            Node: left: (a)<Circle> vs. right: (b)<Circle>
              Containment <center>
                Feature <center> missing on left

            """
        );
    }

    [TestMethod]
    public void Single_Containment_RightUnset()
    {
        var left = lF.NewCircle("a");
        Coord leftCenter = lF.NewCoord("aa");
        leftCenter.X = -10;
        left.Center = leftCenter;
        var right = rF.NewCircle("b");

        AssertOutput([left], [right],
            """
            Node: left: (a)<Circle> vs. right: (b)<Circle>
              Containment <center>
                Feature <center> missing on right

            """
        );
    }

    [TestMethod]
    public void Single_Containment_Disjoint()
    {
        var left = lF.NewLine("a");
        left.Start = lF.NewCoord("aa");
        var right = rF.NewLine("b");
        right.SetEnd(rF.NewCoord("bb"));

        AssertOutput([left], [right],
            """
            Node: left: (a)<Line> vs. right: (b)<Line>
              Containment <start>
                Feature <start> missing on right
              Containment <end>
                Feature <end> missing on left

            """
        );
    }

    [TestMethod]
    public void Multiple_Containment_Different()
    {
        var left = lF.NewGeometry("a");
        var left0 = lF.NewCircle("aa0");
        var left1 = lF.NewLine("aa1");
        left.AddShapes(left0, left1);
        var right = rF.NewGeometry("b");
        var right0 = rF.NewLine("bb0");
        var right1 = rF.NewCircle("bb1");
        right.AddShapes(right0, right1);

        AssertOutput([left], [right],
            """
            Node: left: (a)<Geometry> vs. right: (b)<Geometry>
              Containment <shapes>
                Node: left: (aa0)<Circle> vs. right: (bb0)<Line>
                  Classifiers: left: <Circle> vs. right: <Line>
                Node: left: (aa1)<Line> vs. right: (bb1)<Circle>
                  Classifiers: left: <Line> vs. right: <Circle>

            """
        );
    }

    [TestMethod]
    public void Multiple_Containment_LeftMore()
    {
        var left = lF.NewGeometry("a");
        var left0 = lF.NewCircle("aa0");
        left.AddShapes(left0);
        var right = rF.NewGeometry("b");
        right.AddShapes([]);

        AssertOutput([left], [right],
            """
            Node: left: (a)<Geometry> vs. right: (b)<Geometry>
              Containment <shapes>
                Feature <shapes> missing on right
                Number of nodes: left: 1 vs. right: 0
                Surplus: left: (aa0)<Circle>

            """
        );
    }

    [TestMethod]
    public void Multiple_Containment_RightMore()
    {
        var left = lF.NewGeometry("a");
        var left0 = lF.NewCircle("aa0");
        left.AddShapes(left0);
        var right = rF.NewGeometry("b");
        var right0 = rF.NewCircle("bb0");
        var right1 = rF.NewLine("bb1");
        right.AddShapes(right0, right1);

        AssertOutput([left], [right],
            """
            Node: left: (a)<Geometry> vs. right: (b)<Geometry>
              Containment <shapes>
                Number of nodes: left: 1 vs. right: 2
                Surplus: right: (bb1)<Line>

            """
        );
    }

    [TestMethod]
    public void Single_Reference_Different_Internal()
    {
        var leftA = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aa");
        leftSource.Name = "hi";
        leftA.Source = leftSource;
        var leftB = lF.NewCircle("a0");

        var rightA = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bb");
        rightSource.SetName("bye");
        rightA.SetSource(rightSource);
        var rightB = rF.NewCircle("b0");

        AssertOutput([leftA, leftSource, leftB], [rightA, rightB, rightSource],
            """
            Node: left: (a)<OffsetDuplicate> vs. right: (b)<OffsetDuplicate>
              Reference <source>
                Internal target: left: hi(aa)<Circle> vs. right: bye(bb)<Circle>
            Node: left: hi(aa)<Circle> vs. right: (b0)<Circle>
              Feature <name> missing on right
            Node: left: (a0)<Circle> vs. right: bye(bb)<Circle>
              Feature <name> missing on left

            """
        );
    }

    [TestMethod]
    public void SideLabelsOverride()
    {
        var left0 = lF.NewLine("a");
        var left1 = lF.NewCircle("a");
        var right = rF.NewLine("b0");
        AssertOutput([left0, left1], [right],
            """
            Number of nodes: hello: 2 vs. bye: 1
            Surplus: hello: (a)<Circle>

            """,
            new ComparerOutputConfig { LeftDescription = "hello", RightDescription = "bye" }
        );
    }

    [TestMethod]
    public void FullClassifier()
    {
        var left = lF.NewLine("a");
        var right = rF.NewCircle("b");

        AssertOutput([left], [right],
            """
            Node: left: (a)<Line[key-Line,key-Shapes@1]> vs. right: (b)<Circle[key-Circle,key-Shapes@1]>
              Classifiers: left: <Line[key-Line,key-Shapes@1]> vs. right: <Circle[key-Circle,key-Shapes@1]>

            """,
            new ComparerOutputConfig { FullClassifier = true }
        );
    }

    [TestMethod]
    public void LanguageId()
    {
        var left = lF.NewLine("a");
        var right = rF.NewCircle("b");

        AssertOutput([left], [right],
            """
            Node: left: (a)<Line[key-Line,key-Shapes@1(id-Shapes)]> vs. right: (b)<Circle[key-Circle,key-Shapes@1(id-Shapes)]>
              Classifiers: left: <Line[key-Line,key-Shapes@1(id-Shapes)]> vs. right: <Circle[key-Circle,key-Shapes@1(id-Shapes)]>

            """,
            new ComparerOutputConfig { FullClassifier = true, LanguageId = true }
        );
    }

    [TestMethod]
    public void NodeName()
    {
        var left = lF.NewLine("a");
        left.Name = "Hello";
        var right = rF.NewCircle("b");
        right.SetName("Hello");

        AssertOutput([left], [right],
            """
            Node: left: Hello(a)<Line> vs. right: Hello(b)<Circle>
              Classifiers: left: <Line> vs. right: <Circle>

            """
        );
    }

    [TestMethod]
    public void NoNodeName()
    {
        var left = lF.NewLine("a");
        left.Name = "Hello";
        var right = rF.NewCircle("b");
        right.SetName("Hello");

        AssertOutput([left], [right],
            """
            Node: left: (a)<Line> vs. right: (b)<Circle>
              Classifiers: left: <Line> vs. right: <Circle>

            """,
            new ComparerOutputConfig { NodeName = false }
        );
    }

    private void AssertOutput(List<INode?> left, List<INode?> right, string expectedOutput) =>
        AssertOutput(left, right, expectedOutput, new ComparerOutputConfig());

    private void AssertOutput(List<INode?> left, List<INode?> right, string expectedOutput,
        ComparerOutputConfig outputConfig)
    {
        var comparer = new Comparer(left, right);
        comparer.Compare();
        Assert.AreEqual(expectedOutput, comparer.ToMessage(outputConfig));
    }
}