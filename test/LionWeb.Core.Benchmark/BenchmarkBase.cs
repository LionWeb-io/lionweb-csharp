// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Benchmark;

using Test.Languages.Generated.V2024_1.TestLanguage;
using Test.Serialization;

public abstract class BenchmarkBase
{
    public static IEnumerable<LinkTestConcept> CreateNodes(long count) =>
        CreateNodes<LinkTestConcept>(count, (id, containment_0_1, containment_1, containment_0_n) =>
        {
            var result = new LinkTestConcept(id);

            if (containment_0_1 is not null)
                result.Containment_0_1 = containment_0_1;

            if (containment_1 is not null)
                result.Containment_1 = containment_1;

            if (containment_0_n is not null)
                result.AddContainment_0_n(containment_0_n);

            return result;
        });

    public static IEnumerable<T> CreateNodes<T>(long count, Func<string, T?, T?, List<T>?, T> factory)
    {
        T? optionalContainingInstance = default;
        T? requiredContainingInstance = default;
        T? emptyInstance = default;
        for (long l = 0; l < count; l++)
        {
            var id = $"id{l}_{StringRandomizer.RandomLength()}";

            // if (l % 10_000 == 0)
            // {
            //     TestContext.WriteLine(
            //         $"Creating Line #{l} privateMem: {AsFraction(Process.GetCurrentProcess().PrivateMemorySize64)} gcMem: {AsFraction(GC.GetTotalMemory(false))}");
            // }

            T result;
            if (emptyInstance == null || l % 2 == 0)
            {
                emptyInstance = factory(id, default, default, null);
                result = emptyInstance;
            } else if (l % 3 == 0)
            {
                optionalContainingInstance = factory(id, emptyInstance, default, null);
                result = optionalContainingInstance;
            } else if (l % 17 == 0)
            {
                requiredContainingInstance = factory(id,default, emptyInstance, null);
                result = requiredContainingInstance;
            } else if (l % 37 == 0)
            {
                result = factory(id, default, default, [optionalContainingInstance!, requiredContainingInstance!]);
            } else
            {
                emptyInstance = factory(id, default, default, null);
                result = emptyInstance;
            }

            yield return result;
        }
    }

    static string AsFraction(long value) =>
        $"{value / 1_000_000D:0.000}" + "M";
}