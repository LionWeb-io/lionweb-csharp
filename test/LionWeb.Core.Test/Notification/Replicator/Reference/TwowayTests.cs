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

namespace LionWeb.Core.Test.Notification.Replicator.Reference;

using Languages.Generated.V2024_1.Shapes.M2;

[TestClass]
public class TwowayTests : TwowayReplicatorTestsBase
{
    #region ReferenceAdded

    [TestMethod]
    public void ReferenceAdded_Multiple_Only()
    {
        var bof = new BillOfMaterials("bof");
        var line = new Line("line");
        var node = new Geometry("a") { Shapes = [line] };
        node.AddAnnotations([bof]);

        var clone = new Geometry("a") { Shapes = [new Line("line")] };
        clone.AddAnnotations([new BillOfMaterials("bof")]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.AddMaterials([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_First()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var node = new Geometry("a") { Shapes = [line, circle] };
        node.AddAnnotations([bof]);

        var cloneCircle = new Circle("circle");
        var clone = new Geometry("a") { Shapes = [new Line("line"), cloneCircle] };
        clone.AddAnnotations([new BillOfMaterials("bof") { Materials = [cloneCircle] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.InsertMaterials(0, [line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_Last()
    {
        var circle = new Circle("circle");
        var bof = new BillOfMaterials("bof") { Materials = [circle] };
        var line = new Line("line");
        var node = new Geometry("a") { Shapes = [line, circle] };
        node.AddAnnotations([bof]);

        var cloneCircle = new Circle("circle");
        var clone = new Geometry("a") { Shapes = [new Line("line"), cloneCircle] };
        clone.AddAnnotations([new BillOfMaterials("bof") { Materials = [cloneCircle] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.InsertMaterials(1, [line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od");
        var node = new Geometry("a") { Shapes = [od, circle] };

        var clone = new Geometry("a") { Shapes = [new OffsetDuplicate("od"), new Circle("circle")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        od.Source = circle;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ReferenceDeleted

    [TestMethod]
    public void ReferenceDeleted_Multiple_Only()
    {
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [line] };
        var node = new Geometry("a") { Shapes = [line] };
        node.AddAnnotations([bof]);

        var cloneLine = new Line("line");
        var clone = new Geometry("a") { Shapes = [cloneLine] };
        clone.AddAnnotations([new BillOfMaterials("bof") { Materials = [cloneLine] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.RemoveMaterials([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_First()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [line, circle] };
        var node = new Geometry("a") { Shapes = [line, circle] };
        node.AddAnnotations([bof]);

        var cloneCircle = new Circle("circle");
        var cloneLine = new Line("line");
        var clone = new Geometry("a") { Shapes = [cloneLine, cloneCircle] };
        clone.AddAnnotations([new BillOfMaterials("bof") { Materials = [cloneLine, cloneCircle] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.RemoveMaterials([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_Last()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var bof = new BillOfMaterials("bof") { Materials = [circle, line] };
        var node = new Geometry("a") { Shapes = [line, circle] };
        node.AddAnnotations([bof]);

        var cloneCircle = new Circle("circle");
        var cloneLine = new Line("line");
        var clone = new Geometry("a") { Shapes = [cloneLine, cloneCircle] };
        clone.AddAnnotations([new BillOfMaterials("bof") { Materials = [cloneCircle, cloneLine] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.RemoveMaterials([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceDeleted_Single()
    {
        var circle = new Circle("circle");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var node = new Geometry("a") { Shapes = [od, circle] };

        var cloneCircle = new Circle("circle");
        var clone = new Geometry("a") { Shapes = [new OffsetDuplicate("od") { AltSource = cloneCircle }, cloneCircle] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        od.AltSource = null;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ReferenceChanged

    [TestMethod]
    public void ReferenceChanged_Single()
    {
        var circle = new Circle("circle");
        var line = new Line("line");
        var od = new OffsetDuplicate("od") { AltSource = circle };
        var node = new Geometry("a") { Shapes = [od, circle, line] };

        var cloneCircle = new Circle("circle");
        var clone = new Geometry("a")
        {
            Shapes = [new OffsetDuplicate("od") { AltSource = cloneCircle }, cloneCircle, new Line("line")]
        };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        od.AltSource = line;

        AssertEquals([node], [clone]);
    }

    #endregion
}