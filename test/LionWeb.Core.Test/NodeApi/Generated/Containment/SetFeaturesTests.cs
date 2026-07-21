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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment;

using Languages.Generated.V2024_1.TestLanguage;
using M3;

[TestClass]
public class SetFeaturesTests
{
    #region single

    #region optional

    [TestMethod]
    public void ContainmentSingleOptional_Init()
    {
        var parent = new TestPartition("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Set()
    {
        var parent = new TestPartition("g");
        var doc = new DataTypeTestConcept("myId");
        parent.Data = doc;
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_data },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Set_Reflective()
    {
        var parent = new TestPartition("g");
        var doc = new DataTypeTestConcept("myId");
        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, doc);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_data },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Unset()
    {
        var parent = new TestPartition("g");
        var doc = new DataTypeTestConcept("myId");
        parent.Data = doc;
        parent.Data = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleOptional_Unset_Reflective()
    {
        var parent = new TestPartition("g");
        var doc = new DataTypeTestConcept("myId");
        parent.Data = doc;
        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ContainmentSingleRequired_Init()
    {
        var parent = new LinkTestConcept("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleRequired_Set()
    {
        var parent = new LinkTestConcept("g");
        var coord = new LinkTestConcept("myId");
        parent.Containment_1 = coord;
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_containment_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentSingleRequired_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var coord = new LinkTestConcept("myId");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_containment_1, coord);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_containment_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #region multiple

    #region optional

    [TestMethod]
    public void ContainmentMultipleOptional_Init()
    {
        var parent = new TestPartition("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Add()
    {
        var parent = new TestPartition("g");
        parent.AddLinks([new LinkTestConcept("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Insert()
    {
        var parent = new TestPartition("g");
        parent.InsertLinks(0, [new LinkTestConcept("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Set_Reflective()
    {
        var parent = new TestPartition("g");
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<LinkTestConcept> { new LinkTestConcept("myId") });
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Remove()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("myId");
        parent.AddLinks([value]);
        parent.RemoveLinks([value]);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_RemovePart()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("myA");
        var valueB = new LinkTestConcept("myB");
        parent.AddLinks([valueA, valueB]);
        parent.RemoveLinks([valueA]);
        CollectionAssert.AreEqual(new List<Feature>() { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Reset_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("myId");
        parent.AddLinks([value]);
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<LinkTestConcept>());
        CollectionAssert.AreEqual(new List<Feature>(),
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleOptional_Overwrite_Reflective()
    {
        var parent = new TestPartition("g");
        var value = new LinkTestConcept("myA");
        parent.AddLinks([value]);
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<LinkTestConcept> { new LinkTestConcept("myB") });
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ContainmentMultipleRequired_Init()
    {
        var parent = new TestPartition("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_Add()
    {
        var parent = new TestPartition("g");
        parent.AddLinks([new LinkTestConcept("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_Insert()
    {
        var parent = new TestPartition("g");
        parent.InsertLinks(0, [new LinkTestConcept("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_Set_Reflective()
    {
        var parent = new TestPartition("g");
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<LinkTestConcept> { new LinkTestConcept("myId") });
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ContainmentMultipleRequired_RemovePart()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("myA");
        var valueB = new LinkTestConcept("myB");
        parent.AddLinks([valueA, valueB]);
        parent.RemoveLinks([valueA]);
        CollectionAssert.AreEqual(new List<Feature>() { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }


    [TestMethod]
    public void ContainmentMultipleRequired_Overwrite_Reflective()
    {
        var parent = new TestPartition("g");
        var valueA = new LinkTestConcept("myA");
        parent.AddLinks([valueA]);
        var valueB = new LinkTestConcept("myB");
        parent.Set(TestLanguageLanguage.Instance.TestPartition_links, new List<LinkTestConcept> { valueB });
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.TestPartition_links },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion
}