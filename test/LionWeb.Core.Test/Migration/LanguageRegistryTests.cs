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
using Languages.Generated.V2023_1.TinyRefLang;
using Languages.Generated.V2023_1.WithEnum.M2;
using M1;
using M3;

[TestClass]
public class LanguageRegistryTests : MigrationTestsBase
{
    #region UsedLanguges

    [TestMethod]
    public async Task KnownLanguages()
    {
        var inputA = new EnumHolder("eh");
        var inputB = new Circle("circle");
        var inputC = new BillOfMaterials("bom") { Materials = [inputB] };
        inputA.AddAnnotations([inputC]);
        var inputD = new MyConcept("cons") { SingularRef = inputB };
        MemoryStream inputStream = await Serialize([inputA, inputC, inputD]);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, [])
        {
            DeserializerBuilder = new DeserializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1)
                .WithHandler(new SkipUnknownReferencesDeserializationHandler())
        };
        var migration = new LanguageRegistryMigration(lr =>
        {
            var knownLanguages = lr.KnownLanguages.ToList();

            Assert.AreEqual(3, knownLanguages.Count);
            Assert.IsNotNull(knownLanguages.First(l =>
                l.Key == WithEnumLanguage.Instance.Key && l.Version == WithEnumLanguage.Instance.Version));
            Assert.IsNotNull(knownLanguages.First(l =>
                l.Key == ShapesLanguage.Instance.Key && l.Version == ShapesLanguage.Instance.Version));
            Assert.IsNotNull(knownLanguages.First(l =>
                l.Key == TinyRefLangLanguage.Instance.Key && l.Version == TinyRefLangLanguage.Instance.Version));
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class SkipUnknownReferencesDeserializationHandler : DeserializerExceptionHandler
    {
        public override IReadableNode? UnresolvableReferenceTarget(ICompressedId? targetId, string? resolveInfo,
            Feature reference, IReadableNode node) =>
            null;
    }

    #endregion

    #region LionWebVersion

    [TestMethod]
    public async Task LionWebVersion()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            Assert.AreSame(LionWebVersions.v2024_1_Compatible, lr.LionWebVersion);
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    #endregion

    #region TryGetLanguage

    [TestMethod]
    public async Task TryGetLanguage_Good()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            Assert.IsTrue(lr.TryGetLanguage(LanguageIdentity.FromLanguage(ShapesLanguage.Instance), out var language));
            Assert.AreEqual(ShapesLanguage.Instance.Key, language.Key);
            Assert.AreEqual(ShapesLanguage.Instance.Version, language.Version);
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }
    
    [TestMethod]
    public async Task TryGetLanguage_Bad()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            Assert.IsFalse(lr.TryGetLanguage(LanguageIdentity.FromLanguage(TinyRefLangLanguage.Instance), out var _));
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }
    
    [TestMethod]
    public async Task TryGetLanguage_BuiltIn()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            Assert.IsFalse(lr.TryGetLanguage(LanguageIdentity.FromLanguage(LionWebVersions.v2024_1_Compatible.BuiltIns), out var _));
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }
    
    #endregion

    #region RegisterLanguage

    [TestMethod]
    public async Task RegisterLanguage_New()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            var newLang = TinyRefLangLanguage.Instance;
            Assert.IsFalse(lr.TryGetLanguage(LanguageIdentity.FromLanguage(newLang), out var _));

            var dynamicLanguage = DynamicClone(lr.LionWebVersion, newLang);
            var result = lr.RegisterLanguage(dynamicLanguage);
            Assert.IsTrue(result);
            
            Assert.IsTrue(lr.TryGetLanguage(LanguageIdentity.FromLanguage(newLang), out var language));
            Assert.AreEqual(newLang.Key, language.Key);
            Assert.AreEqual(newLang.Version, language.Version);
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task RegisterLanguage_Existing()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            var newLang = ShapesLanguage.Instance;
            Assert.IsTrue(lr.TryGetLanguage(LanguageIdentity.FromLanguage(newLang), out var _));

            var dynamicLanguage = DynamicClone(lr.LionWebVersion, newLang);
            var result = lr.RegisterLanguage(dynamicLanguage);
            Assert.IsFalse(result);
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    #endregion
    
    #region Lookup

    #region Language

    [TestMethod]
    public async Task Lookup_Language_Same()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            var lookup = lr.Lookup<Language>(ShapesLanguage.Instance);
            
            Assert.IsNotNull(lookup);
            Assert.AreEqual(ShapesLanguage.Instance.Key, lookup.Key);
            Assert.AreEqual(ShapesLanguage.Instance.Version, lookup.Version);
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task Lookup_Language_Other()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            var lookup = lr.Lookup<Language>(Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage.Instance);
            
            Assert.IsNotNull(lookup);
            Assert.AreEqual(ShapesLanguage.Instance.Key, lookup.Key);
            Assert.AreEqual(ShapesLanguage.Instance.Version, lookup.Version);
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task Lookup_Language_Unknown()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            Assert.ThrowsException<AmbiguousLanguageKeyMapping>(() =>
                lr.Lookup<Language>(TinyRefLangLanguage.Instance));
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task Lookup_Language_NonDynamic()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            Assert.ThrowsException<UnknownLookupException>(() =>
                lr.Lookup(ShapesLanguage.Instance));
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task Lookup_Language_OtherVersion()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            var dynamicLanguage = DynamicClone(lr.LionWebVersion, ShapesLanguage.Instance);
            dynamicLanguage.Version = "other";
            
            var lookup = lr.Lookup(dynamicLanguage);
            Assert.IsNotNull(lookup);
            Assert.AreNotEqual(dynamicLanguage.Version, lookup.Version);
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    #endregion

    #region Concept

    [TestMethod]
    public async Task Lookup_Concept_Same()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            var lookup = lr.Lookup(ShapesLanguage.Instance.Circle);
            
            Assert.IsNotNull(lookup);
            Assert.AreEqual(ShapesLanguage.Instance.Circle.Key, lookup.Key);
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task Lookup_Concept_Other()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            var lookup = lr.Lookup(Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage.Instance.Circle);
            
            Assert.IsNotNull(lookup);
            Assert.AreEqual(ShapesLanguage.Instance.Circle.Key, lookup.Key);
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task Lookup_Concept_Unknown()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new LanguageRegistryMigration(lr =>
        {
            Assert.ThrowsException<AmbiguousLanguageKeyMapping>(() => lr.Lookup(TinyRefLangLanguage.Instance.MyConcept));
        });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    #endregion

    #endregion

    private class LanguageRegistryMigration(Action<ILanguageRegistry> action) : IMigration
    {
        public bool Migrated { get; private set; } = false;
        private ILanguageRegistry _languageRegistry;
        public int Priority => 0;

        public void Initialize(ILanguageRegistry languageRegistry)
        {
            _languageRegistry = languageRegistry;
        }

        public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) => !Migrated;

        public MigrationResult Migrate(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            action(_languageRegistry);
            return new MigrationResult(true, inputRootNodes);
        }
    }
}