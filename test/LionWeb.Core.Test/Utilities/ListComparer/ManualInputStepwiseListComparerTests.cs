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

namespace LionWeb.Core.Test.Utilities.ListComparer;

using LionWeb.Core.Utilities.ListComparer;

[TestClass]
public class ManualInputStepwiseListComparerTests : ListComparerTestsBase
{
    private List<IListChange<char>> _inputChanges = [];

    [TestMethod]
    public void AddEveryOther()
    {
        _inputChanges =
        [
            new ListAdded<char>('B', 1),
            new ListAdded<char>('D', 3),
            new ListAdded<char>('G', 6),
            new ListAdded<char>('K', 10)
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
            new ListAdded<char>('K', 10),
            new ListAdded<char>('G', 6),
            new ListAdded<char>('D', 3),
            new ListAdded<char>('B', 1),
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
            new ListAdded<char>('D', 2),
            new ListDeleted<char>('B', 1),
            new ListAdded<char>('H', 5),
            new ListDeleted<char>('F', 4),
        ];

    AssertCompare(
            "aBceFgi",
            "acDegHi",
    [
                new(Delete, 'B', 1),
                new(Delete, 'F', 3),
                new(Add, 'D', 2),
                new(Add, 'H', 5)
            ]
        );
    }

    [TestMethod]
    public void AddDeleteEveryOther2()
    {
        _inputChanges =
        [
            new ListAdded<char>('H', 5),
            new ListDeleted<char>('B', 1),
            new ListAdded<char>('D', 2),
            new ListDeleted<char>('F', 4),
        ];

    AssertCompare(
            "aBceFgi",
            "acDegHi",
    [
                new(Delete, 'B', 1),
                new(Delete, 'F', 3),
                new(Add, 'D', 2),
                new(Add, 'H', 5),
            ]
        );
    }

    [TestMethod]
    public void AddDeleteEveryOther3()
    {
        _inputChanges =
        [
            new ListAdded<char>('H', 5),
            new ListAdded<char>('D', 2),
            new ListDeleted<char>('B', 1),
            new ListDeleted<char>('F', 4),
        ];

    AssertCompare(
            "aBceFgi",
            "acDegHi",
    [
                new(Delete, 'B', 1),
                new(Delete, 'F', 3),
                new(Add, 'D', 2),
                new(Add, 'H', 5),
            ]
        );
    }

    [TestMethod]
    public void AddAddDelete()
    {
        _inputChanges =
        [
            new ListAdded<char>('D', 2),
            new ListAdded<char>('H', 5),
            new ListDeleted<char>('F', 3),
        ];
        
        AssertCompare(
            "aceFgi",
            "acDegHi",
            [
                new(Delete, 'F', 3),
                new(Add, 'D', 2),
                new(Add, 'H', 5)
            ]
        );
    }

    [TestMethod]
    public void Whatever()
    {
        _inputChanges =
        [
            new ListAdded<char>('X', 2),
            new ListAdded<char>('G', 4),
            new ListAdded<char>('H', 5),
            new ListDeleted<char>('D', 3),
            new ListDeleted<char>('E', 4),
            new ListDeleted<char>('F', 5),
        ];
        
        AssertCompare(
            "abcDEF",
            "abXcGH",
            [
                new(Delete, 'D', 3),
                new(Delete, 'E', 3),
                new(Delete, 'F', 3),
                new(Add, 'X', 2),
                new(Add, 'G', 4),
                new(Add, 'H', 5),
            ]
        );
    }


    class DummyListComparer : IListComparer<char>
    {
        private readonly List<IListChange<char>> _changes;

        public DummyListComparer(List<IListChange<char>> changes)
        {
            _changes = changes;
        }

        public List<IListChange<char>> Compare() => _changes;
    }

    protected internal override IListComparer<char> CreateComparer(string left, string right) =>
        new RelativeChangesListComparer<char>(new DummyListComparer(_inputChanges));
}