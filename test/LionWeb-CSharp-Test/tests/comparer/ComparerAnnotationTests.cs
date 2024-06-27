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
// ReSharper disable SuggestVarOrType_SimpleTypes

namespace LionWeb.Utils.Tests.Comparer;

using Core.Utilities;

[TestClass]
public class ComparerAnnotationTests : ComparerTestsBase
{
    [TestMethod]
    public void Same()
    {
        var left = lF.NewCircle("a");
        left.AddAnnotations([lF.NewDocumentation("aa0"), lF.NewBillOfMaterials("aa1")]);
        var right = rF.NewCircle("b");
        right.AddAnnotations([rF.NewDocumentation("bb0"), rF.NewBillOfMaterials("bb1")]);
        AreEqual(left, right);
    }

    [TestMethod]
    public void DifferentTypes()
    {
        var left = lF.NewCircle("a");
        var left0 = lF.NewDocumentation("aa0");
        var left1 = lF.NewBillOfMaterials("aa1");
        left.AddAnnotations([left0, left1]);
        var right = rF.NewCircle("b");
        var right0 = rF.NewBillOfMaterials("bb0");
        var right1 = rF.NewDocumentation("bb1");
        right.AddAnnotations([right0, right1]);

        var parentA = new NodeDifference(left, right);
        var parentB = new AnnotationDifference(left, right) { Parent = parentA };
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
    public void LeftMore()
    {
        var left = lF.NewCircle("a");
        var left0 = lF.NewDocumentation("aa0");
        left.AddAnnotations([left0]);
        var right = rF.NewCircle("b");

        var parentA = new NodeDifference(left, right);
        var parentB = new AnnotationDifference(left, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new NodeCountDifference(left, 1, null, right, 0) { Parent = parentB },
            new LeftSurplusNodeDifference(left, null, left0) { Parent = parentB }
        );
    }

    [TestMethod]
    public void RightMore()
    {
        var left = lF.NewCircle("a");
        var left0 = lF.NewDocumentation("aa0");
        left.AddAnnotations([left0]);
        var right = rF.NewCircle("b");
        var right0 = rF.NewDocumentation("bb0");
        var right1 = rF.NewBillOfMaterials("bb1");
        right.AddAnnotations([right0, right1]);

        var parentA = new NodeDifference(left, right);
        var parentB = new AnnotationDifference(left, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new NodeCountDifference(left, 1, null, right, 2) { Parent = parentB },
            new RightSurplusNodeDifference(right, null, right1) { Parent = parentB }
        );
    }

    [TestMethod]
    public void LeftUnset()
    {
        var left = lF.NewCircle("a");
        var right = rF.NewCircle("b");
        var right0 = rF.NewDocumentation("bb0");
        right.AddAnnotations([right0]);

        var parentA = new NodeDifference(left, right);
        var parentB = new AnnotationDifference(left, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new NodeCountDifference(left, 0, null, right, 1) { Parent = parentB },
            new RightSurplusNodeDifference(right, null, right0) { Parent = parentB }
        );
    }

    [TestMethod]
    public void RightUnset()
    {
        var left = lF.NewCircle("a");
        var left0 = lF.NewDocumentation("aa0");
        var left1 = lF.NewBillOfMaterials("aa1");
        left.AddAnnotations([left0, left1]);
        var right = rF.NewCircle("b");

        var parentA = new NodeDifference(left, right);
        var parentB = new AnnotationDifference(left, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new NodeCountDifference(left, 2, null, right, 0) { Parent = parentB },
            new LeftSurplusNodeDifference(left, null, left0) { Parent = parentB },
            new LeftSurplusNodeDifference(left, null, left1) { Parent = parentB }
        );
    }
}