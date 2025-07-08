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

namespace LionWeb.Core.Test.NodeApi.Dynamic;

using Languages;
using M2;
using M3;

[TestClass]
public class ReferenceTests_VersionSpecifics : DynamicNodeTestsBase
{
    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void SingleArray_Reflective(Type versionIface)
    {
        var parent = newReferenceGeometry("g", versionIface);
        var value = newLine("s", versionIface);
        var values = new DynamicNode[] { value };
        parent.Set(ReferenceGeometry_shapes(versionIface), values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue(
            (parent.Get(ReferenceGeometry_shapes(versionIface)) as IEnumerable<IReadableNode>).Contains(value));
    }

    private DynamicLanguage Lang(Type versionIface) =>
        ShapesDynamic.GetLanguage(LionWebVersions.GetByInterface(versionIface));

    private DynamicNode newReferenceGeometry(string id, Type versionIface) =>
        Lang(versionIface).GetFactory()
            .CreateNode(id, Lang(versionIface).ClassifierByKey("key-ReferenceGeometry")) as DynamicNode ??
        throw new AssertFailedException();

    private DynamicNode newLine(string id, Type versionIface) =>
        Lang(versionIface).GetFactory().CreateNode(id, Lang(versionIface).ClassifierByKey("key-Line")) as DynamicNode ??
        throw new AssertFailedException();

    private new Feature ReferenceGeometry_shapes(Type versionIface) =>
        Lang(versionIface).ClassifierByKey("key-ReferenceGeometry").FeatureByKey("key-shapes-references");
}