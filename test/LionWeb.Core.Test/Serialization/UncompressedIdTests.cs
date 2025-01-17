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

namespace LionWeb.Core.Test.Serialization;

using M1;

[TestClass]
public class UncompressedIdTests
{
    [TestMethod]
    public void Equals()
    {
        var left = ICompressedId.Create("a", new CompressedIdConfig(false));
        var right = ICompressedId.Create("a", new CompressedIdConfig(false));
        var actual = left.Equals(right);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void NotEquals()
    {
        var left = ICompressedId.Create("a", new CompressedIdConfig(false));
        var right = ICompressedId.Create("b", new CompressedIdConfig(false));
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void HashCode()
    {
        Assert.AreEqual(ICompressedId.Create("a", new CompressedIdConfig(false)).GetHashCode(),
            ICompressedId.Create("a", new CompressedIdConfig(false)).GetHashCode());
    }

    [TestMethod]
    public void NotHashCode()
    {
        Assert.AreNotEqual(ICompressedId.Create("a", new CompressedIdConfig(false)).GetHashCode(),
            ICompressedId.Create("b", new CompressedIdConfig(false)).GetHashCode());
    }

    [TestMethod]
    public void ToString_Original()
    {
        Assert.AreEqual("a", ICompressedId.Create("a", new CompressedIdConfig(false, true)).ToString());
    }

    [TestMethod]
    public void KeepOriginal()
    {
        Assert.AreEqual("a", ICompressedId.Create("a", new CompressedIdConfig(false, true)).Original);
    }

    [TestMethod]
    public void Equals_original()
    {
        var left = ICompressedId.Create("a", new CompressedIdConfig(false, true));
        var right = ICompressedId.Create("a", new CompressedIdConfig(false, true));
        var actual = left.Equals(right);
        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void NotEquals_original()
    {
        var left = ICompressedId.Create("a", new CompressedIdConfig(false, true));
        var right = ICompressedId.Create("b", new CompressedIdConfig(false, true));
        var actual = left.Equals(right);
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void HashCode_original()
    {
        Assert.AreEqual(
            ICompressedId.Create("a", new CompressedIdConfig(false, true)).GetHashCode(),
            ICompressedId.Create("a", new CompressedIdConfig(false, true)).GetHashCode());
    }

    [TestMethod]
    public void NotHashCode_original()
    {
        Assert.AreNotEqual(ICompressedId.Create("a", new CompressedIdConfig(false, true)).GetHashCode(),
            ICompressedId.Create("b", new CompressedIdConfig(false, true)).GetHashCode());
    }
}