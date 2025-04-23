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
using System.Text;

[TestClass]
public class ModelMigratorTests
{
    #region MaxRounds

    [TestMethod]
    public async Task MaxRounds()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, []) { MaxMigrationRounds = 17 };
        migrator.RegisterMigration(new InfiniteMigration());

        await Assert.ThrowsExceptionAsync<MaxMigrationRoundsExceededException>(() =>
            migrator.Migrate(inputStream, Stream.Null));
    }

    private class InfiniteMigration : IMigration
    {
        public int Priority => 0;
        public void Initialize(ILanguageRegistry languageRegistry) { }

        public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) => true;

        public MigrationResult Migrate(List<LenientNode> inputRootNodes) => new(true, inputRootNodes);
    }
    
    #endregion

    #region NoMigrations

    [TestMethod]
    public async Task NoMigrations()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, [])
        {
            SerializerBuilder = new SerializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1)
                .WithSerializedEmptyFeatures(false)
        };

        var outputStream = new MemoryStream();
        var migrated = await migrator.Migrate(inputStream, outputStream);
        Assert.IsFalse(migrated);
        
        var resultNodes = await Deserialize(outputStream);
        Assert.AreEqual(1, resultNodes.Count);
    }

    #endregion

    #region NotApplicable

    [TestMethod]
    public async Task NotApplicable()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, []);
        var migration = new NotApplicableMigration();
        migrator.RegisterMigration(migration);

        var outputStream = new MemoryStream();
        var migrated = await migrator.Migrate(inputStream, outputStream);
        Assert.IsFalse(migrated);
        Assert.IsFalse(migration.Executed);
        
        var resultNodes = await Deserialize(outputStream);
        Assert.AreEqual(1, resultNodes.Count);
    }

    private class NotApplicableMigration : IMigration
    {
        public bool Executed { get; set; } = false;
        
        public int Priority => 0;
        public void Initialize(ILanguageRegistry languageRegistry) { }

        public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) => false;

        public MigrationResult Migrate(List<LenientNode> inputRootNodes)
        {
            Executed = true;
            return new MigrationResult(true, inputRootNodes);
        }
    }

    #endregion

    #region SerializerDeserializer

    [TestMethod]
    public void InvalidSerializerBuilder()
    {
        Assert.ThrowsException<VersionMismatchException>(() =>
            new ModelMigrator(LionWebVersions.v2023_1, [])
            {
                SerializerBuilder = new SerializerBuilder().WithLionWebVersion(LionWebVersions.v2024_1)
            });
    }

    [TestMethod]
    public void InvalidDeserializerBuilder()
    {
        Assert.ThrowsException<VersionMismatchException>(() =>
            new ModelMigrator(LionWebVersions.v2023_1, [])
            {
                DeserializerBuilder = new DeserializerBuilder().WithLionWebVersion(LionWebVersions.v2024_1)
            });
    }

    [TestMethod]
    public void DeserializerSerializerMismatch()
    {
        Assert.ThrowsException<VersionMismatchException>(() =>
            new ModelMigrator(LionWebVersions.v2024_1_Compatible, [])
            {
                DeserializerBuilder = new DeserializerBuilder().WithLionWebVersion(LionWebVersions.v2024_1),
                SerializerBuilder = new SerializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1)
            });
    }

    [TestMethod]
    public void SerializerDeserializerMismatch()
    {
        Assert.ThrowsException<VersionMismatchException>(() =>
            new ModelMigrator(LionWebVersions.v2024_1_Compatible, [])
            {
                SerializerBuilder = new SerializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1),
                DeserializerBuilder = new DeserializerBuilder().WithLionWebVersion(LionWebVersions.v2024_1)
            });
    }

    [TestMethod]
    public async Task UseCustomSerializer()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, [])
        {
            SerializerBuilder = new SerializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1)
                .WithSerializedEmptyFeatures(false)
        };
        migrator.RegisterMigration(new OnceMigration());

        var outputStream = new MemoryStream();
        var migrated = await migrator.Migrate(inputStream, outputStream);
        Assert.IsTrue(migrated);
        
        var resultNodes = await Deserialize(outputStream);
        Assert.AreEqual(1, resultNodes.Count);

        // We shouldn't serialize unset features
        var output = Encoding.UTF8.GetString(outputStream.ToArray());
        Assert.IsFalse(output.Contains("\"name\":"), output);
    }

    [TestMethod]
    public async Task UseCustomDeserializer()
    {
        var refTarget = new Circle("circle");
        var input = new ReferenceGeometry("ref") { Shapes = [refTarget] };

        MemoryStream inputStream = await Serialize(input);

        var deserializerBuilder = new DeserializerBuilder()
            .WithLionWebVersion(LionWebVersions.v2023_1)
            .WithDependentNodes([refTarget]);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, []) { DeserializerBuilder = deserializerBuilder };
        migrator.RegisterMigration(new OnceMigration());

        var outputStream = new MemoryStream();
        var migrated = await migrator.Migrate(inputStream, outputStream);
        Assert.IsTrue(migrated);

        outputStream.Seek(0, SeekOrigin.Begin);
        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            deserializerBuilder.WithLanguage(ShapesLanguage.Instance).Build());

        Assert.AreEqual(1, resultNodes.Count);
        Assert.AreSame(refTarget, resultNodes.OfType<ReferenceGeometry>().First().Shapes[0]);
    }

    private class OnceMigration : IMigration
    {
        private bool migrated = false;
        public int Priority => 0;
        public void Initialize(ILanguageRegistry languageRegistry) { }

        public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) => !migrated;

        public MigrationResult Migrate(List<LenientNode> inputRootNodes)
        {
            migrated = true;
            return new MigrationResult(true, inputRootNodes);
        }
    }

    #endregion

    #region SerializedLionWebVersion

    [TestMethod]
    public async Task ReceiveSerializedLionWebVersion()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, []);
        var migration = new SerializedLionWebVersionMigration();
        
        Assert.IsNull(migration.SerializedLionWebVersion);
        
        migrator.RegisterMigration(migration);
        var migrated = await migrator.Migrate(inputStream, Stream.Null);
        Assert.IsTrue(migrated);
        
        Assert.AreEqual(LionWebVersions.v2023_1.VersionString, migration.SerializedLionWebVersion);
    }

    private class SerializedLionWebVersionMigration : IMigrationWithLionWebVersion
    {
        private bool migrated = false;
        public int Priority => 0;
        public void Initialize(ILanguageRegistry languageRegistry) { }

        public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) => !migrated;

        public MigrationResult Migrate(List<LenientNode> inputRootNodes)
        {
            migrated = true;
            return new MigrationResult(true, inputRootNodes);
        }

        public string SerializedLionWebVersion { get; set; }
    }

    #endregion

    #region RootNodes

    [TestMethod]
    public async Task NullRootNodes()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, []);

        migrator.RegisterMigration(new RootNodesMigration([new LenientNode("a", ShapesLanguage.Instance.Line), null]));
        
        await Assert.ThrowsExceptionAsync<InvalidRootNodesException>(() =>
            migrator.Migrate(inputStream, Stream.Null));
    }

    [TestMethod]
    public async Task NonRootNodes()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, []);

        var parent = new LenientNode("a", ShapesLanguage.Instance.Line);
        var child = new LenientNode("b", ShapesLanguage.Instance.Coord);
        parent.Set(ShapesLanguage.Instance.Line_start, child);
        
        migrator.RegisterMigration(new RootNodesMigration([parent, child]));
        
        await Assert.ThrowsExceptionAsync<InvalidRootNodesException>(() =>
            migrator.Migrate(inputStream, Stream.Null));
    }

    [TestMethod]
    public async Task DuplicateRootNodeIds()
    {
        var input = new Circle("circle");
        MemoryStream inputStream = await Serialize(input);

        var migrator = new ModelMigrator(LionWebVersions.v2023_1, []);

        var parent = new LenientNode("a", ShapesLanguage.Instance.Line);
        var child = new LenientNode("a", ShapesLanguage.Instance.Coord);
        
        migrator.RegisterMigration(new RootNodesMigration([parent, child]));
        
        await Assert.ThrowsExceptionAsync<InvalidRootNodesException>(() =>
            migrator.Migrate(inputStream, Stream.Null));
    }

    private class RootNodesMigration(List<LenientNode> rootNodes) : IMigration
    {
        private bool migrated = false;
        public int Priority => 0;
        public void Initialize(ILanguageRegistry languageRegistry) { }

        public bool IsApplicable(ISet<LanguageIdentity> languageIdentities) => !migrated;

        public MigrationResult Migrate(List<LenientNode> inputRootNodes)
        {
            migrated = true;
            return new MigrationResult(true, rootNodes);
        }
    }

    #endregion

    private static async Task<MemoryStream> Serialize(IReadableNode input)
    {
        var inputStream = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(inputStream,
            new SerializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1).Build(), [input]);
        inputStream.Seek(0, SeekOrigin.Begin);
        return inputStream;
    }

    private static async Task<List<IReadableNode>> Deserialize(MemoryStream outputStream)
    {
        outputStream.Seek(0, SeekOrigin.Begin);
        var resultNodes = await JsonUtils.ReadNodesFromStreamAsync(outputStream,
            new DeserializerBuilder().WithLionWebVersion(LionWebVersions.v2023_1).WithLanguage(ShapesLanguage.Instance)
                .Build());
        return resultNodes;
    }
}