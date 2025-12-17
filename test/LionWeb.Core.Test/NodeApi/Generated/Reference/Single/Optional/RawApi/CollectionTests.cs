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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Single.Optional.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<ReferenceTarget>();
        Assert.IsFalse(parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, null));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<ReferenceTarget>{null};
        Assert.IsFalse(parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, null));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var value = new LinkTestConcept("s");
        var values = new List<ReferenceTarget>{ReferenceTarget.FromNode(value)};
        Assert.IsFalse(parent.AddReferencesRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, null));
        Assert.IsNull(parent.Reference_0_1);
        Assert.IsNull(value.GetParent());
    }
    #endregion
}