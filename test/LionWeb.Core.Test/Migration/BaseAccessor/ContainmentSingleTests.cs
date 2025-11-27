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
public class ContainmentSingleTests : MigrationTestsBase
{
    #region SetChild

    [TestMethod]
    public async Task SetChild_Generated()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetChildMigration(ShapesLanguage.Instance.Circle_center, new Coord("c"),
            new Circle("c") { Center = new Coord("co") });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetChild_Generated_Multiple()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetChildMigration(ShapesLanguage.Instance.IShape_fixpoints, new Coord("c"),
            new Circle("c") { Fixpoints = [new Coord("co")] });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetChild_Generated_WrongType()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var expected = new LenientNode("l", ShapesLanguage.Instance.Circle);
        expected.Set(ShapesLanguage.Instance.Circle_center, new Line("l"));

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetChildMigration(ShapesLanguage.Instance.Circle_center, new Line("c"), expected);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetChild_Dynamic()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetChildMigration(dynamic.FindByKey<Containment>(ShapesLanguage.Instance.Circle_center.Key),
            new Coord("c"),
            new Circle("c") { Center = new Coord("co") });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class SetChildMigration(Containment containment, IWritableNode child, IReadableNode expected)
        : MigrationBase<ShapesLanguage>(
            LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            var first = inputRootNodes.First();
            SetChild(first, containment, child);
            var result = Compare(expected, first);
            Assert.IsTrue(result);
            return new(true, inputRootNodes);
        }
    }

    #endregion

    #region TryGetChild

    [TestMethod]
    public async Task TryGetChild_Generated_Unset()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new TryGetChildMigration(ShapesLanguage.Instance.Circle_center, null);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsFalse(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetChild_Generated_Set()
    {
        var input = new Circle("circle") { ShapeDocs = new Documentation("c") { Text = "a" } };
        MemoryStream inputStream = await Serialize([input, input.ShapeDocs]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetChildMigration(ShapesLanguage.Instance.Shape_shapeDocs, new Documentation("c") { Text = "a" });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetChild_Dynamic_Set()
    {
        var input = new Circle("circle") { ShapeDocs = new Documentation("c") { Text = "a" } };
        MemoryStream inputStream = await Serialize([input, input.ShapeDocs]);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetChildMigration(dynamic.FindByKey<Containment>(ShapesLanguage.Instance.Shape_shapeDocs.Key),
                new Documentation("c") { Text = "a" });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetChild_Generated_Multiple_Single()
    {
        var input = new Circle("circle") { Fixpoints = [new Coord("a")] };
        MemoryStream inputStream = await Serialize([input, ..input.Fixpoints]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetChildMigration(ShapesLanguage.Instance.IShape_fixpoints, new Coord("a"));

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetChild_Generated_Multiple_Multiple()
    {
        var input = new Circle("circle") { Fixpoints = [new Coord("a"), new Coord("b")] };
        MemoryStream inputStream = await Serialize([input, ..input.Fixpoints]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetChildMigration(ShapesLanguage.Instance.IShape_fixpoints, null);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsFalse(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class TryGetChildMigration(Containment containment, IReadableNode? expected)
        : MigrationBase<ShapesLanguage>(
            LanguageIdentity.FromLanguage(ShapesLanguage.Instance), ShapesLanguage.Instance)
    {
        public bool Migrated { get; private set; } = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !Migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            Migrated = true;
            var result = false;
            var first = inputRootNodes.First();
            if (TryGetChild(first, containment, out var value))
                result = Compare(expected, value);
            return new(result, inputRootNodes);
        }
    }

    #endregion
}