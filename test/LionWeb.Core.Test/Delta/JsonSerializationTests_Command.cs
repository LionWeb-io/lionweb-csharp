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

using Core.Serialization.Delta.Command;

[TestClass]
public class JsonSerializationTests_Command : JsonSerializationTestsBase
{
    [TestMethod]
    public void CommandResponse()
    {
        var input = new CommandResponse(CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    #region Partitions

    [TestMethod]
    public void AddPartition()
    {
        var input = new AddPartition(Chunk(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeletePartition()
    {
        var input = new DeletePartition(TargetNode(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region Nodes

    [TestMethod]
    public void ChangeClassifier()
    {
        var input = new ChangeClassifier(TargetNode(), MetaPointer(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region Properties

    [TestMethod]
    public void AddProperty()
    {
        var input = new AddProperty(TargetNode(), MetaPointer(), PropertyValue(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteProperty()
    {
        var input = new DeleteProperty(TargetNode(), MetaPointer(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeProperty()
    {
        var input = new ChangeProperty(TargetNode(), MetaPointer(), PropertyValue(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region Children

    [TestMethod]
    public void AddChild()
    {
        var input = new AddChild(TargetNode(), Chunk(), MetaPointer(), Index(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteChild()
    {
        var input = new DeleteChild(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReplaceChild()
    {
        var input = new ReplaceChild(Chunk(), TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveChildFromOtherContainment()
    {
        var input = new MoveChildFromOtherContainment(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveChildFromOtherContainmentInSameParent()
    {
        var input = new MoveChildFromOtherContainmentInSameParent(MetaPointer(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveChildInSameContainment()
    {
        var input = new MoveChildInSameContainment(Index(), TargetNode(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceChildFromOtherContainment()
    {
        var input = new MoveAndReplaceChildFromOtherContainment(TargetNode(), MetaPointer(), Index(), TargetNode(),
            TargetNode(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceChildFromOtherContainmentInSameParent()
    {
        var input = new MoveAndReplaceChildFromOtherContainmentInSameParent(MetaPointer(), Index(), TargetNode(),
            TargetNode(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceChildInSameContainment()
    {
        var input = new MoveAndReplaceChildInSameContainment(Index(), TargetNode(), TargetNode(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region Annotations

    [TestMethod]
    public void AddAnnotation()
    {
        var input = new AddAnnotation(TargetNode(), Chunk(), Index(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteAnnotation()
    {
        var input = new DeleteAnnotation(TargetNode(), Index(), TargetNode(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReplaceAnnotation()
    {
        var input = new ReplaceAnnotation(Chunk(), TargetNode(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAnnotationFromOtherParent()
    {
        var input = new MoveAnnotationFromOtherParent(TargetNode(), Index(), TargetNode(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAnnotationInSameParent()
    {
        var input = new MoveAnnotationInSameParent(Index(), TargetNode(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceAnnotationFromOtherParent()
    {
        var input = new MoveAndReplaceAnnotationFromOtherParent(TargetNode(), Index(), TargetNode(), TargetNode(),
            CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceAnnotationInSameParent()
    {
        var input = new MoveAndReplaceAnnotationInSameParent(Index(), TargetNode(), TargetNode(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region References

    [TestMethod]
    public void AddReference()
    {
        var input = new AddReference(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteReference()
    {
        var input = new DeleteReference(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeReference()
    {
        var input = new ChangeReference(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), TargetNode(),
            ResolveInfo(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveEntryFromOtherReference()
    {
        var input = new MoveEntryFromOtherReference(TargetNode(), MetaPointer(), Index(), TargetNode(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveEntryFromOtherReferenceInSameParent()
    {
        var input = new MoveEntryFromOtherReferenceInSameParent(TargetNode(), MetaPointer(), Index(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveEntryInSameReference()
    {
        var input = new MoveEntryInSameReference(TargetNode(), MetaPointer(), Index(), Index(), TargetNode(),
            ResolveInfo(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceEntryFromOtherReference()
    {
        var input = new MoveAndReplaceEntryFromOtherReference(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceEntryFromOtherReferenceInSameParent()
    {
        var input = new MoveAndReplaceEntryFromOtherReferenceInSameParent(TargetNode(), MetaPointer(), Index(),
            TargetNode(), ResolveInfo(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceEntryInSameReference()
    {
        var input = new MoveAndReplaceEntryInSameReference(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), Index(), TargetNode(), ResolveInfo(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AddReferenceResolveInfo()
    {
        var input = new AddReferenceResolveInfo(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteReferenceResolveInfo()
    {
        var input = new DeleteReferenceResolveInfo(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeReferenceResolveInfo()
    {
        var input = new ChangeReferenceResolveInfo(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            ResolveInfo(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AddReferenceTarget()
    {
        var input = new AddReferenceTarget(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(),
            CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteReferenceTarget()
    {
        var input = new DeleteReferenceTarget(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeReferenceTarget()
    {
        var input = new ChangeReferenceTarget(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            TargetNode(), CommandId(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    [TestMethod]
    public void CompositeCommand()
    {
        var input = new CompositeCommand([
            new DeleteProperty(TargetNode(), MetaPointer(), CommandId(), ProtocolMessages()),
            new DeleteChild(TargetNode(), MetaPointer(), Index(), TargetNode(), CommandId(), ProtocolMessages()),
            new DeleteAnnotation(TargetNode(), Index(), TargetNode(), CommandId(), ProtocolMessages()),
            new DeleteReference(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), CommandId(),
                ProtocolMessages())
        ], ProtocolMessages());
        AssertSerialization(input);
    }
}