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

namespace LionWeb.Core.Test.Serialization;

using Core.Serialization;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;
using NullableReferencesTestLang = Languages.Generated.V2024_1.NullableReferencesTestLang; 

[TestClass]
public class UnresolvedReferenceSerializationTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    private readonly Language _language = TestLanguageLanguage.Instance;

    [TestMethod]
    public void DeserializeMissingReference_Throw()
    {
        var input = new LinkTestConcept("root")
        {
            Reference_0_1 = new LinkTestConcept("ref0"),
            Reference_0_n = [new LinkTestConcept("ref0"), new LinkTestConcept("refN1")]
        };
        var inputPartition = new TestPartition("partition") { Links = [input] };

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build();

        var stream = new MemoryStream();
        JsonUtils.WriteNodesToStream(stream, serializer, [inputPartition, input]);

        var unresolvedReferencesManager = new UnresolvedReferencesManager();
        
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithLanguage(_language)
            .WithHandler(new UnresolvedReferencesDeserializerHandler(unresolvedReferencesManager))
            .Build();

        stream.Position = 0;
        var nodes = JsonUtils.ReadNodesFromStream(stream, deserializer);

        Assert.HasCount(1, nodes);

        var deserializedPartition = nodes.OfType<TestPartition>().Single();
        var deserialized = (LinkTestConcept)deserializedPartition.Links.Single();

        Assert.Throws<UnresolvedReferenceException>(() => deserialized.Reference_0_1);
        Assert.Throws<UnresolvedReferenceException>(() => deserialized.Reference_0_n);

        deserializedPartition.GetNotificationSender()!.ConnectTo(unresolvedReferencesManager);

        var newRef0 = new LinkTestConcept("ref0");
        deserialized.Containment_0_1 = newRef0;

        var newRefN1 = new LinkTestConcept("refN1");
        deserialized.Containment_1 = newRefN1;
        
        Assert.AreSame(newRef0, deserialized.Reference_0_1);
        Assert.AreSame(newRef0, deserialized.Reference_0_n.First());
        Assert.AreSame(newRefN1, deserialized.Reference_0_n.Last());
    }

    [TestMethod]
    public void DeserializeMissingReference_Null()
    {
        var input = new NullableReferencesTestLang.LinkTestConcept("root")
        {
            Reference_0_1 = new NullableReferencesTestLang.LinkTestConcept("ref0"),
            Reference_0_n = [new NullableReferencesTestLang.LinkTestConcept("ref0"), new NullableReferencesTestLang.LinkTestConcept("refN1")]
        };
        var inputPartition = new NullableReferencesTestLang.TestPartition("partition") { Links = [input] };

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build();

        var stream = new MemoryStream();
        JsonUtils.WriteNodesToStream(stream, serializer, [inputPartition, input]);

        var unresolvedReferencesManager = new UnresolvedReferencesManager();
        
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithLanguage(NullableReferencesTestLang.NullableReferencesTestLanguageLanguage.Instance)
            .WithHandler(new UnresolvedReferencesDeserializerHandler(unresolvedReferencesManager))
            .Build();

        stream.Position = 0;
        var nodes = JsonUtils.ReadNodesFromStream(stream, deserializer);

        Assert.HasCount(1, nodes);

        var deserializedPartition = nodes.OfType<NullableReferencesTestLang.TestPartition>().Single();
        var deserialized = (NullableReferencesTestLang.LinkTestConcept)deserializedPartition.Links.Single();

        Assert.IsNull(deserialized.Reference_0_1);
        foreach (var target in deserialized.Reference_0_n)
        {
            Assert.IsNull(target);
        }

        deserializedPartition.GetNotificationSender()!.ConnectTo(unresolvedReferencesManager);

        var newRef0 = new NullableReferencesTestLang.LinkTestConcept("ref0");
        deserialized.Containment_0_1 = newRef0;
        
        Assert.AreSame(newRef0, deserialized.Reference_0_1);
        Assert.AreSame(newRef0, deserialized.Reference_0_n.First());
        
        var newRefN1 = new NullableReferencesTestLang.LinkTestConcept("refN1");
        deserialized.Containment_0_1 = newRefN1;

        Assert.AreSame(newRefN1, deserialized.Reference_0_n.Last());
    }

    private class UnresolvedReferencesDeserializerHandler(UnresolvedReferencesManager unresolvedReferencesManager) : DeserializerExceptionHandler
    {
        public override IReferenceTarget? UnresolvableReferenceTarget(IReferenceTarget target,
            Feature reference, IReadableNode parent) =>
            unresolvedReferencesManager.RegisterUnresolvedReference((IWritableNode)parent, reference, target);
    }
}