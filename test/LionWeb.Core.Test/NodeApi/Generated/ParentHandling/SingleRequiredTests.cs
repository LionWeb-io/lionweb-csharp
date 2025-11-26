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

namespace LionWeb.Core.Test.NodeApi.Generated.ParentHandling;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class SingleRequiredTests
{
    #region SameInOtherInstance

    [TestMethod]
    public void SameInOtherInstance()
    {
        var child = new Coord("myId");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new OffsetDuplicate("tgt");

        target.Offset = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Offset);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
    }

    [TestMethod]
    public void SameInOtherInstance_detach()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new OffsetDuplicate("tgt") { Offset = orphan };

        target.Offset = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Offset);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void SameInOtherInstance_Reflective()
    {
        var child = new Coord("myId");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new OffsetDuplicate("tgt");

        target.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Offset);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
    }

    [TestMethod]
    public void SameInOtherInstance_detach_Reflective()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new OffsetDuplicate("tgt") { Offset = orphan };

        target.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Offset);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region other

    [TestMethod]
    public void Other()
    {
        var child = new Coord("myId");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new Circle("tgt");

        target.Center = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Center);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
    }

    [TestMethod]
    public void Other_detach()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new Circle("tgt") { Center = orphan };

        target.Center = child;

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Center);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void Other_Reflective()
    {
        var child = new Coord("myId");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new Circle("tgt");

        target.Set(ShapesLanguage.Instance.Circle_center, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Center);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
    }

    [TestMethod]
    public void Other_detach_Reflective()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var source = new OffsetDuplicate("src") { Offset = child };
        var target = new Circle("tgt") { Center = orphan };

        target.Set(ShapesLanguage.Instance.Circle_center, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Center);
        Assert.ThrowsException<UnsetFeatureException>(() => source.Offset);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void SameInSameInstance()
    {
        var child = new Coord("myId");
        var parent = new Line("src") { Start = child };

        parent.Start = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Start);
    }

    [TestMethod]
    public void SameInSameInstance_Reflective()
    {
        var child = new Coord("myId");
        var parent = new Line("src") { Start = child };

        parent.Set(ShapesLanguage.Instance.Line_start, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Start);
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void OtherInSameInstance()
    {
        var child = new Coord("myId");
        var parent = new Line("src") { Start = child };

        parent.End = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.End);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Start);
    }

    [TestMethod]
    public void OtherInSameInstance_detach()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var parent = new Line("src") { Start = child, End = orphan };

        parent.End = child;

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.End);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Start);
        Assert.IsNull(orphan.GetParent());
    }

    [TestMethod]
    public void OtherInSameInstance_Reflective()
    {
        var child = new Coord("myId");
        var parent = new Line("src") { Start = child };

        parent.Set(ShapesLanguage.Instance.Line_end, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.End);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Start);
    }

    [TestMethod]
    public void OtherInSameInstance_detach_Reflective()
    {
        var child = new Coord("myId");
        var orphan = new Coord("o");
        var parent = new Line("src") { Start = child, End = orphan };

        parent.Set(ShapesLanguage.Instance.Line_end, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.End);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Start);
        Assert.IsNull(orphan.GetParent());
    }

    #endregion
}