// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Utilities;

using Core.Utilities;
using Languages.Generated.V2023_1.Shapes.M2;

[TestClass]
public class ListComparerTests : AbsoluteIndexListComparerTestsBase
{
    [TestMethod]
    public void Empty() =>
        AssertCompare(
            "",
            "",
            []
        );

    [TestMethod]
    public void LeftEmptyOne() =>
        AssertCompare(
            "",
            "a",
            [
                new(Add, 'a', 0),
            ]
        );

    [TestMethod]
    public void LeftEmptyTwo() =>
        AssertCompare(
            "",
            "ab",
            [
                new(Add, 'a', 0),
                new(Add, 'b', 1),
            ]
        );

    [TestMethod]
    public void RightEmptyOne() =>
        AssertCompare(
            "a",
            "",
            [
                new(Delete, 'a', 0),
            ]
        );

    [TestMethod]
    public void RightEmptyTwo() =>
        AssertCompare(
            "ab",
            "",
            [
                new(Delete, 'a', 0),
                new(Delete, 'b', 1),
            ]
        );

    [TestMethod]
    public void MoveOneFarRight() =>
        AssertCompare(
            "aBcdefghijkl",
            "acdefghiBjkl",
            [
                new(Delete, 'B', 1),
                new(Add, 'B', 8),
            ]
        );

    [TestMethod]
    public void MoveOneFarLeft() =>
        AssertCompare(
            "acdefghiBjkl",
            "aBcdefghijkl",
            [
                new(Delete, 'B', 8),
                new(Add, 'B', 1),
            ]
        );

    [TestMethod]
    public void MoveTwoFarRight() =>
        AssertCompare(
            "aBCdefghijkl",
            "adefghiBCjkl",
            [
                new(Delete, 'B', 1),
                new(Delete, 'C', 2),
                new(Add, 'B', 7),
                new(Add, 'C', 8),
            ]
        );

    [TestMethod]
    public void MoveTwoFarLeft() =>
        AssertCompare(
            "adefghiBCjkl",
            "aBCdefghijkl",
            [
                new(Delete, 'B', 7),
                new(Delete, 'C', 8),
                new(Add, 'B', 1),
                new(Add, 'C', 2),
            ]
        );

    [TestMethod]
    public void MoveTwoSeparatedFarRight() =>
        AssertCompare(
            "aBcDefghijkl",
            "acefghiBDjkl",
            [
                new(Delete, 'B', 1),
                new(Delete, 'D', 3),
                new(Add, 'B', 7),
                new(Add, 'D', 8),
            ]
        );

    [TestMethod]
    public void MoveTwoSeparatedFarLeft() =>
        AssertCompare(
            "acefghiBDjkl",
            "aBcDefghijkl",
            [
                new(Delete, 'B', 7),
                new(Delete, 'D', 8),
                new(Add, 'B', 1),
                new(Add, 'D', 3),
            ]
        );

    [TestMethod]
    public void MoveOneFarRightDeleteAfter() =>
        AssertCompare(
            "aBcdefghijKl",
            "acdefghiBjl",
            [
                new(Delete, 'B', 1),
                new(Delete, 'j', 9),
                new(Delete, 'K', 10),
                new(Add, 'B', 8),
                new(Add, 'j', 9),
            ]
        );

    [TestMethod]
    public void MoveOneFarLeftDeleteAfter() =>
        AssertCompare(
            "acdEfghiBjkl",
            "aBcdfghijkl",
            [
                new(Delete, 'E', 3),
                new(Delete, 'B', 8),
                new(Add, 'B', 1),
            ]
        );

    [TestMethod]
    public void SwapTwo() =>
        AssertCompare(
            "ab",
            "ba",
            [
                new(Delete, 'a', 0),
                new(Add, 'a', 1),
            ]
        );

    [TestMethod]
    public void SwapTwoFar() =>
        AssertCompare(
            "abCdefgHijkl",
            "abHdefgCijkl",
            [
                new(Delete, 'C', 2),
                new(Delete, 'H', 7),
                new(Add, 'H', 2),
                new(Add, 'C', 7),
            ]
        );

