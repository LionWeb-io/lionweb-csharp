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

namespace LionWeb.Core.Test.Serialization;

using Core.Serialization;
using Languages.Generated.V2024_1.Shapes.M2;
using M1;

[TestClass]
public class EmptyFeatureSerializationTests : SerializationTestsBase
{
    [TestMethod]
    public void EmptyFeatures_Serialize()
    {
        var circle = new Circle("circle");
        var node = new OffsetDuplicate("od") { Uuid = "abc", Source = circle, Docs = new Documentation("docs") };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).WithSerializedEmptyFeatures(true)
            .Build();

        var chunk = serializer.SerializeToChunk([node, circle]);

        var lang = ShapesLanguage.Instance;
        var langKey = lang.Key;
        var langVersion = lang.Version;
        var expected = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages = [new SerializedLanguageReference { Key = langKey, Version = langVersion }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "od",
                    Classifier = lang.OffsetDuplicate.ToMetaPointer(),
                    Properties =
                    [
                        new SerializedProperty { Property = lang.IShape_uuid.ToMetaPointer(), Value = "abc" },
                        new SerializedProperty
                        {
                            Property = _lionWebVersion.BuiltIns.INamed_name.ToMetaPointer(), Value = null
                        },
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = lang.OffsetDuplicate_docs.ToMetaPointer(), Children = ["docs"]
                        },
                        new SerializedContainment
                        {
                            Containment = lang.IShape_fixpoints.ToMetaPointer(), Children = []
                        },
                        new SerializedContainment
                        {
                            Containment = lang.OffsetDuplicate_secretDocs.ToMetaPointer(), Children = []
                        },
                        new SerializedContainment
                        {
                            Containment = lang.OffsetDuplicate_offset.ToMetaPointer(), Children = []
                        },
                        new SerializedContainment { Containment = lang.Shape_shapeDocs.ToMetaPointer(), Children = [] },
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = lang.OffsetDuplicate_source.ToMetaPointer(),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "circle", ResolveInfo = null }
                            ]
                        },
                        new SerializedReference
                        {
                            Reference = lang.OffsetDuplicate_altSource.ToMetaPointer(), Targets = []
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "docs",
                    Classifier = lang.Documentation.ToMetaPointer(),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = lang.Documentation_technical.ToMetaPointer(), Value = null
                        },
                        new SerializedProperty { Property = lang.Documentation_text.ToMetaPointer(), Value = null },
                    ],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "od"
                },
                new SerializedNode
                {
                    Id = "circle",
                    Classifier = lang.Circle.ToMetaPointer(),
                    Properties =
                    [
                        new SerializedProperty { Property = lang.IShape_uuid.ToMetaPointer(), Value = null },
                        new SerializedProperty { Property = lang.Circle_r.ToMetaPointer(), Value = null },
                        new SerializedProperty
                        {
                            Property = _lionWebVersion.BuiltIns.INamed_name.ToMetaPointer(), Value = null
                        },
                    ],
                    Containments =
                    [
                        new SerializedContainment { Containment = lang.Circle_center.ToMetaPointer(), Children = [] },
                        new SerializedContainment
                        {
                            Containment = lang.IShape_fixpoints.ToMetaPointer(), Children = []
                        },
                        new SerializedContainment { Containment = lang.Shape_shapeDocs.ToMetaPointer(), Children = [] },
                    ],
                    References = [],
                    Annotations = [],
                    Parent = null
                }
            ]
        };

        AssertEquals(expected, chunk);
    }

    [TestMethod]
    public void EmptyFeatures_Skip()
    {
        var circle = new Circle("circle");
        var node = new OffsetDuplicate("od") { Uuid = "abc", Source = circle, Docs = new Documentation("docs") };

        var serializer = new SerializerBuilder().WithLionWebVersion(_lionWebVersion).WithSerializedEmptyFeatures(false)
            .Build();

        var chunk = serializer.SerializeToChunk([node, circle]);

        var langKey = ShapesLanguage.Instance.Key;
        var langVersion = ShapesLanguage.Instance.Version;
        var expected = new SerializationChunk
        {
            SerializationFormatVersion = _lionWebVersion.VersionString,
            Languages = [new SerializedLanguageReference { Key = langKey, Version = langVersion }],
            Nodes =
            [
                new SerializedNode
                {
                    Id = "od",
                    Classifier = new MetaPointer(langKey, langVersion, ShapesLanguage.Instance.OffsetDuplicate.Key),
                    Properties =
                    [
                        new SerializedProperty
                        {
                            Property = new MetaPointer(langKey, langVersion,
                                ShapesLanguage.Instance.IShape_uuid.Key),
                            Value = "abc"
                        }
                    ],
                    Containments =
                    [
                        new SerializedContainment
                        {
                            Containment = new MetaPointer(langKey, langVersion,
                                ShapesLanguage.Instance.OffsetDuplicate_docs.Key),
                            Children = ["docs"]
                        }
                    ],
                    References =
                    [
                        new SerializedReference
                        {
                            Reference = new MetaPointer(langKey, langVersion,
                                ShapesLanguage.Instance.OffsetDuplicate_source.Key),
                            Targets =
                            [
                                new SerializedReferenceTarget { Reference = "circle", ResolveInfo = null }
                            ]
                        }
                    ],
                    Annotations = [],
                    Parent = null
                },
                new SerializedNode
                {
                    Id = "docs",
                    Classifier = new MetaPointer(langKey, langVersion, ShapesLanguage.Instance.Documentation.Key),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = "od"
                },
                new SerializedNode
                {
                    Id = "circle",
                    Classifier = new MetaPointer(langKey, langVersion, ShapesLanguage.Instance.Circle.Key),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = [],
                    Parent = null
                }
            ]
        };

        AssertEquals(expected, chunk);
    }
}