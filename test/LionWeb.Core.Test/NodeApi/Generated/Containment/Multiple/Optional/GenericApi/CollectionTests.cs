// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Multiple.Optional.GenericApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[0];
        parent.Add(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void Insert_EmptyArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[0];
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void Remove_EmptyArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[0];
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsTrue(parent.Links.Count == 0);
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }


    [TestMethod]
    public void Insert_NullArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    [TestMethod]
    public void Remove_NullArray()
    {
        var parent = new TestPartition("g");
        var values = new LinkTestConcept[] { null };
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values));
        Assert.IsTrue(parent.Links.Count == 0);
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        parent.Add(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }


    [TestMethod]
    public void Insert_SingleArray()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };
        parent.Insert(TestLanguageLanguage.Instance.TestPartition_links, 0, values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.Links.Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleArray_Remove_Empty()
    {
        var parent = new TestPartition("g");
        var line = new LinkTestConcept("myId");
        var values = new LinkTestConcept[] { line };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Links.Contains(line));
    }

    [TestMethod]
    public void SingleArray_Remove_Only()
    {
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [line] };
        var values = new LinkTestConcept[] { line };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { }, parent.Links.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [line, circle] };
        var values = new LinkTestConcept[] { line };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [circle, line] };
        var values = new LinkTestConcept[] { line };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Links.ToList());
    }

    [TestMethod]
    public void SingleArray_Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var parent = new TestPartition("g") { Links = [circleA, line, circleB] };
        var values = new LinkTestConcept[] { line };
        parent.Remove(TestLanguageLanguage.Instance.TestPartition_links, values);
        Assert.AreSame(parent, circleA.GetParent());
        Assert.AreSame(parent, circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Links.ToList());
    }

    #endregion

    #endregion
}