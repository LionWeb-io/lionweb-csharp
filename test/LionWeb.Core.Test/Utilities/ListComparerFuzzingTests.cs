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
using Serialization;

[DebugOnlyTestClass]
public class ListComparerFuzzingTests : AbsoluteIndexListComparerTestsBase
{
    const int Tries = 1000;
    private const int MaxLength = 15;

    public static IEnumerable<object[]> TestData
    {
        get => Enumerable.Range(0, Tries).Select(i => new object[]
        {
            StringRandomizer.Random(new Random().Next(MaxLength)),
            StringRandomizer.Random(new Random().Next(MaxLength)),
        });
    }

    [TestMethod]
    [DynamicData(nameof(TestData))]
    public void Fuzz(string left, string right)
        => AssertCompare(left, right);

    [TestMethod]
    public void Ex0() => AssertCompare("JK", "wm3cKcpKyBnI");

    [TestMethod]
    public void Ex1() => AssertCompare("yiM2g", "4yYqIGg4gAi");

    protected internal override IListComparer<char> CreateComparer(string left, string right)
        => new ListComparer<char>(left.ToList(), right.ToList());
}

[DebugOnlyTestClass]
public class StepwiseFuzzingTests : ListComparerTestsBase
{
    const int Tries = 1000;
    private const int MaxLength = 15;

    public static IEnumerable<object[]> TestData
    {
        get => Enumerable.Range(0, Tries).Select(i => new object[]
        {
            StringRandomizer.Random(new Random().Next(MaxLength)),
            StringRandomizer.Random(new Random().Next(MaxLength)),
        });
    }

    [TestMethod]
    [DynamicData(nameof(TestData))]
    public void Fuzz(string left, string right)
        => AssertCompare(left, right);

    [TestMethod]
    public void Ex0() => AssertCompare("bRaMC1I0P2tFS", "0RYG");

    protected internal override IListComparer<char> CreateComparer(string left, string right)
        => new StepwiseListComparer<char>(left.ToList(), right.ToList());
}

[DebugOnlyTestClass]
// [TestClass]
public class RelativeChangesFuzzingTests : ListComparerTestsBase
{
    const int Tries = 1000;
    private const int MaxLength = 15;

    public static IEnumerable<object[]> TestData
    {
        get => Enumerable.Range(0, Tries).Select(i => new object[]
        {
            StringRandomizer.Random(new Random().Next(MaxLength)),
            StringRandomizer.Random(new Random().Next(MaxLength)),
        });
    }

    [TestMethod]
    [DynamicData(nameof(TestData))]
    public void Fuzz(string left, string right)
        => AssertCompare(left, right);

    [TestMethod]
    public void Ex0() => AssertCompare("bRaMC1I0P2tFS", "0RYG");

    protected internal override IListComparer<char> CreateComparer(string left, string right)
        => new RelativeChangesListComparer<char>(left.ToList(), right.ToList());
}

[DebugOnlyTestClass]
// [TestClass]
public class MoveDetectorFuzzingTests : ListComparerTestsBase
{
    const int Tries = 10;
    private const int MaxLength = 15;

    public static IEnumerable<object[]> TestData
    {
        get => GetTestData();
    }

    private static IEnumerable<object[]> GetTestData()
    {
        var random = new Random();

        return Enumerable.Range(0, Tries).Select(i =>
        {
            var left = StringRandomizer.Random(random.Next(MaxLength), "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            var leftSize = left.Count();
            int oneFifth = (int)(leftSize * 0.2);
            var moveCount = random.Next(oneFifth, oneFifth * 3);
            var addCount = random.Next(0, leftSize);
            var deleteCount = random.Next(0, oneFifth * 2);

            var right = left;

            for (int i1 = 0; i1 < deleteCount; i1++)
                right = right.DRemove(random.Next(right.Length));

            for (int i2 = 0; i2 < addCount; i2++)
                right = right.DAdd(random.Next(right.Length), StringRandomizer.RandomChar("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));

            for (int i3 = 0; i3 < moveCount; i3++)
            {
                var leftIndex = random.Next(right.Length);

                int rightIndex;
                do
                {
                    rightIndex = random.Next(right.Length);
                } while (rightIndex == leftIndex || right[leftIndex] == right[rightIndex]);

                right = right.DMove(leftIndex, rightIndex, right[leftIndex]);
            }

            return new object[] { left, right };
        });
    }

    [TestMethod]
    [DynamicData(nameof(TestData))]
    public void Fuzz(string left, string right)
        => AssertCompare(left, right);

    protected internal override IListComparer<char> CreateComparer(string left, string right)
        => new MoveDetector<char>(new RelativeChangesListComparer<char>(left.ToList(), right.ToList()).Compare());
}