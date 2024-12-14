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

using Core.Utilities;
using Examples.Shapes.M2;

[TestClass]
public class ComparerPropertyTests : ComparerTestsBase
{
    [TestMethod]
    public void Same_String_Null()
    {
        var left = lF.NewDocumentation("a");
        left.Text = null;
        var right = rF.NewDocumentation("b");
        right.SetText(null);
        AreEqual(left, right);
    }

    [TestMethod]
    public void Same_String()
    {
        var left = lF.NewLine("a");
        left.Name = "hi";
        var right = rF.NewLine("b");
        right.SetName("hi");
        AreEqual(left, right);
    }

    [TestMethod]
    public void Different_String()
    {
        var left = lF.NewLine("a");
        left.Name = "hi";
        var right = rF.NewLine("b");
        right.SetName("bye");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new PropertyValueDifference(left, "hi", _builtIns.INamed_name, right, "bye") { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_String_LeftUnset()
    {
        var left = lF.NewLine("a");
        var right = rF.NewLine("b");
        right.SetName("bye");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureLeftDifference(left, _builtIns.INamed_name, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_String_LeftNull()
    {
        var left = lF.NewDocumentation("a");
        left.Text = null;
        var right = rF.NewDocumentation("b");
        right.SetText("bye");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureLeftDifference(left, rightDocumentationText, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_String_RightUnset()
    {
        var left = lF.NewLine("a");
        left.Name = "hi";
        var right = rF.NewLine("b");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureRightDifference(left, _builtIns.INamed_name, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_String_RightNull()
    {
        var left = lF.NewDocumentation("a");
        left.Text = "hi";
        var right = rF.NewDocumentation("b");
        right.SetText(null);

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureRightDifference(left, leftDocumentationText, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Same_Int()
    {
        var left = lF.NewCoord("a");
        left.X = 10;
        var right = rF.NewCoord("b");
        right.SetX(10);
        AreEqual(left, right);
    }

    [TestMethod]
    public void Different_Int()
    {
        var left = lF.NewCoord("a");
        left.X = 10;
        var right = rF.NewCoord("b");
        right.SetX(20);

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new PropertyValueDifference(left, 10, leftCoordX, right, 20) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_Int_LeftUnset()
    {
        var left = lF.NewCoord("a");
        var right = rF.NewCoord("b");
        right.SetX(20);

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureLeftDifference(left, rightCoordX, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_Int_RightUnset()
    {
        var left = lF.NewCoord("a");
        left.X = 10;
        var right = rF.NewCoord("b");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureRightDifference(left, leftCoordX, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Same_Bool()
    {
        var left = lF.NewDocumentation("a");
        left.Technical = true;
        var right = rF.NewDocumentation("b");
        right.SetTechnical(true);
        AreEqual(left, right);
    }

    [TestMethod]
    public void Different_Bool()
    {
        var left = lF.NewDocumentation("a");
        left.Technical = true;
        var right = rF.NewDocumentation("b");
        right.SetTechnical(false);

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new PropertyValueDifference(left, true, leftDocumentationTechnical, right, false) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_Bool_LeftUnset()
    {
        var left = lF.NewDocumentation("a");
        var right = rF.NewDocumentation("b");
        right.SetTechnical(false);

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureLeftDifference(left, rightDocumentationTechnical, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_Bool_RightUnset()
    {
        var left = lF.NewDocumentation("a");
        left.Technical = true;
        var right = rF.NewDocumentation("b");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureRightDifference(left, leftDocumentationTechnical, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Same_Enum()
    {
        // we construct both sides from _leftFactory_, otherwise we have different enum literals, and they are not equal.
        var left = lF.NewMaterialGroup("a");
        left.MatterState = MatterState.liquid;
        var pseudoRight = lF.NewMaterialGroup("b");
        pseudoRight.SetMatterState(lF.GetMatterState("liquid"));
        AreEqual(left, pseudoRight);
    }

    [TestMethod]
    public void Different_Enum_Type()
    {
        var left = lF.NewMaterialGroup("a");
        left.MatterState = MatterState.liquid;
        var right = rF.NewMaterialGroup("b");
        right.SetMatterState(rF.GetMatterState("liquid"));

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new PropertyEnumTypeDifference(left, MatterState.liquid, leftMaterialGroupMatterState, right,
                rF.GetMatterState("liquid")) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_Enum_Value()
    {
        var left = lF.NewMaterialGroup("a");
        left.MatterState = MatterState.liquid;
        var right = rF.NewMaterialGroup("b");
        right.SetMatterState(rF.GetMatterState("gas"));

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new PropertyValueDifference(left, MatterState.liquid, leftMaterialGroupMatterState, right,
                rF.GetMatterState("gas")) { Parent = parent },
            new PropertyEnumTypeDifference(left, MatterState.liquid, leftMaterialGroupMatterState, right,
                rF.GetMatterState("gas")) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_Enum_LeftUnset()
    {
        var left = lF.NewMaterialGroup("a");
        var right = rF.NewMaterialGroup("b");
        right.SetMatterState(rF.GetMatterState("liquid"));

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureLeftDifference(left, rightMaterialGroupMatterState, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_Enum_RightUnset()
    {
        var left = lF.NewMaterialGroup("a");
        left.MatterState = MatterState.liquid;
        var right = rF.NewMaterialGroup("b");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureRightDifference(left, leftMaterialGroupMatterState, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Different_Disjoint()
    {
        var left = lF.NewCoord("a");
        left.X = 10;
        var right = rF.NewCoord("b");
        right.SetY(10);

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new UnsetFeatureRightDifference(left, leftCoordX, right) { Parent = parent },
            new UnsetFeatureLeftDifference(left, rightCoordY, right) { Parent = parent }
        );
    }
}