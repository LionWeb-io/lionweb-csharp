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

namespace LionWeb.Core.Test.NodeApi.Lenient.ParentHandling;

[TestClass]
public class SingleRequiredTests : LenientNodeTestsBase
{
    #region SameInOtherInstance

    [TestMethod]
    public void SingleRequired_SameInOtherInstance_Reflective()
    {
        var child = newCoord("myId");
        var source = newOffsetDuplicate("src");
        source.Set(OffsetDuplicate_offset, child);
        var target = newOffsetDuplicate("tgt");

        target.Set(OffsetDuplicate_offset, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(OffsetDuplicate_offset));
        Assert.ThrowsException<UnsetFeatureException>(() => source.Get(OffsetDuplicate_offset));
    }

    [TestMethod]
    public void SingleRequired_SameInOtherInstance_detach_Reflective()
    {
        var child = newCoord("myId");
        var orphan = newCoord("o");
        var source = newOffsetDuplicate("src");
        source.Set(OffsetDuplicate_offset, child);
        var target = newOffsetDuplicate("tgt");
        target.Set(OffsetDuplicate_offset, orphan);

        target.Set(OffsetDuplicate_offset, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(OffsetDuplicate_offset));
        Assert.ThrowsException<UnsetFeatureException>(() => source.Get(OffsetDuplicate_offset));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region other

    [TestMethod]
    public void SingleRequired_Other_Reflective()
    {
        var child = newCoord("myId");
        var source = newOffsetDuplicate("src");
        source.Set(OffsetDuplicate_offset, child);
        var target = newCircle("tgt");

        target.Set(Circle_center, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(Circle_center));
        Assert.ThrowsException<UnsetFeatureException>(() => source.Get(OffsetDuplicate_offset));
    }

    [TestMethod]
    public void SingleRequired_Other_detach_Reflective()
    {
        var child = newCoord("myId");
        var orphan = newCoord("o");
        var source = newOffsetDuplicate("src");
        source.Set(OffsetDuplicate_offset, child);
        var target = newCircle("tgt");
        target.Set(Circle_center, orphan);

        target.Set(Circle_center, child);

        Assert.AreSame(target, child.GetParent());
        Assert.AreSame(child, target.Get(Circle_center));
        Assert.ThrowsException<UnsetFeatureException>(() => source.Get(OffsetDuplicate_offset));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion

    #region SameInSameInstance

    [TestMethod]
    public void SingleRequired_SameInSameInstance_Reflective()
    {
        var child = newCoord("myId");
        var parent = newLine("src");
        parent.Set(Line_start, child);

        parent.Set(Line_start, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(Line_start));
    }

    #endregion

    #region OtherInSameInstance

    [TestMethod]
    public void SingleRequired_OtherInSameInstance_Reflective()
    {
        var child = newCoord("myId");
        var parent = newLine("src");
        parent.Set(Line_start, child);

        parent.Set(Line_end, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(Line_end));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Line_start));
    }

    [TestMethod]
    public void SingleRequired_OtherInSameInstance_detach_Reflective()
    {
        var child = newCoord("myId");
        var orphan = newCoord("o");
        var parent = newLine("src");
        parent.Set(Line_start, child);
        parent.Set(Line_end, orphan);

        parent.Set(Line_end, child);

        Assert.AreSame(parent, child.GetParent());
        Assert.AreSame(child, parent.Get(Line_end));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Line_start));
        Assert.IsNull(orphan.GetParent());
    }

    #endregion
}