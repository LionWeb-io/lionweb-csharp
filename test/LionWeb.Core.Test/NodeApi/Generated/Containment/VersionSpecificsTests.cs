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

using Languages.Generated.V2023_1.TestLanguage;
using M3;

[TestClass]
public class VersionSpecificsTests
{
    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Single_Add(Type versionIface)
    {
        var parent = newLinkTestConcept("g", versionIface);
        var bom = newAnnotation("myId", versionIface);
        parent.AddAnnotations([bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Single_Insert_Empty(Type versionIface)
    {
        var parent = newLinkTestConcept("g", versionIface);
        var bom = newAnnotation("myId", versionIface);
        parent.InsertAnnotations(0, [bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Single_Insert_One_Before(Type versionIface)
    {
        var doc = newDataTypeTestConcept("cId", versionIface);
        var parent = newLinkTestConcept("g", versionIface);
        parent.AddAnnotations([doc]);
        var bom = newAnnotation("myId", versionIface);
        parent.InsertAnnotations(0, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { bom, doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Single_Remove_Only(Type versionIface)
    {
        var bom = newAnnotation("myId", versionIface);
        var parent = newLinkTestConcept("g", versionIface);
        parent.AddAnnotations([bom]);
        parent.RemoveAnnotations([bom]);
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void SingleArray_Reflective(Type versionIface)
    {
        var parent = newTestPartition("g", versionIface);
        var value = newLinkTestConcept("s", versionIface);
        var values = new INode[] { value };
        parent.Set(TestPartition_links(versionIface), values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(TestPartition_links(versionIface)) as IEnumerable<IReadableNode>).Contains(value));
    }

    private INode newLinkTestConcept(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new LinkTestConcept(id),
        IVersion2024_1 => new Languages.Generated.V2024_1.TestLanguage.LinkTestConcept(id),
        IVersion2024_1_Compatible => new Languages.Generated.V2024_1.TestLanguage.LinkTestConcept(id),
        var v => throw new UnsupportedVersionException(v)
    };

    private INode newAnnotation(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new TestAnnotation(id),
        IVersion2024_1 => new Languages.Generated.V2024_1.TestLanguage.TestAnnotation(id),
        IVersion2024_1_Compatible => new Languages.Generated.V2024_1.TestLanguage.TestAnnotation(id),
        var v => throw new UnsupportedVersionException(v)
    };

    private INode newDataTypeTestConcept(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new TestAnnotation(id),
        IVersion2024_1 => new Languages.Generated.V2024_1.TestLanguage.TestAnnotation(id),
        IVersion2024_1_Compatible => new Languages.Generated.V2024_1.TestLanguage.TestAnnotation(id),
        var v => throw new UnsupportedVersionException(v)
    };
    
    private INode newTestPartition(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new TestPartition(id),
        IVersion2024_1 => new Languages.Generated.V2024_1.TestLanguage.TestPartition(id),
        IVersion2024_1_Compatible => new Languages.Generated.V2024_1.TestLanguage.TestPartition(id),
        var v => throw new UnsupportedVersionException(v)
    };
    
    private Feature TestPartition_links(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => TestLanguageLanguage.Instance.TestPartition_links,
        IVersion2024_1 => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.TestPartition_links,
        IVersion2024_1_Compatible => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.TestPartition_links,
        var v => throw new UnsupportedVersionException(v)
    };
}