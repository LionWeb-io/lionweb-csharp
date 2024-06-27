// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb_CSharp_Test.tests;

[TestClass]
public class PlatformTests
{
    public static IEnumerable<T> Join<T>(IEnumerable<T> values, T joiner)
        => values.SelectMany<T, T>((x, index) => (index + 1 < values.Count()) ? [x, joiner] : [x]);

    [TestMethod]
    public void implement_joiner()
    {
        var first = new []{0, 1, 2};
        var joined = Join(first, -1).ToArray();
        CollectionAssert.AreEqual(new []{0, -1, 1, -1, 2}, joined);
    }

}