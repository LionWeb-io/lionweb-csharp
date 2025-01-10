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

[TestClass]
public class StepwiseFuzzingTests : ListComparerTestsBase
{
    const int Tries = 10;
    private const int MaxLength = 30;

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
        => AssertCompare(left, right, []);

    protected internal override IListComparer<char> CreateComparer(string left, string right)
        => new StepwiseListComparer<char>(left.ToList(), right.ToList());
}