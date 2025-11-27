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

namespace LionWeb.Core.Test.NodeApi.Generated.Reference.Single.Optional;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class SingleTests
{
    #region Single

    [TestMethod]
    public void Single()
    {
        var parent = new OffsetDuplicate("g");
        var reference = new Line("myId");
        parent.AltSource = reference;
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.AltSource);
    }

    [TestMethod]
    public void Single_Setter()
    {
        var parent = new OffsetDuplicate("g");
        var reference = new Line("myId");
        parent.SetAltSource(reference);
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.AltSource);
    }

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        var reference = new Line("myId");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, reference);
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.AltSource);
    }

    [TestMethod]
    public void Single_Constructor()
    {
        var reference = new Line("myId");
        var parent = new OffsetDuplicate("g") { AltSource = reference };
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.AltSource);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var reference = new Line("myId");
        var parent = new OffsetDuplicate("g") { AltSource = reference };
        Assert.AreSame(reference, parent.Get(ShapesLanguage.Instance.OffsetDuplicate_altSource));
    }

    [TestMethod]
    public void Single_TryGet()
    {
        var reference = new Line("myId");
        var parent = new OffsetDuplicate("g") { AltSource = reference };
        Assert.IsTrue(parent.TryGetAltSource(out var o));
        Assert.AreSame(reference, o);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldReference = new Line("old");
        var parent = new OffsetDuplicate("g") { AltSource = oldReference };
        var reference = new Line("myId");
        parent.AltSource = reference;
        Assert.IsNull(oldReference.GetParent());
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.AltSource);
    }

    [TestMethod]
    public void Existing_Setter()
    {
        var oldReference = new Line("old");
        var parent = new OffsetDuplicate("g") { AltSource = oldReference };
        var reference = new Line("myId");
        parent.SetAltSource(reference);
        Assert.IsNull(oldReference.GetParent());
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.AltSource);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldReference = new Line("old");
        var parent = new OffsetDuplicate("g") { AltSource = oldReference };
        var reference = new Line("myId");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, reference);
        Assert.IsNull(oldReference.GetParent());
        Assert.IsNull(reference.GetParent());
        Assert.AreSame(reference, parent.AltSource);
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new OffsetDuplicate("g");
        parent.AltSource = null;
        Assert.IsNull(parent.AltSource);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new OffsetDuplicate("g");
        parent.SetAltSource(null);
        Assert.IsNull(parent.AltSource);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new OffsetDuplicate("g");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_altSource, null);
        Assert.IsNull(parent.AltSource);
    }

    [TestMethod]
    public void Null_Constructor()
    {
        var parent = new OffsetDuplicate("g") { AltSource = null };
        Assert.IsNull(parent.AltSource);
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new OffsetDuplicate("g") { AltSource = null };
        Assert.IsFalse(parent.TryGetAltSource(out var o));
        Assert.IsNull(o);
    }

    #endregion
}