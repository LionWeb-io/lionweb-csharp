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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference;

using Languages.Generated.V2023_1.TestLanguage;
using M3;
using System.Collections;

[TestClass]
public class VersionSpecificsTests
{
    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void SingleArray_Reflective(Type versionIface)
    {
        var parent = newLinkTestConcept("g", versionIface);
        var value = newLinkTestConcept("s", versionIface);
        var values = new INode[] { value };
        parent.Set(LinkTestConcept_reference_0_n(versionIface), values);
        Assert.IsNull(value.GetParent());
        Assert.IsTrue((parent.Get(LinkTestConcept_reference_0_n(versionIface)) as IEnumerable).Cast<INode>().Contains(value));
    }

    private INode newLinkTestConcept(string id, Type versionIface) =>
        LionWebVersions.GetByInterface(versionIface) switch
        {
            IVersion2023_1 => new LinkTestConcept(id),
            IVersion2024_1 => new Languages.Generated.V2024_1.TestLanguage.LinkTestConcept(id),
            IVersion2024_1_Compatible => new Languages.Generated.V2024_1.TestLanguage.LinkTestConcept(id),
            var v => throw new UnsupportedVersionException(v)
        };

    private Feature LinkTestConcept_reference_0_n(Type versionIface) => LionWebVersions.GetByInterface(versionIface) switch
    {
        IVersion2023_1 => TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
        IVersion2024_1 => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
        IVersion2024_1_Compatible => Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance.LinkTestConcept_reference_0_n,
        var v => throw new UnsupportedVersionException(v)
    };
}
