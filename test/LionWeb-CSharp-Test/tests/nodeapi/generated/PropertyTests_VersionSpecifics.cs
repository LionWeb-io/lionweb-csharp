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

using Examples.V2023_1.Shapes.M2;
using M3;

[TestClass]
public class PropertyTests_VersionSpecifics
{
    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void String(Type versionIface)
    {
        var parent = newDocumentation("od", versionIface);
        var value = "Hi";
        parent.Set(Documentation_text(versionIface), value);
        Assert.AreEqual("Hi", parent.Get(Documentation_text(versionIface)));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Integer(Type versionIface)
    {
        var parent = newCircle("od", versionIface);
        var value = 10;
        parent.Set(Circle_r(versionIface), value);
        Assert.AreEqual(10, parent.Get(Circle_r(versionIface)));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Boolean(Type versionIface)
    {
        var parent = newDocumentation("od", versionIface);
        var value = true;
        parent.Set(Documentation_technical(versionIface), value);
        Assert.AreEqual(true, parent.Get(Documentation_technical(versionIface)));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Enum(Type versionIface)
    {
        var parent = newMaterialGroup("od", versionIface);
        var value = MatterState_liquid(versionIface);
        parent.Set(MaterialGroup_matterState(versionIface), value);
        Assert.AreEqual(MatterState_liquid(versionIface), parent.Get(MaterialGroup_matterState(versionIface)));
    }

    private INode newDocumentation(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new Documentation(id),
        IVersion2024_1 => new Examples.V2024_1.Shapes.M2.Documentation(id),
        IVersion2024_1_Compatible => new Examples.V2024_1.Shapes.M2.Documentation(id),
        var v => throw new UnsupportedVersionException(v)
    };

    private Feature Documentation_text(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => ShapesLanguage.Instance.Documentation_text,
        IVersion2024_1 => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.Documentation_text,
        IVersion2024_1_Compatible => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.Documentation_text,
        var v => throw new UnsupportedVersionException(v)
    };

    private Feature Documentation_technical(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => ShapesLanguage.Instance.Documentation_technical,
        IVersion2024_1 => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.Documentation_technical,
        IVersion2024_1_Compatible => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.Documentation_technical,
        var v => throw new UnsupportedVersionException(v)
    };

    private INode newCircle(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new Circle(id),
        IVersion2024_1 => new Examples.V2024_1.Shapes.M2.Circle(id),
        IVersion2024_1_Compatible => new Examples.V2024_1.Shapes.M2.Circle(id),
        var v => throw new UnsupportedVersionException(v)
    };

    private Feature Circle_r(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => ShapesLanguage.Instance.Circle_r,
        IVersion2024_1 => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.Circle_r,
        IVersion2024_1_Compatible => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.Circle_r,
        var v => throw new UnsupportedVersionException(v)
    };

    private INode newMaterialGroup(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new MaterialGroup(id),
        IVersion2024_1 => new Examples.V2024_1.Shapes.M2.MaterialGroup(id),
        IVersion2024_1_Compatible => new Examples.V2024_1.Shapes.M2.MaterialGroup(id),
        var v => throw new UnsupportedVersionException(v)
    };

    private object? MatterState_liquid(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => MatterState.liquid,
        IVersion2024_1 => Examples.V2024_1.Shapes.M2.MatterState.liquid,
        IVersion2024_1_Compatible => Examples.V2024_1.Shapes.M2.MatterState.liquid,
        var v => throw new UnsupportedVersionException(v)
    };

    private Feature MaterialGroup_matterState(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => ShapesLanguage.Instance.MaterialGroup_matterState,
        IVersion2024_1 => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.MaterialGroup_matterState,
        IVersion2024_1_Compatible => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.MaterialGroup_matterState,
        var v => throw new UnsupportedVersionException(v)
    };
}