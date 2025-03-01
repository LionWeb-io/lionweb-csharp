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

namespace LionWeb.Core.Test.Delta;

using Core.Serialization;
using TargetNode = NodeId;
using CommandId = NodeId;
using QueryId = NodeId;
using FreeId = NodeId;
using MessageKind = NodeId;
using MessageDataKey = NodeId;
using PropertyValue = string;
using ResolveInfo = string;
using MetaPointerKey = string;
using Index = int;
using EventId = string;

[TestClass]
public class JsonSerializationTests_Command : JsonSerializationTestsBase
{
    [TestMethod]
    public void CommandResponse()
    {
        var input = new CommandResponse(CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #region Partitions

    [TestMethod]
    public void AddPartition()
    {
        var input = new AddPartition(CreateChunk(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeletePartition()
    {
        var input = new DeletePartition(CreateTargetNode(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Nodes

    [TestMethod]
    public void ChangeClassifier()
    {
        var input = new ChangeClassifier(CreateTargetNode(), CreateMetaPointer(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Properties

    [TestMethod]
    public void AddProperty()
    {
        var input = new AddProperty(CreateProperty(), CreatePropertyValue(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteProperty()
    {
        var input = new DeleteProperty(CreateProperty(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeProperty()
    {
        var input = new ChangeProperty(CreateProperty(), CreatePropertyValue(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Children

    [TestMethod]
    public void AddChild()
    {
        var input = new AddChild(CreateContainment(), CreateChunk(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteChild()
    {
        var input = new DeleteChild(CreateContainment(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReplaceChild()
    {
        var input = new ReplaceChild(CreateContainment(), CreateChunk(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveChildFromOtherContainment()
    {
        var input = new MoveChildFromOtherContainment(CreateContainment(), CreateTargetNode(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveChildFromOtherContainmentInSameParent()
    {
        var input = new MoveChildFromOtherContainmentInSameParent(CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveChildInSameContainment()
    {
        var input = new MoveChildInSameContainment(CreateIndex(), CreateTargetNode(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceChildFromOtherContainment()
    {
        var input = new MoveAndReplaceChildFromOtherContainment(CreateContainment(), CreateTargetNode(),
            CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceChildFromOtherContainmentInSameParent()
    {
        var input = new MoveAndReplaceChildFromOtherContainmentInSameParent(CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceChildInSameContainment()
    {
        var input = new MoveAndReplaceChildInSameContainment(CreateIndex(), CreateTargetNode(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Annotations

    [TestMethod]
    public void AddAnnotation()
    {
        var input = new AddAnnotation(CreateAnnotation(), CreateChunk(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteAnnotation()
    {
        var input = new DeleteAnnotation(CreateAnnotation(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReplaceAnnotation()
    {
        var input = new ReplaceAnnotation(CreateAnnotation(), CreateChunk(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAnnotationFromOtherParent()
    {
        var input = new MoveAnnotationFromOtherParent(CreateAnnotation(), CreateTargetNode(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAnnotationInSameParent()
    {
        var input = new MoveAnnotationInSameParent(CreateIndex(), CreateTargetNode(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceAnnotationFromOtherParent()
    {
        var input = new MoveAndReplaceAnnotationFromOtherParent(CreateAnnotation(), CreateTargetNode(),
            CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceAnnotationInSameParent()
    {
        var input = new MoveAndReplaceAnnotationInSameParent(CreateIndex(), CreateTargetNode(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region References

    [TestMethod]
    public void AddReference()
    {
        var input = new AddReference(CreateReference(), CreateTarget(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteReference()
    {
        var input = new DeleteReference(CreateReference(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeReference()
    {
        var input = new ChangeReference(CreateReference(), CreateTarget(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveEntryFromOtherReference()
    {
        var input = new MoveEntryFromOtherReference(CreateReference(), CreateReference(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveEntryFromOtherReferenceInSameParent()
    {
        var input = new MoveEntryFromOtherReferenceInSameParent(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateMetaPointer(), CreateIndex(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveEntryInSameReference()
    {
        var input = new MoveEntryInSameReference(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateIndex(),
            CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceEntryFromOtherReference()
    {
        var input = new MoveAndReplaceEntryFromOtherReference(CreateReference(), CreateReference(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceEntryFromOtherReferenceInSameParent()
    {
        var input = new MoveAndReplaceEntryFromOtherReferenceInSameParent(CreateTargetNode(), CreateMetaPointer(),
            CreateIndex(), CreateMetaPointer(), CreateIndex(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceEntryInSameReference()
    {
        var input = new MoveAndReplaceEntryInSameReference(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateIndex(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AddReferenceResolveInfo()
    {
        var input = new AddReferenceResolveInfo(CreateReference(), CreateResolveInfo(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteReferenceResolveInfo()
    {
        var input = new DeleteReferenceResolveInfo(CreateReference(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeReferenceResolveInfo()
    {
        var input = new ChangeReferenceResolveInfo(CreateReference(), CreateResolveInfo(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AddReferenceTarget()
    {
        var input = new AddReferenceTarget(CreateReference(), CreateTargetNode(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteReferenceTarget()
    {
        var input = new DeleteReferenceTarget(CreateReference(), CreateCommandId(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeReferenceTarget()
    {
        var input = new ChangeReferenceTarget(CreateReference(), CreateTargetNode(), CreateCommandId(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    [TestMethod]
    public void CompositeCommand()
    {
        var input = new CompositeCommand([
            new DeleteProperty(CreateProperty(), CreateCommandId(), CreateProtocolMessage()),
            new DeleteChild(CreateContainment(), CreateCommandId(), CreateProtocolMessage()),
            new DeleteAnnotation(CreateAnnotation(), CreateCommandId(), CreateProtocolMessage()),
            new DeleteReference(CreateReference(), CreateCommandId(), CreateProtocolMessage())
        ], CreateProtocolMessage());
        AssertSerialization(input);
    }

    private int _nextCommandId = 0;

    private CommandId CreateCommandId() =>
        (++_nextCommandId).ToString();
}