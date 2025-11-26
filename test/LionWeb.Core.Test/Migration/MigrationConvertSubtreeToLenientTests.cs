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

[TestClass]
public class MigrationConvertSubtreeToLenientTests : MigrationTestsBase
{
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
}