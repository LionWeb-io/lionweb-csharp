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

namespace LionWeb.Core.M2.Generated.Test;

using M3;

[TestClass]
public class ContainmentTests_VersionSpecifics
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
        var values = new INode[] { value };
        parent.Set(Geometry_shapes(versionIface), values);
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue((parent.Get(Geometry_shapes(versionIface)) as IEnumerable<IReadableNode>).Contains(value));
    }

    private INode newLine(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new Examples.V2023_1.Shapes.M2.Line(id),
        IVersion2024_1 => new Examples.V2024_1.Shapes.M2.Line(id),
        IVersion2024_1_Compatible => new Examples.V2024_1.Shapes.M2.Line(id),
        var v => throw new UnsupportedVersionException(v)
    };

    private INode newBillOfMaterials(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new Examples.V2023_1.Shapes.M2.BillOfMaterials(id),
        IVersion2024_1 => new Examples.V2024_1.Shapes.M2.BillOfMaterials(id),
        IVersion2024_1_Compatible => new Examples.V2024_1.Shapes.M2.BillOfMaterials(id),
        var v => throw new UnsupportedVersionException(v)
    };

    private INode newDocumentation(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new Examples.V2023_1.Shapes.M2.Documentation(id),
        IVersion2024_1 => new Examples.V2024_1.Shapes.M2.Documentation(id),
        IVersion2024_1_Compatible => new Examples.V2024_1.Shapes.M2.Documentation(id),
        var v => throw new UnsupportedVersionException(v)
    };
    
    private INode newGeometry(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new Examples.V2023_1.Shapes.M2.Geometry(id),
        IVersion2024_1 => new Examples.V2024_1.Shapes.M2.Geometry(id),
        IVersion2024_1_Compatible => new Examples.V2024_1.Shapes.M2.Geometry(id),
        var v => throw new UnsupportedVersionException(v)
    };
    
    private Feature Geometry_shapes(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => Examples.V2023_1.Shapes.M2.ShapesLanguage.Instance.Geometry_shapes,
        IVersion2024_1 => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.Geometry_shapes,
        IVersion2024_1_Compatible => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.Geometry_shapes,
        var v => throw new UnsupportedVersionException(v)
    };
}