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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional.GenericApi;

using Languages.Generated.V2026_1.TestLanguage;

[TestClass]
public class GenericApiTests
{
    #region Single

    [TestMethod]
    public void Reference_Single_Add()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        parent.Add(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(line));
    }

    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 0, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_Empty_UnderBounds()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, -1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Reference_0_n.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_Empty_OverBounds()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 1, [line]));
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Reference_0_n.Contains(line));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [circle] };
        var line = new LinkTestConcept("myId");
        parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 0, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { line, circle }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var circle = new LinkTestConcept("cId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [circle] };
        var line = new LinkTestConcept("myId");
        parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 1, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle, line }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("g") { Reference_0_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 0, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { line, circleA, circleB }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("g") { Reference_0_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 1, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, line, circleB }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var parent = new LinkTestConcept("g") { Reference_0_n = [circleA, circleB] };
        var line = new LinkTestConcept("myId");
        parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 2, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        Assert.IsTrue(parent.Reference_0_n.Contains(line));
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB, line }, parent.Reference_0_n.ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var line = new LinkTestConcept("myId");
        parent.Remove(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, [line]);
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Reference_0_n.Contains(line));
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var circle = new LinkTestConcept("myC");
        var parent = new LinkTestConcept("cs") { Reference_0_n = [circle] };
        var line = new LinkTestConcept("myId");
        parent.Remove(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [line] };
        parent.Remove(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, [line]);
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [line, circle] };
        parent.Remove(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var circle = new LinkTestConcept("cId");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [circle, line] };
        parent.Remove(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, [line]);
        Assert.IsNull(circle.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circle }, parent.Reference_0_n.ToList());
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var circleA = new LinkTestConcept("cIdA");
        var circleB = new LinkTestConcept("cIdB");
        var line = new LinkTestConcept("myId");
        var parent = new LinkTestConcept("g") { Reference_0_n = [circleA, line, circleB] };
        parent.Remove(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, [line]);
        Assert.IsNull(circleA.GetParent());
        Assert.IsNull(circleB.GetParent());
        Assert.IsNull(line.GetParent());
        CollectionAssert.AreEqual(new List<LinkTestConcept> { circleA, circleB }, parent.Reference_0_n.ToList());
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new LinkTestConcept("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Add(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, [null]));
    }

    [TestMethod]
    public void Null_Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 0, null));
    }

    [TestMethod]
    public void Null_Insert_Empty_OutOfBounds()
    {
        var parent = new LinkTestConcept("g");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, 1, [null]));
    }

    [TestMethod]
    public void Null_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.Remove(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, null));
    }

    #endregion
}