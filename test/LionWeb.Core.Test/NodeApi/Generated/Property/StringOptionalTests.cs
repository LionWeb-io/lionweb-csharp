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

namespace LionWeb.Core.Test.NodeApi.Generated.Property;

using LionWeb.Core.Test.Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class StringOptionalTests
{
    #region Single

    [TestMethod]
    public void Property()
    {
        var parent = new Documentation("od");
        var value = "Hi";
        parent.Text = value;
        Assert.AreEqual("Hi", parent.Text);
    }

    [TestMethod]
    public void Setter()
    {
        var parent = new Documentation("od");
        var value = "Hi";
        parent.SetText(value);
        Assert.AreEqual("Hi", parent.Text);
    }

    [TestMethod]
    public void Reflective()
    {
        var parent = new Documentation("od");
        var value = "Hi";
        parent.Set(ShapesLanguage.Instance.Documentation_text, value);
        Assert.AreEqual("Hi", parent.Text);
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = new Documentation("od") { Text = "Hi" };
        Assert.AreEqual("Hi", parent.Get(ShapesLanguage.Instance.Documentation_text));
    }

    [TestMethod]
    public void Bye_Reflective()
    {
        var parent = new Documentation("od");
        var value = "Bye";
        parent.Set(ShapesLanguage.Instance.Documentation_text, value);
        Assert.AreEqual("Bye", parent.Text);
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = new Documentation("od");
        var value = true;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.Documentation_text, value));
        Assert.AreEqual(null, parent.Text);
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = new Documentation("od");
        var value = 10;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(ShapesLanguage.Instance.Documentation_text, value));
        Assert.AreEqual(null, parent.Text);
    }

    [TestMethod]
    public void Constructor()
    {
        var parent = new Documentation("myId") { Text = "Hi" };
        Assert.AreEqual("Hi", parent.Text);
    }

    [TestMethod]
    public void TryGet()
    {
        var parent = new Documentation("myId") { Text = "Hi" };
        Assert.IsTrue(parent.TryGetText(out var o));
        Assert.AreEqual("Hi", o);
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new Documentation("od");
        object value = null;
        parent.Text = (string?)value;
        Assert.AreEqual(null, parent.Text);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new Documentation("od");
        object value = null;
        parent.SetText((string?)value);
        Assert.AreEqual(null, parent.Text);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new Documentation("od");
        object value = null;
        parent.Set(ShapesLanguage.Instance.Documentation_text, null);
        Assert.AreEqual(null, parent.Text);
    }

    [TestMethod]
    public void Null_Get()
    {
        var parent = new Documentation("od");
        Assert.AreEqual(null, parent.Text);
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = new Documentation("od");
        Assert.AreEqual(null, parent.Get(ShapesLanguage.Instance.Documentation_text));
    }

    [TestMethod]
    public void Null_Constructor()
    {
        object value = null;
        var parent = new Documentation("od") { Text = (string?)value };
        Assert.AreEqual(null, parent.Text);
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new Documentation("od");
        Assert.IsFalse(parent.TryGetText(out var o));
        Assert.IsNull(o);
    }

    #endregion
}