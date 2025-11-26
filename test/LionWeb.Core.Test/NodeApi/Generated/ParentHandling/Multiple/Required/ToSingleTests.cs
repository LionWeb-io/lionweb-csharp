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

namespace LionWeb.Core.Test.NodeApi.Generated.ParentHandling.Multiple.Required;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class ToSingleTests
{
    #region Other

    [TestMethod]
    public void Other()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.EvilPart = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.EvilPart);
        Assert.AreEqual(0, source.Shapes.Count);
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var child = new Line("myId");
        var source = new Geometry("src") { Shapes = [child] };
        var target = new CompositeShape("tgt");

        target.Set(ShapesLanguage.Instance.CompositeShape_evilPart, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.EvilPart);
        Assert.AreEqual(0, source.Shapes.Count);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void OtherInSameInstance()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.EvilPart = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.EvilPart);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts);
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var child = new Line("myId");
        var parent = new CompositeShape("src") { Parts = [child] };

        parent.Set(ShapesLanguage.Instance.CompositeShape_evilPart, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.EvilPart);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Parts);
    }

    #endregion
}