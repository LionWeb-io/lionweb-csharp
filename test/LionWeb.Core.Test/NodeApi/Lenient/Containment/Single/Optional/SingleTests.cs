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

namespace LionWeb.Core.Test.NodeApi.Lenient.Containment.Single.Optional;

[TestClass]
public class SingleTests : LenientNodeTestsBase
{
    [TestMethod]
    public void Reflective()
    {
        var parent = newGeometry("g");
        var doc = newDocumentation("myId");
        parent.Set(Geometry_documentation, doc);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Get(Geometry_documentation));
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldDoc = newDocumentation("old");
        var parent = newGeometry("g");
        parent.Set(Geometry_documentation, oldDoc);
        var doc = newDocumentation("myId");
        parent.Set(Geometry_documentation, doc);
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Get(Geometry_documentation));
    }
}