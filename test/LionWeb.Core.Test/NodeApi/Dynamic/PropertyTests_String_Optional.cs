﻿// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.NodeApi.Dynamic;

[TestClass]
public class PropertyTests_String_Optional : DynamicNodeTestsBase
{
    #region Single

    [TestMethod]
    public void Reflective()
    {
        var parent = newDocumentation("od");
        var value = "Hi";
        parent.Set(Documentation_text, value);
        Assert.AreEqual("Hi", parent.Get(Documentation_text));
    }

    [TestMethod]
    public void Get_Reflective()
    {
        var parent = newDocumentation("od");
        parent.Set(Documentation_text, "Hi");
        Assert.AreEqual("Hi", parent.Get(Documentation_text));
    }

    [TestMethod]
    public void Bye_Reflective()
    {
        var parent = newDocumentation("od");
        var value = "Bye";
        parent.Set(Documentation_text, value);
        Assert.AreEqual("Bye", parent.Get(Documentation_text));
    }

    [TestMethod]
    public void Boolean_Reflective()
    {
        var parent = newDocumentation("od");
        var value = true;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(Documentation_text, value));
        Assert.AreEqual(null, parent.Get(Documentation_text));
    }

    [TestMethod]
    public void Integer_Reflective()
    {
        var parent = newDocumentation("od");
        var value = 10;
        Assert.ThrowsException<InvalidValueException>(() => parent.Set(Documentation_text, value));
        Assert.AreEqual(null, parent.Get(Documentation_text));
    }

    #endregion

    #region Null

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = newDocumentation("od");
        object value = null;
        parent.Set(Documentation_text, null);
        Assert.AreEqual(null, parent.Get(Documentation_text));
    }

    [TestMethod]
    public void Null_Get_Reflective()
    {
        var parent = newDocumentation("od");
        Assert.AreEqual(null, parent.Get(Documentation_text));
    }

    #endregion
}