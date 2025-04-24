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
using M1;
using M2;
using M3;
using System.Text;

[TestClass]
public class LionWebVersionMigrationTests
{
    private TestContext testContextInstance;

    [TestMethod]
    public async Task Migrate2023_1_to_2024_1()
    {
        LionWebVersions inputVersion = LionWebVersions.v2023_1;
        LionWebVersions targetVersionCompatible = LionWebVersions.v2024_1_Compatible;
        LionWebVersions targetVersion = LionWebVersions.v2024_1;

        DynamicLanguage language = CreateLanguage2023(inputVersion, out DynamicInterface iface,
            out DynamicConcept concept, out DynamicAnnotation ann, out DynamicEnumeration enm,
            out DynamicProperty propString, out DynamicProperty propEnum, out DynamicProperty propBool);

        var nodes = CreateInstances2023(language, concept, propString, propEnum, enm, ann, propBool);

        MemoryStream output = await Migrate(targetVersionCompatible, language, inputVersion, nodes);

        var targetLanguage =
            CreateLanguage2024(targetVersion, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(targetVersion)
            .WithLanguage(targetLanguage)
            .Build();
        var deserializedNodes = await JsonUtils.ReadNodesFromStreamAsync(output, deserializer);

        Assert.AreEqual(nodes.Count(n => n.GetParent() == null), deserializedNodes.Count);
        Assert.IsTrue(deserializedNodes.All(n => n.GetClassifier().GetLanguage().LionWebVersion == targetVersion));
    }

    [TestMethod]
    public async Task Migrate2023_1_to_2025_1()
    {
        LionWebVersions inputVersion = LionWebVersions.v2023_1;
        LionWebVersions targetVersionCompatible = LionWebVersions.v2025_1_Compatible;
        LionWebVersions targetVersion = LionWebVersions.v2025_1;

        DynamicLanguage language = CreateLanguage2023(inputVersion, out DynamicInterface iface,
            out DynamicConcept concept, out DynamicAnnotation ann, out DynamicEnumeration enm,
            out DynamicProperty propString, out DynamicProperty propEnum, out DynamicProperty propBool);

        var nodes = CreateInstances2023(language, concept, propString, propEnum, enm, ann, propBool);

        MemoryStream output = await Migrate(targetVersionCompatible, language, inputVersion, nodes);

        var targetLanguage =
            CreateLanguage2024(targetVersion, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(targetVersion)
            .WithLanguage(targetLanguage)
            .Build();
        var deserializedNodes = await JsonUtils.ReadNodesFromStreamAsync(output, deserializer);

        Assert.AreEqual(nodes.Count(n => n.GetParent() == null), deserializedNodes.Count);
        Assert.IsTrue(deserializedNodes.All(n => n.GetClassifier().GetLanguage().LionWebVersion == targetVersion));
    }

    [TestMethod]
    public async Task Migrate2024_1_to_2025_1()
    {
        LionWebVersions inputVersion = LionWebVersions.v2024_1;
        LionWebVersions targetVersionCompatible = LionWebVersions.v2025_1_Compatible;
        LionWebVersions targetVersion = LionWebVersions.v2025_1;

        DynamicLanguage language = CreateLanguage2024(inputVersion, out DynamicInterface iface,
            out DynamicProperty propString, out DynamicConcept concept, out DynamicEnumeration enm,
            out DynamicProperty propEnum, out DynamicAnnotation ann, out DynamicStructuredDataType nestedSdt,
            out DynamicStructuredDataType sdt, out DynamicProperty propSdt, out _);

        var factory = language.GetFactory();

        var nodes = CreateInstances2024(inputVersion, factory, concept, propString, propEnum, enm, ann, nestedSdt, sdt, propSdt);

        MemoryStream output = await Migrate(targetVersionCompatible, language, inputVersion, nodes);

        var targetLanguage =
            CreateLanguage2024(targetVersion, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(targetVersion)
            .WithLanguage(targetLanguage)
            .Build();
        var deserializedNodes = await JsonUtils.ReadNodesFromStreamAsync(output, deserializer);

        Assert.AreEqual(nodes.Count(n => n.GetParent() == null), deserializedNodes.Count);
        Assert.IsTrue(deserializedNodes.All(n => n.GetClassifier().GetLanguage().LionWebVersion == targetVersion));
    }

    private async Task<MemoryStream> Migrate(LionWebVersions targetVersionCompatible, DynamicLanguage language,
        LionWebVersions inputVersion, List<INode> nodes)
    {
        var modelMigrator = new ModelMigrator(targetVersionCompatible, [language]);
        modelMigrator.RegisterMigration(
            new LionWebVersionMigration(inputVersion, targetVersionCompatible) { Priority = 0 });

        var input = new MemoryStream();
        await JsonUtils.WriteNodesToStreamAsync(input, new Serializer(inputVersion), nodes);
        input.Seek(0, SeekOrigin.Begin);

        TestContext.WriteLine("serialized before migration:");
        TestContext.WriteLine(Encoding.UTF8.GetString(input.GetBuffer()));

        var output = new MemoryStream();
        await modelMigrator.Migrate(input, output);
        output.Seek(0, SeekOrigin.Begin);
        TestContext.WriteLine("serialized after migration:");
        TestContext.WriteLine(Encoding.UTF8.GetString(output.GetBuffer()));
        return output;
    }

    private static List<INode> CreateInstances2024(LionWebVersions lionWebVersion, INodeFactory factory,
        DynamicConcept concept, DynamicProperty propString, DynamicProperty propEnum, DynamicEnumeration enm,
        DynamicAnnotation ann, DynamicStructuredDataType nestedSdt, DynamicStructuredDataType sdt,
        DynamicProperty propSdt)
    {
        var conceptInstance = factory.CreateNode("conceptId", concept);
        conceptInstance.Set(propString, "conceptInstance");
        conceptInstance.Set(propEnum, factory.GetEnumerationLiteral(enm.Literals.First()));
        conceptInstance.Set(lionWebVersion.BuiltIns.INamed_name, "my concept instance");

        var annInstance = factory.CreateNode("annotationId", ann);
        annInstance.Set(propString, "annInstance");

        conceptInstance.AddAnnotations([annInstance]);

        var nestedSdtInstance = factory.CreateStructuredDataTypeInstance(nestedSdt,
            new FieldValues([(nestedSdt.Fields.First(), factory.GetEnumerationLiteral(enm.Literals.Last()))]));

        var sdtInstance = factory.CreateStructuredDataTypeInstance(sdt,
            new FieldValues([(sdt.Fields.First(), 123), (sdt.Fields.Last(), nestedSdtInstance)]));
        annInstance.Set(propSdt, sdtInstance);
        return [conceptInstance, annInstance];
    }

    private static DynamicLanguage CreateLanguage2024(LionWebVersions lionWebVersion, out DynamicInterface iface,
        out DynamicProperty propString, out DynamicConcept concept, out DynamicEnumeration enm,
        out DynamicProperty propEnum, out DynamicAnnotation ann, out DynamicStructuredDataType nestedSdt,
        out DynamicStructuredDataType sdt, out DynamicProperty propSdt, out DynamicProperty propBool)
    {
        var language = new DynamicLanguage("lang", lionWebVersion)
        {
            Name = "lang-name", Key = "lang-key", Version = "lang-version"
        };

        iface = language.Interface("iface", "iface-key", "iface-name").Extending(lionWebVersion.BuiltIns.INamed);
        propString = iface.Property("propString", "propString-key", "propString-name").IsOptional()
            .OfType(lionWebVersion.BuiltIns.String);

        concept = language.Concept("concept", "concept-key", "concept-name")
            .Implementing(lionWebVersion.BuiltIns.INamed);

        enm = language.Enumeration("enm", "enm-key", "enm-name");
        enm.EnumerationLiteral("litA", "litA-key", "litA-name");
        enm.EnumerationLiteral("litB", "litB-key", "litB-name");

        propEnum = concept.Property("propEnum", "propEnum-key", "propEnum-name").IsOptional().OfType(enm);

        ann = language.Annotation("annotation", "annotation-key", "annotation-name")
            .Annotating(lionWebVersion.BuiltIns.Node).Implementing(lionWebVersion.BuiltIns.INamed);

        propBool = ann.Property("propBool", "propBool-key", "propBool-name").IsOptional()
            .OfType(lionWebVersion.BuiltIns.Boolean);

        nestedSdt = language.StructuredDataType("nestedSdt", "nestedSdt-key", "nestedSdt-name");
        nestedSdt.Field("enumField", "enumField-key", "enumField-name").OfType(enm);

        sdt = language.StructuredDataType("sdt", "sdt-key", "sdt-name");
        sdt.Field("intField", "intField-key", "intField-name").OfType(lionWebVersion.BuiltIns.Integer);
        sdt.Field("nestField", "nestField-key", "nestField-name").OfType(nestedSdt);

        propSdt = ann.Property("propSdt", "propSdt-key", "propSdt-name").IsOptional().OfType(sdt);

        return language;
    }

    private static DynamicLanguage CreateLanguage2023(LionWebVersions lionWebVersion, out DynamicInterface iface,
        out DynamicConcept concept, out DynamicAnnotation ann, out DynamicEnumeration enm,
        out DynamicProperty propString, out DynamicProperty propEnum, out DynamicProperty propBool)
    {
        var language = new DynamicLanguage("lang", lionWebVersion)
        {
            Name = "lang-name", Key = "lang-key", Version = "lang-version"
        };

        iface = language.Interface("iface", "iface-key", "iface-name").Extending(lionWebVersion.BuiltIns.INamed);
        propString = iface.Property("propString", "propString-key", "propString-name").IsOptional()
            .OfType(lionWebVersion.BuiltIns.String);

        concept = language.Concept("concept", "concept-key", "concept-name")
            .Implementing(lionWebVersion.BuiltIns.INamed);

        enm = language.Enumeration("enm", "enm-key", "enm-name");
        enm.EnumerationLiteral("litA", "litA-key", "litA-name");
        enm.EnumerationLiteral("litB", "litB-key", "litB-name");

        propEnum = concept.Property("propEnum", "propEnum-key", "propEnum-name").IsOptional().OfType(enm);

        ann = language.Annotation("annotation", "annotation-key", "annotation-name")
            .Annotating(lionWebVersion.BuiltIns.Node).Implementing(lionWebVersion.BuiltIns.INamed);

        propBool = ann.Property("propBool", "propBool-key", "propBool-name").IsOptional()
            .OfType(lionWebVersion.BuiltIns.Boolean);

        return language;
    }

    private static List<INode> CreateInstances2023(DynamicLanguage language, DynamicConcept concept,
        DynamicProperty propString,
        DynamicProperty propEnum, DynamicEnumeration enm, DynamicAnnotation ann, DynamicProperty propBool)
    {
        var factory = language.GetFactory();

        var conceptInstance = factory.CreateNode("conceptId", concept);
        conceptInstance.Set(propString, "conceptInstance");
        conceptInstance.Set(propEnum, factory.GetEnumerationLiteral(enm.Literals.First()));
        conceptInstance.Set(LionWebVersions.v2023_1.BuiltIns.INamed_name, "my concept instance");

        var annInstance = factory.CreateNode("annotationId", ann);
        annInstance.Set(propString, "annInstance");

        conceptInstance.AddAnnotations([annInstance]);

        annInstance.Set(propBool, true);
        return [conceptInstance, annInstance];
    }

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext
    {
        get { return testContextInstance; }
        set { testContextInstance = value; }
    }
}