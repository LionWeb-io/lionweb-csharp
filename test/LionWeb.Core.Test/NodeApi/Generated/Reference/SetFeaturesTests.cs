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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference;

using Languages.Generated.V2024_1.TestLanguage;
using M3;

[TestClass]
public class SetFeaturesTests
{
    #region single

    #region optional

    [TestMethod]
    public void ReferenceSingleOptional_Init()
    {
        var parent = new LinkTestConcept("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Set()
    {
        var parent = new LinkTestConcept("g");
        parent.Reference_0_1 = new LinkTestConcept("myId");
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, new LinkTestConcept("myId"));
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Unset()
    {
        var parent = new LinkTestConcept("g");
        parent.Reference_0_1 = new LinkTestConcept("myId");
        parent.Reference_0_1 = null;
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleOptional_Unset_Reflective()
    {
        var parent = new LinkTestConcept("g");
        parent.Reference_0_1 = new LinkTestConcept("myId");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, null);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ReferenceSingleRequired_Init()
    {
        var parent = new LinkTestConcept("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleRequired_Set()
    {
        var parent = new LinkTestConcept("g");
        parent.Reference_1 = new LinkTestConcept("myId");
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceSingleRequired_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1, new LinkTestConcept("myId"));
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_1 },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion

    #region multiple

    #region optional

    [TestMethod]
    public void ReferenceMultipleOptional_Init()
    {
        var parent = new LinkTestConcept("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Add()
    {
        var parent = new LinkTestConcept("g");
        parent.AddReference_0_n([new LinkTestConcept("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Insert()
    {
        var parent = new LinkTestConcept("g");
        parent.InsertReference_0_n(0, [new LinkTestConcept("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<LinkTestConcept> { new LinkTestConcept("myId") });
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Remove()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("myId");
        parent.AddReference_0_n([value]);
        parent.RemoveReference_0_n([value]);
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_RemovePart()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("myA");
        var valueB = new LinkTestConcept("myB");
        parent.AddReference_0_n([valueA, valueB]);
        parent.RemoveReference_0_n([valueA]);
        CollectionAssert.AreEqual(new List<Feature>() { TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Reset_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("myId");
        parent.AddReference_0_n([value]);
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<LinkTestConcept>());
        CollectionAssert.AreEqual(new List<Feature>(),
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleOptional_Overwrite_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("myA");
        parent.AddReference_0_n([value]);
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, new List<LinkTestConcept> { new LinkTestConcept("myB") });
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #region required

    [TestMethod]
    public void ReferenceMultipleRequired_Init()
    {
        var parent = new LinkTestConcept("g");
        CollectionAssert.AreEqual(new List<Feature>(), parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Add()
    {
        var parent = new LinkTestConcept("g");
        parent.AddReference_1_n([new LinkTestConcept("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Insert()
    {
        var parent = new LinkTestConcept("g");
        parent.InsertReference_1_n(0, [new LinkTestConcept("myId")]);
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept> { new LinkTestConcept("myId") });
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n },
            parent.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void ReferenceMultipleRequired_RemovePart()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("myA");
        var valueB = new LinkTestConcept("myB");
        parent.AddReference_1_n([valueA, valueB]);
        parent.RemoveReference_1_n([valueA]);
        CollectionAssert.AreEqual(new List<Feature>() { TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n },
            parent.CollectAllSetFeatures().ToList());
    }


    [TestMethod]
    public void ReferenceMultipleRequired_Overwrite_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new LinkTestConcept("myA");
        parent.AddReference_1_n([valueA]);
        var valueB = new LinkTestConcept("myB");
        parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n, new List<LinkTestConcept> { valueB });
        CollectionAssert.AreEqual(new List<Feature> { TestLanguageLanguage.Instance.LinkTestConcept_reference_1_n },
            parent.CollectAllSetFeatures().ToList());
    }

    #endregion

    #endregion
}