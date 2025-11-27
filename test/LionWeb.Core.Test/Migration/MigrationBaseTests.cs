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
using Languages.Generated.V2023_1.Circular.A;
using Languages.Generated.V2023_1.Circular.B;
using Languages.Generated.V2023_1.Shapes.M2;
using M3;

[TestClass]
public class MigrationBaseTests : MigrationTestsBase
{
    #region IllegalMigrationStateException

    [TestMethod]
    public void AccessLanguageRegistryInInitialize()
    {
        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryInInitializeMigration();

        Assert.ThrowsExactly<IllegalMigrationStateException>(() => migrator.RegisterMigration(migration));
    }

    private class LanguageRegistryInInitializeMigration() : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public override void Initialize(ILanguageRegistry languageRegistry)
        {
            var x = LanguageRegistry.LionWebVersion;
            base.Initialize(languageRegistry);
        }

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes) =>
            throw new NotImplementedException();
    }

    #endregion

    #region OriginLanguageIdentity

    [TestMethod]
    public async Task OriginLanguageIdentity()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new OriginLanguageIdentityMigration();

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class OriginLanguageIdentityMigration() : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            Assert.AreEqual(new LanguageIdentity(ShapesLanguage.Instance.Key, ShapesLanguage.Instance.Version),
                OriginLanguageIdentity);
            return new(true, inputRootNodes);
        }
    }

    [TestMethod]
    public async Task OriginLanguageIdentity_Short()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ShortOriginLanguageIdentityMigration();

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class ShortOriginLanguageIdentityMigration()
        : MigrationBase<ShapesLanguage>("asdf", ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated;

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            Assert.AreEqual(new LanguageIdentity(ShapesLanguage.Instance.Key, "asdf"),
                OriginLanguageIdentity);
            return new(true, inputRootNodes);
        }
    }

    #endregion

    #region TargetLang

    [TestMethod]
    public async Task TargetLang()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new TargetLangMigration();

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class TargetLangMigration() : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            Assert.AreSame(ShapesLanguage.Instance, DestinationLanguage);
            return new(true, inputRootNodes);
        }
    }

    #endregion

    #region OriginLanguage

    [TestMethod]
    public async Task OriginLanguage_Provided()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var clone = new DynamicLanguageCloner(LionWebVersions.v2024_1).Clone(ShapesLanguage.Instance);
        clone.Version = "asdf";
        clone.Name = "SomethingElse";
        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [clone]);
        var migration = new ProvidedOriginLanguageMigration();

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class ProvidedOriginLanguageMigration() : MigrationBase<ShapesLanguage>("asdf", ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated;

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            Assert.IsTrue(LanguageRegistry.TryGetLanguage(new LanguageIdentity(ShapesLanguage.Instance.Key, "asdf"),
                out var actual));
            Assert.AreEqual("SomethingElse", actual.Name);
            return new(true, inputRootNodes);
        }
    }

    [TestMethod]
    public async Task OriginLanguage_Implicit()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ImplicitOriginLanguageMigration();

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class ImplicitOriginLanguageMigration() : MigrationBase<ShapesLanguage>("asdf", ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated;

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            Assert.IsFalse(LanguageRegistry.TryGetLanguage(new LanguageIdentity(ShapesLanguage.Instance.Key, "asdf"),
                out  _));
            return new(true, inputRootNodes);
        }
    }

    [TestMethod]
    public async Task OriginLanguage_ImplicitDependant_Listed()
    {
        var input = new AConcept("a");
        MemoryStream inputStream = await Serialize(input);

        var bLangClone = new DynamicLanguageCloner(LionWebVersions.v2023_1).Clone(BLangLanguage.Instance);
        bLangClone.Name = "BLangClone";

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [bLangClone]);
        var migration = new ImplicitDependantOriginLanguageMigration();

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task OriginLanguage_ImplicitDependant()
    {
        var input = new AConcept("a");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ImplicitDependantOriginLanguageMigration();

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class ImplicitDependantOriginLanguageMigration()
        : MigrationBase<ALangLanguage>("asdf", ALangLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated;

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            Assert.IsFalse(LanguageRegistry.TryGetLanguage(new LanguageIdentity(ALangLanguage.Instance.Key, "asdf"),
                out _));
            return new(true, inputRootNodes);
        }
    }

    [TestMethod]
    public async Task OriginLanguage_ImplicitDependant_SameVersion()
    {
        var inputLangA = new DynamicLanguage("langA", LionWebVersions.v2023_1)
        {
            Key = "key-langA", Name = "langA", Version = "version"
        };
        var concept = inputLangA.Concept("concept-id", "concept-key", "concept-name");
        var inputLangB = new DynamicLanguage("langB", LionWebVersions.v2023_1)
        {
            Key = "key-langB", Name = "langB", Version = "version"
        };
        inputLangA.AddDependsOn([inputLangB]);
        var input = inputLangA.GetFactory().CreateNode("n", concept);
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [inputLangA, inputLangB]);
        var migration = new SameVersionOriginLanguageMigration(inputLangA);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class SameVersionOriginLanguageMigration(DynamicLanguage destinationLang)
        : MigrationBase<DynamicLanguage>("asdf", destinationLang)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated;

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            Assert.IsFalse(LanguageRegistry.TryGetLanguage(new LanguageIdentity(DestinationLanguage.Key, "asdf"),
                out _));
            return new(true, inputRootNodes);
        }
    }

    #endregion
}