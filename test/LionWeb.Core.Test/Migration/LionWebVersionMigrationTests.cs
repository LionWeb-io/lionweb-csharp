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

[TestClass]
public class LionWebVersionMigrationTests
{
    [TestMethod]
    public async Task Migrate2023_1_to_2024_1()
    {
        LionWebVersions inputVersion = LionWebVersions.v2023_1;
        LionWebVersions targetVersion = LionWebVersions.v2024_1_Compatible;

        var language = new DynamicLanguage("lang", inputVersion)
        {
            Name = "lang-name", Key = "lang-key", Version = "lang-version"
        };

        var iface = new DynamicInterface("iface", inputVersion, null) { Name = "iface-name", Key = "iface-key" };
        language.AddEntities([iface]);
        var propString = new DynamicProperty("propString", inputVersion, null)
        {
            Name = "propString-name", Key = "propString-key", Optional = true, Type = inputVersion.BuiltIns.String
        };
        iface.AddFeatures([propString]);
        iface.AddExtends([inputVersion.BuiltIns.INamed]);

        var concept = new DynamicConcept("concept", inputVersion, null) { Name = "concept-name", Key = "concept-key", };
        language.AddEntities([concept]);
        var enm = new DynamicEnumeration("enm", inputVersion, null) { Name = "enm-name", Key = "enm-key" };
        language.AddEntities([enm]);
        enm.AddLiterals([
            new DynamicEnumerationLiteral("litA", inputVersion, null) { Name = "litA-name", Key = "litA-key" },
            new DynamicEnumerationLiteral("litB", inputVersion, null) { Name = "litB-name", Key = "litB-key" }
        ]);
        var propEnum = new DynamicProperty("propEnum", inputVersion, null)
        {
            Name = "propEnum-name", Key = "propEnum-key", Optional = true, Type = enm
        };
        concept.AddFeatures([propEnum]);
        concept.AddImplements([inputVersion.BuiltIns.INamed]);

        var ann = new DynamicAnnotation("annotation", inputVersion, null)
        {
            Name = "annotation-name", Key = "annotation-key", Annotates = inputVersion.BuiltIns.Node
        };
        language.AddEntities([ann]);

        var propBool = new DynamicProperty("propBool", inputVersion, null)
        {
            Name = "propBool-name", Key = "propBool-key", Optional = true, Type = inputVersion.BuiltIns.Boolean
        };
        ann.AddFeatures([propBool]);
        ann.AddImplements([inputVersion.BuiltIns.INamed]);

        var factory = language.GetFactory();

        var conceptInstance = factory.CreateNode("conceptId", concept);
        conceptInstance.Set(propString, "conceptInstance");
        conceptInstance.Set(propEnum, factory.GetEnumerationLiteral(enm.Literals.First()));

        var annInstance = factory.CreateNode("annotationId", ann);
        annInstance.Set(propString, "annInstance");

        conceptInstance.AddAnnotations([annInstance]);

        annInstance.Set(propBool, true);

        var modelMigrator = new ModelMigrator(targetVersion, [language, inputVersion.BuiltIns, inputVersion.LionCore]);
        modelMigrator.RegisterMigration(new LionWebVersionMigration(inputVersion, targetVersion) { Priority = 0 });

        var input = new MemoryStream();
        JsonUtils.WriteNodesToStream(input, new Serializer(inputVersion), [conceptInstance, annInstance]);
        input.Seek(0, SeekOrigin.Begin);

        var output = new MemoryStream();
        modelMigrator.Migrate(input, output);
        output.Seek(0, SeekOrigin.Begin);

        var targetLanguages = new DynamicLanguageCloner(targetVersion).Clone([language]).Values;
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(targetVersion)
            .WithLanguages(targetLanguages)
            .Build();
        var nodes = await JsonUtils.ReadNodesFromStreamAsync(output, deserializer);
    }

