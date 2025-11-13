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

using Core.Notification;
using Core.Notification.Pipe;
using Core.Serialization;
using Languages.Generated.V2024_1.TestLanguage;
using M1;
using M3;
using System.Collections;

[TestClass]
public class DeadReferenceSerializationTests
{
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    private readonly Language _language = TestLanguageLanguage.Instance;

    [TestMethod]
    public void DeserializeMissingReference()
    {
        var input = new LinkTestConcept("root")
        {
            Reference_0_1 = new LinkTestConcept("ref0"),
            Reference_0_n = [new LinkTestConcept("ref0"), new LinkTestConcept("refN1")]
        };

        var serializer = new SerializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .Build();

        var stream = new MemoryStream();
        JsonUtils.WriteNodesToStream(stream, serializer, [input]);

        var unresolvedReferencesManager = new UnresolvedReferencesManager();
        
        var deserializer = new DeserializerBuilder()
            .WithLionWebVersion(_lionWebVersion)
            .WithLanguage(_language)
            .WithHandler(new ReceiverDeserializerHandler(unresolvedReferencesManager))
            .Build();

        stream.Position = 0;
        var nodes = JsonUtils.ReadNodesFromStream(stream, deserializer);

        Assert.HasCount(1, nodes);

        var deserialized = nodes.OfType<LinkTestConcept>().Single();

        Assert.IsNull(deserialized.Reference_0_1);
        Assert.IsEmpty(deserialized.Reference_0_n);
        
        deserialized.GetNotificationSender()!.ConnectTo(unresolvedReferencesManager);

        var newRef0 = new LinkTestConcept("ref0");
        deserialized.Containment_0_1 = newRef0;
        
        Assert.AreSame(newRef0, deserialized.Reference_0_1);
        Assert.AreSame(newRef0, deserialized.Reference_0_n.First());
        
        var newRefN1 = new LinkTestConcept("refN1");
        deserialized.Containment_0_1 = newRefN1;

        Assert.AreSame(newRefN1, deserialized.Reference_0_n.Last());
    }

    internal class ReceiverDeserializerHandler(UnresolvedReferencesManager unresolvedReferencesManager) : DeserializerExceptionHandler
    {

        public override bool SkipDeserializingDependentNode(ICompressedId id) =>
            false;

        public override IReferenceDescriptor? UnresolvableReferenceTarget(ICompressedId? targetId,
            ResolveInfo? resolveInfo,
            Feature reference, IReadableNode node) =>
            unresolvedReferencesManager.RegisterUnresolvedReference((IWritableNode)node, reference,
                new ReferenceDescriptor(resolveInfo, targetId?.Original, null));
    }
}

public class UnresolvedReferencesManager : INotificationReceiver
{
    private readonly List<(IWritableNode parent, Feature reference, IReferenceDescriptor descriptor)>
        _unresolvedReferences = [];
    
    public void Receive(INotificationSender correspondingSender, INotification notification)
    {
        if (notification is not INewNodeNotification newNodeNotification)
            return;

        _unresolvedReferences.RemoveAll(e => RemoveMatchingReferences(newNodeNotification, e));
    }

    private bool RemoveMatchingReferences(INewNodeNotification newNodeNotification, (IWritableNode parent, Feature reference, IReferenceDescriptor descriptor) e)
    {
        if (e.descriptor.TargetId != newNodeNotification.NewNode.GetId())
            return false;

        var descriptor = new ReferenceDescriptor(e.descriptor.ResolveInfo, e.descriptor.TargetId, newNodeNotification.NewNode);

        if (!e.parent.TryGet(e.reference, out var value))
        {
            if (e.reference is Reference { Multiple: true })
            {
                value = new List<IReferenceDescriptor> { descriptor };
            } else
            {
                value = descriptor;
            }
        } else
        {
            if (value is IList list)
            {
                value = (List<IReferenceDescriptor>)[..list.Cast<IReadableNode>().Select(ReferenceDescriptorExtensions.FromNode), descriptor];
            } else
            {
                value = descriptor;
            }
        }

        e.parent.Set(e.reference, value);
        return true;
    }

    public IReferenceDescriptor RegisterUnresolvedReference(IWritableNode parent, Feature reference, IReferenceDescriptor descriptor)
    {
        _unresolvedReferences.Add((parent, reference, descriptor));
        return descriptor;
    }
}