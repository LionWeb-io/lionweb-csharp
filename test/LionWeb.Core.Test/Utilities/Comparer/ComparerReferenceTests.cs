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
using M3;

[TestClass]
public class ComparerReferenceTests : ComparerTestsBase
{
    [TestMethod]
    public void Single_Same()
    {
        var left = lF.NewGeometry("0a");
        var leftShape = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aT");
        leftShape.Source = leftSource;
        left.AddShapes([leftShape, leftSource]);
        var right = rF.NewGeometry("0b");
        var rightShape = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bT");
        rightShape.SetSource(rightSource);
        right.AddShapes([rightShape, rightSource]);

        AreEqual(left, right);
    }
    
    [TestMethod]
    public void Single_ToSingleContainment()
    {
        var lionWebVersion = LionWebVersions.v2023_1;
        var lang = new DynamicLanguage("l", lionWebVersion) {Key = "l", Version = "l", Name = "l"};
        var concept = lang.Concept("c", "c", "c");
        var cont = concept.Containment("cont", "cont", "cont").OfType(lionWebVersion.BuiltIns.Node).IsOptional(true)
            .IsMultiple(false);
        var refer = concept.Reference("ref", "ref", "ref").OfType(lionWebVersion.BuiltIns.Node).IsOptional(true).IsMultiple(false);

        var left = SimpleTree(concept, cont, refer);
        var right = SimpleTree(concept, cont, refer);

        AreEqual(left, right);
        var leftHash = new Hasher([left]).Hash();
        var rightHash = new Hasher([right]).Hash();
        
        Assert.AreEqual(leftHash, rightHash);
    }

    private static DynamicNode SimpleTree(DynamicConcept concept, DynamicContainment cont, DynamicReference refer)
    {
        var root = new DynamicNode("root", concept);
        var child = new DynamicNode("child", concept);
        var grandChild = new DynamicNode("grandChild", concept);
        root.Set(cont, child);
        child.Set(cont, grandChild);
        grandChild.Set(refer, child);

        return root;
    }

    [TestMethod]
    public void Single_Same_Root()
    {
        var left = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aT");
        left.Source = leftSource;
        var right = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bT");
        right.SetSource(rightSource);

        AreEqual([left, leftSource], [right, rightSource]);
    }

    [TestMethod]
    public void Single_Same_External()
    {
        var externalSource = lF.NewCircle("aT");

        var left = lF.NewOffsetDuplicate("a");
        left.Source = externalSource;
        var right = rF.NewOffsetDuplicate("b");
        right.SetSource(externalSource);

        AreEqual(left, right);
    }

    [TestMethod]
    public void Single_Same_Null()
    {
        var left = lF.NewOffsetDuplicate("a");
        left.AltSource = null;
        var right = rF.NewOffsetDuplicate("b");
        right.SetAltSource(null);
        AreEqual(left, right);
    }

    [TestMethod]
    public void Single_Different_SameTarget()
    {
        var left = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aa");
        leftSource.Name = "hi";
        left.Source = leftSource;
        var right = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bb");
        rightSource.SetName("bye");
        right.SetSource(rightSource);

        var parent = new NodeDifference(leftSource, rightSource);
        AreDifferent([left, leftSource], [right, rightSource],
            parent,
            new PropertyValueDifference(leftSource, "hi", _builtIns.INamed_name, rightSource, "bye")
            {
                Parent = parent
            }
        );
    }

    [TestMethod]
    public void Single_Different_External()
    {
        var left = lF.NewOffsetDuplicate("a");
        var leftSourceExternal = lF.NewCircle("aT");
        left.Source = leftSourceExternal;

        var right = rF.NewOffsetDuplicate("b");
        var rightSourceExternal = rF.NewCircle("bT");
        right.SetSource(rightSourceExternal);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, leftOffsetDuplicateSource, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new ExternalTargetDifference(left, leftSourceExternal, leftOffsetDuplicateSource, right,
                rightSourceExternal) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Single_Different_Internal()
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

        var parentA = new NodeDifference(leftA, rightA);
        var parentB = new ReferenceDifference(leftA, leftOffsetDuplicateSource, rightA) { Parent = parentA };
        var parentC = new NodeDifference(leftSource, rightB);
        var parentD = new NodeDifference(leftB, rightSource);
        var namedName = _builtIns.INamed_name;
        AreDifferent([leftA, leftSource, leftB], [rightA, rightB, rightSource],
            parentA,
            parentB,
            new InternalTargetDifference(leftA, leftSource, leftOffsetDuplicateSource, rightA, rightSource)
            {
                Parent = parentB
            },
            parentC,
            new UnsetFeatureRightDifference(leftSource, namedName, rightB) { Parent = parentC },
            parentD,
            new UnsetFeatureLeftDifference(leftB, namedName, rightSource) { Parent = parentD }
        );
    }

