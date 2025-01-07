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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class ContainmentTests_Single_Required_Listener
{
    #region Single

    [TestMethod]
    public void Single()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");

        int events = 0;
        parent.Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(coord, args.NewChild);
        };

        offsetDuplicate.Offset = coord;

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Setter()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");

        int events = 0;
        parent.Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(coord, args.NewChild);
        };

        offsetDuplicate.SetOffset(coord);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_Reflective()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");

        int events = 0;
        parent.Listener.ChildAdded += (sender, args) =>
        {
            events++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(coord, args.NewChild);
        };

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_FromOtherParent()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Offset = coord };

        int events = 0;
        parent.Listener.ChildMovedFromOtherContainment += (sender, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        };

        offsetDuplicate.Offset = coord;

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_FromOtherParent_Reflective()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Offset = coord };

        int events = 0;
        parent.Listener.ChildMovedFromOtherContainment += (sender, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        };

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_FromOtherParent_Replace()
    {
        var replacedCoord = new Coord("replaced");
        var offsetDuplicate = new OffsetDuplicate("od") { Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Offset = coord };

        int events = 0;
        parent.Listener.ChildMovedFromOtherContainment += (sender, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        };
        parent.Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(replacedCoord, args.DeletedChild);
        };

        offsetDuplicate.Offset = coord;

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Single_FromOtherParent_Replace_Reflective()
    {
        var replacedCoord = new Coord("replaced");
        var offsetDuplicate = new OffsetDuplicate("od") { Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };
        var coord = new Coord("myId");
        var oldParent = new OffsetDuplicate("oldParent") { Offset = coord };

        int events = 0;
        parent.Listener.ChildMovedFromOtherContainment += (sender, args) =>
        {
            events++;
            Assert.AreSame(oldParent, args.OldParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.NewParent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        };
        parent.Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(replacedCoord, args.DeletedChild);
        };

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Single_FromSameParent()
    {
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord] };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) =>
        {
            events++;
            Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        };

        offsetDuplicate.Offset = coord;

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_FromSameParent_Reflective()
    {
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord] };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) =>
        {
            events++;
            Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        };

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Single_FromSameParent_Replace()
    {
        var replacedCoord = new Coord("replacedCoord");
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord], Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) =>
        {
            events++;
            Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        };
        parent.Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(replacedCoord, args.DeletedChild);
        };

        offsetDuplicate.Offset = coord;

        Assert.AreEqual(2, events);
    }

    [TestMethod]
    public void Single_FromSameParent_Replace_Reflective()
    {
        var replacedCoord = new Coord("replacedCoord");
        var coord = new Coord("myId");
        var offsetDuplicate = new OffsetDuplicate("od") { Fixpoints = [coord], Offset = replacedCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) =>
        {
            events++;
            Assert.AreSame(ShapesLanguage.Instance.IShape_fixpoints, args.OldContainment);
            Assert.AreEqual(0, args.OldIndex);
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.NewContainment);
            Assert.AreEqual(0, args.NewIndex);
            Assert.AreEqual(coord, args.MovedChild);
        };
        parent.Listener.ChildDeleted += (sender, args) =>
        {
            events++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(replacedCoord, args.DeletedChild);
        };

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(2, events);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildReplaced += (sender, args) =>
        {
            events++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(coord, args.NewChild);
            Assert.AreEqual(oldCoord, args.ReplacedChild);
        };

        offsetDuplicate.Offset = coord;

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var coord = new Coord("myId");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildReplaced += (sender, args) =>
        {
            events++;
            Assert.AreSame(offsetDuplicate, args.Parent);
            Assert.AreSame(ShapesLanguage.Instance.OffsetDuplicate_offset, args.Containment);
            Assert.AreEqual(0, args.Index);
            Assert.AreEqual(coord, args.NewChild);
            Assert.AreEqual(oldCoord, args.ReplacedChild);
        };

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, coord);

        Assert.AreEqual(1, events);
    }

    [TestMethod]
    public void Existing_Same()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildReplaced += (sender, args) => events++;
        parent.Listener.ChildAdded += (sender, args) => events++;
        parent.Listener.ChildDeleted += (sender, args) => events++;
        parent.Listener.ChildMovedFromOtherContainment += (sender, args) => events++;
        parent.Listener.ChildMovedInSameContainment += (sender, args) => events++;
        parent.Listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) => events++;

        offsetDuplicate.Offset = oldCoord;

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Existing_Same_Reflective()
    {
        var oldCoord = new Coord("old");
        var offsetDuplicate = new OffsetDuplicate("g") { Offset = oldCoord };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildReplaced += (sender, args) => events++;
        parent.Listener.ChildAdded += (sender, args) => events++;
        parent.Listener.ChildDeleted += (sender, args) => events++;
        parent.Listener.ChildMovedFromOtherContainment += (sender, args) => events++;
        parent.Listener.ChildMovedInSameContainment += (sender, args) => events++;
        parent.Listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) => events++;

        offsetDuplicate.Set(ShapesLanguage.Instance.OffsetDuplicate_offset, oldCoord);

        Assert.AreEqual(0, events);
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var offsetDuplicate = new OffsetDuplicate("od");
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildReplaced += (sender, args) => events++;
        parent.Listener.ChildAdded += (sender, args) => events++;
        parent.Listener.ChildDeleted += (sender, args) => events++;
        parent.Listener.ChildMovedFromOtherContainment += (sender, args) => events++;
        parent.Listener.ChildMovedInSameContainment += (sender, args) => events++;
        parent.Listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) => events++;

        Assert.ThrowsException<InvalidValueException>(() => offsetDuplicate.Offset = null);

        Assert.AreEqual(0, events);
    }

    [TestMethod]
    public void Null_Existing()
    {
        var offsetDuplicate = new OffsetDuplicate("od") { Offset = new Coord("myId") };
        var parent = new Geometry("g") { Shapes = [offsetDuplicate] };

        int events = 0;
        parent.Listener.ChildReplaced += (sender, args) => events++;
        parent.Listener.ChildAdded += (sender, args) => events++;
        parent.Listener.ChildDeleted += (sender, args) => events++;
        parent.Listener.ChildMovedFromOtherContainment += (sender, args) => events++;
        parent.Listener.ChildMovedInSameContainment += (sender, args) => events++;
        parent.Listener.ChildMovedFromOtherContainmentInSameParent += (sender, args) => events++;

        Assert.ThrowsException<InvalidValueException>(() => offsetDuplicate.Offset = null);

        Assert.AreEqual(0, events);
    }

    #endregion
}