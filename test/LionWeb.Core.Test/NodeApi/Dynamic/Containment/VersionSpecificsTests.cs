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

namespace LionWeb.Core.Test.NodeApi.Dynamic.Containment;

using Languages;
using M2;
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
        var parent = newLine("g", versionIface);
        var bom = newBillOfMaterials("myId", versionIface);
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
        var parent = newLine("g", versionIface);
        var bom = newBillOfMaterials("myId", versionIface);
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
        var doc = newDocumentation("cId", versionIface);
        var parent = newLine("g", versionIface);
        parent.AddAnnotations([doc]);
        var bom = newBillOfMaterials("myId", versionIface);
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
        var bom = newBillOfMaterials("myId", versionIface);
        var parent = newLine("g", versionIface);
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
        var parent = newGeometry("g", versionIface);
        var value = newLine("s", versionIface);
        var values = new DynamicNode[] { value };
        parent.Set(Geometry_shapes(versionIface), values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes(versionIface)) as IEnumerable<IReadableNode>).Contains(value));
    }

    private DynamicLanguage Lang(Type versionIface) =>
        ShapesDynamic.GetLanguage(LionWebVersions.GetByInterface(versionIface));

    private DynamicNode newLine(string id, Type versionIface) =>
        Lang(versionIface).GetFactory().CreateNode(id, Lang(versionIface).ClassifierByKey("key-Line")) as DynamicNode ??
        throw new AssertFailedException();

    private DynamicNode newBillOfMaterials(string id, Type versionIface) =>
        Lang(versionIface).GetFactory()
            .CreateNode(id, Lang(versionIface).ClassifierByKey("key-BillOfMaterials")) as DynamicNode ??
        throw new AssertFailedException();

    private DynamicNode newDocumentation(string id, Type versionIface) =>
        Lang(versionIface).GetFactory()
            .CreateNode(id, Lang(versionIface).ClassifierByKey("key-Documentation")) as DynamicNode ??
        throw new AssertFailedException();
    
    private DynamicNode newGeometry(string id, Type versionIface) =>
        Lang(versionIface).GetFactory().CreateNode(id, Lang(versionIface).ClassifierByKey("key-Geometry")) as DynamicNode ??
        throw new AssertFailedException();
    
    private Feature Geometry_shapes(Type versionIface) =>
        Lang(versionIface).ClassifierByKey("key-Geometry").FeatureByKey("key-shapes");
}