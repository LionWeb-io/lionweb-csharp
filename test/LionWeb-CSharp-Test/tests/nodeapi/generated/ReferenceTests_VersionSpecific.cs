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
using System.Collections;

[TestClass]
public class ReferenceTests_VersionSpecifics
{
    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void SingleArray_Reflective(Type versionIface)
    {
        var parent = newReferenceGeometry("g", versionIface);
        var value = newLine("s", versionIface);
        var values = new INode[] { value };
        parent.Set(ReferenceGeometry_shapes(versionIface), values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(ReferenceGeometry_shapes(versionIface)) as IEnumerable).Cast<INode>().Contains(value));
    }

    private INode newReferenceGeometry(string id, Type versionIface) =>
        LionWebVersions.GetByInterface(versionIface) switch
        {
            IVersion2023_1 => new Examples.V2023_1.Shapes.M2.ReferenceGeometry(id),
            IVersion2024_1 => new Examples.V2024_1.Shapes.M2.ReferenceGeometry(id),
            IVersion2024_1_Compatible => new Examples.V2024_1.Shapes.M2.ReferenceGeometry(id),
            var v => throw new UnsupportedVersionException(v)
        };

    private INode newLine(string id, Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => new Examples.V2023_1.Shapes.M2.Line(id),
        IVersion2024_1 => new Examples.V2024_1.Shapes.M2.Line(id),
        IVersion2024_1_Compatible => new Examples.V2024_1.Shapes.M2.Line(id),
        var v => throw new UnsupportedVersionException(v)
    };

    private Feature ReferenceGeometry_shapes(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => Examples.V2023_1.Shapes.M2.ShapesLanguage.Instance.ReferenceGeometry_shapes,
        IVersion2024_1 => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.ReferenceGeometry_shapes,
        IVersion2024_1_Compatible => Examples.V2024_1.Shapes.M2.ShapesLanguage.Instance.ReferenceGeometry_shapes,
        var v => throw new UnsupportedVersionException(v)
    };
}