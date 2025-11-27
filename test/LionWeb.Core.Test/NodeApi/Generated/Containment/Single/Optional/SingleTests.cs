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

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Single()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.Documentation = doc;
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.SetDocumentation(doc);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.Set(ShapesLanguage.Instance.Geometry_documentation, doc);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Constructor()
    {
        var doc = new Documentation("myId");
        var parent = new Geometry("g") { Documentation = doc };
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var doc = new Documentation("myId");
        var parent = new Geometry("g") { Documentation = doc };
        Assert.AreSame(doc, parent.Get(ShapesLanguage.Instance.Geometry_documentation));
    }

    [TestMethod]
    public void TryGet()
    {
        var doc = new Documentation("myId");
        var parent = new Geometry("g") { Documentation = doc };
        Assert.IsTrue(parent.TryGetDocumentation(out var o));
        // `o` should NOT have a warning
        Documentation documentation = o;
        Assert.AreSame(doc, documentation);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };
        var doc = new Documentation("myId");
        parent.Documentation = doc;
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Existing_Setter()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };
        var doc = new Documentation("myId");
        parent.SetDocumentation(doc);
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };
        var doc = new Documentation("myId");
        parent.Set(ShapesLanguage.Instance.Geometry_documentation, doc);
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    #endregion
}