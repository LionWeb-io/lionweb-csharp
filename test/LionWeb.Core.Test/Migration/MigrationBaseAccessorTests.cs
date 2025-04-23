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
using Core.Serialization;
using Languages.Generated.V2023_1.Shapes.M2;
using M1;
using M2;
using M3;

[TestClass]
public class MigrationBaseAccessorTests : MigrationTestsBase
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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

    #region SetChildren

    [TestMethod]
    public async Task SetChildren_Generated()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetChildrenMigration(ShapesLanguage.Instance.IShape_fixpoints,
            [new Coord("c")],
            new Circle("c") { Fixpoints = [new Coord("co")] });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetChildren_Generated_Multiple()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetChildrenMigration(ShapesLanguage.Instance.IShape_fixpoints,
            [new Coord("c"), new Coord("d")],
            new Circle("c") { Fixpoints = [new Coord("co"), new Coord("do")] });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetChildren_Generated_WrongType()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var expected = new LenientNode("l", ShapesLanguage.Instance.Circle);
        expected.Set(ShapesLanguage.Instance.IShape_fixpoints, new Line("l"));

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetChildrenMigration(ShapesLanguage.Instance.IShape_fixpoints, [new Line("c")], expected);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetChildren_Dynamic()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetChildrenMigration(
            dynamic.FindByKey<Containment>(ShapesLanguage.Instance.IShape_fixpoints.Key),
            [new Coord("c")],
            new Circle("c") { Fixpoints = [new Coord("co")] });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class SetChildrenMigration(
        Containment containment,
        IEnumerable<IWritableNode> children,
        IReadableNode expected)
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
            SetChildren(first, containment, children);
            var result = Compare(expected, first);
            Assert.IsTrue(result);
            return new(true, inputRootNodes);
        }
    }

    #endregion

    #region TryGetChildren

    [TestMethod]
    public async Task TryGetChildren_Generated_Unset()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new TryGetChildrenMigration(ShapesLanguage.Instance.Circle_center, null);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsFalse(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetChildren_Generated_Set()
    {
        var input = new Circle("circle") { ShapeDocs = new Documentation("c") { Text = "a" } };
        MemoryStream inputStream = await Serialize([input, input.ShapeDocs]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetChildrenMigration(ShapesLanguage.Instance.Shape_shapeDocs,
                [new Documentation("c") { Text = "a" }]);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetChildren_Dynamic_Set()
    {
        var input = new Circle("circle") { ShapeDocs = new Documentation("c") { Text = "a" } };
        MemoryStream inputStream = await Serialize([input, input.ShapeDocs]);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetChildrenMigration(dynamic.FindByKey<Containment>(ShapesLanguage.Instance.Shape_shapeDocs.Key),
                [new Documentation("c") { Text = "a" }]);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetChildren_Generated_Multiple_Single()
    {
        var input = new Circle("circle") { Fixpoints = [new Coord("a")] };
        MemoryStream inputStream = await Serialize([input, ..input.Fixpoints]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetChildrenMigration(ShapesLanguage.Instance.IShape_fixpoints, [new Coord("a")]);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetChildren_Generated_Multiple_Multiple()
    {
        var input = new Circle("circle") { Fixpoints = [new Coord("a"), new Coord("b")] };
        MemoryStream inputStream = await Serialize([input, ..input.Fixpoints]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetChildrenMigration(ShapesLanguage.Instance.IShape_fixpoints, [new Coord("a"), new Coord("b")]);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class TryGetChildrenMigration(Containment containment, List<IReadableNode>? expected)
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
            if (TryGetChildren(first, containment, out var value))
                result = Compare(expected, value.Cast<IReadableNode>().ToList());
            return new(result, inputRootNodes);
        }
    }

    #endregion

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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
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

    #region SetReferences

    [TestMethod]
    public async Task SetReferences_Generated()
    {
        var target = new Line("l");
        var input = new ReferenceGeometry("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetReferencesMigration(ShapesLanguage.Instance.ReferenceGeometry_shapes,
            [target],
            new ReferenceGeometry("c") { Shapes = [target] });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetReferences_Generated_Multiple()
    {
        var targetA = new Line("a");
        var targetB = new Line("b");
        var input = new ReferenceGeometry("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetReferencesMigration(ShapesLanguage.Instance.ReferenceGeometry_shapes,
            [targetA, targetB],
            new ReferenceGeometry("c") { Shapes = [targetA, targetB] });

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetReferences_Generated_WrongType()
    {
        var target = new Coord("l");
        var input = new ReferenceGeometry("circle");
        MemoryStream inputStream = await Serialize(input);

        var expected = new LenientNode("l", ShapesLanguage.Instance.ReferenceGeometry);
        expected.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, target);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetReferencesMigration(ShapesLanguage.Instance.ReferenceGeometry_shapes,
            [target], expected);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task SetReferences_Dynamic()
    {
        var target = new ReadOnlyLine("l", null) { Name = null, Start = null, End = null, Uuid = null };
        var input = new LenientNode("l",
            Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage.Instance.ReferenceGeometry);
        IEnumerable<IReadableNode> inputs = [input];
        var inputStream = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(inputStream,
            new SerializerBuilder().WithLionWebVersion(LionWebVersions.Current).Build(), inputs);
        inputStream.Seek(0, SeekOrigin.Begin);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var expected = new LenientNode("l",
            Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage.Instance.ReferenceGeometry);
        expected.Set(ShapesLanguage.Instance.ReferenceGeometry_shapes, target);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, [ShapesLanguage.Instance]);
        var migration = new SetReferencesMigration(
            dynamic.FindByKey<Reference>(ShapesLanguage.Instance.ReferenceGeometry_shapes.Key),
            [target], expected);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class SetReferencesMigration(
        Reference reference,
        IEnumerable<IReadableNode> targets,
        IReadableNode expected)
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
            SetReferences(first, reference, targets);
            var result = Compare(expected, first);
            Assert.IsTrue(result);
            return new(true, inputRootNodes);
        }
    }

    #endregion

    #region TryGetReferences

    [TestMethod]
    public async Task TryGetReferences_Generated_Unset()
    {
        var input = new OffsetDuplicate("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new TryGetReferencesMigration(ShapesLanguage.Instance.OffsetDuplicate_source, null);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsFalse(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetReferences_Generated_Set()
    {
        var target = new Line("l");
        var input = new OffsetDuplicate("circle") { Source = target };
        MemoryStream inputStream = await Serialize([input, input.Source]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration = new TryGetReferencesMigration(ShapesLanguage.Instance.OffsetDuplicate_source, [target]);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetReferences_Dynamic_Set()
    {
        var target = new ReadOnlyLine("l", null) { Name = null, Start = null, End = null, Uuid = null };
        var input = new LenientNode("circle", Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage.Instance.OffsetDuplicate);
        input.Set(Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage.Instance.OffsetDuplicate_source, target);
        IEnumerable<IReadableNode> inputs = [input, target];
        var inputStream = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(inputStream,
            new SerializerBuilder().WithLionWebVersion(LionWebVersions.Current).Build(), inputs);
        inputStream.Seek(0, SeekOrigin.Begin);

        var dynamic = DynamicClone(LionWebVersions.v2024_1_Compatible, ShapesLanguage.Instance);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetReferencesMigration(dynamic.FindByKey<Reference>(ShapesLanguage.Instance.OffsetDuplicate_source.Key), [target]);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetReferences_Generated_Multiple_Single()
    {
        var target = new Line("l");
        var input = new ReferenceGeometry("circle") { Shapes = [target] };
        MemoryStream inputStream = await Serialize([input, ..input.Shapes]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetReferencesMigration(ShapesLanguage.Instance.ReferenceGeometry_shapes, [target]);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    [TestMethod]
    public async Task TryGetReferences_Generated_Multiple_Multiple()
    {
        var targetA = new Line("a");
        var targetB = new Line("b");
        var input = new ReferenceGeometry("circle") { Shapes = [targetA, targetB] };
        MemoryStream inputStream = await Serialize([input, ..input.Shapes]);

        var migrator = new ModelMigrator(LionWebVersions.v2024_1_Compatible, []);
        var migration =
            new TryGetReferencesMigration(ShapesLanguage.Instance.ReferenceGeometry_shapes, [targetA, targetB]);

        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        Assert.IsTrue(migration.Migrated);
    }

    private class TryGetReferencesMigration(Reference reference, List<IReadableNode>? expected)
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
            if (TryGetReferences(first, reference, out var value))
                result = Compare(expected, value);
            return new(result, inputRootNodes);
        }
    }

    #endregion
}