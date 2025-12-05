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

using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class TwowayTests : TwowayReplicatorTestsBase
{
    #region ReferenceAdded

    [TestMethod]
    public void ReferenceAdded_Multiple_Only()
    {
        var bof = new LinkTestConcept("bof");
        var line = new LinkTestConcept("line");
        var node = new TestPartition("a") { Links =  [line] };
        node.AddLinks([bof]);

        var clone = new TestPartition("a") { Links =  [new LinkTestConcept("line")] };
        clone.AddLinks([new LinkTestConcept("bof")]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.AddReference_0_n([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_First()
    {
        var circle = new LinkTestConcept("circle");
        var bof = new LinkTestConcept("bof") { Reference_0_n =  [circle] };
        var line = new LinkTestConcept("line");
        var node = new TestPartition("a") { Links =  [line, circle] };
        node.AddLinks([bof]);

        var cloneCircle = new LinkTestConcept("circle");
        var clone = new TestPartition("a") { Links =  [new LinkTestConcept("line"), cloneCircle] };
        clone.AddLinks([new LinkTestConcept("bof") { Reference_0_n =  [cloneCircle] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.InsertReference_0_n(0, [line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Multiple_Last()
    {
        var circle = new LinkTestConcept("circle");
        var bof = new LinkTestConcept("bof") { Reference_0_n =  [circle] };
        var line = new LinkTestConcept("line");
        var node = new TestPartition("a") { Links =  [line, circle] };
        node.AddLinks([bof]);

        var cloneCircle = new LinkTestConcept("circle");
        var clone = new TestPartition("a") { Links =  [new LinkTestConcept("line"), cloneCircle] };
        clone.AddLinks([new LinkTestConcept("bof") { Reference_0_n =  [cloneCircle] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.InsertReference_0_n(1, [line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceAdded_Single()
    {
        var circle = new LinkTestConcept("circle");
        var od = new LinkTestConcept("od");
        var node = new TestPartition("a") { Links =  [od, circle] };

        var clone = new TestPartition("a") { Links =  [new LinkTestConcept("od"), new LinkTestConcept("circle")] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        od.Reference_0_1 = circle;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ReferenceDeleted

    [TestMethod]
    public void ReferenceDeleted_Multiple_Only()
    {
        var line = new LinkTestConcept("line");
        var bof = new LinkTestConcept("bof") { Reference_0_n =  [line] };
        var node = new TestPartition("a") { Links =  [line] };
        node.AddLinks([bof]);

        var cloneLine = new LinkTestConcept("line");
        var clone = new TestPartition("a") { Links =  [cloneLine] };
        clone.AddLinks([new LinkTestConcept("bof") { Reference_0_n =  [cloneLine] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.RemoveReference_0_n([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_First()
    {
        var circle = new LinkTestConcept("circle");
        var line = new LinkTestConcept("line");
        var bof = new LinkTestConcept("bof") { Reference_0_n =  [line, circle] };
        var node = new TestPartition("a") { Links =  [line, circle] };
        node.AddLinks([bof]);

        var cloneCircle = new LinkTestConcept("circle");
        var cloneLine = new LinkTestConcept("line");
        var clone = new TestPartition("a") { Links =  [cloneLine, cloneCircle] };
        clone.AddLinks([new LinkTestConcept("bof") { Reference_0_n =  [cloneLine, cloneCircle] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.RemoveReference_0_n([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceDeleted_Multiple_Last()
    {
        var circle = new LinkTestConcept("circle");
        var line = new LinkTestConcept("line");
        var bof = new LinkTestConcept("bof") { Reference_0_n =  [circle, line] };
        var node = new TestPartition("a") { Links =  [line, circle] };
        node.AddLinks([bof]);

        var cloneCircle = new LinkTestConcept("circle");
        var cloneLine = new LinkTestConcept("line");
        var clone = new TestPartition("a") { Links =  [cloneLine, cloneCircle] };
        clone.AddLinks([new LinkTestConcept("bof") { Reference_0_n =  [cloneCircle, cloneLine] }]);

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        bof.RemoveReference_0_n([line]);

        AssertEquals([node], [clone]);
    }

    [TestMethod]
    public void ReferenceDeleted_Single()
    {
        var circle = new LinkTestConcept("circle");
        var od = new LinkTestConcept("od") { Reference_0_1 = circle };
        var node = new TestPartition("a") { Links =  [od, circle] };

        var cloneCircle = new LinkTestConcept("circle");
        var clone = new TestPartition("a") { Links =  [new LinkTestConcept("od") { Reference_0_1 = cloneCircle }, cloneCircle] };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        od.Reference_0_1 = null;

        AssertEquals([node], [clone]);
    }

    #endregion

    #region ReferenceChanged

    [TestMethod]
    public void ReferenceChanged_Single()
    {
        var circle = new LinkTestConcept("circle");
        var line = new LinkTestConcept("line");
        var od = new LinkTestConcept("od") { Reference_1 = circle };
        var node = new TestPartition("a") { Links =  [od, circle, line] };

        var cloneCircle = new LinkTestConcept("circle");
        var clone = new TestPartition("a")
        {
            Links =  [new LinkTestConcept("od") { Reference_1 = cloneCircle }, cloneCircle, new LinkTestConcept("line")]
        };

        var (replicator, cloneReplicator) = CreateReplicators(node, clone);


        od.Reference_1 = line;

        AssertEquals([node], [clone]);
    }

    #endregion
}