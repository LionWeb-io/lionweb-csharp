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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Required.RawApi;

using Languages.Generated.V2024_1.TestLanguage;
using System.Collections;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var values = new LinkTestConcept[0];
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var values = new ArrayList();
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var values = new List<INode>();
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var values = new LinkTestConcept[] { null };
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var values = new ArrayList() { null };
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var values = new List<INode>() { null };
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var value = new LinkTestConcept("s");
        var values = new LinkTestConcept[] { value };

        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var value = new LinkTestConcept("s");
        var values = new object[] { value };
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var value = new LinkTestConcept("s");
        var values = new ArrayList() { value };
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var value = new LinkTestConcept("s");
        var values = new List<INode>() { value };
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
        Assert.ThrowsExactly<UnsetFeatureException>(() => parent.Containment_1);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var value = new LinkTestConcept("c");
        var values = new ArrayList() { value };
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new LinkTestConcept("od");
        var value = new LinkTestConcept("c");
        var values = new object[] { value };
        Assert.IsTrue(parent.SetContainmentRaw(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, null));
    }

    #endregion
}