    [TestMethod]
    public void Hirschberg() =>
        AssertCompare(
            "abcdef",
            "bcda",
            [
                new(Delete, 'a', 0),
                new(Delete, 'e', 4),
                new(Delete, 'f', 5),
                new(Add, 'a', 3),
            ]
        );

    [TestMethod]
    public void LongListMove() =>
        AssertCompare(
            "a" + new string('x', 2000),
            new string('x', 2000) + "a",
            [
                new(Delete, 'a',0),
                new(Add, 'a', 2000)
            ]
        );

    [TestMethod]
    public void LongListSame() =>
        AssertCompare(
            new string('x', 2000),
            new string('x', 2000),
            []
        );

    [TestMethod]
    public void SwapTwoNodes()
    {
        var a = new Line("a");
        var b = new Line("b");

        var comparer = new ListComparer<INode>([a, b], [b, a]);
        var changes = comparer.Compare();
        CollectionAssert.AreEqual(
            new List<IListComparer<INode>.IChange>
            {
                new IListComparer<INode>.Deleted(a, 0),
                new IListComparer<INode>.Added(a, 1),
            },
            changes
        );
    }

    [TestMethod]
    public void SwapTwoNodes_SameId()
    {
        var a = new Line("x");
        var b = new Line("x");

        var comparer = new ListComparer<INode>([a, b], [b, a]);
        var changes = comparer.Compare();
        CollectionAssert.AreEqual(
            new List<IListComparer<INode>.IChange>
            {
                new IListComparer<INode>.Deleted(a, 0),
                new IListComparer<INode>.Added(a, 1),
            },
            changes
        );
    }

    private class NodeIdComparer : IEqualityComparer<INode>
    {
        public bool Equals(INode? x, INode? y) =>
            x?.GetId() == y?.GetId();

        public int GetHashCode(INode obj) =>
            obj.GetId().GetHashCode();
    }

    [TestMethod]
    public void SwapTwoNodes_SameId_CustomComparer()
    {
        var a = new Line("x");
        var b = new Line("x");


        var comparer = new ListComparer<INode>([a, b], [b, a], new NodeIdComparer());
        var changes = comparer.Compare();
        CollectionAssert.AreEqual(
            new List<IListComparer<INode>.IChange> { },
            changes
        );
    }

    [TestMethod]
    public void AddDeleteMove() =>
        AssertCompare(
            "abcDefgHij",
            "abceKfgiDj",
            [
                new(Delete, 'D', 3),
                new(Delete, 'H', 7),
                new(Add, 'K', 4),
                new(Add, 'D', 8),
            ]
        );

    [TestMethod]
    public void Replaced() =>
        AssertCompare(
            "aBc",
            "aDc",
            [
                new(Delete, 'B', 1),
                new(Add, 'D', 1),
            ]
        );

    protected internal override IListComparer<char> CreateComparer(string left, string right)
        => new ListComparer<char>(left.ToList(), right.ToList());
}

public abstract class AbsoluteIndexListComparerTestsBase : ListComparerTestsBase
{
    protected override List<IListComparer<char>.IChange> AssertCompare(string left, string right)
    {
        var comparer = CreateComparer(left, right);
        var changes = comparer.Compare();

        Console.WriteLine("originalChanges: \n" + string.Join("\n", changes));

        List<string> steps = [left, IndexString(left)];
        var previous = left;

        int deleteDelta = 0;
        foreach (var deleted in changes.OfType<IListComparer<char>.Deleted>())
        {
            var line = previous.Remove(deleted.Index - deleteDelta, 1);
            deleteDelta++;

            previous = line;

            var index = IndexString(line);
            steps.Add("   " + deleted + "\n" + line + "\n" + index);
        }

        foreach (var added in changes.OfType<IListComparer<char>.Added>().OrderBy(a => a.Index))
        {
            var line = added.Index <= previous.Length 
                    ? previous.Insert(added.Index, added.Element.ToString())
                    : previous + added.Element.ToString();

            previous = line;

            var index = IndexString(line);
            steps.Add("   " + added + "\n" + line + "\n" + index);
        }

        steps.Add(right);

        TestContext.WriteLine(string.Join("\n", steps));

        Assert.AreEqual(right, previous);

        return changes;
    }
}
