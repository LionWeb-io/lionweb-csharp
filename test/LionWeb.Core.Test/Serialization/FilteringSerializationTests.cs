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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.Test.Serialization;

using Core.Utilities;
using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class FilteringSerializationTests : SerializationTestsBase
{
    [TestMethod]
    public void String()
    {
        var node = new LinkTestConcept("n") { Name = "hello" };

        var serializationChunk = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithFilter(_ => true)
            .Build()
            .SerializeToChunk([node]);

        var deserialized = new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        AssertEquals([node], deserialized);
    }

    [TestMethod]
    public void ManyLevelDescendants()
    {
        var grandchildAA = new LinkTestConcept("gAA");
        var grandchildAB = new LinkTestConcept("gAB");
        var childA = new LinkTestConcept("cA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var childB = new LinkTestConcept("cB") { Containment_0_1 = new LinkTestConcept("gB") };
        var node = new LinkTestConcept("n") { Containment_0_n = [childA], Containment_0_1 = childB };

        AssertSerializeDeserialize([childA, grandchildAA, grandchildAB, node], node, n => n.GetId() != "cB");
    }

    [TestMethod]
    public void ManyLevelDescendants_Self()
    {
        var grandchildAA = new LinkTestConcept("gAA");
        var grandchildAB = new LinkTestConcept("gAB");
        var childA = new LinkTestConcept("cA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var childB = new LinkTestConcept("cB") { Containment_0_1 = new LinkTestConcept("gB") };
        var node = new LinkTestConcept("n") { Containment_0_n = [childA], Containment_0_1 = childB };

        AssertSerializeDeserialize([node, childA, grandchildAA, grandchildAB], node, n => n.GetId() != "cB");
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotations()
    {
        var grandchildAA = new LinkTestConcept("gAA");
        var grandchildAB = new LinkTestConcept("gAB");
        var childA = new LinkTestConcept("cA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var childB = new LinkTestConcept("cB") { Containment_0_1 = new LinkTestConcept("gB") };
        var node = new LinkTestConcept("n") { Containment_0_n = [childA], Containment_0_1 = childB };

        AssertSerializeDeserialize([childA, grandchildAA, grandchildAB, node], node, n => n.GetId() != "cB");
    }

    [TestMethod]
    public void ManyLevelDescendants_Self_Annotations()
    {
        var grandchildAA = new LinkTestConcept("gAA");
        var grandchildAB = new LinkTestConcept("gAB");
        var childA = new LinkTestConcept("cA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var childB = new LinkTestConcept("cB") { Containment_0_1 = new LinkTestConcept("gB") };
        var node = new LinkTestConcept("n") { Containment_0_n = [childA], Containment_0_1 = childB };

        AssertSerializeDeserialize([node, childA, grandchildAA, grandchildAB], node, n => n.GetId() != "cB");
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation()
    {
        var grandchildAA = new LinkTestConcept("gAA");
        var grandchildAAAnn = new TestAnnotation("gAAa");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new LinkTestConcept("gAB");
        var childA = new LinkTestConcept("cA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var childB = new LinkTestConcept("cB") { Containment_0_1 = new LinkTestConcept("gB") };
        var node = new LinkTestConcept("n") { Containment_0_n = [childA], Containment_0_1 = childB };

        var grandchild = new LinkTestConcept("aG");
        var grandchildAnn = new TestAnnotation("aGa");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new TestAnnotation("a") { Containment = grandchild };
        var annAnn = new TestAnnotation("aa");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);

        AssertSerializeDeserialize([childA, grandchildAA, grandchildAB, node], node, n => n.GetId() != "cB" && n is not IAnnotationInstance);
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation_Self()
    {
        var grandchildAA = new LinkTestConcept("gAA");
        var grandchildAAAnn = new TestAnnotation("gAAa");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new LinkTestConcept("gAB");
        var childA = new LinkTestConcept("cA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var childB = new LinkTestConcept("cB") { Containment_0_1 = new LinkTestConcept("gB") };
        var node = new LinkTestConcept("n") { Containment_0_n = [childA], Containment_0_1 = childB };

        var grandchild = new LinkTestConcept("aG");
        var grandchildAnn = new TestAnnotation("aGa");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new TestAnnotation("a") { Containment = grandchild };
        var annAnn = new TestAnnotation("aa");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);

        AssertSerializeDeserialize([node, childA, grandchildAA, grandchildAB], node, n => n.GetId() != "cB" && n is not IAnnotationInstance);
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation_Annotations()
    {
        var grandchildAA = new LinkTestConcept("gAA");
        var grandchildAAAnn = new TestAnnotation("gAAa");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new LinkTestConcept("gAB");
        var childA = new LinkTestConcept("cA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var childB = new LinkTestConcept("cB") { Containment_0_1 = new LinkTestConcept("gB") };
        var node = new LinkTestConcept("n") { Containment_0_n = [childA], Containment_0_1 = childB };

        var grandchild = new LinkTestConcept("aG");
        var grandchildAnn = new TestAnnotation("aGa");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new TestAnnotation("a") { Containment = grandchild };
        var annAnn = new TestAnnotation("aa");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);

        AssertSerializeDeserialize([
            childA,
            grandchildAB,
            ann,
            grandchild,
            grandchildAnn,
            annAnn,
            node
        ], node, n => n.GetId() != "cB" && n.GetId() != "gAA");
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation_Self_Annotations()
    {
        var grandchildAA = new LinkTestConcept("gAA");
        var grandchildAAAnn = new TestAnnotation("gAAa");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new LinkTestConcept("gAB");
        var childA = new LinkTestConcept("cA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var childB = new LinkTestConcept("cB") { Containment_0_1 = new LinkTestConcept("gB") };
        var node = new LinkTestConcept("n") { Containment_0_n = [childA], Containment_0_1 = childB };

        var grandchild = new LinkTestConcept("aG");
        var grandchildAnn = new TestAnnotation("aGa");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new TestAnnotation("a") { Containment = grandchild };
        var annAnn = new TestAnnotation("aa");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);

        AssertSerializeDeserialize([
            node,
            childA,
            grandchildAA,
            grandchildAAAnn,
            grandchildAB
        ], node, n => n.GetId() != "cB" && n.GetId() != "a");
    }

    private void AssertSerializeDeserialize(List<INode> expected, LinkTestConcept node, Func<IReadableNode, bool> filter)
    {
        var serializationChunk = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithFilter(filter)
            .Build()
            .SerializeToChunk([node]);
        var deserialized = new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

        CollectionAssert.AreEquivalent(
            expected,
            M1Extensions.Descendants(deserialized.First(), true, true).ToList(),
            new NodeIdComparer<IReadableNode>()
        );
    }
}