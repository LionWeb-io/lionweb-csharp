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

        DynamicLanguage language = CreateLanguage2023(inputVersion, out DynamicInterface iface, out DynamicConcept concept, out DynamicAnnotation ann, out DynamicEnumeration enm, out DynamicProperty propString, out DynamicProperty propEnum, out DynamicProperty propBool);

        var nodes = CreateInstances2023(language, concept, propString, propEnum, enm, ann, propBool);

        MemoryStream output = Migrate(targetVersionCompatible, language, inputVersion, nodes);

        var targetLanguage = CreateLanguage2023(targetVersion, out _,out _,out _,out _,out _,out _,out _);
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

        DynamicLanguage language = CreateLanguage2023(inputVersion, out DynamicInterface iface, out DynamicConcept concept, out DynamicAnnotation ann, out DynamicEnumeration enm, out DynamicProperty propString, out DynamicProperty propEnum, out DynamicProperty propBool);

        var nodes = CreateInstances2023(language, concept, propString, propEnum, enm, ann, propBool);

        MemoryStream output = Migrate(targetVersionCompatible, language, inputVersion, nodes);

        var targetLanguage = CreateLanguage2023(targetVersion, out _,out _,out _,out _,out _,out _,out _);
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

        DynamicLanguage language = CreateLanguage2024(inputVersion, out DynamicInterface iface, out DynamicProperty propString, out DynamicConcept concept, out DynamicEnumeration enm, out DynamicProperty propEnum, out DynamicAnnotation ann, out DynamicStructuredDataType nestedSdt, out DynamicStructuredDataType sdt, out DynamicProperty propSdt);

        var factory = language.GetFactory();

        var  nodes = CreateInstances2024(factory, concept, propString, propEnum, enm, ann, nestedSdt, sdt, propSdt);

        MemoryStream output = Migrate(targetVersionCompatible, language, inputVersion, nodes);

        var targetLanguage = CreateLanguage2024(targetVersion, out _,out _,out _,out _,out _,out _,out _,out _,out _);
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(targetVersion)
            .WithLanguage(targetLanguage)
            .Build();
        var deserializedNodes = await JsonUtils.ReadNodesFromStreamAsync(output, deserializer);
        
        Assert.AreEqual(nodes.Count(n => n.GetParent() == null), deserializedNodes.Count);
        Assert.IsTrue(deserializedNodes.All(n => n.GetClassifier().GetLanguage().LionWebVersion == targetVersion));
    }

    private MemoryStream Migrate(LionWebVersions targetVersionCompatible, DynamicLanguage language,
        LionWebVersions inputVersion, List<INode> nodes)
    {
        var modelMigrator = new ModelMigrator(targetVersionCompatible, [language, inputVersion.BuiltIns, inputVersion.LionCore, targetVersionCompatible.BuiltIns, targetVersionCompatible.LionCore]);
        modelMigrator.RegisterMigration(new LionWebVersionMigration(inputVersion, targetVersionCompatible) { Priority = 0 });

        var input = new MemoryStream();
        JsonUtils.WriteNodesToStream(input, new Serializer(inputVersion), nodes);
        input.Seek(0, SeekOrigin.Begin);

        TestContext.WriteLine("serialized before migration:");
        TestContext.WriteLine(Encoding.UTF8.GetString(input.GetBuffer()));

        var output = new MemoryStream();
        modelMigrator.Migrate(input, output);
        output.Seek(0, SeekOrigin.Begin);
        TestContext.WriteLine("serialized after migration:");
        TestContext.WriteLine(Encoding.UTF8.GetString(output.GetBuffer()));
        return output;
    }

    private static List<INode> CreateInstances2024(INodeFactory factory, DynamicConcept concept, DynamicProperty propString,
        DynamicProperty propEnum, DynamicEnumeration enm, DynamicAnnotation ann, DynamicStructuredDataType nestedSdt,
        DynamicStructuredDataType sdt, DynamicProperty propSdt)
    {
        var conceptInstance = factory.CreateNode("conceptId", concept);
        conceptInstance.Set(propString, "conceptInstance");
        conceptInstance.Set(propEnum, factory.GetEnumerationLiteral(enm.Literals.First()));

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

    private static DynamicLanguage CreateLanguage2024(LionWebVersions inputVersion, out DynamicInterface iface,
        out DynamicProperty propString, out DynamicConcept concept, out DynamicEnumeration enm,
        out DynamicProperty propEnum, out DynamicAnnotation ann, out DynamicStructuredDataType nestedSdt,
        out DynamicStructuredDataType sdt, out DynamicProperty propSdt)
    {
        var language = new DynamicLanguage("lang", inputVersion)
        {
            Name = "lang-name", Key = "lang-key", Version = "lang-version"
        };

        iface = new DynamicInterface("iface", inputVersion, null) { Name = "iface-name", Key = "iface-key" };
        language.AddEntities([iface]);
        propString = new DynamicProperty("propString", inputVersion, null)
        {
            Name = "propString-name", Key = "propString-key", Optional = true, Type = inputVersion.BuiltIns.String
        };
        iface.AddFeatures([propString]);
        iface.AddExtends([inputVersion.BuiltIns.INamed]);

        concept = new DynamicConcept("concept", inputVersion, null) { Name = "concept-name", Key = "concept-key", };
        language.AddEntities([concept]);
        enm = new DynamicEnumeration("enm", inputVersion, null) { Name = "enm-name", Key = "enm-key" };
        language.AddEntities([enm]);
        enm.AddLiterals([
            new DynamicEnumerationLiteral("litA", inputVersion, null) { Name = "litA-name", Key = "litA-key" },
            new DynamicEnumerationLiteral("litB", inputVersion, null) { Name = "litB-name", Key = "litB-key" }
        ]);
        propEnum = new DynamicProperty("propEnum", inputVersion, null)
        {
            Name = "propEnum-name", Key = "propEnum-key", Optional = true, Type = enm
        };
        concept.AddFeatures([propEnum]);
        concept.AddImplements([inputVersion.BuiltIns.INamed]);

        ann = new DynamicAnnotation("annotation", inputVersion, null)
        {
            Name = "annotation-name", Key = "annotation-key", Annotates = inputVersion.BuiltIns.Node
        };
        language.AddEntities([ann]);

        nestedSdt = new DynamicStructuredDataType("nestedSdt", inputVersion, null)
        {
            Name = "nestedSdt-name", Key = "nestedSdt-key"
        };
        nestedSdt.AddFields([
            new DynamicField("enumField", inputVersion, null)
            {
                Name = "enumField-name", Key = "enumField-key", Type = enm
            }
        ]);
        language.AddEntities([nestedSdt]);

        sdt = new DynamicStructuredDataType("sdt", inputVersion, null) { Name = "sdt-name", Key = "sdt-key" };
        sdt.AddFields([
            new DynamicField("intField", inputVersion, null)
            {
                Name = "intField-name", Key = "intField-key", Type = inputVersion.BuiltIns.Integer
            },
            new DynamicField("nestField", inputVersion, null)
            {
                Name = "nestField-name", Key = "nestField-key", Type = nestedSdt
            }
        ]);
        language.AddEntities([sdt]);

        propSdt = new DynamicProperty("propSdt", inputVersion, null)
        {
            Name = "propSdt-name", Key = "propSdt-key", Optional = true, Type = sdt
        };
        ann.AddFeatures([propSdt]);
        ann.AddImplements([inputVersion.BuiltIns.INamed]);
        return language;
    }

    private static DynamicLanguage CreateLanguage2023(LionWebVersions inputVersion, out DynamicInterface iface,
        out DynamicConcept concept, out DynamicAnnotation ann, out DynamicEnumeration enm, out DynamicProperty propString,
        out DynamicProperty propEnum, out DynamicProperty propBool)
    {
        var language = new DynamicLanguage("lang", inputVersion)
        {
            Name = "lang-name", Key = "lang-key", Version = "lang-version"
        };

        iface = new DynamicInterface("iface", inputVersion, null) { Name = "iface-name", Key = "iface-key" };
        language.AddEntities([iface]);
        propString = new DynamicProperty("propString", inputVersion, null)
        {
            Name = "propString-name", Key = "propString-key", Optional = true, Type = inputVersion.BuiltIns.String
        };
        iface.AddFeatures([propString]);
        iface.AddExtends([inputVersion.BuiltIns.INamed]);

        concept = new DynamicConcept("concept", inputVersion, null) { Name = "concept-name", Key = "concept-key", };
        language.AddEntities([concept]);
        enm = new DynamicEnumeration("enm", inputVersion, null) { Name = "enm-name", Key = "enm-key" };
        language.AddEntities([enm]);
        enm.AddLiterals([
            new DynamicEnumerationLiteral("litA", inputVersion, null) { Name = "litA-name", Key = "litA-key" },
            new DynamicEnumerationLiteral("litB", inputVersion, null) { Name = "litB-name", Key = "litB-key" }
        ]);
        propEnum = new DynamicProperty("propEnum", inputVersion, null)
        {
            Name = "propEnum-name", Key = "propEnum-key", Optional = true, Type = enm
        };
        concept.AddFeatures([propEnum]);
        concept.AddImplements([inputVersion.BuiltIns.INamed]);

        ann = new DynamicAnnotation("annotation", inputVersion, null)
        {
            Name = "annotation-name", Key = "annotation-key", Annotates = inputVersion.BuiltIns.Node
        };
        language.AddEntities([ann]);

        propBool = new DynamicProperty("propBool", inputVersion, null)
        {
            Name = "propBool-name", Key = "propBool-key", Optional = true, Type = inputVersion.BuiltIns.Boolean
        };
        ann.AddFeatures([propBool]);
        ann.AddImplements([inputVersion.BuiltIns.INamed]);
        return language;
    }

    private static List<INode> CreateInstances2023(DynamicLanguage language, DynamicConcept concept, DynamicProperty propString,
        DynamicProperty propEnum, DynamicEnumeration enm, DynamicAnnotation ann, DynamicProperty propBool)
    {
        var factory = language.GetFactory();

        var conceptInstance = factory.CreateNode("conceptId", concept);
        conceptInstance.Set(propString, "conceptInstance");
        conceptInstance.Set(propEnum, factory.GetEnumerationLiteral(enm.Literals.First()));

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