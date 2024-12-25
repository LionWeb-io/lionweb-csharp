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
public class PropertyTests_Integer_Required
{
    #region Single

    [TestMethod]
    public void Property()
    {
        var parent = new Circle("od");
        var value = 10;
        parent.R = value;
        Assert.AreEqual(10, parent.R);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new Circle("od");
        var value = 10;
        parent.SetR(value);
        Assert.AreEqual(10, parent.R);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new Circle("od");
        var value = 10;
        parent.Set(ShapesLanguage.Instance.Circle_r, value);
        Assert.AreEqual(10, parent.R);
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = new Circle("od") { R = 10 };
        Assert.AreEqual(10, parent.Get(ShapesLanguage.Instance.Circle_r));
    }

    [TestMethod]
    public void Long_Reflective()
    {
        var parent = new Circle("od");
        var value = 10L;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.Circle_r, value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.R);
    }

    [TestMethod]
    public void String_Reflective()
    {
        var parent = new Circle("od");
        var value = "10";
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.Circle_r, value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.R);
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = new Circle("od");
        var value = true;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.Circle_r, value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.R);
    }

    [TestMethod]
    public void Constructor()
    {
        var parent = new Circle("myId") { R = 10 };
        Assert.AreEqual(10, parent.R);
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new Circle("od");
        object value = null;
        Assert.ThrowsException<NullReferenceException>(() => parent.R = (int)value);
        Assert.ThrowsException<UnsetFeatureException>(() => parent.R);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new Circle("od");
        object value = null;
        Assert.ThrowsException<NullReferenceException>(() => parent.SetR((int)value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.R);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new Circle("od");
        object value = null;
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Circle_r, value));
        Assert.ThrowsException<UnsetFeatureException>(() => parent.R);
    }

    [TestMethod]
    public void Null_Get()
    {
        var parent = new Circle("od");
        Assert.ThrowsException<UnsetFeatureException>(() => parent.R);
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = new Circle("od");
        Assert.ThrowsException<UnsetFeatureException>(() => parent.Get(ShapesLanguage.Instance.Circle_r));
    }

    [TestMethod]
    public void Null_Constructor()
    {
        var parent = new Circle("od");
        object value = null;
        Assert.ThrowsException<NullReferenceException>(
            () => new Circle("od") { R = (int)value });
    }

    #endregion
}