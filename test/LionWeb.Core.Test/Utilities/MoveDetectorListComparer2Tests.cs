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
public class MoveDetectorListComparer2Tests : ListComparerTestsBase
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
    public void DeleteEveryOther() =>
        AssertCompare(
            "aBcDefGhijKl",
            "acefhijl",
            [
                new(Delete, 'B', 1),
                new(Delete, 'D', 2),
                new(Delete, 'G', 4),
                new(Delete, 'K', 7),
            ]
        );

    [TestMethod]
    public void AddEveryOther() =>
        AssertCompare(
            "acefhijl",
            "aBcDefGhijKl",
            [
                new(Add, 'B', 1),
                new(Add, 'D', 3),
                new(Add, 'G', 6),
                new(Add, 'K', 10),
            ]
        );

    [TestMethod]
    public void AddDeleteEveryOther() =>
        AssertCompare(
            "aBceFgi",
            "acDegHi",
            [
                new(Delete, 'B', 1),
                new(Move, 'e', 2, 3),
                new(Delete, 'F', 2),
                new(Add, 'D', 2),
                new(Add, 'H', 5),
            ]
        );

    [TestMethod]
    public void DeleteAddAdd() =>
        AssertCompare(
            "aBcegi",
            "acDegHi",
            [
                new(Delete, 'B', 1),
                new(Add, 'D', 2),
                new(Add, 'H', 5),
            ]
        );

    [TestMethod]
    public void AddDeleteAdd() =>
        AssertCompare(
            "abdeFghjk",
            "abCdeghIjk",
            [
                new(Delete, 'F', 4),
                new(Add, 'C', 2),
                new(Add, 'I', 7),
            ]
        );

    [TestMethod]
    public void DeleteAddDelete() =>
        AssertCompare(
            "aBceFgi",
            "acDegi",
            [
                new(Delete, 'B', 1),
                new(Move, 'e', 2, 3),
                new(Delete, 'F', 2),
                new(Add, 'D', 2),
            ]
        );

    [TestMethod]
    public void AddDelete() =>
        AssertCompare(
            "abCdegh",
            "abdeFgh",
            [
                new(Delete, 'C', 2),
                new(Add, 'F', 4),
            ]
        );

    [TestMethod]
    public void AddMultipleThenMoveRight() =>
        AssertCompare(
            "acefGi",
            "aBcDeGfHi",
            [
                new(Move, 'G', 4, 3),
                new(Add, 'B', 1),
                new(Add, 'D', 3),
                new(Add, 'H', 7),
            ]
        );

    [TestMethod]
    public void AddMultipleThenMoveRightJ() =>
        AssertCompare(
            "acefGi",
            "aBcDeJfHi",
            [
                new(Delete, 'G', 4),
                new(Add, 'B', 1),
                new(Add, 'D', 3),
                new(Add, 'J', 5),
                new(Add, 'H', 7),
            ]
        );

    [TestMethod]
    public void MoveEveryOtherForward() =>
        AssertCompare(
            "aBcDe",
            "acBeD",
            [
                new(Move, 'B', 1, 3),
                new(Move, 'D', 2, 4),
            ]
        );

    [TestMethod]
    public void MoveEveryOtherSwap() =>
        AssertCompare(
            "abcdXefgYhij",
            "abcdYefgXhij",
            [
                new(Move, 'X', 4, 8),
                new(Move, 'Y', 7, 4),
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
                new(Move, 'B', 1, 8),
                new(Move, 'C', 1, 8),
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
                new(Move, 'B', 1, 8),
                new(Move, 'D', 2, 8),
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
                new(Move, 'B', 1, 10),
                new(Move, 'j', 8, 10),
                new(Delete, 'K', 8),
            ]
        );

    [TestMethod]
    public void MoveOneFarLeftDeleteAfter() =>
        AssertCompare(
            "acdEfghiBjkl",
            "aBcdfghijkl",
            [
                new(Delete, 'E', 3),
                new(Move, 'B', 8, 1),
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
                new(Move, 'C', 2, 7),
                new(Move, 'H', 6, 2),
            ]
        );

    [TestMethod]
    public void Hirschberg() =>
        AssertCompare(
            "AbcdEF",
            "bcdA",
            [
                new(Move, 'A', 0, 5),
                new(Delete, 'E', 3),
                new(Delete, 'F', 3),
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
            "abcDefgHij",
            "abceKfgiDj",
            [
                new(Move, 'D', 3, 8),
                new(Delete, 'H', 6),
                new(Add, 'K', 4),
            ]
        );

    [TestMethod]
    public void MoveDeleteDelete() =>
        AssertCompare(
            "abcDeKfgHij",
            "abcefgiDj",
            [
                new(Move, 'D', 3, 9),
                new(Delete, 'K', 4),
                new(Delete, 'H', 6),
            ]
        );

    [TestMethod]
    [Ignore]
    public void Replaced() =>
        AssertCompare(
            "aBc",
            "aDc",
            [
                new(Replace, 'B', 1, right: 'D')
            ]
        );
    protected internal override IListComparer<char> CreateComparer(string left, string right)
        => new MoveDetector<char>(new RelativeChangesListComparer<char>(left.ToList(), right.ToList()).Compare());
}