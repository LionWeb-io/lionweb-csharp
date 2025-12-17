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

namespace LionWeb.Core.Test;

using Core.Utilities;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M2;

[TestClass]
public class ModelAccessTests
{
    private static readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    
    [TestMethod]
    public void test_multi_valued_containment_access()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(TestLanguageLanguage.Instance) as TestPartition;
        Assert.AreEqual(1, geometry.Links.Count);
        var shape = geometry.Links[0];
        Assert.IsInstanceOfType<LinkTestConcept>(shape);
        var line = shape as LinkTestConcept;
        Assert.AreEqual("line1", line.Name);
        var named = line as INamed;
        Assert.IsTrue(named.TryGetName(out var n));
        Assert.AreEqual("line1", n);
        Assert.AreEqual(geometry, line.GetParent());
        var start = line.Containment_0_1;
        Assert.AreEqual(line, start.GetParent());
    }

    [TestMethod]
    public void test_factory_method_with_out_var()
    {
        var factory = TestLanguageLanguage.Instance.GetFactory();
        var line = new LinkTestConcept(IdUtils.NewId())
        {
            Name = "line1",
            Containment_0_1 = new LinkTestConcept(IdUtils.NewId()) { Name = "-1" },
            Containment_1 = new LinkTestConcept(IdUtils.NewId()) { Name = "1" }
        };
        var start = line.Containment_0_1;
        var end = line.Containment_1;
        Assert.AreEqual("line1", line.Name);
        Assert.AreEqual(start, line.Containment_0_1);
        Assert.IsInstanceOfType<LinkTestConcept>(start);
        Assert.AreEqual(end, line.Containment_1);
        Assert.IsInstanceOfType<LinkTestConcept>(end);
    }

    [TestMethod]
    public void test_nullable_features_can_be_set_to_null()
    {
        // no compiler warnings should be given on any of these lines:
        var documentation = TestLanguageLanguage.Instance.GetFactory().CreateDataTypeTestConcept();
        documentation.SetStringValue_0_1(null);
        Assert.IsNull(documentation.StringValue_0_1);
        var geometry = TestLanguageLanguage.Instance.GetFactory().CreateLinkTestConcept();
        geometry.SetContainment_0_1(null);
        Assert.IsNull(geometry.Containment_0_1);
        geometry.Containment_0_1 = new LinkTestConcept("child");
        geometry.SetContainment_0_1(new LinkTestConcept("child"));
        Assert.IsNotNull(geometry.Containment_0_1);
    }

    [TestMethod]
    public void test_AllChildren()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(TestLanguageLanguage.Instance) as TestPartition;
        var allChildren = geometry.Children(false, true);
        Assert.AreEqual(1, allChildren.Count());
        Assert.AreEqual(geometry.Links[0], allChildren.ElementAt(0));
    }

    [TestMethod]
    public void test_GetThisAndAllChildren()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(TestLanguageLanguage.Instance) as TestPartition;
        var thisAndAllChildren = geometry.Children(true, true);
        Assert.AreEqual(2, thisAndAllChildren.Count());
        Assert.AreEqual(geometry, thisAndAllChildren.ElementAt(0));
        Assert.AreEqual(geometry.Links[0], thisAndAllChildren.ElementAt(1));
    }

    [TestMethod]
    public void test_AllDescendants()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(TestLanguageLanguage.Instance) as TestPartition;
        var allDescendants = geometry.Descendants(false, true);
        Assert.AreEqual(3, allDescendants.Count());
        Assert.IsFalse(allDescendants.Contains(geometry));
        var line = geometry.Links[0] as LinkTestConcept;
        Assert.AreEqual(line, allDescendants.ElementAt(0));
        Assert.AreEqual(line.Containment_0_1, allDescendants.ElementAt(1));
        Assert.AreEqual(line.Containment_1, allDescendants.ElementAt(2));
    }

    [TestMethod]
    public void test_AllNodes()
    {
        var geometry = new ExampleModels(_lionWebVersion).ExampleModel(TestLanguageLanguage.Instance) as TestPartition;
        var allNodes = geometry.Descendants(true, true);
        Assert.AreEqual(4, allNodes.Count());
        Assert.AreEqual(geometry, allNodes.ElementAt(0));
        var line = geometry.Links[0] as LinkTestConcept;
        Assert.AreEqual(line, allNodes.ElementAt(1));
        Assert.AreEqual(line.Containment_0_1, allNodes.ElementAt(2));
        Assert.AreEqual(line.Containment_1, allNodes.ElementAt(3));
    }
}