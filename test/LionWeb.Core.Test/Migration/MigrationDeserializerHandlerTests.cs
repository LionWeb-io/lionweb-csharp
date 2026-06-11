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

namespace LionWeb.Core.Test.Migration;

using Core.Migration;
using Core.Serialization;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;

[TestClass]
public class MigrationDeserializerHandlerTests : MigrationTestsBase
{
    [TestMethod]
    public async Task MultipleFeatureUsages_OneInstance()
    {
        List<IReadableNode> inputNodes =
        [
            new LinkTestConcept("a") { Containment_0_1 = new LinkTestConcept("a_c") },
            new LinkTestConcept("b") { Containment_0_1 = new LinkTestConcept("b_c") }
        ];

        var lionWebVersion = TestLanguageLanguage.Instance.LionWebVersion;
        List<Language> languages = [TestLanguageLanguage.Instance];
        var modelMigrator = new ModelMigrator(lionWebVersion, []);
        modelMigrator.RegisterMigration(new DummyMigration());

        var inputStream = new MemoryStream();
        var serializer = new SerializerBuilder().WithLionWebVersion(lionWebVersion).Build();
        await JsonUtils.WriteNodesToStreamAsync(inputStream, serializer, inputNodes.SelectMany(n => M1Extensions.Descendants(n, true, true)));
        inputStream.Seek(0, SeekOrigin.Begin);

        var outputStream = new MemoryStream();
        var migrated = await modelMigrator.MigrateAsync(inputStream, outputStream);
        outputStream.Seek(0, SeekOrigin.Begin);
        Assert.IsTrue(migrated);

        var deserializer = new DeserializerBuilder().WithLionWebVersion(lionWebVersion).WithLanguages(languages).Build();
        var outputNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream, deserializer);

        Assert.AreEqual(inputNodes.Count, outputNodes.Count);
    }

    private class DummyMigration : IMigration
    {
        private bool _done = false;
        
        public int Priority => 0;
        public void Initialize(ILanguageRegistry languageRegistry) { }

        public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) => !_done;

        public MigrationResult Migrate(List<LenientNode> inputRootNodes)
        {
            HashSet<Classifier> classifiers = [];
            HashSet<Feature> features = [];
            
            foreach (var node in inputRootNodes.Descendants())
            {
                classifiers.Add(node.GetClassifier());
                foreach (var feature in node.CollectAllSetFeatures())
                {
                    features.Add(feature);
                }
            }

            Assert.ContainsSingle(classifiers);
            Assert.ContainsSingle(features);

            _done = true;
            return new MigrationResult(true, inputRootNodes);
        }
    }
}