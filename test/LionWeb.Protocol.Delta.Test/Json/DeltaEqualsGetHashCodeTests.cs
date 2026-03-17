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

namespace LionWeb.Protocol.Delta.Test.Json;

using Core.Serialization;
using Message;
using Message.Command;
using Message.Event;
using Message.Query;

[TestClass]
public class DeltaEqualsGetHashCodeTests : JsonTestsBase
{
    #region Command

    [TestMethod]
    public void AddProperty_Different_Self()
    {
        var a = new AddProperty("a", new MetaPointer("myLang", "v0", "key"), "x", "aa",
            CreateAdditionalInfos("msgA"));
        var b = new AddProperty("b", new MetaPointer("myLang", "v0", "key"), "x", "aa",
            CreateAdditionalInfos("msgA"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void AddProperty_Different_Generic()
    {
        var a = new AddProperty("a", new MetaPointer("myLang", "v0", "key"), "x", "aa",
            CreateAdditionalInfos("msgA"));
        var b = new AddProperty("a", new MetaPointer("myLang", "v0", "key"), "x", "aa",
            CreateAdditionalInfos("msgB"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void AddProperty_Same()
    {
        var a = new AddProperty("a", new MetaPointer("myLang", "v0", "key"), "x", "aa",
            CreateAdditionalInfos("msgA"));
        var b = new AddProperty("a", new MetaPointer("myLang", "v0", "key"), "x", "aa",
            CreateAdditionalInfos("msgA"));

        Assert.IsTrue(a.Equals(b));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void CompositeCommand_Different_Self()
    {
        var a = new CompositeCommand([
            new DeleteProperty("a", new MetaPointer("myLang", "v0", "key"), "x", CreateAdditionalInfos("msgA")),
        ], "cc", CreateAdditionalInfos("msgAA"));
        var b = new CompositeCommand([
            new DeleteProperty("b", new MetaPointer("myLang", "v0", "key"), "x", CreateAdditionalInfos("msgA")),
        ], "cc", CreateAdditionalInfos("msgAA"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void CompositeCommand_Different_Generic()
    {
        var a = new CompositeCommand([
            new DeleteProperty("a", new MetaPointer("myLang", "v0", "key"), "x", CreateAdditionalInfos("msgA")),
        ], "cc", CreateAdditionalInfos("msgAA"));
        var b = new CompositeCommand([
            new DeleteProperty("a", new MetaPointer("myLang", "v0", "key"), "x", CreateAdditionalInfos("msgA")),
        ], "cc", CreateAdditionalInfos("msgBB"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void CompositeCommand_Same()
    {
        var a = new CompositeCommand([
            new DeleteProperty("a", new MetaPointer("myLang", "v0", "key"), "x", CreateAdditionalInfos("msgA")),
        ], "cc", CreateAdditionalInfos("msgAA"));
        var b = new CompositeCommand([
            new DeleteProperty("a", new MetaPointer("myLang", "v0", "key"), "x", CreateAdditionalInfos("msgA")),
        ], "cc", CreateAdditionalInfos("msgAA"));

        Assert.IsTrue(a.Equals(b));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    #endregion

    #region Event

    [TestMethod]
    public void PropertyAdded_Different_Self()
    {
        var a = new PropertyAdded("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("x", "y")],
            CreateAdditionalInfos("msgA"));
        var b = new PropertyAdded("b", new MetaPointer("myLang", "v0", "key"), "vv",  [new CommandSource("x", "y")],
            CreateAdditionalInfos("msgA"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void PropertyAdded_Different_Generic()
    {
        var a = new PropertyAdded("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("x", "y")],
            CreateAdditionalInfos("msgA"));
        var b = new PropertyAdded("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("z", "y")],
            CreateAdditionalInfos("msgA"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void PropertyAdded_Same()
    {
        var a = new PropertyAdded("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("x", "y")],
            CreateAdditionalInfos("msgA"));
        var b = new PropertyAdded("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("x", "y")],
            CreateAdditionalInfos("msgA"));

        Assert.IsTrue(a.Equals(b));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void CompositeEvent_Different_Self()
    {
        var a = new CompositeEvent(
        [
            new PropertyDeleted("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("x", "y")],
                CreateAdditionalInfos("msgA")),
        ], CreateAdditionalInfos("msgAA"));
        var b = new CompositeEvent(
        [
            new PropertyDeleted("b", new MetaPointer("myLang", "v0", "key"), "vv",  [new CommandSource("x", "y")],
                CreateAdditionalInfos("msgA")),
        ], CreateAdditionalInfos("msgAA"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void CompositeEvent_Different_Generic()
    {
        var a = new CompositeEvent(
        [
            new PropertyDeleted("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("x", "y")],
                CreateAdditionalInfos("msgA")),
        ], CreateAdditionalInfos("msgAA"));
        var b = new CompositeEvent(
        [
            new PropertyDeleted("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("x", "z")],
                CreateAdditionalInfos("msgA")),
        ], CreateAdditionalInfos("msgAA"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void CompositeEvent_Same()
    {
        var a = new CompositeEvent(
        [
            new PropertyDeleted("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("x", "y")],
                CreateAdditionalInfos("msgA")),
        ], CreateAdditionalInfos("msgAA"));
        var b = new CompositeEvent(
        [
            new PropertyDeleted("a", new MetaPointer("myLang", "v0", "key"), "vv", [new CommandSource("x", "y")],
                CreateAdditionalInfos("msgA")),
        ], CreateAdditionalInfos("msgAA"));

        Assert.IsTrue(a.Equals(b));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    #endregion

    #region Query

    [TestMethod]
    public void SubscribePartitionRequest_Different_Self()
    {
        var a = new SubscribeToPartitionContentsRequest("a", "x", CreateAdditionalInfos("msgA"));
        var b = new SubscribeToPartitionContentsRequest("b", "x", CreateAdditionalInfos("msgA"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void SubscribePartitionRequest_Different_Generic()
    {
        var a = new SubscribeToPartitionContentsRequest("a", "x", CreateAdditionalInfos("msgA"));
        var b = new SubscribeToPartitionContentsRequest("a", "x", CreateAdditionalInfos("msgB"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void SubscribePartitionRequest_Same()
    {
        var a = new SubscribeToPartitionContentsRequest("a", "x", CreateAdditionalInfos("msgA"));
        var b = new SubscribeToPartitionContentsRequest("a", "x", CreateAdditionalInfos("msgA"));

        Assert.IsTrue(a.Equals(b));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void ReconnectResponse_Different_Self()
    {
        var a = new ReconnectResponse(1, "q", CreateAdditionalInfos("msgA"));
        var b = new ReconnectResponse(2, "q", CreateAdditionalInfos("msgA"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void ReconnectResponse_Different_Generic()
    {
        var a = new ReconnectResponse(1, "q", CreateAdditionalInfos("msgA"));
        var b = new ReconnectResponse(1, "q", CreateAdditionalInfos("msgB"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void ReconnectResponse_Same()
    {
        var a = new ReconnectResponse(1, "q", CreateAdditionalInfos("msgA"));
        var b = new ReconnectResponse(1, "q", CreateAdditionalInfos("msgA"));

        Assert.IsTrue(a.Equals(b));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetAvailableIdsResponse_Different_Self()
    {
        var a = new GetAvailableIdsResponse(["a", "b"], "q", CreateAdditionalInfos("msgA"));
        var b = new GetAvailableIdsResponse(["a"], "q", CreateAdditionalInfos("msgA"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetAvailableIdsResponse_Different_Generic()
    {
        var a = new GetAvailableIdsResponse(["a", "b"], "q", CreateAdditionalInfos("msgA"));
        var b = new GetAvailableIdsResponse(["a", "b"], "q", CreateAdditionalInfos("msgB"));

        Assert.IsFalse(a.Equals(b));
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetAvailableIdsResponse_Same()
    {
        var a = new GetAvailableIdsResponse(["a", "b"], "q", CreateAdditionalInfos("msgA"));
        var b = new GetAvailableIdsResponse(["a", "b"], "q", CreateAdditionalInfos("msgA"));

        Assert.IsTrue(a.Equals(b));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    #endregion

    private AdditionalInfo[] CreateAdditionalInfos(string message) =>
    [
        new AdditionalInfo("MyKind", message,
            [new AdditionalInfoData("key0", "value0"), new AdditionalInfoData("key1", "value1")]
        )
    ];
}