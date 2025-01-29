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
using Languages.Generated.V2024_1.SDTLang;

[TestClass]
public class RelativeChangesListComparerTests : ListComparerTestsBase
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
            "a"
        );

    [TestMethod]
    public void LeftEmptyTwo() =>
        AssertCompare(
            "",
            "ab"
        );

    [TestMethod]
    public void DeleteEveryOther() =>
        AssertCompare(
            "aBcDefGhijKl",
            "acefhijl"
        );

    [TestMethod]
    public void AddEveryOther() =>
        AssertCompare(
            "acefhijl",
            "aBcDefGhijKl"
        );

    [TestMethod]
    public void AddDeleteEveryOther() =>
        AssertCompare(
            "aBceFgi",
            "acDegHi"
        );

    [TestMethod]
    public void DeleteAddAdd() =>
        AssertCompare(
            "aBcegi",
            "acDegHi"
        );

    [TestMethod]
    public void AddDeleteAdd() =>
        AssertCompare(
            "abdeFghjk",
            "abCdeghIjk"
        );

    [TestMethod]
    public void DeleteAddDelete() =>
        AssertCompare(
            "aBceFgi",
            "acDegi"
        );

    [TestMethod]
    public void AddDelete() =>
        AssertCompare(
            "abCdegh",
            "abdeFgh"
        );

    [TestMethod]
    public void AddMultipleThenMoveRight() =>
        AssertCompare(
            "acefGi",
            "aBcDeGfHi"
        );

    [TestMethod]
    public void AddMultipleThenMoveRightJ() =>
        AssertCompare(
            "acefGi",
            "aBcDeJfHi"
        );

    [TestMethod]
    public void MoveEveryOtherForward() =>
        AssertCompare(
            "aBcDe",
            "acBeD"
        );

    [TestMethod]
    public void MoveEveryOtherSwap() =>
        AssertCompare(
            "abcdXefgYhij",
            "abcdYefgXhij"
        );

    [TestMethod]
    public void RightEmptyOne() =>
        AssertCompare(
            "a",
            ""
        );

    [TestMethod]
    public void RightEmptyTwo() =>
        AssertCompare(
            "ab",
            ""
        );

    [TestMethod]
    public void MoveOneFarRight() =>
        AssertCompare(
            "aBcdefghijkl",
            "acdefghiBjkl"
        );

    [TestMethod]
    public void MoveOneFarLeft() =>
        AssertCompare(
            "acdefghiBjkl",
            "aBcdefghijkl"
        );

    [TestMethod]
    public void MoveTwoFarRight() =>
        AssertCompare(
            "aBCdefghijkl",
            "adefghiBCjkl"
        );

    [TestMethod]
    public void MoveTwoFarLeft() =>
        AssertCompare(
            "adefghiBCjkl",
            "aBCdefghijkl"
        );

    [TestMethod]
    public void MoveTwoSeparatedFarRight() =>
        AssertCompare(
            "aBcDefghijkl",
            "acefghiBDjkl"
        );

    [TestMethod]
    public void MoveTwoSeparatedFarLeft() =>
        AssertCompare(
            "acefghiBDjkl",
            "aBcDefghijkl"
        );

    [TestMethod]
    public void MoveOneFarRightDeleteAfter() =>
        AssertCompare(
            "aBcdefghijKl",
            "acdefghiBjl"
        );

    [TestMethod]
    public void MoveOneFarLeftDeleteAfter() =>
        AssertCompare(
            "acdEfghiBjkl",
            "aBcdfghijkl"
        );

    [TestMethod]
    public void SwapTwo() =>
        AssertCompare(
            "ab",
            "ba"
        );

    [TestMethod]
    public void SwapTwoFar() =>
        AssertCompare(
            "abCdefgHijkl",
            "abHdefgCijkl"
        );

    [TestMethod]
    public void Hirschberg() =>
        AssertCompare(
            "AbcdEF",
            "bcdA"
        );

    [TestMethod]
    public void LongListMove() =>
        AssertCompare(
            "a" + new string('x', 2000),
            new string('x', 2000) + "a",
            [
                new(Delete, 'a', 0),
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
    public void AddDeleteMove() =>
        AssertCompare(
            "abcDefgHij",
            "abceKfgiDj"
        );

    [TestMethod]
    public void MoveDeleteDelete() =>
        AssertCompare(
            "abcDeKfgHij",
            "abcefgiDj"
        );

    [TestMethod]
    [Ignore]
    public void Replaced() =>
        AssertCompare(
            "aBc",
            "aDc"
        );
    protected internal override IListComparer<char> CreateComparer(string left, string right)
        => new RelativeChangesListComparer<char>(left.ToList(), right.ToList());
}