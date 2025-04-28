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
using Core.Utilities;
using Languages.Generated.V2023_1.Shapes.M2;
using M1;
using M2;
using M3;

[TestClass]
public class ShapesMigrationTests
{
    private static readonly ShapesLanguage _v2023_1 = ShapesLanguage.Instance;

    private static readonly Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage _v2024_1 =
        Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage.Instance;

    #region ShapesMigration_NoVersionChange

    [TestMethod]
    public async Task From2023_1_to_2023_1()
    {
        var toVersion = LionWebVersions.v2023_1;

        var input = Instance2023();

        var outputStream = new MemoryStream();
        var result = await Migrate(toVersion, input, [_v2023_1], outputStream,
            [new LionWebVersionMigration(LionWebVersions.v2023_1, toVersion) { Priority = 0 }, new ShapesMigration()]);

        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(toVersion).WithLanguage(_v2023_1).Build());

        Assert.IsTrue(result);
        Assert.AreEqual(1, resultNodes.Count);
        Assert.AreEqual(input.AsString(), resultNodes.OfType<INode>().First().AsString());
    }

    [TestMethod]
    public async Task From2023_1_to_2024_1()
    {
        var toVersion = LionWebVersions.v2024_1_Compatible;

        var input = Instance2023();

        var outputStream = new MemoryStream();
        var result = await Migrate(toVersion, input, [_v2024_1], outputStream,
            [new LionWebVersionMigration(LionWebVersions.v2023_1, toVersion) { Priority = 0 }, new ShapesMigration()]);

        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(toVersion).WithLanguage(_v2024_1).Build());

        Assert.IsTrue(result);
        Assert.AreEqual(1, resultNodes.Count);
        Assert.AreEqual(input.AsString(), resultNodes.OfType<INode>().First().AsString());
    }

    [TestMethod]
    public async Task From2023_1_to_2024_1_NoLang()
    {
        var toVersion = LionWebVersions.v2024_1_Compatible;

        var input = Instance2023();

        var outputStream = new MemoryStream();
        var result = await Migrate(toVersion, input, [], outputStream,
            [new LionWebVersionMigration(LionWebVersions.v2023_1, toVersion) { Priority = 0 }, new ShapesMigration()]);

        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(toVersion).WithLanguage(_v2024_1).Build());

        Assert.IsTrue(result);
        Assert.AreEqual(1, resultNodes.Count);
        Assert.AreEqual(input.AsString(), resultNodes.OfType<INode>().First().AsString());
    }

    private class ShapesMigration() : MigrationBase<Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage>(
        LanguageIdentity.FromLanguage(_v2023_1), _v2024_1)
    {
        private bool _migrated = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !_migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            _migrated = true;
            return new MigrationResult(true, inputRootNodes);
        }
    }

    #endregion

    #region ShapesMigration_VersionChange

    [TestMethod]
    public async Task From2023_1_to_2023_1_v2()
    {
        var toVersion = LionWebVersions.v2023_1;

        var targetLang = new DynamicLanguageCloner(toVersion).Clone([_v2023_1]).Values.First();
        targetLang.Version = "v2";

        var input = Instance2023();

        var outputStream = new MemoryStream();
        var result = await Migrate(toVersion, input, [targetLang], outputStream,
            [new LionWebVersionMigration(LionWebVersions.v2023_1, toVersion) { Priority = 0 }, new ShapesMigrationV2(targetLang)]);

        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(toVersion).WithLanguage(targetLang).Build());

        Assert.IsTrue(result);
        Assert.AreEqual(1, resultNodes.Count);
        Assert.AreEqual("v2", resultNodes.First().GetClassifier().GetLanguage().Version);
        Assert.AreEqual(input.AsString(), resultNodes.OfType<INode>().First().AsString());
    }

    private class ShapesMigrationV2(DynamicLanguage targetLang) : MigrationBase<DynamicLanguage>(
        LanguageIdentity.FromLanguage(_v2023_1), targetLang)
    {
        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes) =>
            new(true, inputRootNodes);
    }

    #endregion
    
    #region Add2023Fixpoint

    [TestMethod]
    public async Task Add2023Fixpoints()
    {
        var toVersion = LionWebVersions.v2024_1_Compatible;

        var input = Instance2023();

        var outputStream = new MemoryStream();
        var result = await Migrate(toVersion, input, [_v2024_1], outputStream,
        [
            new LionWebVersionMigration(LionWebVersions.v2023_1, toVersion)
            {
                Priority = IMigration.DefaultPriority * 2
            },
            new Add2023Fixpoint()
        ]);

        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(toVersion).WithLanguage(_v2024_1).Build());

        Assert.IsTrue(result);
        Assert.AreEqual(1, resultNodes.Count);
        Assert.AreEqual(3, resultNodes.OfType<Languages.Generated.V2024_1.Shapes.M2.Circle>().First().Fixpoints.Count);
    }

    private class Add2023Fixpoint() : MigrationBase<Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage>(
        LanguageIdentity.FromLanguage(_v2023_1), _v2024_1)
    {
        private bool _migrated = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !_migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            _migrated = true;

            bool result = false;

            foreach (var node in AllInstancesOf(inputRootNodes, _v2023_1.IShape))
            {
                if (TryGetChildren(node, _v2023_1.IShape_fixpoints, out var fixpoints))
                {
                    if (fixpoints.Count < 3)
                    {
                        List<IWritableNode> newFixpoints = [..fixpoints];
                        for (int i = fixpoints.Count; i < 3; i++)
                        {
                            newFixpoints.Add(new Coord($"fixpoint-{i}") { X = i, Y = i, Z = i });
                        }

                        SetChildren(node, _v2023_1.IShape_fixpoints, newFixpoints);
                        result = true;
                    }
                }
            }

            return new MigrationResult(result, inputRootNodes);
        }
    }

    #endregion

    #region Add2024Fixpoint

    [TestMethod]
    public async Task Add2024Fixpoints()
    {
        var toVersion = LionWebVersions.v2024_1_Compatible;

        var input = Instance2023();

        var outputStream = new MemoryStream();
        var result = await Migrate(toVersion, input, [_v2024_1], outputStream,
        [
            new LionWebVersionMigration(LionWebVersions.v2023_1, toVersion)
            {
                Priority = IMigration.DefaultPriority * 2
            },
            new Add2024Fixpoint()
        ]);

        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(toVersion).WithLanguage(_v2024_1).Build());

        Assert.IsTrue(result);
        Assert.AreEqual(1, resultNodes.Count);
        Assert.AreEqual(3, resultNodes.OfType<Languages.Generated.V2024_1.Shapes.M2.Circle>().First().Fixpoints.Count);
    }

    private class Add2024Fixpoint() : MigrationBase<Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage>(
        LanguageIdentity.FromLanguage(_v2023_1), _v2024_1)
    {
        private bool _migrated = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !_migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            _migrated = true;

            bool result = false;

            foreach (var node in AllInstancesOf(inputRootNodes, _v2023_1.IShape))
            {
                if (TryGetChildren(node, _v2023_1.IShape_fixpoints, out var fixpoints))
                {
                    if (fixpoints.Count < 3)
                    {
                        List<IWritableNode> newFixpoints = [..fixpoints];
                        for (int i = fixpoints.Count; i < 3; i++)
                        {
                            newFixpoints.Add(
                                new Languages.Generated.V2024_1.Shapes.M2.Coord($"fixpoint-{i}")
                                {
                                    X = i, Y = i, Z = i
                                });
                        }

                        SetChildren(node, _v2023_1.IShape_fixpoints, newFixpoints);
                        result = true;
                    }
                }
            }

            return new MigrationResult(result, inputRootNodes);
        }
    }

    #endregion

    #region Add2024Fixpoint_2024Structure

    [TestMethod]
    public async Task Add2024Fixpoints_2024Structure()
    {
        var toVersion = LionWebVersions.v2024_1_Compatible;

        var input = Instance2023();

        var outputStream = new MemoryStream();
        var result = await Migrate(toVersion, input, [_v2024_1], outputStream,
        [
            new LionWebVersionMigration(LionWebVersions.v2023_1, toVersion)
            {
                Priority = IMigration.DefaultPriority * 2
            },
            new Add2024Fixpoint_2024Structure()
        ]);

        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(toVersion).WithLanguage(_v2024_1).Build());

        Assert.IsTrue(result);
        Assert.AreEqual(1, resultNodes.Count);
        Assert.AreEqual(3, resultNodes.OfType<Languages.Generated.V2024_1.Shapes.M2.Circle>().First().Fixpoints.Count);
    }

    private class Add2024Fixpoint_2024Structure() : MigrationBase<Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage>(
        LanguageIdentity.FromLanguage(_v2023_1), _v2024_1)
    {
        private bool _migrated = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !_migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            _migrated = true;

            bool result = false;

            foreach (var node in AllInstancesOf(inputRootNodes, _v2024_1.IShape))
            {
                if (TryGetChildren(node, _v2024_1.IShape_fixpoints, out var fixpoints))
                {
                    if (fixpoints.Count < 3)
                    {
                        List<IWritableNode> newFixpoints = [..fixpoints];
                        for (int i = fixpoints.Count; i < 3; i++)
                        {
                            newFixpoints.Add(
                                new Languages.Generated.V2024_1.Shapes.M2.Coord($"fixpoint-{i}")
                                {
                                    X = i, Y = i, Z = i
                                });
                        }

                        SetChildren(node, _v2024_1.IShape_fixpoints, newFixpoints);
                        result = true;
                    }
                }
            }

            return new MigrationResult(result, inputRootNodes);
        }
    }

    #endregion

    #region Add2024Fixpoint_MixedStructure

    [TestMethod]
    public async Task Add2024Fixpoints_MixedStructure()
    {
        var toVersion = LionWebVersions.v2024_1_Compatible;

        var input = Instance2023();

        var outputStream = new MemoryStream();
        var result = await Migrate(toVersion, input, [_v2024_1], outputStream,
        [
            new LionWebVersionMigration(LionWebVersions.v2023_1, toVersion)
            {
                Priority = IMigration.DefaultPriority * 2
            },
            new Add2024Fixpoint_MixedStructure()
        ]);

        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(toVersion).WithLanguage(_v2024_1).Build());

        Assert.IsTrue(result);
        Assert.AreEqual(1, resultNodes.Count);
        Assert.AreEqual(3, resultNodes.OfType<Languages.Generated.V2024_1.Shapes.M2.Circle>().First().Fixpoints.Count);
    }

    private class Add2024Fixpoint_MixedStructure()
        : MigrationBase<Languages.Generated.V2024_1.Shapes.M2.ShapesLanguage>(
            LanguageIdentity.FromLanguage(_v2023_1), _v2024_1)
    {
        private bool _migrated = false;

        public override bool IsApplicable(ISet<LanguageIdentity> languageIdentities) =>
            !_migrated && base.IsApplicable(languageIdentities);

        protected override MigrationResult MigrateInternal(List<LenientNode> inputRootNodes)
        {
            _migrated = true;

            bool result = false;

            foreach (var node in AllInstancesOf(inputRootNodes, _v2023_1.IShape))
            {
                if (TryGetChildren(node, _v2023_1.IShape_fixpoints, out var fixpoints))
                {
                    if (fixpoints.Count < 3)
                    {
                        List<IWritableNode> newFixpoints = [..fixpoints];
                        for (int i = fixpoints.Count; i < 3; i++)
                        {
                            newFixpoints.Add(
                                new Languages.Generated.V2024_1.Shapes.M2.Coord($"fixpoint-{i}")
                                {
                                    X = i, Y = i, Z = i
                                });
                        }

                        SetChildren(node, _v2024_1.IShape_fixpoints, newFixpoints);
                        result = true;
                    }
                }
            }

            return new MigrationResult(result, inputRootNodes);
        }
    }

    #endregion

    private static Circle Instance2023() =>
        new("c")
        {
            R = 5,
            Center = new Coord("cent") { X = 1, Y = 2, Z = 3 },
            Fixpoints = [new Coord("fp0") { X = 1 }, new Coord("fp1") { Y = 2 }],
            Name = "my circle",
            ShapeDocs = new Documentation("doc") { Technical = true },
            Uuid = "abc"
        };

    private async Task<bool> Migrate(LionWebVersions toVersion, IReadableNode inputNode, List<Language> languages,
        Stream outputStream, List<IMigration> migrations)
    {
        var fromVersion = LionWebVersions.v2023_1;

        var inputStream = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(inputStream,
            new SerializerBuilder().WithLionWebVersion(fromVersion).Build(),
            M1Extensions.Descendants(inputNode, true, true));
        inputStream.Seek(0, SeekOrigin.Begin);

        var migrator = new ModelMigrator(toVersion, languages);
        foreach (var migration in migrations)
        {
            migrator.RegisterMigration(migration);
        }

        var result = await migrator.MigrateAsync(inputStream, outputStream);
        outputStream.Seek(0, SeekOrigin.Begin);

        return result;
    }
}