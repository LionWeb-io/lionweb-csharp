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

namespace LionWeb.Core.M2.Dynamic.Test;

[TestClass]
public class PropertyTests_Integer_Required : DynamicNodeTestsBase
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
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(Circle_r, value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Circle_r));
    }

    [TestMethod]
    public void String_Reflective()
    {
        var parent = newCircle("od");
        var value = "10";
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(Circle_r, value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Circle_r));
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = newCircle("od");
        var value = true;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(Circle_r, value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Circle_r));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = newCircle("od");
        object value = null;
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(Circle_r, value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Circle_r));
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = newCircle("od");
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(Circle_r));
    }

    #endregion
}