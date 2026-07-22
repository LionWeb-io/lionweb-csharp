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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Optional;

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Single()
    {
        var parent = new TestPartition("g");
        var doc = new DataTypeTestConcept("myId");
        parent.Data = doc;
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Data);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new TestPartition("g");
        var doc = new DataTypeTestConcept("myId");
        parent.SetData(doc);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Data);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new TestPartition("g");
        var doc = new DataTypeTestConcept("myId");
        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, doc);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Data);
    }

    [TestMethod]
    public void Constructor()
    {
        var doc = new DataTypeTestConcept("myId");
        var parent = new TestPartition("g") { Data = doc };
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Data);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var doc = new DataTypeTestConcept("myId");
        var parent = new TestPartition("g") { Data = doc };
        Assert.AreSame(doc, parent.Get(TestLanguageLanguage.Instance.TestPartition_data));
    }

    [TestMethod]
    public void TryGet()
    {
        var doc = new DataTypeTestConcept("myId");
        var parent = new TestPartition("g") { Data = doc };
        Assert.IsTrue(parent.TryGetData(out var o));
        // `o` should NOT have a warning
        DataTypeTestConcept documentation = o;
        Assert.AreSame(doc, documentation);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldDoc = new DataTypeTestConcept("old");
        var parent = new TestPartition("g") { Data = oldDoc };
        var doc = new DataTypeTestConcept("myId");
        parent.Data = doc;
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Data);
    }

    [TestMethod]
    public void Existing_Setter()
    {
        var oldDoc = new DataTypeTestConcept("old");
        var parent = new TestPartition("g") { Data = oldDoc };
        var doc = new DataTypeTestConcept("myId");
        parent.SetData(doc);
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Data);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldDoc = new DataTypeTestConcept("old");
        var parent = new TestPartition("g") { Data = oldDoc };
        var doc = new DataTypeTestConcept("myId");
        parent.Set(TestLanguageLanguage.Instance.TestPartition_data, doc);
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Data);
    }

    #endregion
}