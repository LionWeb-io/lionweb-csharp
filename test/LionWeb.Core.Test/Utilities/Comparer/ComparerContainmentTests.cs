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

namespace LionWeb.Core.Test.Utilities.Comparer;

using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class ComparerContainmentTests : ComparerTestsBase
{
    [TestMethod]
    public void Single_Same()
    {
        var left = lF.NewCircle("a");
        left.Center = lF.NewCoord("aa");
        var right = rF.NewCircle("b");
        right.SetCenter(rF.NewCoord("bb"));
        AreEqual(left, right);
    }

    [TestMethod]
    public void Single_Same_Null()
    {
        var left = lF.NewGeometry("a");
        left.Documentation = null;
        var right = rF.NewGeometry("b");
        right.SetDocumentation(null);
        AreEqual(left, right);
    }

    [TestMethod]
    public void Single_Different()
    {
        var left = lF.NewCircle("a");
        Coord leftCenter = lF.NewCoord("aa");
        leftCenter.X = -10;
        left.Center = leftCenter;
        var right = rF.NewCircle("b");
        var rightCenter = rF.NewCoord("bb");
        rightCenter.SetX(+10);
        right.SetCenter(rightCenter);

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, leftCircleCoord, right) { Parent = parentA };
        var parentC = new NodeDifference(leftCenter, rightCenter) { Parent = parentB };
        AreDifferent(left, right,
            parentA,
            parentB,
            parentC,
            new PropertyValueDifference(leftCenter, -10, leftCoordX, rightCenter, 10) { Parent = parentC }
        );
    }

    [TestMethod]
    public void Single_LeftNull()
    {
        var left = lF.NewGeometry("a");
        left.Documentation = null;
        var right = rF.NewGeometry("b");
        var rightDoc = rF.NewDocumentation("bb");
        rightDoc.SetText("+10");
        right.SetDocumentation(rightDoc);

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, rightGeometryDocumentation, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new UnsetFeatureLeftDifference(left, rightGeometryDocumentation, right) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Single_LeftUnset()
    {
        var left = lF.NewCircle("a");
        var right = rF.NewCircle("b");
        var rightCenter = rF.NewCoord("bb");
        rightCenter.SetX(+10);
        right.SetCenter(rightCenter);

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, rightCircleCoord, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new UnsetFeatureLeftDifference(left, rightCircleCoord, right) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Single_RightNull()
    {
        var left = lF.NewGeometry("a");
        Documentation leftDoc = lF.NewDocumentation("aa");
        leftDoc.Text = "-10";
        left.Documentation = leftDoc;
        var right = rF.NewGeometry("b");
        right.SetDocumentation(null);

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, leftGeometryDocumentation, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftGeometryDocumentation, right) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Single_RightUnset()
    {
        var left = lF.NewCircle("a");
        Coord leftCenter = lF.NewCoord("aa");
        leftCenter.X = -10;
        left.Center = leftCenter;
        var right = rF.NewCircle("b");

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, leftCircleCoord, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftCircleCoord, right) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Single_Disjoint()
    {
        var left = lF.NewLine("a");
        left.Start = lF.NewCoord("aa");
        var right = rF.NewLine("b");
        right.SetEnd(rF.NewCoord("bb"));

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, leftLineStart, right) { Parent = parentA };
        var parentC = new ContainmentDifference(left, rightLineEnd, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftLineStart, right) { Parent = parentB },
            parentC,
            new UnsetFeatureLeftDifference(left, rightLineEnd, right) { Parent = parentC }
        );
    }

    [TestMethod]
    public void Multiple_Same()
    {
        var left = lF.NewGeometry("a");
        left.AddShapes(lF.NewCircle("aa0"), lF.NewLine("aa1"));
        var right = rF.NewGeometry("b");
        right.AddShapes(rF.NewCircle("bb0"), rF.NewLine("bb1"));
        AreEqual(left, right);
    }

    [TestMethod]
    public void Multiple_Same_Null()
    {
        var left = lF.NewGeometry("a");
        var right = rF.NewGeometry("b");
        AreEqual(left, right);
    }

    [TestMethod]
    public void Multiple_DifferentTypes()
    {
        var left = lF.NewGeometry("a");
        var left0 = lF.NewCircle("aa0");
        var left1 = lF.NewLine("aa1");
        left.AddShapes(left0, left1);
        var right = rF.NewGeometry("b");
        var right0 = rF.NewLine("bb0");
        var right1 = rF.NewCircle("bb1");
        right.AddShapes(right0, right1);

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, leftGeometryShapes, right) { Parent = parentA };
        var parentC = new NodeDifference(left0, right0) { Parent = parentB };
        var parentD = new NodeDifference(left1, right1) { Parent = parentB };
        AreDifferent(left, right,
            parentA,
            parentB,
            parentC,
            new ClassifierDifference(left0, right0) { Parent = parentC },
            parentD,
            new ClassifierDifference(left1, right1) { Parent = parentD }
        );
    }

    [TestMethod]
    public void Multiple_LeftMore()
    {
        var left = lF.NewGeometry("a");
        var left0 = lF.NewCircle("aa0");
        left.AddShapes(left0);
        var right = rF.NewGeometry("b");
        right.AddShapes([]);

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, leftGeometryShapes, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftGeometryShapes, right){Parent = parentB},
            new NodeCountDifference(left, 1, leftGeometryShapes, right, 0) { Parent = parentB },
            new LeftSurplusNodeDifference(left, leftGeometryShapes, left0) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Multiple_RightMore()
    {
        var left = lF.NewGeometry("a");
        var left0 = lF.NewCircle("aa0");
        left.AddShapes(left0);
        var right = rF.NewGeometry("b");
        var right0 = rF.NewCircle("bb0");
        var right1 = rF.NewLine("bb1");
        right.AddShapes(right0, right1);

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, leftGeometryShapes, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new NodeCountDifference(left, 1, leftGeometryShapes, right, 2) { Parent = parentB },
            new RightSurplusNodeDifference(right, leftGeometryShapes, right1) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Multiple_LeftUnset()
    {
        var left = lF.NewGeometry("a");
        var right = rF.NewGeometry("b");
        var right0 = rF.NewCircle("bb0");
        right.AddShapes(right0);

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, rightGeometryShapes, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new UnsetFeatureLeftDifference(left, rightGeometryShapes, right) { Parent = parentB },
            new NodeCountDifference(left, 0, rightGeometryShapes, right, 1) { Parent = parentB },
            new RightSurplusNodeDifference(right, rightGeometryShapes, right0) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Multiple_RightUnset()
    {
        var left = lF.NewGeometry("a");
        var left0 = lF.NewCircle("aa0");
        var left1 = lF.NewLine("aa1");
        left.AddShapes(left0, left1);
        var right = rF.NewGeometry("b");

        var parentA = new NodeDifference(left, right);
        var parentB = new ContainmentDifference(left, leftGeometryShapes, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftGeometryShapes, right) { Parent = parentB },
            new NodeCountDifference(left, 2, leftGeometryShapes, right, 0) { Parent = parentB },
            new LeftSurplusNodeDifference(left, leftGeometryShapes, left0) { Parent = parentB },
            new LeftSurplusNodeDifference(left, leftGeometryShapes, left1) { Parent = parentB }
        );
    }
}