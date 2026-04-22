// Copyright 2026 TRUMPF Laser SE and other contributors
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

namespace LionWeb.Core.Test.Deserialization;

using Core.Migration;
using Core.Serialization;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;

public abstract class OverlappingContainmentTestsBase
{
    protected static readonly LionWebVersions LionWebVersion = LionWebVersions.Current;

    protected static SerializedContainment CreateContainment01(params NodeId[] childrenIds) =>
        new() { Containment = new MetaPointer("TestLanguage", "0", "LinkTestConcept-containment_0_1"), Children = childrenIds };

    protected static SerializedContainment CreateContainment0n(params NodeId[] childrenIds) =>
        new() { Containment = new MetaPointer("TestLanguage", "0", "LinkTestConcept-containment_0_n"), Children = childrenIds };

    protected abstract IDeserializer CreateDeserializer();

    protected static SerializationChunk CreateChunk(params SerializedNode[] nodes) =>
        new SerializationChunk
        {
            SerializationFormatVersion = LionWebVersion.VersionString,
            Languages =
            [
                new SerializedLanguageReference { Key = "TestLanguage", Version = "0" }
            ],
            Nodes = nodes
        };


    protected static SerializedNode CreateLinkTestConcept(NodeId id, params SerializedContainment[] containments) =>
        CreateLinkTestConcept(id, id, containments);

    private static SerializedNode CreateLinkTestConcept(NodeId id, string name, params SerializedContainment[] containments) =>
        new()
        {
            Id = id,
            Classifier = new MetaPointer("TestLanguage", "0", "LinkTestConcept"),
            Properties =
            [
                new SerializedProperty { Property = new MetaPointer("LionCore-builtins", "2024.1", "LionCore-builtins-INamed-name"), Value = name }
            ],
            Containments = containments,
            References = [],
            Annotations = [],
            Parent = null
        };
}

[TestClass]
public class OverlappingContainmentTestsGenerated : OverlappingContainmentTestsBase
{
    #region Single

    [TestMethod]
    public void Single_SameContainment_SameChildTwice()
    {
            var serializationChunk = CreateChunk(
                CreateLinkTestConcept("A", CreateContainment0n("B", "B")),
                CreateLinkTestConcept("B")
            );
        Assert.ThrowsExactly<ArgumentException>(() => CreateDeserializer().Deserialize(serializationChunk));
    }

    [TestMethod]
    public void Single_SameContainmentTwice_DifferentChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment01("B"), CreateContainment01("C")),
            CreateLinkTestConcept("B"),
            CreateLinkTestConcept("C")
        );

        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(2, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = nodes.OfType<INode>().Last();
        Assert.AreEqual("B", b.GetId());
        Assert.IsNull(b.GetParent());
        Assert.IsFalse(b.Children().Any());

        INode c = a.Children().First();
        Assert.AreEqual("C", c.GetId());
        Assert.AreSame(a, c.GetParent());
        Assert.IsFalse(c.Children().Any());
    }

    [TestMethod]
    public void Single_SameContainmentTwice_SameChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment01("B"), CreateContainment01("B")),
            CreateLinkTestConcept("B")
        );
        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(1, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.Children().Any());
    }

    #endregion

    #region Multi

    [TestMethod]
    public void Multi_SameContainment_SameChildTwice()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment0n("B", "B")),
            CreateLinkTestConcept("B")
        );
        
        Assert.ThrowsExactly<ArgumentException>(() => CreateDeserializer().Deserialize(serializationChunk));
    }

    [TestMethod]
    public void Multi_SameContainmentTwice_DifferentChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment0n("B"), CreateContainment0n("C")),
            CreateLinkTestConcept("B"),
            CreateLinkTestConcept("C")
        );

        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(2, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = nodes.OfType<INode>().Last();
        Assert.AreEqual("B", b.GetId());
        Assert.IsNull(b.GetParent());
        Assert.IsFalse(b.Children().Any());

        INode c = a.Children().First();
        Assert.AreEqual("C", c.GetId());
        Assert.AreSame(a, c.GetParent());
        Assert.IsFalse(c.Children().Any());
    }

    [TestMethod]
    public void Multi_SameContainmentTwice_SameChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment0n("B"), CreateContainment0n("B")),
            CreateLinkTestConcept("B")
        );
        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(1, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.Children().Any());
    }

    #endregion

    protected override IDeserializer CreateDeserializer() =>
        new DeserializerBuilder()
            .WithLanguage(TestLanguageLanguage.Instance)
            .Build();
}

