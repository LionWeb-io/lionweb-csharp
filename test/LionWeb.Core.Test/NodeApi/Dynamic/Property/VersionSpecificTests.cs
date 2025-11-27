// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Test.NodeApi.Dynamic.Property;

using M2;
using M3;

[TestClass]
public class VersionSpecificTests
{
    int id = 0;

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void String(Type versionIface)
    {
        INode node = CreateNode(versionIface, out Classifier classifier);

        var feature = classifier.FeatureByKey("key-String");
        node.Set(feature, "Hi");

        Assert.AreEqual("Hi", node.Get(feature));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Integer(Type versionIface)
    {
        INode node = CreateNode(versionIface, out Classifier classifier);

        var feature = classifier.FeatureByKey("key-Integer");
        node.Set(feature, 42);

        Assert.AreEqual(42, node.Get(feature));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void Boolean(Type versionIface)
    {
        INode node = CreateNode(versionIface, out Classifier classifier);

        var feature = classifier.FeatureByKey("key-Boolean");
        node.Set(feature, true);

        Assert.AreEqual(true, node.Get(feature));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2024_1))]
    [DataRow(typeof(IVersion2024_1_Compatible))]
    public void StructuredDataType(Type versionIface)
    {
        INode node = CreateNode(versionIface, out Classifier classifier);

        var feature = classifier.FeatureByKey("key-sdt");
        var language = classifier.GetLanguage();
        var sdt = language.Entities.OfType<StructuredDataType>().First();
        var sdtInstance = language.GetFactory().CreateStructuredDataTypeInstance(sdt, new FieldValues([(sdt.Fields.First(), "hello")]));
        node.Set(feature, sdtInstance);

        var expected = language.GetFactory().CreateStructuredDataTypeInstance(sdt, new FieldValues([(sdt.Fields.First(), "hello")]));
        Assert.AreEqual(expected, node.Get(feature));
    }

    [TestMethod]
    [DataRow(typeof(IVersion2023_1))]
    public void Json(Type versionIface)
    {
        INode node = CreateNode(versionIface, out Classifier classifier);

        var feature = classifier.FeatureByKey("key-JSON");
        node.Set(feature, "Hi");

        Assert.AreEqual("Hi", node.Get(feature));
    }

    private INode CreateNode(Type versionIface, out Classifier classifier)
    {
        var language = CreateLanguage(LionWebVersions.GetByInterface(versionIface));

        classifier = language.Entities.OfType<Classifier>().First();
        var node = language.GetFactory().CreateNode(NextId("node"), classifier);
        return node;
    }

    private DynamicLanguage CreateLanguage(LionWebVersions lionWebVersion)
    {
        var idBase = lionWebVersion.VersionString.Replace('.', '_');

        var language =
            new DynamicLanguage(NextId(idBase), lionWebVersion)
            {
                Key = "key-myLanguage", Version = "1", Name = "myLanguage"
            };
        var myConcept = language.Concept(NextId(idBase), "key-myConcept", "myConcept");

        foreach (var primitiveType in lionWebVersion.BuiltIns.Entities.OfType<PrimitiveType>())
        {
            myConcept
                .Property($"id-{primitiveType.Name}", $"key-{primitiveType.Name}", $"prop{primitiveType.Name}")
                .OfType(primitiveType)
                .IsOptional(true);
        }

        if (lionWebVersion.LionCore is ILionCoreLanguageWithStructuredDataType)
        {
            var sdt = language.StructuredDataType(NextId(idBase), "key-mySdt", "mySdt");
            var field = sdt.Field(NextId(idBase), "key-myField", "myField")
                .OfType(lionWebVersion.BuiltIns.String);
            myConcept
                .Property("id-sdt", "key-sdt", "propSdt")
                .OfType(sdt)
                .IsOptional(true);
        }

        return language;
    }

    private string NextId(string idBase) => idBase + id++;
}