    [TestMethod]
    [Ignore("LionWeb 2025.1 not yet supported")]
    public async Task Migrate2024_1_to_2025_1()
    {
        LionWebVersions inputVersion = LionWebVersions.v2023_1;
        LionWebVersions targetVersion = LionWebVersions.v2024_1_Compatible;

        var language = new DynamicLanguage("lang", inputVersion)
        {
            Name = "lang-name", Key = "lang-key", Version = "lang-version"
        };

        var iface = new DynamicInterface("iface", inputVersion, null) { Name = "iface-name", Key = "iface-key" };
        language.AddEntities([iface]);
        var propString = new DynamicProperty("propString", inputVersion, null)
        {
            Name = "propString-name", Key = "propString-key", Optional = true, Type = inputVersion.BuiltIns.String
        };
        iface.AddFeatures([propString]);
        iface.AddExtends([inputVersion.BuiltIns.INamed]);

        var concept = new DynamicConcept("concept", inputVersion, null) { Name = "concept-name", Key = "concept-key", };
        language.AddEntities([concept]);
        var enm = new DynamicEnumeration("enm", inputVersion, null) { Name = "enm-name", Key = "enm-key" };
        language.AddEntities([enm]);
        enm.AddLiterals([
            new DynamicEnumerationLiteral("litA", inputVersion, null) { Name = "litA-name", Key = "litA-key" },
            new DynamicEnumerationLiteral("litB", inputVersion, null) { Name = "litB-name", Key = "litB-key" }
        ]);
        var propEnum = new DynamicProperty("propEnum", inputVersion, null)
        {
            Name = "propEnum-name", Key = "propEnum-key", Optional = true, Type = enm
        };
        concept.AddFeatures([propEnum]);
        concept.AddImplements([inputVersion.BuiltIns.INamed]);

        var ann = new DynamicAnnotation("annotation", inputVersion, null)
        {
            Name = "annotation-name", Key = "annotation-key", Annotates = inputVersion.BuiltIns.Node
        };
        language.AddEntities([ann]);

        var nestedSdt =
            new DynamicStructuredDataType("nestedSdt", inputVersion, null)
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

        var sdt = new DynamicStructuredDataType("sdt", inputVersion, null) { Name = "sdt-name", Key = "sdt-key" };
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

        var propSdt = new DynamicProperty("propSdt", inputVersion, null)
        {
            Name = "propSdt-name", Key = "propSdt-key", Optional = true, Type = sdt
        };
        ann.AddFeatures([propSdt]);
        ann.AddImplements([inputVersion.BuiltIns.INamed]);

        var factory = language.GetFactory();

        var conceptInstance = factory.CreateNode("conceptId", concept);
        conceptInstance.Set(propString, "conceptInstance");
        conceptInstance.Set(propEnum, factory.GetEnumerationLiteral(enm.Literals.First()));

        var annInstance = factory.CreateNode("annotationId", ann);
        annInstance.Set(propString, "annInstance");

        conceptInstance.AddAnnotations([annInstance]);

        var nestedSdtInstance = factory.CreateStructuredDataTypeInstance(nestedSdt,
            new FieldValues([(nestedSdt.Fields.First(), enm.Literals.Last())]));

        var sdtInstance = factory.CreateStructuredDataTypeInstance(sdt,
            new FieldValues([(sdt.Fields.First(), 123), (sdt.Fields.Last(), nestedSdtInstance)]));
        annInstance.Set(propSdt, sdtInstance);

        var modelMigrator = new ModelMigrator(targetVersion, [language]);
        modelMigrator.RegisterMigration(new LionWebVersionMigration(inputVersion, targetVersion) { Priority = 0 });

        var input = new MemoryStream();
        JsonUtils.WriteNodesToStream(input, new Serializer(inputVersion), [conceptInstance, annInstance]);
        input.Seek(0, SeekOrigin.Begin);

        var output = new MemoryStream();
        modelMigrator.Migrate(input, output);
        output.Seek(0, SeekOrigin.Begin);

        var targetLanguages = new DynamicLanguageCloner(targetVersion).Clone([language]).Values;
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(targetVersion)
            .WithLanguages(targetLanguages)
            .Build();
        var nodes = await JsonUtils.ReadNodesFromStreamAsync(output, deserializer);
    }
}