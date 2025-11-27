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

namespace LionWeb.Core.Test.Notification.Replicator.Property;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class TwowayTests : TwowayReplicatorTestsBase 
{
    [TestMethod]
    public void PropertyAdded()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };

        var cloneCircle = new Circle("c");
        var clone = new Geometry("a") { Shapes = [cloneCircle] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);

        circle.Name = "Hello";
        cloneCircle.Name = "World";

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var docs = new Documentation("c") { Text = "Hello" };
        var node = new Geometry("a") { Documentation = docs };

        var cloneDocs = new Documentation("c") { Text = "Hello" };
        var clone = new Geometry("a") { Documentation = cloneDocs };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);

        docs.Text = "Bye";
        cloneDocs.Text = null;

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var docs = new Documentation("c") { Text = "Hello" };
        var node = new Geometry("a") { Documentation = docs };

        var cloneDocs = new Documentation("c") { Text = "Hello" };
        var clone = new Geometry("a") { Documentation = cloneDocs };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);

        docs.Text = null;
        cloneDocs.Text = "Bye";

        AssertEquals([node], [clone]);
    }
}