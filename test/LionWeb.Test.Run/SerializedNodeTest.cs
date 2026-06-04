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

namespace LionWeb.Test.Run;

using Core;
using Core.Benchmark;
using Core.M1;
using Core.Serialization;
using Core.Test.Languages.Generated.V2024_1.TestLanguage;

public class SerializedNodeTest
{
    public Dictionary<string, object> CreateSerializedNodes() =>
        new()
        {
            { "Serialized_Empty", new SerializedNode() { Id = null, Classifier = null } },
            { "Serialized_Minimal", new SerializedNode() { Id = "myId", Classifier = Classifier() } },
            {
                "Serialized_Set", new SerializedNode()
                {
                    Id = "myId",
                    Classifier = Classifier(),
                    Properties = [],
                    Containments = [],
                    References = [],
                    Annotations = []
                }
            },
            { "Serialized_Annotation_Set", new SerializedNode() { Id = "myId", Classifier = Classifier(), Annotations = [] } },
            { "Serialized_Annotation_Filled", new SerializedNode() { Id = "myId", Classifier = Classifier(), Annotations = ["ann"] } },
            { "Property_Empty", new SerializedProperty { Property = null } },
            { "Property_Minimal", new SerializedProperty { Property = Property() } },
            { "Property_Filled", new SerializedProperty { Property = Property(), Value = "myProp" } },
            { "Containment_Empty", new SerializedContainment() { Containment = null, Children = null } },
            { "Containment_Minimal", new SerializedContainment() { Containment = Containment(), Children = null } },
            { "Containment_Set", new SerializedContainment() { Containment = Containment(), Children = [] } },
            { "Containment_Filled", new SerializedContainment() { Containment = Containment(), Children = ["childId"] } },
            { "Reference_Empty", new SerializedReference() { Reference = null, Targets = null } },
            { "Reference_Minimal", new SerializedReference() { Reference = Reference(), Targets = null } },
            { "Reference_Set", new SerializedReference() { Reference = Reference(), Targets = [] } },
            { "Reference_Filled", new SerializedReference() { Reference = Reference(), Targets = [new SerializedReferenceTarget() { Reference = "targetId" }] } },
            { "Target_Empty", new  SerializedReferenceTarget() },
            { "Target_Reference", new  SerializedReferenceTarget() {Reference = "ref"} },
            { "Target_Info", new  SerializedReferenceTarget() {ResolveInfo = "ref text"} },
            { "Target_Filled", new  SerializedReferenceTarget() {ResolveInfo = "ref text", Reference = "ref"} },
        };

    public void SerializeNodes(Stream stream) => 
        JsonUtils.WriteNodesToStream(stream, new SerializerBuilder().WithLionWebVersion(LionWebVersions.Current).Build(), SerializerBenchmark.CreateNodes(32_000));

    public List<IReadableNode> DeserializeNodes(Stream stream) => 
        JsonUtils.ReadNodesFromStream(stream, new DeserializerBuilder().WithLionWebVersion(LionWebVersions.Current).WithLanguage(TestLanguageLanguage.Instance).Build());


    private static MetaPointer Classifier() => new("myLang", "0", "myKey");
    private static MetaPointer Property() => new("myLang", "0", "myProp");
    private static MetaPointer Containment() => new("myLang", "0", "myChild");
    private static MetaPointer Reference() => new("myLang", "0", "myRef");
}