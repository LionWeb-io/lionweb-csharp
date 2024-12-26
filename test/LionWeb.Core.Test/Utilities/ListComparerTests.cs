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
    public void SwappedTwo() =>
        AssertCompare(
            "ab",
            "ba",
            [
                new(Move, 'a', 0, 1)
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
                new(Move, 'a', 0, 5),
                new(Remove, 'e', 4)
            ]
        );

    private const int Add = 0;
    private const int Remove = 1;
    private const int Move = 2;

    private void AssertCompare(string left, string right, R[] results)
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
}

internal record struct R(
    int changeKind,
    char left,
    LeftIndex leftIndex,
    RightIndex? rightIndex = null);