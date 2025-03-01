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
        var input = new PartitionAdded(CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void PartitionDeleted()
    {
        var input = new PartitionDeleted(CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Nodes

    [TestMethod]
    public void ClassifierChanged()
    {
        var input = new ClassifierChanged(CreateTargetNode(), CreateMetaPointer(), CreateMetaPointer(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Properties

    [TestMethod]
    public void PropertyAdded()
    {
        var input = new PropertyAdded(CreateProperty(), CreatePropertyValue(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var input = new PropertyDeleted(CreateProperty(), CreatePropertyValue(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var input = new PropertyChanged(CreateProperty(), CreatePropertyValue(), CreatePropertyValue(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Children

    [TestMethod]
    public void ChildAdded()
    {
        var input = new ChildAdded(CreateContainment(), CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildDeleted()
    {
        var input = new ChildDeleted(CreateContainment(), CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildReplaced()
    {
        var input = new ChildReplaced(CreateContainment(), CreateChunk(), CreateChunk(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment()
    {
        var input = new ChildMovedFromOtherContainment(CreateContainment(), CreateTargetNode(), CreateContainment(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent()
    {
        var input = new ChildMovedFromOtherContainmentInSameParent(CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedInSameContainment()
    {
        var input = new ChildMovedInSameContainment(CreateIndex(), CreateTargetNode(), CreateTargetNode(),
            CreateMetaPointer(), CreateIndex(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment()
    {
        var input = new ChildMovedAndReplacedFromOtherContainment(CreateContainment(), CreateTargetNode(),
            CreateContainment(), CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent()
    {
        var input = new ChildMovedAndReplacedFromOtherContainmentInSameParent(CreateMetaPointer(), CreateIndex(),
            CreateTargetNode(), CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateChunk(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment()
    {
        var input = new ChildMovedAndReplacedInSameContainment(CreateIndex(), CreateTargetNode(), CreateTargetNode(),
            CreateMetaPointer(), CreateIndex(), CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Annotations

    [TestMethod]
    public void AnnotationAdded()
    {
        var input = new AnnotationAdded(CreateAnnotation(), CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationDeleted()
    {
        var input = new AnnotationDeleted(CreateAnnotation(), CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationReplaced()
    {
        var input = new AnnotationReplaced(CreateAnnotation(), CreateChunk(), CreateChunk(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedFromOtherParent()
    {
        var input = new AnnotationMovedFromOtherParent(CreateAnnotation(), CreateTargetNode(), CreateAnnotation(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedInSameParent()
    {
        var input = new AnnotationMovedInSameParent(CreateIndex(), CreateTargetNode(), CreateTargetNode(),
            CreateIndex(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedFromOtherParent()
    {
        var input = new AnnotationMovedAndReplacedFromOtherParent(CreateAnnotation(), CreateTargetNode(),
            CreateAnnotation(), CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent()
    {
        var input = new AnnotationMovedAndReplacedInSameParent(CreateIndex(), CreateTargetNode(), CreateTargetNode(),
            CreateIndex(), CreateChunk(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region References

    [TestMethod]
    public void ReferenceAdded()
    {
        var input = new ReferenceAdded(CreateReference(), CreateTarget(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceDeleted()
    {
        var input = new ReferenceDeleted(CreateReference(), CreateTarget(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceChanged()
    {
        var input = new ReferenceChanged(CreateReference(), CreateTarget(), CreateTarget(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedFromOtherReference()
    {
        var input = new EntryMovedFromOtherReference(CreateReference(), CreateReference(), CreateTarget(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedFromOtherReferenceInSameParent()
    {
        var input = new EntryMovedFromOtherReferenceInSameParent(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateMetaPointer(), CreateIndex(), CreateTarget(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedInSameReference()
    {
        var input = new EntryMovedInSameReference(CreateTargetNode(), CreateMetaPointer(), CreateIndex(), CreateIndex(),
            CreateTarget(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedFromOtherReference()
    {
        var input = new EntryMovedAndReplacedFromOtherReference(CreateReference(), CreateReference(), CreateTarget(),
            CreateTarget(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedFromOtherReferenceInSameParent()
    {
        var input = new EntryMovedAndReplacedFromOtherReferenceInSameParent(CreateTargetNode(), CreateMetaPointer(),
            CreateIndex(), CreateMetaPointer(), CreateIndex(), CreateTarget(), CreateTarget(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedInSameReference()
    {
        var input = new EntryMovedAndReplacedInSameReference(CreateTargetNode(), CreateMetaPointer(), CreateIndex(),
            CreateIndex(), CreateTarget(), CreateTarget(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoAdded()
    {
        var input = new ReferenceResolveInfoAdded(CreateReference(), CreateResolveInfo(), CreateTargetNode(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoDeleted()
    {
        var input = new ReferenceResolveInfoDeleted(CreateReference(), CreateTargetNode(), CreateResolveInfo(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoChanged()
    {
        var input = new ReferenceResolveInfoChanged(CreateReference(), CreateResolveInfo(), CreateTargetNode(),
            CreateResolveInfo(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetAdded()
    {
        var input = new ReferenceTargetAdded(CreateReference(), CreateTargetNode(), CreateResolveInfo(), CreateOrigin(),
            CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetDeleted()
    {
        var input = new ReferenceTargetDeleted(CreateReference(), CreateResolveInfo(), CreateTargetNode(),
            CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetChanged()
    {
        var input = new ReferenceTargetChanged(CreateReference(), CreateTargetNode(), CreateResolveInfo(),
            CreateTargetNode(), CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    #region Miscellaneous

    [TestMethod]
    public void CompositeEvent()
    {
        var input = new CompositeEvent(
        [
            new PropertyDeleted(CreateProperty(), CreatePropertyValue(), CreateOrigin(), CreateProtocolMessage()),
            new ChildDeleted(CreateContainment(), CreateChunk(), CreateOrigin(), CreateProtocolMessage()),
            new AnnotationDeleted(CreateAnnotation(), CreateChunk(), CreateOrigin(), CreateProtocolMessage()),
            new ReferenceDeleted(CreateReference(), CreateTarget(), CreateOrigin(), CreateProtocolMessage())
        ], CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void NoOpEvent()
    {
        var input = new NoOpEvent(CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    [TestMethod]
    public void Error()
    {
        var input = new Error("myError", CreateOrigin(), CreateProtocolMessage());
        AssertSerialization(input);
    }

    #endregion

    private CommandSource[] CreateOrigin() =>
    [
        CreateCommandSource(),
        CreateCommandSource()
    ];

    private CommandSource CreateCommandSource() =>
        new CommandSource(CreateTargetNode());
}