[TestClass]
public class OverlappingContainmentTestsDynamic : OverlappingContainmentTestsBase
{
    #region Single

    [TestMethod]
    public void Single_SameContainment_SameChildTwice()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment01("B", "B")),
            CreateLinkTestConcept("B")
        );

        Assert.ThrowsExactly<InvalidValueException>(() => CreateDeserializer().Deserialize(serializationChunk));
    }

    [TestMethod]
    public void Single_SameContainmentTwice_DifferentChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment01("B"), CreateContainment01("C")),
            CreateLinkTestConcept("B"),
            CreateLinkTestConcept("C")
        );

        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(2, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = nodes.OfType<INode>().Last();
        Assert.AreEqual("B", b.GetId());
        Assert.IsNull(b.GetParent());
        Assert.IsFalse(b.Children().Any());

        INode c = a.Children().First();
        Assert.AreEqual("C", c.GetId());
        Assert.AreSame(a, c.GetParent());
        Assert.IsFalse(c.Children().Any());
    }

    [TestMethod]
    public void Single_SameContainmentTwice_SameChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment01("B"), CreateContainment01("B")),
            CreateLinkTestConcept("B")
        );
        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(1, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.Children().Any());
    }

    #endregion

    #region Multi

    [TestMethod]
    public void Multi_SameContainment_SameChildTwice()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment0n("B", "B")),
            CreateLinkTestConcept("B")
        );

        var nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(1, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        // Assert.AreEqual(1, a.Children().Count());
        // TODO Wrong
        Assert.AreEqual(2, a.Children().Count());

        INode b1 = a.Children().First();
        Assert.AreEqual("B", b1.GetId());
        Assert.AreSame(a, b1.GetParent());
        Assert.IsFalse(b1.Children().Any());

        // TODO Wrong
        INode b2 = a.Children().Last();
        Assert.AreSame(b1, b2);
    }

    [TestMethod]
    public void Multi_SameContainmentTwice_DifferentChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment0n("B"), CreateContainment0n("C")),
            CreateLinkTestConcept("B"),
            CreateLinkTestConcept("C")
        );

        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(2, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = nodes.OfType<INode>().Last();
        Assert.AreEqual("B", b.GetId());
        Assert.IsNull(b.GetParent());
        Assert.IsFalse(b.Children().Any());

        INode c = a.Children().First();
        Assert.AreEqual("C", c.GetId());
        Assert.AreSame(a, c.GetParent());
        Assert.IsFalse(c.Children().Any());
    }

    [TestMethod]
    public void Multi_SameContainmentTwice_SameChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment0n("B"), CreateContainment0n("B")),
            CreateLinkTestConcept("B")
        );
        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(1, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.Children().Any());
    }

    #endregion

    protected override IDeserializer CreateDeserializer()
    {
        DynamicLanguage dynamicLanguage = new DynamicLanguageCloner(LionWebVersion).Clone(TestLanguageLanguage.Instance);
        return new DeserializerBuilder()
            .WithLanguage(dynamicLanguage)
            .Build();
    }
}

[TestClass]
public class OverlappingContainmentTestsLenient : OverlappingContainmentTestsBase
{
    #region Single

