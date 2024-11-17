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

// ReSharper disable SuggestVarOrType_Elsewhere

namespace LionWeb_CSharp_Test.tests;

using LionWeb.Core;

[TestClass]
public class NullableLazyEqualsSameTypeTests
{
    #region NonNull

    [TestMethod]
    public void Same_NonNull_Set()
    {
        var lazy = new NullableLazy<object>(new());
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void Same_NonNull_Unset()
    {
        var lazy = new NullableLazy<object>(null);
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void NonNull_Set()
    {
        var a = new NullableLazy<object>(new());
        var b = new NullableLazy<object>(new());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_SameContents()
    {
        var contents = new object();
        var a = new NullableLazy<object>(contents);
        var b = new NullableLazy<object>(contents);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_EqualContents()
    {
        var a = new NullableLazy<int>(42);
        var b = new NullableLazy<int>(42);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_DifferentContents()
    {
        var a = new NullableLazy<int>(42);
        var b = new NullableLazy<int>(23);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_Unset()
    {
        var a = new NullableLazy<object>(null);
        var b = new NullableLazy<object>(null);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_FirstSet()
    {
        var a = new NullableLazy<object>(new object());
        var b = new NullableLazy<object>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_SecondSet()
    {
        var a = new NullableLazy<object>(null);
        var b = new NullableLazy<object>(new object());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_Unset_Null()
    {
        var a = new NullableLazy<object>(null);
        NullableLazy<object>? b = null;
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_Set_Null()
    {
        var a = new NullableLazy<object>(new object());
        NullableLazy<object>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion

    #region Nullable

    [TestMethod]
    public void Same_Nullable_Set()
    {
        var lazy = new NullableLazy<object?>(new());
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void Same_Nullable_Unset()
    {
        var lazy = new NullableLazy<object?>(null);
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void Nullable_Set()
    {
        var a = new NullableLazy<object?>(new());
        var b = new NullableLazy<object?>(new());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Nullable_SameContents()
    {
        var contents = new object();
        var a = new NullableLazy<object?>(contents);
        var b = new NullableLazy<object?>(contents);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Nullable_EqualContents()
    {
        var a = new NullableLazy<int?>(42);
        var b = new NullableLazy<int?>(42);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Nullable_DifferentContents()
    {
        var a = new NullableLazy<int?>(42);
        var b = new NullableLazy<int?>(23);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Nullable_Unset()
    {
        var a = new NullableLazy<object?>(null);
        var b = new NullableLazy<object?>(null);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Nullable_FirstSet()
    {
        var a = new NullableLazy<object?>(new object());
        var b = new NullableLazy<object?>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Nullable_SecondSet()
    {
        var a = new NullableLazy<object?>(null);
        var b = new NullableLazy<object?>(new object());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Nullable_Unset_Null()
    {
        var a = new NullableLazy<object?>(null);
        NullableLazy<object?>? b = null;
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Nullable_Set_Null()
    {
        var a = new NullableLazy<object?>(new object());
        NullableLazy<object?>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion

    #region FirstNullable

    [TestMethod]
    public void FirstNullable_Set()
    {
        var a = new NullableLazy<object?>(new());
        var b = new NullableLazy<object>(new());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void FirstNullable_SameContents()
    {
        var contents = new object();
        var a = new NullableLazy<object?>(contents);
        var b = new NullableLazy<object>(contents);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void FirstNullable_EqualContents()
    {
        var a = new NullableLazy<int?>(42);
        var b = new NullableLazy<int>(42);
        // Assert.IsTrue(a.Equals(b));
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void FirstNullable_DifferentContents()
    {
        var a = new NullableLazy<int?>(42);
        var b = new NullableLazy<int>(23);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void FirstNullable_Unset()
    {
        var a = new NullableLazy<object?>(null);
        var b = new NullableLazy<object>(null);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void FirstNullable_FirstSet()
    {
        var a = new NullableLazy<object?>(new object());
        var b = new NullableLazy<object>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void FirstNullable_SecondSet()
    {
        var a = new NullableLazy<object?>(null);
        var b = new NullableLazy<object>(new object());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void FirstNullable_Unset_Null()
    {
        var a = new NullableLazy<object?>(null);
        NullableLazy<object>? b = null;
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void FirstNullable_Set_Null()
    {
        var a = new NullableLazy<object?>(new object());
        NullableLazy<object>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion

    #region SecondNullable

    [TestMethod]
    public void SecondNullable_Set()
    {
        var a = new NullableLazy<object>(new());
        var b = new NullableLazy<object?>(new());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void SecondNullable_SameContents()
    {
        var contents = new object();
        var a = new NullableLazy<object>(contents);
        var b = new NullableLazy<object?>(contents);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void SecondNullable_EqualContents()
    {
        var a = new NullableLazy<int>(42);
        var b = new NullableLazy<int?>(42);
        // Assert.IsTrue(a.Equals(b));
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void SecondNullable_DifferentContents()
    {
        var a = new NullableLazy<int>(42);
        var b = new NullableLazy<int?>(23);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void SecondNullable_Unset()
    {
        var a = new NullableLazy<object>(null);
        var b = new NullableLazy<object?>(null);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void SecondNullable_FirstSet()
    {
        var a = new NullableLazy<object>(new object());
        var b = new NullableLazy<object?>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void SecondNullable_SecondSet()
    {
        var a = new NullableLazy<object>(null);
        var b = new NullableLazy<object?>(new object());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void SecondNullable_Unset_Null()
    {
        var a = new NullableLazy<object>(null);
        NullableLazy<object?>? b = null;
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void SecondNullable_Set_Null()
    {
        var a = new NullableLazy<object>(new object());
        NullableLazy<object?>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion
}