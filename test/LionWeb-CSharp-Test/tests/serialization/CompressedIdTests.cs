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

namespace LionWeb_CSharp_Test.tests.serialization;

using LionWeb.Core.M1;

[TestClass]
public class CompressedIdTests
{
    [TestMethod]
    public void Equals()
    {
        var left = CompressedId.Create("a", false);
        var right = CompressedId.Create("a", false);
        var actual = left.Equals(right);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void NotEquals()
    {
        var left = CompressedId.Create("a", false);
        var right = CompressedId.Create("b", false);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void HashCode()
    {
        Assert.AreEqual(CompressedId.Create("a", false).GetHashCode(),
            CompressedId.Create("a", false).GetHashCode());
    }

    [TestMethod]
    public void NotHashCode()
    {
        Assert.AreNotEqual(CompressedId.Create("a", false).GetHashCode(),
            CompressedId.Create("b", false).GetHashCode());
    }

    [TestMethod]
    public void ToString_Compressed()
    {
        Assert.AreNotEqual("a", CompressedId.Create("a", false).ToString());
    }

    [TestMethod]
    public void ToString_Original()
    {
        Assert.AreEqual("a", CompressedId.Create("a", true).ToString());
    }

    [TestMethod]
    public void KeepOriginal()
    {
        Assert.AreEqual("a", CompressedId.Create("a", true).Original);
    }

    [TestMethod]
    public void DontKeepOriginal()
    {
        Assert.IsNull(CompressedId.Create("a", false).Original);
    }

    [TestMethod]
    public void Equals_original()
    {
        var left = CompressedId.Create("a", true);
        var right = CompressedId.Create("a", true);
        var actual = left.Equals(right);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void NotEquals_original()
    {
        var left = CompressedId.Create("a", true);
        var right = CompressedId.Create("b", true);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void HashCode_original()
    {
        Assert.AreEqual(
            CompressedId.Create("a", true).GetHashCode(),
            CompressedId.Create("a", true).GetHashCode());
    }

    [TestMethod]
    public void NotHashCode_original()
    {
        Assert.AreNotEqual(CompressedId.Create("a", true).GetHashCode(),
            CompressedId.Create("b", true).GetHashCode());
    }

    [TestMethod]
    public void Equals_mixed()
    {
        var left = CompressedId.Create("a", true);
        var right = CompressedId.Create("a", false);
        var actual = left.Equals(right);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void NotEquals_mixed()
    {
        var left = CompressedId.Create("a", true);
        var right = CompressedId.Create("b", false);
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void HashCode_mixed()
    {
        Assert.AreEqual(
            CompressedId.Create("a", true).GetHashCode(),
            CompressedId.Create("a", false).GetHashCode());
    }

    [TestMethod]
    public void NotHashCode_mixed()
    {
        Assert.AreNotEqual(CompressedId.Create("a", true).GetHashCode(),
            CompressedId.Create("b", false).GetHashCode());
    }
}