    [TestMethod]
    public void Single_Different_InternalExternal()
    {
        var external = rF.NewCircle("ex");

        var leftA = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aa");
        leftA.Source = leftSource;

        var rightA = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bb");
        rightA.SetSource(external);

        var parentA = new NodeDifference(leftA, rightA);
        var parentB = new ReferenceDifference(leftA, leftOffsetDuplicateSource, rightA) { Parent = parentA };
        AreDifferent([leftA, leftSource], [rightA, rightSource],
            parentA,
            parentB,
            new ExternalTargetRightDifference(leftA, leftSource, leftOffsetDuplicateSource, rightA, external)
            {
                Parent = parentB
            }
        );
    }

    [TestMethod]
    public void Single_Different_ExternalInternal()
    {
        var external = lF.NewCircle("ex");

        var leftA = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aa");
        leftA.Source = external;

        var rightA = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bb");
        rightA.SetSource(rightSource);

        var parentA = new NodeDifference(leftA, rightA);
        var parentB = new ReferenceDifference(leftA, leftOffsetDuplicateSource, rightA) { Parent = parentA };
        AreDifferent([leftA, leftSource], [rightA, rightSource],
            parentA,
            parentB,
            new ExternalTargetLeftDifference(leftA, external, leftOffsetDuplicateSource, rightA, rightSource)
            {
                Parent = parentB
            }
        );
    }

    [TestMethod]
    public void Single_LeftNull()
    {
        var left = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aa");
        left.AltSource = null;
        var right = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bb");
        right.SetAltSource(rightSource);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, rightOffsetDuplicateAltSource, right) { Parent = parentA };
        AreDifferent([left, leftSource], [right, rightSource],
            parentA,
            parentB,
            new UnsetFeatureLeftDifference(left, rightOffsetDuplicateAltSource, right) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Single_LeftUnset()
    {
        var left = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aa");
        var right = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bb");
        right.SetSource(rightSource);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, rightOffsetDuplicateSource, right) { Parent = parentA };
        AreDifferent([left, leftSource], [right, rightSource],
            parentA,
            parentB,
            new UnsetFeatureLeftDifference(left, rightOffsetDuplicateSource, right) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Single_RightNull()
    {
        var left = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aa");
        left.AltSource = leftSource;
        var right = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bb");
        right.SetAltSource(null);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, leftOffsetDuplicateAltSource, right) { Parent = parentA };
        AreDifferent([left, leftSource], [right, rightSource],
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftOffsetDuplicateAltSource, right) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Single_RightUnset()
    {
        var left = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aa");
        left.Source = leftSource;
        var right = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bb");

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, leftOffsetDuplicateSource, right) { Parent = parentA };
        AreDifferent([left, leftSource], [right, rightSource],
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftOffsetDuplicateSource, right) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Single_Disjoint()
    {
        var left = lF.NewOffsetDuplicate("a");
        var leftSource = lF.NewCircle("aa");
        left.AltSource = leftSource;
        var right = rF.NewOffsetDuplicate("b");
        var rightSource = rF.NewCircle("bb");
        right.SetSource(rightSource);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, leftOffsetDuplicateAltSource, right) { Parent = parentA };
        var parentC = new ReferenceDifference(left, rightOffsetDuplicateSource, right) { Parent = parentA };
        AreDifferent([left, leftSource], [right, rightSource],
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftOffsetDuplicateAltSource, right) { Parent = parentB },
            parentC,
            new UnsetFeatureLeftDifference(left, rightOffsetDuplicateSource, right) { Parent = parentC }
        );
    }

    [TestMethod]
    public void Multiple_Same_Internal()
    {
        var left = lF.NewBillOfMaterials("a");
        var left0 = lF.NewCircle("aa0");
        var left1 = lF.NewLine("aa1");
        left.AddMaterials(left0, left1);
        var right = rF.NewBillOfMaterials("b");
        var right0 = rF.NewCircle("bb0");
        var right1 = rF.NewLine("bb1");
        right.AddMaterials(right0, right1);
        AreEqual([left, left0, left1], [right, right0, right1]);
    }

    [TestMethod]
    public void Multiple_Same_External()
    {
        var external0 = lF.NewCircle("bb0");
        var external1 = lF.NewLine("bb1");

        var left = lF.NewBillOfMaterials("a");
        left.AddMaterials(external0, external1);
        var right = rF.NewBillOfMaterials("b");
        right.AddMaterials(external0, external1);
        AreEqual([left], [right]);
    }

    [TestMethod]
    public void Multiple_Same_Mixed()
    {
        var external = lF.NewCircle("ex");

        var left = lF.NewBillOfMaterials("a");
        var left0 = lF.NewCircle("aa0");
        left.AddMaterials(left0, external);
        var right = rF.NewBillOfMaterials("b");
        var right0 = rF.NewCircle("bb0");
        right.AddMaterials(right0, external);
        AreEqual([left, left0], [right, right0]);
    }

    [TestMethod]
    public void Multiple_Same_Null()
    {
        var left = lF.NewBillOfMaterials("a");
        var right = rF.NewBillOfMaterials("b");
        AreEqual(left, right);
    }

