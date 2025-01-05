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
using LeftIndex = int;
using RightIndex = int;

public abstract class ListComparerTestsBase
{
    protected const int Add = 0;
    protected const int Delete = 1;
    protected const int Move = 2;

    protected void AssertCompare(string left, string right, EasyToEnterResult[] results)
    {
        var comparer = CreateComparer(left, right);
        var changes = comparer.Compare();
        CollectionAssert.AreEquivalent(
            results.Select(r => (IListComparer<char>.Change)(r.changeKind switch
            {
                Add => new IListComparer<char>.Added(r.left, r.leftIndex),
                Delete => new IListComparer<char>.Deleted(r.left, r.leftIndex),
                Move => new IListComparer<char>.Moved(r.left, r.leftIndex, (char)r.left, (Int32)r.rightIndex),
                _ => throw new InvalidOperationException()
            })).ToList(),
            changes
        );
    }

    protected internal abstract IListComparer<char> CreateComparer(string left, string right);

    protected record struct EasyToEnterResult(
        int changeKind,
        char left,
        LeftIndex leftIndex,
        RightIndex? rightIndex = null);
}