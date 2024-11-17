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
public class NullableLazyEqualsSubTypeTests
{
    #region NonNull

    [TestMethod]
    public void Equals_Same_NonNull_Set()
    {
        var lazy = new NullableLazy<object>(new NullReferenceException());
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void Equals_SameType_NonNull_Set()
    {
        var a = new NullableLazy<object>(new NullReferenceException());
        var b = new NullableLazy<object>(new NullReferenceException());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_NonNull_SameContents()
    {
        var contents = new NullReferenceException();
        var a = new NullableLazy<object>(contents);
        var b = new NullableLazy<object>(contents);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_NonNull_EqualContents()
    {
        var a = new NullableLazy<object>(42);
        var b = new NullableLazy<object>(42);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_NonNull_DifferentContents()
    {
        var a = new NullableLazy<object>(42);
        var b = new NullableLazy<object>(23);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_NonNull_FirstSet()
    {
        var a = new NullableLazy<object>(new NullReferenceException());
        var b = new NullableLazy<object>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_NonNull_SecondSet()
    {
        var a = new NullableLazy<object>(null);
        var b = new NullableLazy<object>(new NullReferenceException());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_NonNull_Set_Null()
    {
        var a = new NullableLazy<object>(new NullReferenceException());
        NullableLazy<object>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion

    #region Nullable

    [TestMethod]
    public void Equals_Same_Nullable_Set()
    {
        var lazy = new NullableLazy<object?>(new NullReferenceException());
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void Equals_SameType_Nullable_Set()
    {
        var a = new NullableLazy<object?>(new NullReferenceException());
        var b = new NullableLazy<object?>(new NullReferenceException());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_Nullable_SameContents()
    {
        var contents = new NullReferenceException();
        var a = new NullableLazy<object?>(contents);
        var b = new NullableLazy<object?>(contents);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_Nullable_EqualContents()
    {
        var a = new NullableLazy<object?>(42);
        var b = new NullableLazy<object?>(42);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_Nullable_DifferentContents()
    {
        var a = new NullableLazy<object?>(42);
        var b = new NullableLazy<object?>(23);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_Nullable_FirstSet()
    {
        var a = new NullableLazy<object?>(new NullReferenceException());
        var b = new NullableLazy<object?>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_Nullable_SecondSet()
    {
        var a = new NullableLazy<object?>(null);
        var b = new NullableLazy<object?>(new NullReferenceException());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_Nullable_Set_Null()
    {
        var a = new NullableLazy<object?>(new NullReferenceException());
        NullableLazy<object?>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion

    #region FirstNullable

    [TestMethod]
    public void Equals_SameType_FirstNullable_Set()
    {
        var a = new NullableLazy<object?>(new NullReferenceException());
        var b = new NullableLazy<object>(new NullReferenceException());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_FirstNullable_SameContents()
    {
        var contents = new NullReferenceException();
        var a = new NullableLazy<object?>(contents);
        var b = new NullableLazy<object>(contents);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_FirstNullable_EqualContents()
    {
        var a = new NullableLazy<object?>(42);
        var b = new NullableLazy<object>(42);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_FirstNullable_DifferentContents()
    {
        var a = new NullableLazy<object?>(42);
        var b = new NullableLazy<object>(23);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_FirstNullable_FirstSet()
    {
        var a = new NullableLazy<object?>(new NullReferenceException());
        var b = new NullableLazy<object>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_FirstNullable_SecondSet()
    {
        var a = new NullableLazy<object?>(null);
        var b = new NullableLazy<object>(new NullReferenceException());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_FirstNullable_Set_Null()
    {
        var a = new NullableLazy<object?>(new NullReferenceException());
        NullableLazy<object>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion

    #region SecondNullable

    [TestMethod]
    public void Equals_SameType_SecondNullable_Set()
    {
        var a = new NullableLazy<object>(new NullReferenceException());
        var b = new NullableLazy<object?>(new NullReferenceException());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_SecondNullable_SameContents()
    {
        var contents = new NullReferenceException();
        var a = new NullableLazy<object>(contents);
        var b = new NullableLazy<object?>(contents);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_SecondNullable_EqualContents()
    {
        var a = new NullableLazy<object>(42);
        var b = new NullableLazy<object?>(42);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_SecondNullable_DifferentContents()
    {
        var a = new NullableLazy<object>(42);
        var b = new NullableLazy<object?>(23);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_SecondNullable_FirstSet()
    {
        var a = new NullableLazy<object>(new NullReferenceException());
        var b = new NullableLazy<object?>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SameType_SecondNullable_SecondSet()
    {
        var a = new NullableLazy<object>(null);
        var b = new NullableLazy<object?>(new NullReferenceException());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Equals_SecondNullable_Set_Null()
    {
        var a = new NullableLazy<object>(new NullReferenceException());
        NullableLazy<object?>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion
}