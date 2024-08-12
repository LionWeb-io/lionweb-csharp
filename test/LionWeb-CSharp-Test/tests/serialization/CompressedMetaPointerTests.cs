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

namespace LionWeb_CSharp_Test.tests.serialization;

using LionWeb.Core.M1;
using LionWeb.Core.Serialization;

[TestClass]
public class CompressedMetaPointerTests
{
    [TestMethod]
    public void Equals()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false);
        var right = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false);
        var actual = left.Equals(right);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void NotEquals()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false);
        var right = CompressedMetaPointer.Create(new MetaPointer("x", "y", "z"), false);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void HashCode()
    {
        Assert.AreEqual(CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false).GetHashCode());
    }

    [TestMethod]
    public void NotHashCode()
    {
        Assert.AreNotEqual(CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("x", "y", "z"), false).GetHashCode());
    }

    [TestMethod]
    public void ToString_Compressed()
    {
        Assert.AreNotEqual(new MetaPointer("a", "b", "c").ToString(),
            CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false).ToString());
    }

    [TestMethod]
    public void ToString_Original()
    {
        Assert.AreEqual(new MetaPointer("a", "b", "c").ToString(),
            CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true).ToString());
    }

    [TestMethod]
    public void NotEquals_DifferentLanguage()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false);
        var right = CompressedMetaPointer.Create(new MetaPointer("x", "b", "c"), false);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void NotHashCode_DifferentLanguage()
    {
        Assert.AreNotEqual(CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("x", "b", "c"), false).GetHashCode());
    }

    [TestMethod]
    public void NotEquals_DifferentVersion()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false);
        var right = CompressedMetaPointer.Create(new MetaPointer("a", "y", "c"), false);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void NotHashCode_DifferentVersion()
    {
        Assert.AreNotEqual(CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("a", "y", "c"), false).GetHashCode());
    }

    [TestMethod]
    public void NotEquals_DifferentKey()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false);
        var right = CompressedMetaPointer.Create(new MetaPointer("a", "b", "z"), false);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void NotHashCode_DifferentKey()
    {
        Assert.AreNotEqual(CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("a", "b", "z"), false).GetHashCode());
    }

    [TestMethod]
    public void NotEquals_DifferentLengths()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "bb", "ccc"), false);
        var right = CompressedMetaPointer.Create(new MetaPointer("aaa", "bb", "c"), false);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void NotHashCode_DifferentLengths()
    {
        Assert.AreNotEqual(CompressedMetaPointer.Create(new MetaPointer("a", "bb", "ccc"), false).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("aaa", "bb", "c"), false).GetHashCode());
    }

    [TestMethod]
    public void KeepOriginal()
    {
        Assert.AreEqual(new MetaPointer("a", "b", "c"),
            CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true).Original);
    }

    [TestMethod]
    public void DontKeepOriginal()
    {
        Assert.IsNull(CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false).Original);
    }

    [TestMethod]
    public void Equals_original()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true);
        var right = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true);
        var actual = left.Equals(right);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void NotEquals_original()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true);
        var right = CompressedMetaPointer.Create(new MetaPointer("x", "y", "z"), true);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void HashCode_original()
    {
        Assert.AreEqual(
            CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true).GetHashCode());
    }

    [TestMethod]
    public void NotHashCode_original()
    {
        Assert.AreNotEqual(CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("x", "y", "z"), true).GetHashCode());
    }

    [TestMethod]
    public void Equals_mixed()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true);
        var right = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false);
        var actual = left.Equals(right);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void NotEquals_mixed()
    {
        var left = CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true);
        var right = CompressedMetaPointer.Create(new MetaPointer("x", "y", "z"), false);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void HashCode_mixed()
    {
        Assert.AreEqual(
            CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), false).GetHashCode());
    }

    [TestMethod]
    public void NotHashCode_mixed()
    {
        Assert.AreNotEqual(CompressedMetaPointer.Create(new MetaPointer("a", "b", "c"), true).GetHashCode(),
            CompressedMetaPointer.Create(new MetaPointer("x", "y", "z"), false).GetHashCode());
    }
}