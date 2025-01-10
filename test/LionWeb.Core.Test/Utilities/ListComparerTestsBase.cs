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
    protected const int Replace = 2;
    protected const int Move = 3;

    protected virtual List<IListComparer<char>.IChange> AssertCompare(string left, string right)
    {
        var comparer = CreateComparer(left, right);
        var changes = comparer.Compare();

        List<string> steps = [left, IndexString(left)];
        var previous = left;

        foreach (var change in changes)
        {
            var line = change switch
            {
                IListComparer<char>.Added added => previous.Insert(added.Index, added.Element.ToString()),
                IListComparer<char>.Deleted deleted => previous.Remove(deleted.Index, 1),
                IListComparer<char>.Replaced replaced => previous.Remove(replaced.Index, 1)
                    .Insert(replaced.Index, replaced.RightElement.ToString()),
                IListComparer<char>.Moved moved => previous.Remove(moved.LeftIndex, 1).Insert(moved.RightIndex, moved.RightElement.ToString()),
            };

            previous = line;

            var index = IndexString(line);
            steps.Add("   " + change + "\n" + line + "\n" + index);
        }

        steps.Add(right);

        TestContext.WriteLine(string.Join("\n", steps));

        Assert.AreEqual(right, previous);

        return changes;
    }

    protected string IndexString(string l) => string.Join("", Enumerable.Range(0, l.Length).Select(n => n % 10));

    protected void AssertCompare(string left, string right, EasyToEnterResult[] results)
    {
        var changes = AssertCompare(left, right);

        CollectionAssert.AreEqual(
            results.Select(r => (IListComparer<char>.IChange)(r.changeKind switch
            {
                Add => new IListComparer<char>.Added(r.left, r.leftIndex),
                Delete => new IListComparer<char>.Deleted(r.left, r.leftIndex),
                Replace => new IListComparer<char>.Replaced(r.left, r.leftIndex, (char)r.right),
                Move => new IListComparer<char>.Moved(
                    r.left,
                    r.leftIndex,
                    (char)(r.right ?? r.left),
                    (int)r.rightIndex
                ),
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
        RightIndex? rightIndex = null,
        char? right = null);

    private TestContext testContextInstance;

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext
    {
        get { return testContextInstance; }
        set { testContextInstance = value; }
    }
}