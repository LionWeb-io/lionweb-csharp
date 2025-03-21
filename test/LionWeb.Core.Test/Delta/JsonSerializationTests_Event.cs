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

[TestClass]
public class JsonSerializationTests_Event : JsonSerializationTestsBase
{
    #region Partitions

    [TestMethod]
    public void PartitionAdded()
    {
        var input = new PartitionAdded(CreateChunk(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void PartitionDeleted()
    {
        var input = new PartitionDeleted(CreateChunk(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Nodes

    [TestMethod]
    public void ClassifierChanged()
    {
        var input = new ClassifierChanged(CreateTargetNode(), CreateMetaPointer(), CreateMetaPointer(), NextSequence(),
            CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Properties

    [TestMethod]
    public void PropertyAdded()
    {
        var input = new PropertyAdded(CreateTargetNode(), CreateMetaPointer(), CreatePropertyValue(), NextSequence(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var input = new PropertyDeleted(CreateTargetNode(), CreateMetaPointer(), CreatePropertyValue(), NextSequence(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var input = new PropertyChanged(CreateTargetNode(), CreateMetaPointer(), CreatePropertyValue(),
            CreatePropertyValue(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Children

    [TestMethod]
    public void ChildAdded()
    {
        var input = new ChildAdded(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateChunk(),
            NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildDeleted()
    {
        var input = new ChildDeleted(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateChunk(),
            NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildReplaced()
    {
        var input = new ChildReplaced(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateChunk(),
            CreateChunk(), NextSequence(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment()
    {
        var input = new ChildMovedFromOtherContainment(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateTargetNode(), CreateMetaPointer(), CreateIndex(), NextSequence(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent()
    {
        var input = new ChildMovedFromOtherContainmentInSameParent(CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateTargetNode(), CreateMetaPointer(), CreateIndex(), NextSequence(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedInSameContainment()
    {
        var input = new ChildMovedInSameContainment(CreateIndex(), CreateTargetNode(), CreateTargetNode(),
            CreateMetaPointer(), CreateIndex(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment()
    {
        var input = new ChildMovedAndReplacedFromOtherContainment(CreateTargetNode(), CreateMetaPointer(),
            CreateIndex(), CreateTargetNode(), CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateChunk(),
            NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent()
    {
        var input = new ChildMovedAndReplacedFromOtherContainmentInSameParent(CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateChunk(), NextSequence(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment()
    {
        var input = new ChildMovedAndReplacedInSameContainment(CreateIndex(), CreateTargetNode(), CreateTargetNode(),
            CreateMetaPointer(), CreateIndex(), CreateChunk(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Annotations

    [TestMethod]
    public void AnnotationAdded()
    {
        var input = new AnnotationAdded(CreateTargetNode(), CreateIndex(), CreateChunk(), NextSequence(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationDeleted()
    {
        var input = new AnnotationDeleted(CreateTargetNode(), CreateIndex(), CreateChunk(), NextSequence(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationReplaced()
    {
        var input = new AnnotationReplaced(CreateTargetNode(), CreateIndex(), CreateChunk(), CreateChunk(),
            NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedFromOtherParent()
    {
        var input = new AnnotationMovedFromOtherParent(CreateTargetNode(), CreateIndex(), CreateTargetNode(),
            CreateTargetNode(), CreateIndex(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedInSameParent()
    {
        var input = new AnnotationMovedInSameParent(CreateIndex(), CreateTargetNode(), CreateTargetNode(),
            CreateIndex(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedFromOtherParent()
    {
        var input = new AnnotationMovedAndReplacedFromOtherParent(CreateTargetNode(), CreateIndex(), CreateTargetNode(),
            CreateTargetNode(), CreateIndex(), CreateChunk(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent()
    {
        var input = new AnnotationMovedAndReplacedInSameParent(CreateIndex(), CreateTargetNode(), CreateTargetNode(),
            CreateIndex(), CreateChunk(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region References

    [TestMethod]
    public void ReferenceAdded()
    {
        var input = new ReferenceAdded(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateTarget(),
            NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceDeleted()
    {
        var input = new ReferenceDeleted(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateTarget(),
            NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceChanged()
    {
        var input = new ReferenceChanged(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateTarget(),
            CreateTarget(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedFromOtherReference()
    {
        var input = new EntryMovedFromOtherReference(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateTarget(), NextSequence(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedFromOtherReferenceInSameParent()
    {
        var input = new EntryMovedFromOtherReferenceInSameParent(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateMetaPointer(), CreateIndex(), CreateTarget(), NextSequence(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedInSameReference()
    {
        var input = new EntryMovedInSameReference(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateIndex(),
            CreateTarget(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedFromOtherReference()
    {
        var input = new EntryMovedAndReplacedFromOtherReference(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateTarget(), CreateTarget(), NextSequence(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedFromOtherReferenceInSameParent()
    {
        var input = new EntryMovedAndReplacedFromOtherReferenceInSameParent(CreateTargetNode(), CreateMetaPointer(),
            CreateIndex(), CreateMetaPointer(), CreateIndex(), CreateTarget(), CreateTarget(), NextSequence(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedInSameReference()
    {
        var input = new EntryMovedAndReplacedInSameReference(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateIndex(), CreateTarget(), CreateTarget(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoAdded()
    {
        var input = new ReferenceResolveInfoAdded(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateResolveInfo(), CreateTargetNode(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoDeleted()
    {
        var input = new ReferenceResolveInfoDeleted(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateResolveInfo(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoChanged()
    {
        var input = new ReferenceResolveInfoChanged(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateResolveInfo(), CreateTargetNode(), CreateResolveInfo(), NextSequence(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetAdded()
    {
        var input = new ReferenceTargetAdded(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateTargetNode(),
            CreateResolveInfo(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetDeleted()
    {
        var input = new ReferenceTargetDeleted(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateResolveInfo(), CreateTargetNode(), NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetChanged()
    {
        var input = new ReferenceTargetChanged(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateResolveInfo(), CreateTargetNode(), NextSequence(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Miscellaneous

    [TestMethod]
    public void CompositeEvent()
    {
        var input = new CompositeEvent(
        [
            new PropertyDeleted(CreateTargetNode(), CreateMetaPointer(), CreatePropertyValue(), 0, CreateOrigin(),
                CreateProtocolMessage()),
            new ChildDeleted(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateChunk(), 0, CreateOrigin(),
                CreateProtocolMessage()),
            new AnnotationDeleted(CreateTargetNode(), CreateIndex(), CreateChunk(), 0, CreateOrigin(),
                CreateProtocolMessage()),
            new ReferenceDeleted(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateTarget(), 0,
                CreateOrigin(), CreateProtocolMessage())
        ], NextSequence(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void NoOpEvent()
    {
        var input = new NoOpEvent(NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void Error()
    {
        var input = new Error("myError", NextSequence(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    private long _nextSequence = 0;

    private long NextSequence() =>
        _nextSequence++;

    private CommandSource[] CreateOrigin() =>
    [
        CreateCommandSource(),
        CreateCommandSource()
    ];

    private CommandSource CreateCommandSource() =>
        new CommandSource("myParticipation", CreateTargetNode());
}