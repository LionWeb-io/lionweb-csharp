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
public class ListComparerTests
{
    [TestMethod]
    public void Empty()
    {
        var changes = new ListComparer<string>([], []).Compare();

        Assert.AreEqual(0, changes.Count);
    }

    [TestMethod]
    public void LeftEmptyOne()
    {
        var changes = new ListComparer<string>([], ["a"]).Run();

        CollectionAssert.AreEqual(
            new List<ListComparer<string>.ListChange> { new ListComparer<string>.ListAdded("a", 0) },
            changes
        );
    }

    [TestMethod]
    public void LeftEmptyTwo()
    {
        var changes = new ListComparer<string>([], ["a", "b"]).Compare();

        CollectionAssert.AreEqual(
            new List<ListComparer<string>.ListChange>
            {
                new ListComparer<string>.ListAdded("a", 0), new ListComparer<string>.ListAdded("b", 1)
            },
            changes
        );
    }

    [TestMethod]
    public void RightEmptyOne()
    {
        var changes = new ListComparer<string>(["a"], []).Compare();

        CollectionAssert.AreEqual(
            new List<ListComparer<string>.ListChange> { new ListComparer<string>.ListRemoved("a", 0) },
            changes
        );
    }

    [TestMethod]
    public void RightEmptyTwo()
    {
        var changes = new ListComparer<string>(["a", "b"], []).Compare();

        CollectionAssert.AreEqual(
            new List<ListComparer<string>.ListChange>
            {
                new ListComparer<string>.ListRemoved("a", 0), new ListComparer<string>.ListRemoved("b", 1)
            },
            changes
        );
    }

    [TestMethod]
    public void SwappedTwo()
    {
        var changes = new ListComparer<string>(["a", "b"], ["b", "a"]).Run();

        CollectionAssert.AreEqual(
            new List<ListComparer<char>.ListChange> { new ListComparer<char>.ListMoved('a', 0, 'a', 1) },
            changes
        );
    }

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
    public void Hirschberg2()
    {
        const string left = "abcdef";
        const string right = "bcda";
        var comparer = new ListComparer<char>(left.ToList(), right.ToList());
        var changes = comparer.Run();
        CollectionAssert.AreEquivalent(
            new List<ListComparer<char>.ListChange>
            {
                new ListComparer<char>.ListMoved('a', 0, 'a', 5),
                new ListComparer<char>.ListRemoved('e', 4),
            },
            changes
        );
    }
}