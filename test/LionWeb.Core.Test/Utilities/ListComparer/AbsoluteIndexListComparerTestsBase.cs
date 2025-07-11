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

using Core.Utilities.ListComparer;

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