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
using Languages.Generated.V2024_1.TinyRefLang;
using M2;
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

        Assert.ThrowsException<IllegalMigrationStateException>(() => migrator.RegisterMigration(migration));
    }

    private class LanguageRegistryInInitializeMigration() : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public override void Initialize(ILanguageRegistry languageRegistry)
        {
            var x = LanguageRegistry.LionWebVersion;
            Console.WriteLine(x);
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
        await Assert.ThrowsExceptionAsync<UnknownLookupException>(async () =>
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

    #region IsInstanceOf

    [TestMethod]
    public async Task IsInstanceOf_Direct()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new IsInstanceOfMigration(ShapesLanguage.Instance.Circle);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task IsInstanceOf_Incompatible()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new IsInstanceOfMigration(ShapesLanguage.Instance.Line);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsFalse(migrated);
    }

    [TestMethod]
    public async Task IsInstanceOf_Supertype()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new IsInstanceOfMigration(ShapesLanguage.Instance.IShape);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task IsInstanceOf_Supertype_NoLanguage()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new IsInstanceOfMigration(ShapesLanguage.Instance.IShape);

        migrator.RegisterMigration(migration);
        await Assert.ThrowsExceptionAsync<UnknownLookupException>(() =>
            migrator.MigrateAsync(inputStream, Stream.Null));
    }

    private class IsInstanceOfMigration(Classifier classifier) : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            return new(IsInstanceOf(inputRootNodes.First(), classifier), inputRootNodes);
        }
    }

    #endregion

    #region AllInstancesOf

    [TestMethod]
    public async Task AllInstancesOf_Root()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new AllInstancesOfMigration(ShapesLanguage.Instance.Circle);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task AllInstancesOf_Child()
    {
        var input = new Circle("circle") { Center = new Coord("c") };
        MemoryStream inputStream = await Serialize([input, input.Center]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new AllInstancesOfMigration(ShapesLanguage.Instance.Coord);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task AllInstancesOf_None()
    {
        var input = new Circle("circle") { Center = new Coord("c") };
        MemoryStream inputStream = await Serialize([input, input.Center]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new AllInstancesOfMigration(ShapesLanguage.Instance.Line);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsFalse(migrated);
    }

    private class AllInstancesOfMigration(Classifier classifier) : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            return new(AllInstancesOf(inputRootNodes, classifier).Any(), inputRootNodes);
        }
    }

    #endregion

    #region ConvertSubtreeToLenient

    [TestMethod]
    public async Task ConvertSubtreeToLenient_Lenient()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_LenientMigration();

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task ConvertSubtreeToLenient_Empty()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_ReadableMigration(input);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task ConvertSubtreeToLenient_Annotation()
    {
        var input = new Circle("circle");
        var ann = new BillOfMaterials("bof");
        input.AddAnnotations([ann]);
        MemoryStream inputStream = await Serialize([input, ann]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_ReadableMigration(input);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task ConvertSubtreeToLenient_Property()
    {
        var input = new Circle("circle") { Name = "hello" };
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_ReadableMigration(input);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task ConvertSubtreeToLenient_SingleContainment()
    {
        var coord = new Coord("c");
        var input = new Circle("circle") { Center = coord };
        MemoryStream inputStream = await Serialize([input, coord]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_ReadableMigration(input);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task ConvertSubtreeToLenient_MultiContainment_Single()
    {
        var coord = new Coord("c");
        var input = new Circle("circle") { Fixpoints = [coord] };
        MemoryStream inputStream = await Serialize([input, coord]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_ReadableMigration(input);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task ConvertSubtreeToLenient_MultiContainment_Multi()
    {
        var coordA = new Coord("a");
        var coordB = new Coord("b");
        var input = new Circle("circle") { Fixpoints = [coordA, coordB] };
        MemoryStream inputStream = await Serialize([input, coordA, coordB]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_ReadableMigration(input);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task ConvertSubtreeToLenient_SingleReference()
    {
        var source = new Circle("c");
        var input = new OffsetDuplicate("od") { AltSource = source };
        MemoryStream inputStream = await Serialize([input, source]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_ReadableMigration(input);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task ConvertSubtreeToLenient_MultiReference_Single()
    {
        var source = new Circle("c");
        var input = new ReferenceGeometry("rg") { Shapes = [source] };
        MemoryStream inputStream = await Serialize([input, source]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_ReadableMigration(input);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    [TestMethod]
    public async Task ConvertSubtreeToLenient_MultiReference_Multi()
    {
        var sourceA = new Circle("a");
        var sourceB = new Circle("b");
        var input = new ReferenceGeometry("rg") { Shapes = [sourceA, sourceB] };
        MemoryStream inputStream = await Serialize([input, sourceA, sourceB]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new ConvertSubtreeToLenient_ReadableMigration(input);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
    }

    private class ConvertSubtreeToLenient_LenientMigration() : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            var converted = ConvertSubtreeToLenient(inputRootNodes.First());
            Assert.AreSame(inputRootNodes.First(), converted);
            return new(true, inputRootNodes);
        }
    }

    private class ConvertSubtreeToLenient_ReadableMigration(IReadableNode node) : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            var converted = ConvertSubtreeToLenient(node);
            var result = Compare(node, converted);
            Assert.IsTrue(result);
            return new(result, inputRootNodes);
        }
    }

    #endregion
}