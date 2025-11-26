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
using M3;

[TestClass]
public class MigrationInstanceOfTests : MigrationTestsBase
{
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
}