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

[TestClass]
public class StepwiseListComparerTests : ListComparerTestsBase
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
                new(Delete, 'b', 0),
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
    public void MoveOneFarRightDeleteAfter() =>
        AssertCompare(
            "aBcdefghijKl",
            "acdefghiBjl",
            [
                new(Move, 'B', 1, 8),
                new(Delete, 'K', 10)
            ]
        );

    [TestMethod]
    public void MoveOneFarLeftDeleteAfter() =>
        AssertCompare(
            "acdEfghiBjkl",
            "aBcdfghijkl",
            [
                new(Move, 'B', 8, 1),
                new(Delete, 'E', 3)
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
                new(Move, 'C', 3, 7)
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
            "AbcdEF",
            "bcdA",
            [
                new(Move, 'A', 0, 3),
                new(Delete, 'E', 4),
                new(Delete, 'F', 4),
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
    public void AddDeleteMove() =>
        AssertCompare(
            "aBcD",
            "aEcB",
            [
                new(Add, 'E', 1),
                new(Move, 'B', 2, 3),
                new(Delete, 'D', 4),
            ]
        );

    protected internal override IListComparer<char> CreateComparer(string left, string right)
        => new StepwiseListComparer<char>(left.ToList(), right.ToList());
}