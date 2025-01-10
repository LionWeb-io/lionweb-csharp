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

namespace LionWeb.Core.Test.Utilities;

using Core.Utilities;

[TestClass]
public class ManualInputStepwiseListComparerTests : ListComparerTestsBase
{
    private List<IListComparer<char>.IChange> _inputChanges = [];

    [TestMethod]
    public void AddEveryOther()
    {
        _inputChanges =
        [
            new IListComparer<char>.Added('B', 1),
            new IListComparer<char>.Added('D', 3),
            new IListComparer<char>.Added('G', 6),
            new IListComparer<char>.Added('K', 10)
        ];

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
    }

    [TestMethod]
    public void AddEveryOtherReverse()
    {
        _inputChanges =
        [
            new IListComparer<char>.Added('K', 10),
            new IListComparer<char>.Added('G', 6),
            new IListComparer<char>.Added('D', 3),
            new IListComparer<char>.Added('B', 1),
        ];

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
    }

    [TestMethod]
    public void AddDeleteEveryOther()
    {
        _inputChanges =
        [
            new IListComparer<char>.Added('D', 2),
            new IListComparer<char>.Deleted('B', 1),
            new IListComparer<char>.Added('H', 5),
            new IListComparer<char>.Deleted('F', 4),
        ];

    AssertCompare(
            "aBceFgi",
            "acDegHi",
    [
                new(Delete, 'B', 1),
                new(Add, 'D', 2),
                new(Delete, 'F', 4),
                new(Add, 'H', 5),
            ]
        );
    }

    [TestMethod]
    public void AddDeleteEveryOther2()
    {
        _inputChanges =
        [
            new IListComparer<char>.Added('H', 5),
            new IListComparer<char>.Deleted('B', 1),
            new IListComparer<char>.Added('D', 2),
            new IListComparer<char>.Deleted('F', 4),
        ];

    AssertCompare(
            "aBceFgi",
            "acDegHi",
    [
                new(Delete, 'B', 1),
                new(Add, 'D', 2),
                new(Delete, 'F', 4),
                new(Add, 'H', 5),
            ]
        );
    }

    [TestMethod]
    public void AddDeleteEveryOther3()
    {
        _inputChanges =
        [
            new IListComparer<char>.Added('H', 5),
            new IListComparer<char>.Added('D', 2),
            new IListComparer<char>.Deleted('B', 1),
            new IListComparer<char>.Deleted('F', 4),
        ];

    AssertCompare(
            "aBceFgi",
            "acDegHi",
    [
                new(Delete, 'B', 1),
                new(Add, 'D', 2),
                new(Delete, 'F', 4),
                new(Add, 'H', 5),
            ]
        );
    }

    [TestMethod]
    public void AddAddDelete()
    {
        _inputChanges =
        [
            new IListComparer<char>.Added('D', 2),
            new IListComparer<char>.Added('H', 5),
            new IListComparer<char>.Deleted('F', 3),
        ];
        
        AssertCompare(
            "aceFgi",
            "acDegHi",
            [
                new(Add, 'D', 2),
                new(Delete, 'F', 4),
                new(Add, 'H', 5),
            ]
        );
    }

    [TestMethod]
    public void Whatever()
    {
        _inputChanges =
        [
            new IListComparer<char>.Added('X', 2),
            new IListComparer<char>.Added('G', 4),
            new IListComparer<char>.Added('H', 5),
            new IListComparer<char>.Deleted('D', 3),
            new IListComparer<char>.Deleted('E', 4),
            new IListComparer<char>.Deleted('F', 5),
        ];
        
        AssertCompare(
            "abcDEF",
            "abXcGH",
            [
                new(Add, 'X', 2),
                new(Delete, 'D', 4),
                new(Delete, 'E', 4),
                new(Delete, 'F', 4),
                new(Add, 'G', 4),
                new(Add, 'H', 5),
            ]
        );
    }


    class DummyListComparer : IListComparer<char>
    {
        private readonly List<IListComparer<char>.IChange> _changes;

        public DummyListComparer(List<IListComparer<char>.IChange> changes)
        {
            _changes = changes;
        }

        public List<IListComparer<char>.IChange> Compare() => _changes;
    }

    protected internal override IListComparer<char> CreateComparer(string left, string right) =>
        new StepwiseListComparer<char>(Math.Max(left.Length, right.Length), left.Length, new DummyListComparer(_inputChanges));
}