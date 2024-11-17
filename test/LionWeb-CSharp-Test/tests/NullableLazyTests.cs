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
public class NullableLazyTests
{
    internal readonly record struct TestRecordA(int x);

    internal enum TestRecordEnum { A, B }

    #region Equals

    [TestMethod]
    public void Assign_Primitive()
    {
        int? value = 42;
        var lazy = new NullableLazy<int>(value);
        Assert.IsTrue(lazy.Equals(lazy));
    }
    
    [TestMethod]
    public void Assign_Enum()
    {
        TestRecordEnum? value = TestRecordEnum.A;
        var lazy = new NullableLazy<TestRecordEnum>(value);
        Assert.IsTrue(lazy.Equals(lazy));
    }
    
    [TestMethod]
    public void Assign_Record()
    {
        TestRecordA? value = new TestRecordA { x = 42 };
        var lazy = new NullableLazy<TestRecordA>(value);
        Assert.IsTrue(lazy.Equals(lazy));
    }
    
    [TestMethod]
    public void Same_NonNull_Set()
    {
        var lazy = new NullableLazy<TestRecordA>(new());
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void Same_NonNull_Unset()
    {
        var lazy = new NullableLazy<TestRecordA>(null);
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void NonNull_Set()
    {
        var a = new NullableLazy<TestRecordA>(new() { x = 1 });
        var b = new NullableLazy<TestRecordA>(new() { x = 2 });
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_SameContents()
    {
        var contents = new TestRecordA();
        var a = new NullableLazy<TestRecordA>(contents);
        var b = new NullableLazy<TestRecordA>(contents);
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
        var a = new NullableLazy<TestRecordA>(null);
        var b = new NullableLazy<TestRecordA>(null);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_FirstSet()
    {
        var a = new NullableLazy<TestRecordA>(new());
        var b = new NullableLazy<TestRecordA>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_SecondSet()
    {
        var a = new NullableLazy<TestRecordA>(null);
        var b = new NullableLazy<TestRecordA>(new());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_Unset_Null()
    {
        var a = new NullableLazy<TestRecordA>(null);
        NullableLazy<TestRecordA>? b = null;
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_Set_Null()
    {
        var a = new NullableLazy<TestRecordA>(new());
        NullableLazy<TestRecordA>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion
    
    #region NonNull

    [TestMethod]
    public void GetHashCode_Set()
    {
        var value = new TestRecordA();
        var lazy = new NullableLazy<TestRecordA>(value);
        Assert.AreEqual(value.GetHashCode(), lazy.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_Unset()
    {
        var lazy = new NullableLazy<TestRecordA>(null);
        Assert.AreEqual(0, lazy.GetHashCode());
    }

    #endregion
}