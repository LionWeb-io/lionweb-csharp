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

namespace LionWeb.Core.M2.Lenient.Test;

[TestClass]
public class PropertyTests_Integer_Required : LenientNodeTestsBase
{
    #region Single

    [TestMethod]
    public void Reflective()
    {
        var parent = newCircle("od");
        var value = 10;
        parent.Set(Circle_r, value);
        Assert.AreEqual(10, parent.Get(Circle_r));
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = newCircle("od");
        parent.Set(Circle_r, 10);
        Assert.AreEqual(10, parent.Get(Circle_r));
    }

    [TestMethod]
    public void Long_Reflective()
    {
        var parent = newCircle("od");
        var value = 10L;
        parent.Set(Circle_r, value);
        Assert.AreEqual(10L, parent.Get(Circle_r));
    }

    [TestMethod]
    public void String_Reflective()
    {
        var parent = newCircle("od");
        var value = "10";
        parent.Set(Circle_r, value);
        Assert.AreEqual("10", parent.Get(Circle_r));
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = newCircle("od");
        var value = true;
        parent.Set(Circle_r, value);
        Assert.AreEqual(true, parent.Get(Circle_r));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = newCircle("od");
        object value = null;
        parent.Set(Circle_r, value);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Circle_r));
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = newCircle("od");
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Circle_r));
    }

    #endregion

    #region metamodelViolation

    [TestMethod]
    public void Integers_Reflective()
    {
        var parent = newCircle("od");
        var value = new List<int> { 1, -1 };
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(Circle_r, value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Circle_r));
    }

    [TestMethod]
    public void Node_Reflective()
    {
        var parent = newCircle("od");
        var value = newCoord("c");
        parent.Set(Circle_r, value);
        Assert.AreSame(value, parent.Get(Circle_r));
    }

    [TestMethod]
    public void Nodes_Reflective()
    {
        var parent = newCircle("od");
        var valueA = newCoord("cA");
        var valueB = newCoord("cB");
        var value = new List<IReadableNode> { valueA, valueB };
        parent.Set(Circle_r, value);
        CollectionAssert.AreEqual(new List<IReadableNode>() { valueA, valueB },
            parent.Get(Circle_r) as List<IReadableNode>);
    }

    #endregion
}