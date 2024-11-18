// Copyright 2024 TRUMPF Laser SE and other contributors
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

namespace LionWeb_CSharp_Test.tests.serialization;

using Examples.Shapes.M2;
using languages;
using LionWeb.Core;
using LionWeb.Core.M3;
using LionWeb.Core.Serialization;

[TestClass]
public class SerializationChunkExtensionsTests
{
    [TestMethod]
    public void language_example()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = []
        };
        
        SerializedNode nodeA = serializationChunk.Node("A");
        nodeA.Classifier(ShapesLanguage.Instance.Coord);
        nodeA.Property(ShapesLanguage.Instance.Coord_x, "20");
        nodeA.Property(ShapesLanguage.Instance.Coord_y, "10");
        nodeA.Property(ShapesLanguage.Instance.Coord_z);

        SerializedNode nodeB = serializationChunk.Node("B");
        nodeB.Classifier(ShapesLanguage.Instance.Circle);
        nodeB.Property(ShapesLanguage.Instance.Circle_r);
        
        Assert.AreEqual(2, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_classifier_with_MetaPointer()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = []
        };
        var metaPointer = new MetaPointer("key-Shapes", "1", "key-OffsetDuplicate");
        serializationChunk.Node("A").Classifier(metaPointer);

        MetaPointer classifier = serializationChunk.Nodes.First(node => node.Id == "A").Classifier;
        Assert.AreSame(metaPointer, classifier);
        Assert.AreEqual(1, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_classifier_with_Concept()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = []
        };

        Concept offsetDuplicate = ShapesLanguage.Instance.OffsetDuplicate;
        serializationChunk.Node("A").Classifier(offsetDuplicate);

        MetaPointer classifier = serializationChunk.Nodes.First(node => node.Id == "A").Classifier;
        Assert.AreEqual(offsetDuplicate.ToMetaPointer(), classifier);
        Assert.AreEqual(1, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_property_with_MetaPointer()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = []
        };
        var metaPointer = new MetaPointer("LionCore-builtins", "2023.1", "LionCore-builtins-INamed-name");
        serializationChunk.Node("A").Property(metaPointer);

        SerializedProperty serializedProperty = serializationChunk.Nodes.First(node => node.Id == "A").Properties[0];
        Assert.AreSame(metaPointer, serializedProperty.Property);
        Assert.AreEqual(1, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_property_with_value()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = []
        };
        var metaPointer = new MetaPointer("LionCore-builtins", "2023.1", "LionCore-builtins-INamed-name");
        var propertyValue = "property-value";
        serializationChunk.Node("A").Property(metaPointer, propertyValue);

        SerializedProperty serializedProperty = serializationChunk.Nodes.First(node => node.Id == "A").Properties[0];
        Assert.AreSame(metaPointer, serializedProperty.Property);
        Assert.AreSame(propertyValue, serializedProperty.Value);
        Assert.AreEqual(1, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_property_multiple()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = []
        };

        var metaPointer1 = new MetaPointer("id-x", "key-x", "x");
        var metaPointer2 = new MetaPointer("id-y", "key-y", "y");

        SerializedNode serializedNode = serializationChunk.Node("A").Classifier(ShapesLanguage.Instance.Coord);
        serializedNode.Property(metaPointer1);
        serializedNode.Property(metaPointer2);

        SerializedNode first = serializationChunk.Nodes.First(node => node.Id == "A");
        SerializedProperty serializedProperty1 = first.Properties[0];
        Assert.AreSame(metaPointer1, serializedProperty1.Property);

        SerializedProperty serializedProperty2 = first.Properties[1];
        Assert.AreSame(metaPointer2, serializedProperty2.Property);

        Assert.AreEqual(2, serializationChunk.Nodes[0].Properties.Length);
        Assert.AreEqual(1, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_property_with_Property()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = []
        };

        Property coordX = ShapesLanguage.Instance.Coord_x;
        Property coordY = ShapesLanguage.Instance.Coord_y;

        SerializedNode serializedNode = serializationChunk.Node("A").Classifier(ShapesLanguage.Instance.Coord);
        serializedNode.Property(coordX);
        serializedNode.Property(coordY);

        SerializedNode first = serializationChunk.Nodes.First(node => node.Id == "A");
        SerializedProperty serializedProperty1 = first.Properties[0];
        Assert.AreEqual(coordX.ToMetaPointer(), serializedProperty1.Property);

        SerializedProperty serializedProperty2 = first.Properties[1];
        Assert.AreEqual(coordY.ToMetaPointer(), serializedProperty2.Property);

        Assert.AreEqual(2, serializationChunk.Nodes[0].Properties.Length);
        Assert.AreEqual(1, serializationChunk.Nodes.Length);
    }

    [TestMethod]
    public void test_property_with_Property_with_values()
    {
        var serializationChunk = new SerializationChunk
        {
            SerializationFormatVersion = ReleaseVersion.Current,
            Languages =
            [
                new SerializedLanguageReference { Key = "key-Shapes", Version = "1" }
            ],
            Nodes = []
        };

        Property coordX = ShapesLanguage.Instance.Coord_x;
        Property coordY = ShapesLanguage.Instance.Coord_y;
        var valueX = "3";
        var valueY = "4";

        SerializedNode serializedNode = serializationChunk.Node("A").Classifier(ShapesLanguage.Instance.Coord);
        serializedNode.Property(coordX, valueX);
        serializedNode.Property(coordY, valueY);

        SerializedNode first = serializationChunk.Nodes.First(node => node.Id == "A");
        SerializedProperty serializedPropertyX = first.Properties[0];
        Assert.AreEqual(coordX.ToMetaPointer(), serializedPropertyX.Property);
        Assert.AreEqual(valueX, serializedPropertyX.Value);

        SerializedProperty serializedPropertyY = first.Properties[1];
        Assert.AreEqual(coordY.ToMetaPointer(), serializedPropertyY.Property);
        Assert.AreEqual(valueY, serializedPropertyY.Value);

        Assert.AreEqual(2, serializationChunk.Nodes[0].Properties.Length);
        Assert.AreEqual(1, serializationChunk.Nodes.Length);
    }
}