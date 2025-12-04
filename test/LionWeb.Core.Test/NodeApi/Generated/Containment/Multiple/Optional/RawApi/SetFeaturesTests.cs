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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SetFeaturesTests
{
    [TestMethod]
    public void ContainmentMultipleOptional_Init()
    {
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.TryGetContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, out var result));
        Assert.IsInstanceOfType<List<LinkTestConcept>>(result);
        Assert.IsEmpty((List<LinkTestConcept>)result);
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Add()
    {
        var parent = new LinkTestConcept("g");
        var child = new LinkTestConcept("myId");
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, child));
        Assert.IsTrue(parent.TryGetContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, out var result));
        Assert.IsInstanceOfType<List<LinkTestConcept>>(result);
        CollectionAssert.AreEqual(new List<INode> { child }, (List<LinkTestConcept>)result);
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Insert()
    {
        var parent = new LinkTestConcept("g");
        var child = new LinkTestConcept("myId");
        Assert.IsTrue(parent.InsertContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, 0,
            child));
        Assert.IsTrue(parent.TryGetContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, out var result));
        Assert.IsInstanceOfType<List<LinkTestConcept>>(result);
        CollectionAssert.AreEqual(new List<INode> { child }, (List<LinkTestConcept>)result);
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var child = new LinkTestConcept("myId");
        Assert.IsFalse(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, child));
        Assert.IsTrue(parent.TryGetContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, out var result));
        Assert.IsInstanceOfType<List<LinkTestConcept>>(result);
        Assert.IsEmpty((List<LinkTestConcept>)result);
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Remove()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("myId");
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, value));
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, value));
        Assert.IsTrue(parent.TryGetContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, out var result));
        Assert.IsInstanceOfType<List<LinkTestConcept>>(result);
        Assert.IsEmpty((List<LinkTestConcept>)result);
    }

    [TestMethod]
    public void ContainmentMultipleOptional_RemovePart()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("myA");
        var valueB = new LinkTestConcept("myB");
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, valueA));
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, valueB));
        Assert.IsTrue(parent.RemoveContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, valueA));
        Assert.IsTrue(parent.TryGetContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, out var result));
        Assert.IsInstanceOfType<List<LinkTestConcept>>(result);
        CollectionAssert.AreEqual(new List<INode> { valueB }, (List<LinkTestConcept>)result);
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Reset_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("myId");
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, value));
        Assert.IsFalse(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, null));
        Assert.IsTrue(parent.TryGetContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, out var result));
        Assert.IsInstanceOfType<List<LinkTestConcept>>(result);
        CollectionAssert.AreEqual(new List<INode> { value }, (List<LinkTestConcept>)result);
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Overwrite_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("myA");
        Assert.IsTrue(parent.AddContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, value));
        var child = new LinkTestConcept("myB");
        Assert.IsFalse(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, child));
        Assert.IsTrue(parent.TryGetContainmentsRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_0_n, out var result));
        Assert.IsInstanceOfType<List<LinkTestConcept>>(result);
        CollectionAssert.AreEqual(new List<INode> { value }, (List<LinkTestConcept>)result);
    }
}