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

namespace LionWeb.Core.Test.Migration;

using Core.Migration;
using Languages.Generated.V2023_1.Shapes.M2;
using Languages.Generated.V2024_1.TinyRefLang;
using M2;
using M3;

[TestClass]
public class MigrationModificationTests : MigrationTestsBase
{
    #region RemovedOriginLanguageIdentity

    [TestMethod]
    public async Task RemovedOriginLanguageIdentity()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [TinyRefLangLanguage.Instance]);
        var migration = new RemovedOriginLanguageIdentityMigration();

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);

        var knownLanguages = migrator.KnownLanguages.ToList();
        Assert.IsFalse(knownLanguages.Any(l => l.Key == ShapesLanguage.Instance.Key));
    }

    private class RemovedOriginLanguageIdentityMigration() : MigrationBase<TinyRefLangLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance),
        TinyRefLangLanguage.Instance)
    {
        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes) =>
            new(true, [CreateNode(TinyRefLangLanguage.Instance.MyConcept)]);
    }

    #endregion

    #region ChangedKeyAndVersion

    [TestMethod]
    public async Task ChangedKeyAndVersion()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var targetLang = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);
        targetLang.Version = "v2";

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ChangedKeyAndVersionMigration(targetLang);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);

        var knownLanguages = migrator.KnownLanguages.ToList();
        Assert.AreEqual(1, knownLanguages.Count);
        Assert.AreEqual(ShapesLanguage.Instance.Key, knownLanguages[0].Key);
        Assert.AreEqual("v2", knownLanguages[0].Version);
    }

    private class ChangedKeyAndVersionMigration(Language destination) : MigrationBase<Language>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), destination)
    {
        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes) =>
            new(true, [CreateNode(DestinationLanguage.Entities.OfType<Concept>().First())]);
    }

    #endregion

    #region CreateNode

    [TestMethod]
    public async Task CreateNode_GeneratedClassifier()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new MigrationBaseMigration(ShapesLanguage.Instance.Line);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task CreateNode_GeneratedClassifier_WithoutLanguage()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new MigrationBaseMigration(ShapesLanguage.Instance.Line);

        migrator.RegisterMigration(migration);
        await Assert.ThrowsExactlyAsync<UnknownLookupException>(async () =>
            await migrator.MigrateAsync(inputStream, Stream.Null));
    }

    [TestMethod]
    public async Task CreateNode_DynamicClassifier()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var targetLang = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new MigrationBaseMigration(targetLang.FindByKey<Classifier>(ShapesLanguage.Instance.Line.Key));

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task CreateNode_DynamicClassifier_WithoutLanguage()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var targetLang = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new MigrationBaseMigration(targetLang.FindByKey<Classifier>(ShapesLanguage.Instance.Line.Key));

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    private class MigrationBaseMigration(Classifier classifier) : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            return new(true, [CreateNode(classifier)]);
        }
    }

    #endregion
}