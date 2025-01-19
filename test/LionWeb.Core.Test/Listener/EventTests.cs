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

namespace LionWeb.Core.Test.Listener;

using Core.Utilities;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;
using Comparer = Core.Utilities.Comparer;

[TestClass]
public class EventTests
{
    #region Properties

    #region PropertyAdded
    
    [TestMethod]
    public void PropertyAdded()
    {
        var circle = new Circle("c");
        var node = new Geometry("a") { Shapes = [circle] };
        
        var cloneCircle = new Circle("c");
        var clone = new Geometry("a") { Shapes = [cloneCircle] };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        circle.Name = "Hello";
        
        AssertEquals([node], [clone]);
    }

    #endregion
    
    #endregion

    #region Children

    #region ChildAdded

    [TestMethod]
    public void ChildAdded_Multiple_Only()
    {
        var node = new Geometry("a");
       
        var clone = new Geometry("a");

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Circle("added");
        node.AddShapes([added]);
        
        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_First()
    {
        var node = new Geometry("a")
        {
            Shapes = [
                new Line("l")
            ]
        };
        
        var clone = new Geometry("a")
        {
            Shapes = [
                new Line("l")
            ]
        };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Circle("added");
        node.InsertShapes(0, [added]);
        
        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
    }

    [TestMethod]
    public void ChildAdded_Multiple_Last()
    {
        var node = new Geometry("a")
        {
            Shapes = [
                new Line("l")
            ]
        };
        
        var clone = new Geometry("a")
        {
            Shapes = [
                new Line("l")
            ]
        };

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Circle("added");
        node.InsertShapes(1, [added]);
        
        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[1]);
    }

    [TestMethod]
    public void ChildAdded_Single()
    {
        var node = new Geometry("a");
        
        var clone = new Geometry("a");

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Documentation("added");
        node.Documentation = added;
        
        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Documentation);
    }

    [TestMethod]
    public void ChildAdded_Deep()
    {
        var node = new Geometry("a");
        
        var clone = new Geometry("a");

        var applier = new PartitionEventApplier(clone);
        applier.Subscribe(node.Listener);

        var added = new Circle("added")
        {
            Center = new Coord("coord")
            {
                X = 1,
                Y = 2,
                Z = 3
            }
        };
        node.AddShapes([added]);
        
        AssertEquals([node], [clone]);
        Assert.AreNotSame(added, clone.Shapes[0]);
    }

    #endregion

    #endregion

    private void AssertEquals(IEnumerable<INode?> expected, IEnumerable<INode?> actual)
    {
        List<IDifference> differences = new Comparer(expected.ToList(), actual.ToList()).Compare().ToList();
        Assert.IsFalse(differences.Count != 0, differences.DescribeAll(new()));
    }
    
}