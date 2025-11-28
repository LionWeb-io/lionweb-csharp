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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.RawApi;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class NullTests
{
    [TestMethod]
    public void Null()
    {
        var parent = new LinkTestConcept("g");
        Assert.IsFalse(parent.AddAnnotationsRaw([null]));
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new LinkTestConcept("g");
        Assert.IsFalse(parent.SetRaw(null, null));
    }

    [TestMethod]
    public void Null_Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        Assert.IsFalse(parent.InsertAnnotationsRaw(0, [null]));
    }

    [TestMethod]
    public void Null_Insert_Empty_OutOfBounds()
    {
        var parent = new LinkTestConcept("g");
        Assert.IsFalse(parent.InsertAnnotationsRaw(1, [null]));
    }

    [TestMethod]
    public void Null_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        Assert.IsFalse(parent.RemoveAnnotationsRaw([null]));
    }
}