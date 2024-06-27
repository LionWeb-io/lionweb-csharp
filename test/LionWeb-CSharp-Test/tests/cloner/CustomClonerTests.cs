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

namespace LionWeb.Utils.Tests.Cloner;

using Core;
using Core.M3;
using Core.Utilities;
using Examples.Shapes.M2;

[TestClass]
public class CustomClonerTests : ClonerTestsBase
{
    [TestMethod]
    public void CustomId()
    {
        Line lineA = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineA.Name = "LineA";

        Circle circle = ShapesLanguage.Instance.GetFactory().CreateCircle();
        circle.Name = "Circle";

        Line lineB = ShapesLanguage.Instance.GetFactory().CreateLine();
        lineB.Name = "LineB";

        var bom = ShapesLanguage.Instance.GetFactory().CreateBillOfMaterials();
        bom.AddMaterials([lineB, circle]);
        MaterialGroup materialGroup = ShapesLanguage.Instance.GetFactory().CreateMaterialGroup();
        materialGroup.AddMaterials([circle, lineB]);
        bom.AddGroups([materialGroup]);

        lineA.AddAnnotations([bom]);

        int id = 0;
        var actual = CustomIdCloner.Clone([lineA, lineB], () => id++.ToString()).ToList();

        var actualLineA = actual.First();
        Assert.AreEqual("0", actualLineA.GetId());

        var actualBom = actualLineA.GetAnnotations().First() as BillOfMaterials;
        Assert.AreEqual("1", actualBom.GetId());

        var actualLineB = actualBom.Materials.First();
        Assert.AreEqual("3", actualLineB.GetId());

        var actualMaterialGroup = actualBom.Groups.First();
        Assert.AreEqual("2", actualMaterialGroup.GetId());
    }

    private class CustomIdCloner : Cloner
    {
        private readonly Func<string> _idProvider;

        public CustomIdCloner(IEnumerable<INode> inputNodes, Func<string> idProvider) : base(inputNodes)
        {
            _idProvider = idProvider;
        }

        public static IEnumerable<INode> Clone(IEnumerable<INode> nodes, Func<string> idProvider)
        {
            HashSet<INode> hashSet;
            if (nodes is HashSet<INode> set)
            {
                hashSet = set;
            } else
            {
                hashSet = [..nodes];
            }

            return FilterCorresponding(hashSet, new CustomIdCloner(hashSet, idProvider).Clone());
        }

        protected override string GetNewId(INode inputNode) => _idProvider();
    }
    
    [TestMethod]
    public void NoGroups()
    {
        Line lineA = ShapesLanguage.Instance.GetFactory().NewLine("lineA");
        lineA.Name = "LineA";

        Circle circle = ShapesLanguage.Instance.GetFactory().NewCircle("circle");
        circle.Name = "Circle";

        Line lineB = ShapesLanguage.Instance.GetFactory().NewLine("lineB");
        lineB.Name = "LineB";

        var bom = ShapesLanguage.Instance.GetFactory(). NewBillOfMaterials("bom");
        bom.AddMaterials([lineB, circle]);
        MaterialGroup materialGroup = ShapesLanguage.Instance.GetFactory().NewMaterialGroup("materialGroup");
        materialGroup.AddMaterials([circle, lineB]);
        bom.AddGroups([materialGroup]);

        lineA.AddAnnotations([bom]);

        var actual = NoGroupsCloner.Clone([lineA, lineB]).ToList();

        Assert.AreEqual(2, actual.Count);

        Assert.IsInstanceOfType<Line>(actual.First());
        var actualLineA = actual.First() as Line;

        Assert.AreEqual(1, actualLineA.GetAnnotations().Count);
        Assert.IsInstanceOfType<BillOfMaterials>(actualLineA.GetAnnotations().First());
        BillOfMaterials actualBom = actualLineA.GetAnnotations().First() as BillOfMaterials;
        Assert.AreNotSame(bom, actualBom);
        Assert.AreNotEqual(bom.GetId(), actualBom.GetId());

        Assert.AreEqual(2, actualBom.Materials.Count);

        Assert.IsInstanceOfType<Line>(actualBom.Materials.First());
        Line actualLineB = actualBom.Materials.First() as Line;
        Assert.AreNotSame(lineB, actualLineB);
        Assert.AreNotEqual(lineB.GetId(), actualLineB.GetId());

        Assert.AreSame(circle, bom.Materials.Last());

        Assert.AreEqual(0, actualBom.Groups.Count);
    }

    private class NoGroupsCloner : Cloner
    {
        public NoGroupsCloner(IEnumerable<INode> inputNodes) : base(inputNodes)
        {
        }

        public static IEnumerable<INode> Clone(IEnumerable<INode> nodes)
        {
            HashSet<INode> hashSet;
            if (nodes is HashSet<INode> set)
            {
                hashSet = set;
            } else
            {
                hashSet = [..nodes];
            }

            return FilterCorresponding(hashSet, new NoGroupsCloner(hashSet).Clone());
        }

        protected override bool IncludeChild(INode inputNode, Containment cont) => cont switch
        {
            var v when v.EqualsIdentity(ShapesLanguage.Instance.BillOfMaterials_groups)  => false,
            _ => true
        };
    }
}