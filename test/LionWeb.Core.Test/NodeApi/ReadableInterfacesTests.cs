// Copyright 2025 TRUMPF Laser SE and other contributors
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
// SPDX-FileCopyrightText: 2025 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Core.Test.NodeApi;

using Languages.Generated.V2024_1.NamedLang;
using M2;

[TestClass]
public class ReadableInterfacesTests
{
    [TestMethod]
    public void WritableInterface()
    {
        Iface node = new IfaceConcept("i");
        Assert.IsInstanceOfType<IReadableNode>(node);
        Assert.IsInstanceOfType<IWritableNode>(node);
        Assert.IsInstanceOfType<INode>(node);
        Assert.IsNotInstanceOfType<INamed>(node);
        Assert.IsNotInstanceOfType<INamedWritable>(node);

        var iface = typeof(Iface);
        Assert.IsTrue(iface.IsAssignableTo(typeof(IReadableNode)));
        Assert.IsTrue(iface.IsAssignableTo(typeof(IWritableNode)));
        Assert.IsTrue(iface.IsAssignableTo(typeof(INode)));
        Assert.IsFalse(iface.IsAssignableTo(typeof(INamed)));
        Assert.IsFalse(iface.IsAssignableTo(typeof(INamedWritable)));
    }
    
    [TestMethod]
    public void WritableNamedInterface()
    {
        NamedIface node = new NamedIfaceConcept("i");
        Assert.IsInstanceOfType<INode>(node);
        Assert.IsInstanceOfType<IReadableNode>(node);
        Assert.IsInstanceOfType<IWritableNode>(node);
        Assert.IsInstanceOfType<INamed>(node);
        Assert.IsInstanceOfType<INamedWritable>(node);
        
        node.SetName("hi");
        Assert.AreEqual("hi", node.Name);

        var iface = typeof(NamedIface);
        Assert.IsTrue(iface.IsAssignableTo(typeof(IReadableNode)));
        Assert.IsTrue(iface.IsAssignableTo(typeof(IWritableNode)));
        Assert.IsTrue(iface.IsAssignableTo(typeof(INode)));
        Assert.IsTrue(iface.IsAssignableTo(typeof(INamed)));
        Assert.IsTrue(iface.IsAssignableTo(typeof(INamedWritable)));
    }
    
    [TestMethod]
    public void ReadableInterface()
    {
        Languages.Generated.V2024_1.NamedLangReadInterfaces.Iface node = new Languages.Generated.V2024_1.NamedLangReadInterfaces.IfaceConcept("i");
        Assert.IsInstanceOfType<IReadableNode>(node);
        Assert.IsInstanceOfType<INode>(node);
        Assert.IsInstanceOfType<IWritableNode>(node);
        Assert.IsNotInstanceOfType<INamed>(node);
        Assert.IsNotInstanceOfType<INamedWritable>(node);
        
        var iface = typeof(Languages.Generated.V2024_1.NamedLangReadInterfaces.Iface);
        Assert.IsTrue(iface.IsAssignableTo(typeof(IReadableNode)));
        Assert.IsFalse(iface.IsAssignableTo(typeof(IWritableNode)));
        Assert.IsFalse(iface.IsAssignableTo(typeof(INode)));
        Assert.IsFalse(iface.IsAssignableTo(typeof(INamed)));
        Assert.IsFalse(iface.IsAssignableTo(typeof(INamedWritable)));
    }
    
    [TestMethod]
    public void ReadableNamedInterface()
    {
        Languages.Generated.V2024_1.NamedLangReadInterfaces.NamedIface node = new Languages.Generated.V2024_1.NamedLangReadInterfaces.NamedIfaceConcept("i");
        Assert.IsInstanceOfType<IReadableNode>(node);
        Assert.IsInstanceOfType<INode>(node);
        Assert.IsInstanceOfType<IWritableNode>(node);
        Assert.IsInstanceOfType<INamed>(node);
        Assert.IsInstanceOfType<INamedWritable>(node);

        ((Languages.Generated.V2024_1.NamedLangReadInterfaces.NamedIfaceConcept)node).SetName("hi");
        Assert.AreEqual("hi", node.Name);
        
        var iface = typeof(Languages.Generated.V2024_1.NamedLangReadInterfaces.NamedIface);
        Assert.IsTrue(iface.IsAssignableTo(typeof(IReadableNode)));
        Assert.IsFalse(iface.IsAssignableTo(typeof(IWritableNode)));
        Assert.IsFalse(iface.IsAssignableTo(typeof(INode)));
        Assert.IsTrue(iface.IsAssignableTo(typeof(INamed)));
        Assert.IsFalse(iface.IsAssignableTo(typeof(INamedWritable)));
    }
}