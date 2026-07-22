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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Multiple.Optional;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class NullTests
{
    [TestMethod]
    public void Null()
    {
        var parent = new LinkTestConcept("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.AddReference_0_n(null));
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new LinkTestConcept("g");
        Assert.ThrowsExactly<InvalidValueException>(() =>
            parent.Set(TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n, null));
    }

    [TestMethod]
    public void Constructor()
    {
        Assert.ThrowsExactly<InvalidValueException>(() => new LinkTestConcept("g") { Reference_0_n = [null] });
    }

    [TestMethod]
    public void Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.InsertReference_0_n(0, null));
    }

    [TestMethod]
    public void Insert_Empty_OutOfBounds()
    {
        var parent = new LinkTestConcept("g");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.InsertReference_0_n(1, null));
    }

    [TestMethod]
    public void Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        Assert.ThrowsExactly<InvalidValueException>(() => parent.RemoveReference_0_n(null));
    }

    [TestMethod]
    public void TryGet()
    {
        var parent = new LinkTestConcept("g");
        Assert.IsFalse(parent.TryGetReference_0_n(out var o));
        Assert.IsFalse(o.Any());
    }
}