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

namespace LionWeb.Core.Test.Notification;

using Core.Utilities;
using Languages.Generated.V2025_1.Shapes.M2;

[TestClass]
public class IdComparerTests : NotificationTestsBase
{
    [TestMethod]
    public void TestNodeIdComparer_Fails()
    {
        var circle1 = new Circle("c");
        var circle2 = new Circle("d");

        List<IDifference> differences = new IdComparer(new List<INode?> { circle1 }, new List<INode?> { circle2 }).Compare().ToList();
        Assert.HasCount(1, differences, differences.DescribeAll(new()));
    }

    [TestMethod]
    public void TestNodeIdComparer_Succeeds()
    {
        var circle1 = new Circle("a");
        var circle2 = new Circle("a");

        List<IDifference> differences = new IdComparer(new List<INode?> { circle1 }, new List<INode?> { circle2 }).Compare().ToList();
        Assert.HasCount(0, differences, differences.DescribeAll(new()));
    }
}