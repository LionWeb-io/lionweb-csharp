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

namespace LionWeb.Core.Test.NodeApi.Generated.Containment.Single.Required;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class SingleTests
{
    [TestMethod]
    public void Single()
    {
        var parent = new OffsetDuplicate("od");
        var coord = new Coord("myId");
        parent.Offset = coord;
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new OffsetDuplicate("od");
        var coord = new Coord("myId");
        parent.SetOffset(coord);
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new OffsetDuplicate("od");
        var coord = new Coord("myId");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Constructor()
    {
        var coord = new Coord("myId");
        var parent = new OffsetDuplicate("od") { Offset = coord };
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var coord = new Coord("myId");
        var parent = new OffsetDuplicate("od") { Offset = coord };
        Assert.AreSame(coord, parent.Get(ShapesLanguage.Instance.OffsetDuplicate_offset));
    }

    [TestMethod]
    public void TryGet()
    {
        var coord = new Coord("myId");
        var parent = new OffsetDuplicate("od") { Offset = coord };
        Assert.IsTrue(parent.TryGetOffset(out var o));
        Assert.AreSame(coord, o);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldCoord = new Coord("old");
        var parent = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        parent.Offset = coord;
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Existing_Setter()
    {
        var oldCoord = new Coord("old");
        var parent = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        parent.SetOffset(coord);
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldCoord = new Coord("old");
        var parent = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        parent.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);
        Assert.IsNull(oldCoord.GetParent());
        Assert.AreSame(parent, coord.GetParent());
        Assert.AreSame(coord, parent.Offset);
    }

    #endregion
}