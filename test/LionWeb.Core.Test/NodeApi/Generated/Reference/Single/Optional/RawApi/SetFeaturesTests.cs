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

using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;
using System.Collections;

[TestClass]
public class SetFeaturesTests
{
    [TestMethod]
    public void ReferenceSingleOptional_Set_Reflective()
    {
        var parent = new LinkTestConcept("g");
        var other = new LinkTestConcept("myId");
        var target = ReferenceTarget.FromNode(other);
        Assert.IsTrue(parent.SetReferenceRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, target));
        Assert.IsTrue(parent.TryGetReferenceRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1,
            out var result));
        Assert.AreSame(target, result);
    }

    [TestMethod]
    public void ReferenceSingleOptional_Unset_Reflective()
    {
        var parent = new LinkTestConcept("g") { Reference_0_1 = new LinkTestConcept("myId") };
        Assert.IsTrue(parent.SetReferenceRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_1, null));
        Assert.IsTrue(parent.TryGetReferenceRaw(TestLanguageLanguage.Instance.LinkTestConcept_reference_1,
            out var result));
        Assert.IsNull(result);
    }
}