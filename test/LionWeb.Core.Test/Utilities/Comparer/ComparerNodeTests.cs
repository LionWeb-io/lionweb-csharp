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
using M2;

[TestClass]
public class ComparerNodeTests : ComparerTestsBase
{
    [TestMethod]
    public void Concept_Same()
    {
        var left = lF.NewLine("a");
        var right = rF.NewLine("b");
        AreEqual(left, right);
    }

    [TestMethod]
    public void Concept_Different()
    {
        var left = lF.NewLine("a");
        var right = rF.NewCircle("b");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new ClassifierDifference(left, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Concept_Annotation()
    {
        var left = lF.NewLine("a");
        var right = rF.NewDocumentation("b");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new IncompatibleClassifierDifference(left, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Annotation_Concept()
    {
        var left = lF.NewDocumentation("b");
        var right = rF.NewLine("a");

        var parent = new NodeDifference(left, right);
        AreDifferent(left, right,
            parent,
            new IncompatibleClassifierDifference(left, right) { Parent = parent }
        );
    }

    [TestMethod]
    public void Annotation_Concept_Ignored_Same()
    {
        var left = lF.NewDocumentation("b");
        var right = new LenientNode("r", ShapesLanguage.Instance.Documentation);

        Comparer comparer = new Comparer([(IReadableNode?)left], [right])
        {
            BehaviorConfig = new()
            {
                CompareCompatibleClassifier = false
            }
        };
        Assert.IsTrue(comparer.AreEqual(), comparer.ToMessage(OutputConfig));
    }

    [TestMethod]
    public void Annotation_Concept_Ignored_Different()
    {
        var left = lF.NewDocumentation("b");
        var right = rF.NewLine("a");

        Comparer comparer = new Comparer([(IReadableNode?)left], [right])
        {
            BehaviorConfig = new()
            {
                CompareCompatibleClassifier = false
            }
        };
        
        var actual = comparer.Compare().Distinct().ToList();
        var parent = new NodeDifference(left, right);
        List<IDifference> expected = [parent, new ClassifierDifference(left, right) { Parent = parent } ];

        CollectionAssert.AreEqual(expected, actual, comparer.ToMessage(OutputConfig));
    }

    [TestMethod]
    public void Single_Null_Same()
    {
        AreEqual((INode)null, (INode)null);
    }

    [TestMethod]
    public void Multi_Null_Same()
    {
        var left = lF.NewLine("a");
        var right = rF.NewLine("b");
        AreEqual([left, null], [right, null]);
    }

    [TestMethod]
    public void Single_Null_Different_Left()
    {
        var right = rF.NewLine("b");
        AreDifferent(null, right,
            new LeftNullNodeDifference(null, null, null, right)
        );
    }

    [TestMethod]
    public void Single_Null_Different_Right()
    {
        var left = lF.NewLine("a");
        AreDifferent(left, null,
            new RightNullNodeDifference(null, null, null, left)
        );
    }

    [TestMethod]
    public void Multi_Null_Different_Left()
    {
        var left = lF.NewLine("a");
        var right0 = rF.NewCircle("b0");
        var right1 = rF.NewLine("b1");
        AreDifferent([null, left], [right0, right1],
            new LeftNullNodeDifference(null, null, null, right0)
        );
    }

    [TestMethod]
    public void Multi_Null_Different_Right()
    {
        var left0 = lF.NewLine("a");
        var left1 = lF.NewCircle("a");
        var right = rF.NewCircle("b0");
        AreDifferent([left0, left1], [null, right],
            new RightNullNodeDifference(null, null, null, left0)
        );
    }

    [TestMethod]
    public void Multi_Different_Count_Left()
    {
        var left0 = lF.NewLine("a");
        var left1 = lF.NewCircle("a");
        var right = rF.NewLine("b0");
        AreDifferent([left0, left1], [right],
            new NodeCountDifference(null, 2, null, null, 1),
            new LeftSurplusNodeDifference(null, null, null, left1)
        );
    }

    [TestMethod]
    public void Multi_Different_Count_Right()
    {
        var right = rF.NewLine("b0");
        AreDifferent([], [right],
            new NodeCountDifference(null, 0, null, null, 1),
            new RightSurplusNodeDifference(null, null, null, right)
        );
    }

    [TestMethod]
    public void createEnum()
    {
        var enumerationLiteral = rF.GetEnumerationLiteral(rightLanguage.Enumerations().First().Literals.First());
        Assert.IsNotNull(enumerationLiteral);
        Assert.AreEqual("MatterState", enumerationLiteral.GetType().Name);
        Assert.AreEqual("solid", enumerationLiteral.ToString());
    }
}