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
using LeftIndex = int;
using RightIndex = int;

[TestClass]
public class ListComparerTests
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
                new(Remove, 'a', 0),
            ]
        );

    [TestMethod]
    public void RightEmptyTwo() =>
        AssertCompare(
            "ab",
            "",
            [
                new(Remove, 'a', 0),
                new(Remove, 'b', 1),
            ]
        );

    [TestMethod]
    public void MoveOneFarRight() =>
        AssertCompare(
            "aBcdefghijkl",
            "acdefghiBjkl",
            [
                new(Move, 'B', 1, 8),
            ]
        );

    [TestMethod]
    public void MoveOneFarLeft() =>
        AssertCompare(
            "acdefghiBjkl",
            "aBcdefghijkl",
            [
                new(Move, 'B', 8, 1),
            ]
        );

    [TestMethod]
    public void MoveTwoFarRight() =>
        AssertCompare(
            "aBCdefghijkl",
            "adefghiBCjkl",
            [
                new(Move, 'B', 1, 7),
                new(Move, 'C', 2, 8),
            ]
        );

    [TestMethod]
    public void MoveTwoFarLeft() =>
        AssertCompare(
            "adefghiBCjkl",
            "aBCdefghijkl",
            [
                new(Move, 'B', 7, 1),
                new(Move, 'C', 8, 2),
            ]
        );

    [TestMethod]
    public void MoveTwoSeparatedFarRight() =>
        AssertCompare(
            "aBcDefghijkl",
            "acefghiBDjkl",
            [
                new(Move, 'B', 1, 7),
                new(Move, 'D', 3, 8),
            ]
        );

    [TestMethod]
    public void MoveTwoSeparatedFarLeft() =>
        AssertCompare(
            "acefghiBDjkl",
            "aBcDefghijkl",
            [
                new(Move, 'B', 7, 1),
                new(Move, 'D', 8, 3),
            ]
        );

    [TestMethod]
    public void MoveOneFarRightRemoveAfter() =>
        AssertCompare(
            "aBcdefghijKl",
            "acdefghiBjl",
            [
                new(Move, 'B', 1, 8),
                new(Remove, 'K', 10)
            ]
        );

    [TestMethod]
    public void MoveOneFarLeftRemoveAfter() =>
        AssertCompare(
            "acdEfghiBjkl",
            "aBcdfghijkl",
            [
                new(Move, 'B', 8, 1),
                new (Remove, 'E', 3)
            ]
        );

    [TestMethod]
    public void SwapTwo() =>
        AssertCompare(
            "ab",
            "ba",
            [
                new(Move, 'a', 0, 1)
            ]
        );

    [TestMethod]
    public void SwapTwoFar() =>
        AssertCompare(
            "abCdefgHijkl",
            "abHdefgCijkl",
            [
                new(Move, 'H', 7, 2),
                new(Move, 'C', 2, 7)
            ]
        );

    [TestMethod]
    public void Hirschberg()
    {
        const string left = "abcdef";
        const string right = "bcda";
        var hirschberg = new Hirschberg(left, right, new int[left.Length + 1, right.Length + 1]);
        hirschberg.run();
        Console.WriteLine(string.Join("\n", hirschberg.actions));
    }

    [TestMethod]
    public void Hirschberg2() =>
        AssertCompare(
            "abcdef",
            "bcda",
            [
                new(Move, 'a', 0, 3),
                new(Remove, 'e', 4),
                new(Remove, 'f', 5),
            ]
        );

    [TestMethod]
    public void LongListMove() =>
        AssertCompare(
            "a" + new string('x', 2000),
            new string('x', 2000) + "a",
            [
                new(Move, 'a', 0, 2000)
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
        var changes = comparer.Run();
        CollectionAssert.AreEquivalent(
            new List<ListComparer<INode>.ListChange> { new ListComparer<INode>.ListMoved(a, 0, a, 1) },
            changes
        );
    }

    [TestMethod]
    public void SwapTwoNodes_SameId()
    {
        var a = new Line("x");
        var b = new Line("x");

        var comparer = new ListComparer<INode>([a, b], [b, a]);
        var changes = comparer.Run();
        CollectionAssert.AreEquivalent(
            new List<ListComparer<INode>.ListChange> { new ListComparer<INode>.ListMoved(a, 0, a, 1) },
            changes
        );
    }

    private class NodeIdComparer : IEqualityComparer<INode>
    {
        public bool Equals(INode? x, INode? y) =>
            x.GetId() == y.GetId();

        public int GetHashCode(INode obj) =>
            obj.GetId().GetHashCode();
    }

    [TestMethod]
    public void SwapTwoNodes_SameId_CustomComparer()
    {
        var a = new Line("x");
        var b = new Line("x");


        var comparer = new ListComparer<INode>([a, b], [b, a], new NodeIdComparer());
        var changes = comparer.Run();
        CollectionAssert.AreEquivalent(
            new List<ListComparer<INode>.ListChange> { },
            changes
        );
    }

    private const int Add = 0;
    private const int Remove = 1;
    private const int Move = 2;

    private void AssertCompare(string left, string right, EasyToEnterResult[] results)
    {
        var comparer = new ListComparer<char>(left.ToList(), right.ToList());
        var changes = comparer.Run();
        CollectionAssert.AreEquivalent(
            results.Select(r => (ListComparer<char>.ListChange)(r.changeKind switch
            {
                Add => new ListComparer<char>.ListAdded(r.left, r.leftIndex),
                Remove => new ListComparer<char>.ListRemoved(r.left, r.leftIndex),
                Move => new ListComparer<char>.ListMoved(r.left, r.leftIndex, (char)r.left, (RightIndex)r.rightIndex),
                _ => throw new InvalidOperationException()
            })).ToList(),
            changes
        );
    }

    private record struct EasyToEnterResult(
        int changeKind,
        char left,
        LeftIndex leftIndex,
        RightIndex? rightIndex = null);
}