    [TestMethod]
    public void Single_SameContainment_SameChildTwice()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment01("B", "B")),
            CreateLinkTestConcept("B")
        );


        var nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(1, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        // Assert.AreEqual(1, a.Children().Count());
        // TODO Wrong
        Assert.AreEqual(2, a.Children().Count());

        INode b1 = a.Children().First();
        Assert.AreEqual("B", b1.GetId());
        Assert.AreSame(a, b1.GetParent());
        Assert.IsFalse(b1.Children().Any());

        // TODO Wrong
        INode b2 = a.Children().Last();
        Assert.AreSame(b1, b2);
    }

    [TestMethod]
    public void Single_SameContainmentTwice_DifferentChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment01("B"), CreateContainment01("C")),
            CreateLinkTestConcept("B"),
            CreateLinkTestConcept("C")
        );

        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(2, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = nodes.OfType<INode>().Last();
        Assert.AreEqual("B", b.GetId());
        Assert.IsNull(b.GetParent());
        Assert.IsFalse(b.Children().Any());

        INode c = a.Children().First();
        Assert.AreEqual("C", c.GetId());
        Assert.AreSame(a, c.GetParent());
        Assert.IsFalse(c.Children().Any());
    }

    [TestMethod]
    public void Single_SameContainmentTwice_SameChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment01("B"), CreateContainment01("B")),
            CreateLinkTestConcept("B")
        );
        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(1, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.Children().Any());
    }

    #endregion

    #region Multi

    [TestMethod]
    public void Multi_SameContainment_SameChildTwice()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment0n("B", "B")),
            CreateLinkTestConcept("B")
        );

        var nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(1, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        // Assert.AreEqual(1, a.Children().Count());
        // TODO Wrong
        Assert.AreEqual(2, a.Children().Count());

        INode b1 = a.Children().First();
        Assert.AreEqual("B", b1.GetId());
        Assert.AreSame(a, b1.GetParent());
        Assert.IsFalse(b1.Children().Any());

        // TODO Wrong
        INode b2 = a.Children().Last();
        Assert.AreSame(b1, b2);
    }

    [TestMethod]
    public void Multi_SameContainmentTwice_DifferentChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment0n("B"), CreateContainment0n("C")),
            CreateLinkTestConcept("B"),
            CreateLinkTestConcept("C")
        );

        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(2, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = nodes.OfType<INode>().Last();
        Assert.AreEqual("B", b.GetId());
        Assert.IsNull(b.GetParent());
        Assert.IsFalse(b.Children().Any());

        INode c = a.Children().First();
        Assert.AreEqual("C", c.GetId());
        Assert.AreSame(a, c.GetParent());
        Assert.IsFalse(c.Children().Any());
    }

    [TestMethod]
    public void Multi_SameContainmentTwice_SameChild()
    {
        var serializationChunk = CreateChunk(
            CreateLinkTestConcept("A", CreateContainment0n("B"), CreateContainment0n("B")),
            CreateLinkTestConcept("B")
        );
        List<IReadableNode> nodes = CreateDeserializer().Deserialize(serializationChunk);

        Assert.HasCount(1, nodes);
        INode a = nodes.OfType<INode>().First();
        Assert.AreEqual("A", a.GetId());
        Assert.IsNull(a.GetParent());
        Assert.AreEqual(1, a.Children().Count());

        INode b = a.Children().First();
        Assert.AreEqual("B", b.GetId());
        Assert.AreSame(a, b.GetParent());
        Assert.IsFalse(b.Children().Any());
    }

    #endregion

    protected override IDeserializer CreateDeserializer()
    {
        DynamicLanguage dynamicLanguage = new DynamicLanguageCloner(LionWebVersion).Clone(TestLanguageLanguage.Instance);
        dynamicLanguage.NodeFactory = new MigrationFactory(dynamicLanguage);
        return new DeserializerBuilder()
            .WithLanguage(dynamicLanguage)
            .Build();
    }
}