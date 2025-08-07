// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

namespace LionWeb.Core.Test;

using System.Reflection;

[TestClass]
public class NodeIdXTests
{
    #region Empty

    [TestMethod]
    public void Empty_Create()
    {
        var id = new NodeIdX("");
        Assert.IsNotNull(id);
    }

    [TestMethod]
    public void Empty_IsBytes()
    {
        var id = new NodeIdX("");
        var value = Value(id);

        Assert.AreEqual(0, ((byte[])value).Length);

        var invalidId = InvalidId(id);
        Assert.AreSame("", invalidId);
    }

    [TestMethod]
    public void Empty_Equals()
    {
        var a = new NodeIdX("");
        var b = new NodeIdX("");

        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Empty_HashCode()
    {
        var a = new NodeIdX("");
        var b = new NodeIdX("");

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void Empty_ToString()
    {
        var id = new NodeIdX("");

        Assert.AreEqual("", id.ToString());
    }

    [TestMethod]
    public void Empty_PrintMembers()
    {
        var host = new NodeHost("a", new NodeIdX(""), "b");

        Assert.AreEqual("NodeHost { a = a, nodeId = , b = b }", host.ToString());
    }

    #endregion

    #region SingleChar

    [TestMethod]
    public void SingleChar_Create()
    {
        var id = new NodeIdX("B");
        Assert.IsNotNull(id);
    }

    [TestMethod]
    public void SingleChar_IsBytes()
    {
        var id = new NodeIdX("7");
        var value = Value(id);

        Assert.IsInstanceOfType<byte[]>(value);

        var invalidId = InvalidId(id);
        Assert.IsNull(invalidId);
    }

    [TestMethod]
    public void SingleChar_Equals_True()
    {
        var a = new NodeIdX("b");
        var b = new NodeIdX("b");

        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void SingleChar_Equals_False()
    {
        var a = new NodeIdX("c");
        var b = new NodeIdX("d");

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void SingleChar_HashCode_Equal()
    {
        var a = new NodeIdX("2");
        var b = new NodeIdX("2");

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void SingleChar_HashCode_Different()
    {
        var a = new NodeIdX("3");
        var b = new NodeIdX("D");

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void SingleChar_ToString()
    {
        var id = new NodeIdX("-");

        Assert.AreEqual("-", id.ToString());
    }

    [TestMethod]
    public void SingleChar_PrintMembers()
    {
        var host = new NodeHost("a", new NodeIdX("_"), "b");

        Assert.AreEqual("NodeHost { a = a, nodeId = _, b = b }", host.ToString());
    }

    #endregion

    #region TwoChars

    [TestMethod]
    public void TwoChars_Create()
    {
        var id = new NodeIdX("Bx");
        Assert.IsNotNull(id);
    }

    [TestMethod]
    public void TwoChars_IsBytes()
    {
        var id = new NodeIdX("e7");
        var value = Value(id);

        Assert.IsInstanceOfType<byte[]>(value);

        var invalidId = InvalidId(id);
        Assert.IsNull(invalidId);
    }

    [TestMethod]
    public void TwoChars_Equals_True()
    {
        var a = new NodeIdX("EE");
        var b = new NodeIdX("EE");

        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void TwoChars_Equals_False()
    {
        var a = new NodeIdX("E7");
        var b = new NodeIdX("E_");

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void TwoChars_HashCode_Equal()
    {
        var a = new NodeIdX("2-");
        var b = new NodeIdX("2-");

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void TwoChars_HashCode_Different()
    {
        var a = new NodeIdX("3-");
        var b = new NodeIdX("-3");

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void TwoChars_ToString()
    {
        var id = new NodeIdX("1-");

        Assert.AreEqual("1-", id.ToString());
    }

    [TestMethod]
    public void TwoChars_PrintMembers()
    {
        var host = new NodeHost("a", new NodeIdX("__"), "b");

        Assert.AreEqual("NodeHost { a = a, nodeId = __, b = b }", host.ToString());
    }

    #endregion

    #region ThreeChars

    [TestMethod]
    public void ThreeChars_Create()
    {
        var id = new NodeIdX("B_x");
        Assert.IsNotNull(id);
    }

    [TestMethod]
    public void ThreeChars_IsBytes()
    {
        var id = new NodeIdX("ee7");
        var value = Value(id);

        Assert.IsInstanceOfType<byte[]>(value);

        var invalidId = InvalidId(id);
        Assert.IsNull(invalidId);
    }

    [TestMethod]
    public void ThreeChars_Equals_True()
    {
        var a = new NodeIdX("EbE");
        var b = new NodeIdX("EbE");

        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void ThreeChars_Equals_False()
    {
        var a = new NodeIdX("El7");
        var b = new NodeIdX("EL7");

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void ThreeChars_HashCode_Equal()
    {
        var a = new NodeIdX("i2-");
        var b = new NodeIdX("i2-");

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void ThreeChars_HashCode_Different()
    {
        var a = new NodeIdX("3y-");
        var b = new NodeIdX("3Y-");

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void ThreeChars_ToString()
    {
        var id = new NodeIdX("d1-");

        Assert.AreEqual("d1-", id.ToString());
    }

    [TestMethod]
    public void ThreeChars_PrintMembers()
    {
        var host = new NodeHost("a", new NodeIdX("_-_"), "b");

        Assert.AreEqual("NodeHost { a = a, nodeId = _-_, b = b }", host.ToString());
    }

    #endregion

    #region FourChars

    [TestMethod]
    public void FourChars_Create()
    {
        var id = new NodeIdX("B_8x");
        Assert.IsNotNull(id);
    }

    [TestMethod]
    public void FourChars_IsBytes()
    {
        var id = new NodeIdX("eEe7");
        var value = Value(id);

        Assert.IsInstanceOfType<byte[]>(value);

        var invalidId = InvalidId(id);
        Assert.IsNull(invalidId);
    }

    [TestMethod]
    public void FourChars_Equals_True()
    {
        var a = new NodeIdX("EbE-");
        var b = new NodeIdX("EbE-");

        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void FourChars_Equals_False()
    {
        var a = new NodeIdX("-El7");
        var b = new NodeIdX("EL7-");

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void FourChars_HashCode_Equal()
    {
        var a = new NodeIdX("i_2-");
        var b = new NodeIdX("i_2-");

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void FourChars_HashCode_Different()
    {
        var a = new NodeIdX("A3y-");
        var b = new NodeIdX("3AY-");

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void FourChars_ToString()
    {
        var id = new NodeIdX("dU1-");

        Assert.AreEqual("dU1-", id.ToString());
    }

    [TestMethod]
    public void FourChars_PrintMembers()
    {
        var host = new NodeHost("a", new NodeIdX("_4-_"), "b");

        Assert.AreEqual("NodeHost { a = a, nodeId = _4-_, b = b }", host.ToString());
    }

    #endregion

    #region FiveChars

    [TestMethod]
    public void FiveChars_Create()
    {
        var id = new NodeIdX("B_8-x");
        Assert.IsNotNull(id);
    }

    [TestMethod]
    public void FiveChars_IsBytes()
    {
        var id = new NodeIdX("eE-_7");
        var value = Value(id);

        Assert.IsInstanceOfType<byte[]>(value);

        var invalidId = InvalidId(id);
        Assert.IsNull(invalidId);
    }

    [TestMethod]
    public void FiveChars_Equals_True()
    {
        var a = new NodeIdX("3b_E-");
        var b = new NodeIdX("3b_E-");

        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void FiveChars_Equals_False()
    {
        var a = new NodeIdX("-El7_");
        var b = new NodeIdX("EL7-_");

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void FiveChars_HashCode_Equal()
    {
        var a = new NodeIdX("i_2-A");
        var b = new NodeIdX("i_2-A");

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void FiveChars_HashCode_Different()
    {
        var a = new NodeIdX("_A3y-");
        var b = new NodeIdX("_3AY-");

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void FiveChars_ToString()
    {
        var id = new NodeIdX("d_U1-");

        Assert.AreEqual("d_U1-", id.ToString());
    }

    [TestMethod]
    public void FiveChars_PrintMembers()
    {
        var host = new NodeHost("a", new NodeIdX("_T4-_"), "b");

        Assert.AreEqual("NodeHost { a = a, nodeId = _T4-_, b = b }", host.ToString());
    }

    #endregion

    #region Invalid

    [TestMethod]
    public void Invalid_Create()
    {
        var id = new NodeIdX("**");
        Assert.IsNotNull(id);
    }

    [TestMethod]
    public void Invalid_IsString()
    {
        var id = new NodeIdX("!^@$#");
        var value = Value(id);

        Assert.AreEqual(0, ((byte[])value).Length);

        var invalidId = InvalidId(id);
        Assert.IsNotNull(invalidId);
    }

    [TestMethod]
    public void Invalid_Equals_True()
    {
        var a = new NodeIdX("^");
        var b = new NodeIdX("^");

        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Invalid_Equals_False()
    {
        var a = new NodeIdX("#");
        var b = new NodeIdX("##");

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Invalid_HashCode_Equal()
    {
        var a = new NodeIdX("@");
        var b = new NodeIdX("@");

        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void Invalid_HashCode_Different()
    {
        var a = new NodeIdX("!");
        var b = new NodeIdX("?");

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void Invalid_ToString()
    {
        var id = new NodeIdX("?");

        Assert.AreEqual("?", id.ToString());
    }

    [TestMethod]
    public void Invalid_PrintMembers()
    {
        var host = new NodeHost("a", new NodeIdX("!!!"), "b");

        Assert.AreEqual("NodeHost { a = a, nodeId = !!!, b = b }", host.ToString());
    }

    #endregion

    #region Mix

    [TestMethod]
    public void Mix_Invalid_Equals()
    {
        var a = new NodeIdX("A");
        var b = new NodeIdX("@");

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Mix_Invalid_HashCode()
    {
        var a = new NodeIdX("#");
        var b = new NodeIdX("d");

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void Mix_Length_Equals()
    {
        var a = new NodeIdX("A");
        var b = new NodeIdX("AA");

        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void Mix_Length_HashCode()
    {
        var a = new NodeIdX("x");
        var b = new NodeIdX("xx");

        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    #endregion

    private static object? InvalidId(NodeIdX id)
    {
        var invalidIdField = typeof(NodeIdX).GetField("_invalidId", BindingFlags.NonPublic | BindingFlags.Instance);
        var invalidId = invalidIdField.GetValue(id);
        return invalidId;
    }

    private static object? Value(NodeIdX id)
    {
        var valueField = typeof(NodeIdX).GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance);
        var value = valueField.GetValue(id);
        return value;
    }
}

internal record NodeHost(string a, NodeIdX nodeId, string b);