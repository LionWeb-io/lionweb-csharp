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
using Core.Serialization;
using Languages.Generated.V2023_1.Shapes.M2;
using M1;
using M2;
using M3;

[TestClass]
public class ReferenceSingleTests : MigrationTestsBase
{
    #region SetReference

    [TestMethod]
    public async Task SetReference_Generated()
    {
        var target = new ReadOnlyLine("l", null) { Name = null, Start = null, End = null, Uuid = null };
        var input = new OffsetDuplicate("ref");
        MemoryStream inputStream = await Serialize(input);

        var expected = new LenientNode("l", ShapesLanguage.Instance.OffsetDuplicate);
        expected.Set(ShapesLanguage.Instance.OffsetDuplicate_source, target);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetReferenceMigration(ShapesLanguage.Instance.OffsetDuplicate_source, target, expected);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetReference_Generated_Multiple()
    {
        var target = new ReadOnlyLine("l", null) { Name = null, Start = null, End = null, Uuid = null };
        var input = new ReferenceGeometry("ref");
        MemoryStream inputStream = await Serialize(input);

        var expected = new LenientNode("l", ShapesLanguage.Instance.ReferenceGeometry);
        expected.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, target);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetReferenceMigration(ShapesLanguage.Instance.ReferenceGeometry_shapes, target, expected);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetReference_Generated_WrongType()
    {
        var target = new Coord("l");
        var input = new OffsetDuplicate("ref");
        MemoryStream inputStream = await Serialize(input);

        var expected = new LenientNode("l", ShapesLanguage.Instance.OffsetDuplicate);
        expected.Set(ShapesLanguage.Instance.OffsetDuplicate_source, target);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetReferenceMigration(ShapesLanguage.Instance.OffsetDuplicate_source, target, expected);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetReference_Dynamic()
    {
        var target = new ReadOnlyLine("l", null) { Name = null, Start = null, End = null, Uuid = null };
        var input = new OffsetDuplicate("ref");
        MemoryStream inputStream = await Serialize(input);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var expected = new LenientNode("l", ShapesLanguage.Instance.OffsetDuplicate);
        expected.Set(ShapesLanguage.Instance.OffsetDuplicate_source, target);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetReferenceMigration(
            dynamic.FindByKey<Reference>(ShapesLanguage.Instance.OffsetDuplicate_source.Key), target, expected);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class SetReferenceMigration(Reference reference, IReadableNode target, IReadableNode expected)
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
            SetReference(first, reference, target);
            var result = Compare(expected, first);
            Assert.IsTrue(result);
            return new(true, inputRootNodes);
        }
    }

    #endregion

    #region TryGetReference

    [TestMethod]
    public async Task TryGetReference_Generated_Unset()
    {
        var input = new OffsetDuplicate("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new TryGetReferenceMigration(ShapesLanguage.Instance.OffsetDuplicate_source, null);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsFalse(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetReference_Generated_Set()
    {
        var target = new Line("l");
        var input = new OffsetDuplicate("circle") { Source = target };
        MemoryStream inputStream = await Serialize([input, target]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetReferenceMigration(ShapesLanguage.Instance.OffsetDuplicate_source, target);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetReference_Dynamic_Set()
    {
        var target = new ReadOnlyLine("l", null) { Name = null, Start = null, End = null, Uuid = null };
        var input = new LenientNode("od",
            Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage.Instance.OffsetDuplicate);
        input.Set(Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage.Instance.OffsetDuplicate_source, target);
        IEnumerable<IReadableNode> inputs = [input, target];
        var inputStream = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(inputStream,
            new SerializerBuilder().WithLionWebVersion(LionWebVersions.Current).Build(), inputs);
        inputStream.Seek(0, SeekOrigin.Begin);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetReferenceMigration(
                dynamic.FindByKey<Reference>(ShapesLanguage.Instance.OffsetDuplicate_source.Key),
                target);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetReference_Generated_Multiple_Single()
    {
        var target = new Line("l");
        var input = new ReferenceGeometry("circle") { Shapes = [target] };
        MemoryStream inputStream = await Serialize([input, ..input.Shapes]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetReferenceMigration(ShapesLanguage.Instance.ReferenceGeometry_shapes, target);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetReference_Generated_Multiple_Multiple()
    {
        var targetA = new Line("a");
        var targetB = new Line("b");
        var input = new ReferenceGeometry("circle") { Shapes = [targetA, targetB] };
        MemoryStream inputStream = await Serialize([input, ..input.Shapes]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetReferenceMigration(ShapesLanguage.Instance.ReferenceGeometry_shapes, null);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.MigrateAsync(inputStream, Stream.Null);
        Assert.IsFalse(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class TryGetReferenceMigration(Reference reference, IReadableNode? expected)
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
            if (TryGetReference(first, reference, out var value))
                result = Compare(expected, value);
            return new(result, inputRootNodes);
        }
    }

    #endregion
}