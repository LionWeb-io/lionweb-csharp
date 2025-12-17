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

using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;

[TestClass]
public class FollowingSiblingTests
{
    [TestMethod]
    public void FollowingSibling()
    {
        var circleA = new LinkTestConcept("a");
        var circleB = new LinkTestConcept("b");
        var circleC = new LinkTestConcept("c");
        var circleD = new LinkTestConcept("d");
        var ancestor = new TestPartition("a") { Links = [circleA, circleB, circleC, circleD] };
        CollectionAssert.AreEqual(new List<INode> { circleD }, circleC.FollowingSiblings().ToList());
    }

    [TestMethod]
    public void Self()
    {
        var circleA = new LinkTestConcept("a");
        var circleB = new LinkTestConcept("b");
        var circleC = new LinkTestConcept("c");
        var circleD = new LinkTestConcept("d");
        var ancestor = new TestPartition("a") { Links = [circleA, circleB, circleC, circleD] };
        CollectionAssert.AreEqual(new List<INode> { circleC, circleD }, circleC.FollowingSiblings(true).ToList());
    }

    [TestMethod]
    public void NoParent()
    {
        var circleA = new LinkTestConcept("a");
        Assert.ThrowsExactly<TreeShapeException>(() => circleA.FollowingSiblings());
    }

    [TestMethod]
    public void SingleContainment()
    {
        var coord = new LinkTestConcept("a");
        var circle = new LinkTestConcept("b") { Containment_0_1 = coord };
        Assert.ThrowsExactly<TreeShapeException>(() => coord.FollowingSiblings());
    }

    [TestMethod]
    public void NoFollowingSibling()
    {
        var circleA = new LinkTestConcept("a");
        var circleB = new LinkTestConcept("b");
        var ancestor = new TestPartition("a") { Links = [circleA, circleB] };
        CollectionAssert.AreEqual(new List<INode> { }, circleB.FollowingSiblings().ToList());
    }

    [TestMethod]
    public void NoFollowingSibling_Self()
    {
        var circleA = new LinkTestConcept("a");
        var circleB = new LinkTestConcept("b");
        var ancestor = new TestPartition("a") { Links = [circleA, circleB] };
        CollectionAssert.AreEqual(new List<INode> { circleB }, circleB.FollowingSiblings(true).ToList());
    }
}