    [TestMethod]
    public void Multiple_DifferentTargets_Internal()
    {
        var left = lF.NewBillOfMaterials("a");
        var left0 = lF.NewCircle("aa0");
        var left1 = lF.NewLine("aa1");
        left.AddMaterials(left0, left1);
        var right = rF.NewBillOfMaterials("b");
        var right0 = rF.NewCircle("bb0");
        var right1 = rF.NewLine("bb1");
        right.AddMaterials(right1, right0);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, leftBillOfMaterialsMaterials, right) { Parent = parentA };
        AreDifferent([left, left0, left1], [right, right0, right1],
            parentA,
            parentB,
            new InternalTargetDifference(left, left0, leftBillOfMaterialsMaterials, right, right1) { Parent = parentB },
            new InternalTargetDifference(left, left1, leftBillOfMaterialsMaterials, right, right0) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Multiple_DifferentTargets_External()
    {
        var left = lF.NewBillOfMaterials("a");
        var left0 = lF.NewCircle("aa0");
        var left1 = lF.NewLine("aa1");
        left.AddMaterials(left0, left1);
        var right = rF.NewBillOfMaterials("b");
        var right0 = rF.NewCircle("bb0");
        var right1 = rF.NewLine("bb1");
        right.AddMaterials(right1, right0);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, leftBillOfMaterialsMaterials, right) { Parent = parentA };
        AreDifferent([left], [right],
            parentA,
            parentB,
            new ExternalTargetDifference(left, left0, leftBillOfMaterialsMaterials, right, right1) { Parent = parentB },
            new ExternalTargetDifference(left, left1, leftBillOfMaterialsMaterials, right, right0) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Multiple_LeftMore()
    {
        var left = lF.NewBillOfMaterials("a");
        var left0 = lF.NewCircle("aa0");
        left.AddMaterials(left0);
        var right = rF.NewBillOfMaterials("b");
        right.AddMaterials([]);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, leftBillOfMaterialsMaterials, right) { Parent = parentA };
        AreDifferent(left, right,
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftBillOfMaterialsMaterials, right) { Parent = parentB },
            new NodeCountDifference(left, 1, leftBillOfMaterialsMaterials, right, 0) { Parent = parentB },
            new LeftSurplusNodeDifference(left, leftBillOfMaterialsMaterials, left0) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Multiple_RightMore()
    {
        var left = lF.NewBillOfMaterials("a");
        var left0 = lF.NewCircle("aa0");
        var left1 = lF.NewLine("aa1");
        left.AddMaterials(left0);
        var right = rF.NewBillOfMaterials("b");
        var right0 = rF.NewCircle("bb0");
        var right1 = rF.NewLine("bb1");
        right.AddMaterials(right0, right1);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, leftBillOfMaterialsMaterials, right) { Parent = parentA };
        AreDifferent([left, left0, left1], [right, right0, right1],
            parentA,
            parentB,
            new NodeCountDifference(left, 1, leftBillOfMaterialsMaterials, right, 2) { Parent = parentB },
            new RightSurplusNodeDifference(right, leftBillOfMaterialsMaterials, right1) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Multiple_LeftUnset()
    {
        var left = lF.NewBillOfMaterials("a");
        var left0 = lF.NewCircle("aa0");
        var right = rF.NewBillOfMaterials("b");
        var right0 = rF.NewCircle("bb0");
        right.AddMaterials(right0);

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, rightBillOfMaterialsMaterials, right) { Parent = parentA };
        AreDifferent([left, left0], [right, right0],
            parentA,
            parentB,
            new UnsetFeatureLeftDifference(left, rightBillOfMaterialsMaterials, right) { Parent = parentB },
            new NodeCountDifference(left, 0, rightBillOfMaterialsMaterials, right, 1) { Parent = parentB },
            new RightSurplusNodeDifference(right, rightBillOfMaterialsMaterials, right0) { Parent = parentB }
        );
    }

    [TestMethod]
    public void Multiple_RightUnset()
    {
        var left = lF.NewBillOfMaterials("a");
        var left0 = lF.NewCircle("aa0");
        var left1 = lF.NewLine("aa1");
        left.AddMaterials(left0, left1);
        var right = rF.NewBillOfMaterials("b");
        var right0 = rF.NewCircle("bb0");
        var right1 = rF.NewLine("bb1");

        var parentA = new NodeDifference(left, right);
        var parentB = new ReferenceDifference(left, leftBillOfMaterialsMaterials, right) { Parent = parentA };
        AreDifferent([left, left0, left1], [right, right0, right1],
            parentA,
            parentB,
            new UnsetFeatureRightDifference(left, leftBillOfMaterialsMaterials, right) { Parent = parentB },
            new NodeCountDifference(left, 2, leftBillOfMaterialsMaterials, right, 0) { Parent = parentB },
            new LeftSurplusNodeDifference(left, leftBillOfMaterialsMaterials, left0) { Parent = parentB },
            new LeftSurplusNodeDifference(left, leftBillOfMaterialsMaterials, left1) { Parent = parentB }
        );
    }
}