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

namespace LionWeb.Core.Test.Migration.BaseAccessor;

using Core.Migration;
using Languages.Generated.V2023_1.Shapes.M2;
using M2;
using M3;

[TestClass]
public class PropertyTests : MigrationTestsBase
{
    #region SetProperty

    [TestMethod]
    public async Task SetProperty_Generated()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new SetPropertyMigration(ShapesLanguage.Instance.Circle_r, 15);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetProperty_Generated_WrongType()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new SetPropertyMigration(ShapesLanguage.Instance.Circle_r, "asdf");

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetProperty_Dynamic()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new SetPropertyMigration(dynamic.FindByKey<Property>(ShapesLanguage.Instance.Circle_r.Key), 15);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class SetPropertyMigration(Property property, object? value) : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            SetProperty(inputRootNodes.First(), property, value);
            return new(true, inputRootNodes);
        }
    }

    #endregion

    #region TryGetProperty

    [TestMethod]
    public async Task TryGetProperty_Generated_Unset()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new TryGetPropertyMigration(ShapesLanguage.Instance.Circle_r, 15);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsFalse(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetProperty_Generated_Set()
    {
        var input = new Circle("circle") { R = 15 };
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new TryGetPropertyMigration(ShapesLanguage.Instance.Circle_r, "15");

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetProperty_Dynamic_Set()
    {
        var input = new Circle("circle") { R = 15 };
        MemoryStream inputStream = await Serialize(input);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetPropertyMigration(dynamic.FindByKey<Property>(ShapesLanguage.Instance.Circle_r.Key), "15");

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class TryGetPropertyMigration(Property property, object? expectedValue) : MigrationBase<ShapesLanguage>(
        LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            var result = false;
            if (TryGetProperty(inputRootNodes.First(), property, out var value))
                result = Equals(value, expectedValue);
            return new(result, inputRootNodes);
        }
    }

    #endregion
}