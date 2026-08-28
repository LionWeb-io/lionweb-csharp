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

using Core.Serialization;
using Core.Utilities;
using Languages.Generated.V2024_1.TestLanguage;
using M1;

[TestClass]
public class FilteringSerializationTests : SerializationTestsBase
{
    [TestMethod]
    public void NoOp()
    {
        var node = new LinkTestConcept("node") { Name = "hello" };

        var serializationChunk = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithFilter(_ => true)
            .Build()
            .SerializeToChunk([node]);

        List<IReadableNode> deserialized = Deserialize(serializationChunk);
        AssertEquals([node], deserialized);
    }

    [TestMethod]
    public void ManyLevelDescendants()
    {
        var grandchildAA = new LinkTestConcept("grandchildAA");
        var grandchildAB = new LinkTestConcept("grandchildAB");
        var childA = new LinkTestConcept("childA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var grandchildB = new LinkTestConcept("grandchildB");
        var childB = new LinkTestConcept("childB") { Containment_0_1 = grandchildB };
        grandchildB.Reference_0_1 = childB;
        var node = new LinkTestConcept("node") { Containment_0_n = [childA], Containment_0_1 = childB, Reference_0_n = [grandchildAA, grandchildAB, childA, grandchildB, childB] };

        Func<IReadableNode, bool> filter = n => n.GetId() != "childB";
        List<INode> expected = [childA, grandchildAA, grandchildAB, node];
        AssertSerializeDeserializeIncludingAncestors(expected, node, filter);
        AssertSerializeDeserialize([.. expected, grandchildB], node, filter);
    }

    [TestMethod]
    public void ManyLevelDescendants_Self()
    {
        var grandchildAA = new LinkTestConcept("grandchildAA");
        var grandchildAB = new LinkTestConcept("grandchildAB");
        var childA = new LinkTestConcept("childA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var grandchildB = new LinkTestConcept("grandchildB");
        var childB = new LinkTestConcept("childB") { Containment_0_1 = grandchildB };
        grandchildB.Reference_0_1 = childB;
        var node = new LinkTestConcept("node") { Containment_0_n = [childA], Containment_0_1 = childB, Reference_0_n = [grandchildAA, grandchildAB, childA, grandchildB, childB] };

        Func<IReadableNode, bool> filter = n => n.GetId() != "childB";
        List<INode> expected = [node, childA, grandchildAA, grandchildAB];
        AssertSerializeDeserializeIncludingAncestors(expected, node, filter);
        AssertSerializeDeserialize([.. expected, grandchildB], node, filter);
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotations()
    {
        var grandchildAA = new LinkTestConcept("grandchildAA");
        var grandchildAB = new LinkTestConcept("grandchildAB");
        var childA = new LinkTestConcept("childA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var grandchildB = new LinkTestConcept("grandchildB");
        var childAnn = new TestAnnotation("childAnn") { Containment = grandchildB };
        var node = new LinkTestConcept("node") { Containment_0_n = [childA], Reference_0_n = [grandchildAA, grandchildAB, childA, grandchildB] }
            .WithAnnotation(childAnn);

        Func<IReadableNode, bool> filter = n => n.GetId() != "childAnn";
        List<INode> expected = [childA, grandchildAA, grandchildAB, node];
        AssertSerializeDeserializeIncludingAncestors(expected, node, filter);
        AssertSerializeDeserialize([.. expected, grandchildB], node, filter);
    }

    [TestMethod]
    public void ManyLevelDescendants_Self_Annotations()
    {
        var grandchildAA = new LinkTestConcept("grandchildAA");
        var grandchildAB = new LinkTestConcept("grandchildAB");
        var childA = new LinkTestConcept("childA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var grandchildB = new LinkTestConcept("grandchildB");
        var childAnn = new TestAnnotation("childAnn") { Containment = grandchildB };
        var node = new LinkTestConcept("node") { Containment_0_n = [childA], Reference_0_n = [grandchildAA, grandchildAB, childA, grandchildB] }
            .WithAnnotation(childAnn);

        Func<IReadableNode, bool> filter = n => n.GetId() != "childAnn";
        List<INode> expected = [node, childA, grandchildAA, grandchildAB];
        AssertSerializeDeserializeIncludingAncestors(expected, node, filter);
        AssertSerializeDeserialize([.. expected, grandchildB], node, filter);
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation()
    {
        var grandchildAA = new LinkTestConcept("grandchildAA");
        var grandchildAAAnn = new TestAnnotation("grandchildAAAnn");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new LinkTestConcept("grandchildAB");
        var childA = new LinkTestConcept("childA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var grandchildB = new LinkTestConcept("grandchildB");
        var childB = new LinkTestConcept("childB") { Containment_0_1 = grandchildB };
        grandchildB.Reference_0_1 = childB;
        var node = new LinkTestConcept("node") { Containment_0_n = [childA], Containment_0_1 = childB, Reference_0_n = [grandchildAA, grandchildAB, childA, grandchildB, childB] };

        var grandchild = new LinkTestConcept("grandchild");
        var grandchildAnn = new TestAnnotation("grandchildAnn");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new TestAnnotation("ann") { Containment = grandchild, Ref = grandchildAnn };
        var annAnn = new TestAnnotation("annAnn");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);

        Func<IReadableNode, bool> filter = n => n.GetId() != "childB" && n is not IAnnotationInstance;
        List<INode> expected = [childA, grandchildAA, grandchildAB, node];
        AssertSerializeDeserializeIncludingAncestors(expected, node, filter);
        AssertSerializeDeserialize([.. expected, grandchildB, grandchild], node, filter);
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation_Self()
    {
        var grandchildAA = new LinkTestConcept("grandchildAA");
        var grandchildAAAnn = new TestAnnotation("grandchildAAAnn");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new LinkTestConcept("grandchildAB");
        var childA = new LinkTestConcept("childA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var grandchildB = new LinkTestConcept("grandchildB");
        var childB = new LinkTestConcept("childB") { Containment_0_1 = grandchildB };
        grandchildB.Reference_0_1 = childB;
        var node = new LinkTestConcept("node") { Containment_0_n = [childA], Containment_0_1 = childB, Reference_0_n = [grandchildAA, grandchildAB, childA, grandchildB, childB] };

        var grandchild = new LinkTestConcept("grandchild");
        var grandchildAnn = new TestAnnotation("grandchildAnn");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new TestAnnotation("ann") { Containment = grandchild, Ref = grandchildAnn };
        var annAnn = new TestAnnotation("annAnn");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);

        Func<IReadableNode, bool> filter = n => n.GetId() != "childB" && n is not IAnnotationInstance;
        List<INode> expected = [node, childA, grandchildAA, grandchildAB];
        AssertSerializeDeserializeIncludingAncestors(expected, node, filter);
        AssertSerializeDeserialize([.. expected, grandchildB, grandchild], node, filter);
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation_Annotations()
    {
        var grandchildAA = new LinkTestConcept("grandchildAA");
        var grandchildAAAnn = new TestAnnotation("grandchildAAAnn");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new LinkTestConcept("grandchildAB");
        var childA = new LinkTestConcept("childA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var grandchildB = new LinkTestConcept("grandchildB");
        var childB = new LinkTestConcept("childB") { Containment_0_1 = grandchildB };
        grandchildB.Reference_0_1 = childB;
        var node = new LinkTestConcept("node") { Containment_0_n = [childA], Containment_0_1 = childB, Reference_0_n = [grandchildAA, grandchildAB, childA, grandchildB, childB] };

        var grandchild = new LinkTestConcept("grandchild");
        var grandchildAnn = new TestAnnotation("grandchildAnn");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new TestAnnotation("ann") { Containment = grandchild, Ref = grandchildAnn };
        var annAnn = new TestAnnotation("annAnn");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);

        Func<IReadableNode, bool> filter = n => n.GetId() != "childB" && n.GetId() != "grandchildAA";
        List<INode> expected = [childA, grandchildAB, ann, grandchild, grandchildAnn, annAnn, node];
        AssertSerializeDeserializeIncludingAncestors(expected, node, filter);
        AssertSerializeDeserialize([.. expected, grandchildB, grandchildAAAnn], node, filter);
    }

    [TestMethod]
    public void ManyLevelDescendants_Annotation_Self_Annotations()
    {
        var grandchildAA = new LinkTestConcept("grandchildAA");
        var grandchildAAAnn = new TestAnnotation("grandchildAAAnn");
        grandchildAA.AddAnnotations([grandchildAAAnn]);
        var grandchildAB = new LinkTestConcept("grandchildAB");
        var childA = new LinkTestConcept("childA") { Containment_0_1 = grandchildAA, Containment_1 = grandchildAB };
        var grandchildB = new LinkTestConcept("grandchildB");
        var childB = new LinkTestConcept("childB") { Containment_0_1 = grandchildB };
        grandchildB.Reference_0_1 = childB;
        var node = new LinkTestConcept("node") { Containment_0_n = [childA], Containment_0_1 = childB, Reference_0_n = [grandchildAA, grandchildAB, childA, grandchildB, childB] };

        var grandchild = new LinkTestConcept("grandchild");
        var grandchildAnn = new TestAnnotation("grandchildAnn");
        grandchild.AddAnnotations([grandchildAnn]);
        var ann = new TestAnnotation("ann") { Containment = grandchild, Ref = grandchildAnn };
        var annAnn = new TestAnnotation("annAnn");
        ann.AddAnnotations([annAnn]);
        node.AddAnnotations([ann]);

        Func<IReadableNode, bool> filter = n => n.GetId() != "childB" && n.GetId() != "ann";
        List<INode> expected = [node, childA, grandchildAA, grandchildAAAnn, grandchildAB];
        AssertSerializeDeserializeIncludingAncestors(expected, node, filter);
        AssertSerializeDeserialize([.. expected, grandchildB, grandchild, grandchildAnn, annAnn], node, filter);
    }

    private void AssertSerializeDeserializeIncludingAncestors(List<INode> expected, LinkTestConcept node, Func<IReadableNode, bool> filter)
    {
        var serializationChunk = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithFilterIncludingAncestors(filter)
            .Build()
            .SerializeToChunk([node]);

        List<IReadableNode> deserialized = Deserialize(serializationChunk);
        AssertEquivalent(expected, deserialized);
    }

    private void AssertSerializeDeserialize(List<INode> expected, LinkTestConcept node, Func<IReadableNode, bool> filter)
    {
        var serializationChunk = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithFilter(filter)
            .Build()
            .SerializeToChunk([node]);

        List<IReadableNode> deserialized = Deserialize(serializationChunk);
        AssertEquivalent(expected, deserialized);
    }

    private static List<IReadableNode> Deserialize(SerializationChunk serializationChunk) =>
        new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build()
            .Deserialize(serializationChunk);

    private static void AssertEquivalent(List<INode> expected, List<IReadableNode> deserialized) =>
        CollectionAssert.AreEquivalent(
            expected,
            deserialized
                .SelectMany(n => M1Extensions.Descendants(n, true, true))
                .ToList(),
            new NodeIdComparer<IReadableNode>()
        );
}