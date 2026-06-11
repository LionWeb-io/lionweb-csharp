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

namespace LionWeb.Core.Benchmark;

using BenchmarkDotNet.Attributes;
using M1;
using M3;
using Migration;
using Serialization;
using Test.Languages.Generated.V2023_1.TestLanguage;

[MemoryDiagnoser]
// [NativeMemoryProfiler]
public class MigrationBenchmark : SerializerBenchmarkBase
{
    private List<LinkTestConcept> _v2023Nodes;
    private MemoryStream _input;
    private TestLanguageLanguage _v2023Language;
    private Test.Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage _v2024Language;
    private Test.Languages.Generated.V2026_1.TestLanguage.TestLanguageLanguage _v2026Language;
    private DynamicLanguage _v2026LanguageClone;

    [IterationSetup]
    public void CreateNodes()
    {
        _v2023Nodes = SerializerBenchmark.CreateNodes<LinkTestConcept>(10_500L, (id, containment_0_1, containment_1, containment_0_n) =>
        {
            var result = new LinkTestConcept(id);

            if (containment_0_1 is not null)
                result.Containment_0_1 = containment_0_1;

            if (containment_1 is not null)
                result.Containment_1 = containment_1;

            if (containment_0_n is not null)
                result.AddContainment_0_n(containment_0_n);

            return result;
        }).ToList();

        _input = new MemoryStream();
        // JsonUtils.WriteNodesToStream(_input, new SerializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1).Build(), _v2023Nodes.SelectMany(n => n.Descendants(true, true)));
        JsonUtils.WriteNodesToStream(_input, new SerializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1).Build(), _v2023Nodes);
        _input.Seek(0, SeekOrigin.Begin);
        
        _v2023Language = TestLanguageLanguage.Instance;
        _v2024Language = Test.Languages.Generated.V2024_1.TestLanguage.TestLanguageLanguage.Instance;
        _v2026Language = Test.Languages.Generated.V2026_1.TestLanguage.TestLanguageLanguage.Instance;
        _v2026LanguageClone = new DynamicLanguageCloner(LionWebVersions.v2026_1).Clone(_v2026Language);
        _v2026LanguageClone.Version = "clone";
    }

    [Benchmark]
    public async Task<bool> Migrate_2023_2026()
    {
        var modelMigrator = new ModelMigrator(LionWebVersions.v2026_1_Compatible, [_v2026LanguageClone]);
        modelMigrator.RegisterMigration(new LionWebVersionMigration(LionWebVersions.v2023_1, LionWebVersions.v2024_1) { Priority = 20 });
        modelMigrator.RegisterMigration(new LionWebVersionMigration(LionWebVersions.v2024_1, LionWebVersions.v2026_1) { Priority = 10 });
        modelMigrator.RegisterMigration(new LanguageVersionMigration(new HashSet<LanguageVersionMigration.VersionMapping>{new(_v2026Language.Key, _v2026Language.Version, _v2026LanguageClone.Version)}) { Priority = 0 });

        var output = new MemoryStream();

        var migrated = await modelMigrator.MigrateAsync(_input, output);

        return migrated;

        // var deserializer = new DeserializerBuilder().WithLionWebVersion(_v2026LanguageClone.LionWebVersion).WithLanguage(_v2026LanguageClone).Build();
        // List<IReadableNode> v2026Nodes = await JsonUtils.ReadNodesFromStreamAsync(output, deserializer);
        //
        // return _v2023Nodes.Count == v2026Nodes.Count;
    }
}