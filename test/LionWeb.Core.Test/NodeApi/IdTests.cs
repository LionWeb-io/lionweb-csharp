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

namespace LionWeb.Core.Test.NodeApi;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class IdTests
{
    [TestMethod]
    public void SetId_Constructor()
    {
        var line = new Line("myId");
        Assert.AreEqual("myId", line.GetId());
    }

    [TestMethod]
    public void SetId_Dynamic()
    {
        var line = new DynamicNode("myId", ShapesLanguage.Instance.Line);
        Assert.AreEqual("myId", line.GetId());
    }

    [TestMethod]
    public void SetId_Factory()
    {
        var line = ShapesLanguage.Instance.GetFactory().NewLine("myId");
        Assert.AreEqual("myId", line.GetId());
    }

    [TestMethod]
    public void SetId_Factory_Reflective()
    {
        var line = ShapesLanguage.Instance.GetFactory().CreateNode("myId", ShapesLanguage.Instance.Line);
        Assert.AreEqual("myId", line.GetId());
    }

    [TestMethod]
    public void SetId_Constructor_null()
    {
        Assert.ThrowsException<InvalidIdException>(() => new Line(null));
    }

    [TestMethod]
    public void SetId_Dynamic_null()
    {
        Assert.ThrowsException<InvalidIdException>(() => new DynamicNode(null, ShapesLanguage.Instance.Line));
    }

    [TestMethod]
    public void SetId_Factory_null()
    {
        Assert.ThrowsException<InvalidIdException>(() => ShapesLanguage.Instance.GetFactory().NewLine(null));
    }

    [TestMethod]
    public void SetId_Factory_Reflective_null()
    {
        Assert.ThrowsException<InvalidIdException>(() =>
            ShapesLanguage.Instance.GetFactory().CreateNode(null, ShapesLanguage.Instance.Line));
    }

    [TestMethod]
    public void SetId_Constructor_invalid_dot()
    {
        Assert.ThrowsException<InvalidIdException>(() => new Line("my.id")
        {
            End = null
        });
    }

    [TestMethod]
    public void SetId_Factory_invalid_dot()
    {
        Assert.ThrowsException<InvalidIdException>(() => ShapesLanguage.Instance.GetFactory().NewLine("my.id"));
    }

    [TestMethod]
    public void SetId_Factory_Reflective_invalid_dot()
    {
        Assert.ThrowsException<InvalidIdException>(() =>
            ShapesLanguage.Instance.GetFactory().CreateNode("my.id", ShapesLanguage.Instance.Line));
    }

    [TestMethod]
    public void SetId_Constructor_invalid_equal()
    {
        Assert.ThrowsException<InvalidIdException>(() => new Line("myid="));
    }

    [TestMethod]
    public void SetId_Factory_invalid_equal()
    {
        Assert.ThrowsException<InvalidIdException>(() => ShapesLanguage.Instance.GetFactory().NewLine("myid="));
    }

    [TestMethod]
    public void SetId_Factory_Reflective_invalid_equal()
    {
        Assert.ThrowsException<InvalidIdException>(() =>
            ShapesLanguage.Instance.GetFactory().CreateNode("myid=", ShapesLanguage.Instance.Line));
    }

    [TestMethod]
    public void GetId()
    {
        var line = new Line("myId");
        Assert.AreEqual("myId", line.GetId());
        Assert.AreEqual("myId", (line as IReadableNode).GetId());
        Assert.AreEqual("myId", (line as IWritableNode).GetId());
    }
}