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

namespace LionWeb.Core.Test.NodeApi.TreeTraversal;

using Languages.Generated.V2024_1.Shapes.M2;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;

[TestClass]
public class PartitionTests
{
    [TestMethod]
    public void NoParent_Self()
    {
        var node = new Geometry("p");

        Assert.AreSame(node, node.GetPartition());
    }

    [TestMethod]
    public void NoParent_None()
    {
        var node = new Circle("p");

        Assert.IsNull(node.GetPartition());
    }

    [TestMethod]
    public void Parent_Parent()
    {
        var node = new LinkTestConcept("p");
        var partition = new TestPartition("parent") { Links = [node] };

        Assert.AreSame(partition, node.GetPartition());
    }

    [TestMethod]
    public void Parent_None()
    {
        var node = new Coord("c");
        _ = new Circle("p") { Center = node };

        Assert.IsNull(node.GetPartition());
    }

    [TestMethod]
    public void NotSelf_Parent_Parent()
    {
        var node = new Circle("p");
        var partition = new Geometry("partition") { Shapes = [node] };

        Assert.AreSame(partition, node.GetPartition());
    }

    [TestMethod]
    public void Ancestor()
    {
        var node = new LinkTestConcept("p");
        var partition =
            new TestPartition("parent") { Links = [new LinkTestConcept("middle") { Containment_1 = node }] };

        Assert.AreSame(partition, node.GetPartition());
    }

    [TestMethod]
    public void NotRoot()
    {
        var lang = new DynamicLanguage("lang", LionWebVersions.Current)
        {
            Name = "lang", Key = "lang", Version = "lang"
        };
        var iface = lang.Interface("iface", "iface", "iface");
        var cont = iface.Containment("cont", "cont", "cont").IsMultiple(false).IsOptional(true).OfType(iface);
        var root = lang.Concept("root", "root", "root").IsPartition(false).Implementing(iface);
        var partition = lang.Concept("partition", "partition", "partition").IsPartition(true).Implementing(iface);

        var r = lang.GetFactory().CreateNode("rootInstance", root);
        DynamicPartitionInstance p =
            (DynamicPartitionInstance)lang.GetFactory().CreateNode("partitionInstance", partition);
        var c = lang.GetFactory().CreateNode("childPartitionInstance", partition);
        var l = lang.GetFactory().CreateNode("leafInstance", partition);

        r.Set(cont, p);
        p.Set(cont, c);
        c.Set(cont, l);

        Assert.AreSame(p, l.GetPartition());
    }
}