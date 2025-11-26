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

namespace LionWeb.Core.Test.Serialization.LionWebVersion;

using Core.Serialization;
using Core.Utilities;
using M1;
using M2;
using M3;
using LionWebVersions = LionWebVersions;

public abstract class LionWebVersionsTestBase
{
    int id = 0;

    protected List<IReadableNode> Deserialize(DynamicLanguage language, LionWebVersions lionWebVersion,
        SerializationChunk chunk)
    {
        var deserializer = new DeserializerBuilder()
            .WithLanguage(language)
            .WithLionWebVersion(lionWebVersion)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();
        List<IReadableNode> nodes = deserializer.Deserialize(chunk);
        return nodes;
    }

    protected DynamicLanguage Serialize(LionWebVersions lionWebVersion, out INode conceptNodeA, out INode conceptNodeB,
        out SerializationChunk chunk)
    {
        DynamicLanguage language = CreateInstances(lionWebVersion, out conceptNodeA, out conceptNodeB,
            out List<IReadableNode> input);

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(lionWebVersion)
            .WithCompressedIds(new(KeepOriginal: true))
            .Build();

        chunk = serializer.SerializeToChunk(input);
        return language;
    }

    protected DynamicLanguage CreateInstances(LionWebVersions lionWebVersion, out INode conceptNodeA,
        out INode conceptNodeB,
        out List<IReadableNode> input)
    {
        var language = CreateLanguage(lionWebVersion.VersionString.Replace(".", "_"), lionWebVersion);
        var myConcept = language.ClassifierByKey("key-myConcept") as Concept;
        var myAnnotation = language.ClassifierByKey("key-myAnnotation") as Annotation;

        var factory = language.GetFactory();

        conceptNodeA = factory.CreateNode(NextId("n"), myConcept);
        conceptNodeA.Set(lionWebVersion.BuiltIns.INamed_name, "A");

        conceptNodeB = factory.CreateNode(NextId("n"), myConcept);
        conceptNodeB.Set(lionWebVersion.BuiltIns.INamed_name, "B");
        conceptNodeB.Set(myConcept.FeatureByKey("key-myRef"), conceptNodeA);

        var annotation = factory.CreateNode(NextId("n"), myAnnotation);
        conceptNodeA.AddAnnotations([annotation]);

        input = [conceptNodeA, conceptNodeB, annotation];
        return language;
    }

    protected DynamicLanguage CreateLanguage(string idBase, LionWebVersions lionWebVersion)
    {
        var language =
            new DynamicLanguage(NextId(idBase), lionWebVersion)
            {
                Key = "key-myLanguage", Version = "1", Name = "myLanguage"
            };
        var myConcept = language.Concept(NextId(idBase), "key-myConcept", "myConcept")
            .Implementing(lionWebVersion.BuiltIns.INamed);
        var myAnnotation = language.Annotation(NextId(idBase), "key-myAnnotation", "myAnnotation")
            .Annotating(myConcept);
        myConcept.Reference(NextId(idBase), "key-myRef", "myRef")
            .OfType(lionWebVersion.BuiltIns.Node)
            .IsOptional(true)
            .IsMultiple(false);

        return language;
    }

    protected string NextId(string idBase) => idBase + id++;

    protected static void AssertEquals(List<IReadableNode?> expected, List<IReadableNode> nodes)
    {
        var differences = new Comparer(expected, nodes!).Compare().ToList();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
    }

    protected static void AssertEquals(DynamicLanguage language, List<IReadableNode> deserialized)
    {
        var differences = new Comparer([language], deserialized!).Compare().ToList();
        Assert.IsFalse(differences.Any(), differences.DescribeAll(new()));
    }

    /// <summary>
    /// Gets or sets the test context which provides
    /// information about and functionality for the current test run.
    /// </summary>
    public TestContext TestContext { get; set; }
}