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
// ReSharper disable EqualExpressionComparison

namespace LionWeb_CSharp_Test.tests;

using LionWeb.Core;

[TestClass]
public class NullableStructMemberTests
{
    private readonly record struct TestRecordA(int x);

    private enum TestRecordEnum { A, B }

    #region Equals

    [TestMethod]
    public void Assign_Primitive()
    {
        int? value = 42;
        var lazy = new NullableStructMember<int>(value);
        Assert.IsTrue(lazy.Equals(lazy));
    }
    
    [TestMethod]
    public void Assign_Enum()
    {
        TestRecordEnum? value = TestRecordEnum.A;
        var lazy = new NullableStructMember<TestRecordEnum>(value);
        Assert.IsTrue(lazy.Equals(lazy));
    }
    
    [TestMethod]
    public void Assign_Record()
    {
        TestRecordA? value = new TestRecordA { x = 42 };
        var lazy = new NullableStructMember<TestRecordA>(value);
        Assert.IsTrue(lazy.Equals(lazy));
    }
    
    [TestMethod]
    public void Same_NonNull_Set()
    {
        var lazy = new NullableStructMember<TestRecordA>(new());
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void Same_NonNull_Unset()
    {
        var lazy = new NullableStructMember<TestRecordA>(null);
        Assert.IsTrue(lazy.Equals(lazy));
    }

    [TestMethod]
    public void NonNull_Set()
    {
        var a = new NullableStructMember<TestRecordA>(new() { x = 1 });
        var b = new NullableStructMember<TestRecordA>(new() { x = 2 });
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_SameContents()
    {
        var contents = new TestRecordA();
        var a = new NullableStructMember<TestRecordA>(contents);
        var b = new NullableStructMember<TestRecordA>(contents);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_EqualContents()
    {
        var a = new NullableStructMember<int>(42);
        var b = new NullableStructMember<int>(42);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_DifferentContents()
    {
        var a = new NullableStructMember<int>(42);
        var b = new NullableStructMember<int>(23);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_Unset()
    {
        var a = new NullableStructMember<TestRecordA>(null);
        var b = new NullableStructMember<TestRecordA>(null);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_FirstSet()
    {
        var a = new NullableStructMember<TestRecordA>(new());
        var b = new NullableStructMember<TestRecordA>(null);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_SecondSet()
    {
        var a = new NullableStructMember<TestRecordA>(null);
        var b = new NullableStructMember<TestRecordA>(new());
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_Unset_Null()
    {
        var a = new NullableStructMember<TestRecordA>(null);
        NullableStructMember<TestRecordA>? b = null;
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void NonNull_Set_Null()
    {
        var a = new NullableStructMember<TestRecordA>(new());
        NullableStructMember<TestRecordA>? b = null;
        Assert.IsFalse(a.Equals(b));
    }

    #endregion
    
    #region GetHashCode

    [TestMethod]
    public void GetHashCode_Set()
    {
        var value = new TestRecordA();
        var lazy = new NullableStructMember<TestRecordA>(value);
        Assert.AreEqual(value.GetHashCode(), lazy.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_Unset()
    {
        var lazy = new NullableStructMember<TestRecordA>(null);
        Assert.AreEqual(0, lazy.GetHashCode());
    }

    #endregion
}