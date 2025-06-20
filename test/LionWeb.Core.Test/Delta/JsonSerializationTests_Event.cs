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

using Core.Serialization.Delta.Event;

[TestClass]
public class JsonSerializationTests_Event : JsonSerializationTestsBase
{
    #region Partitions

    [TestMethod]
    public void PartitionAdded()
    {
        var input = new PartitionAdded(Chunk(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void PartitionDeleted()
    {
        var input = new PartitionDeleted(TargetNode(), Descendants(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region Nodes

    [TestMethod]
    public void ClassifierChanged()
    {
        var input = new ClassifierChanged(TargetNode(), MetaPointer(), MetaPointer(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region Properties

    [TestMethod]
    public void PropertyAdded()
    {
        var input = new PropertyAdded(TargetNode(), MetaPointer(), PropertyValue(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var input = new PropertyDeleted(TargetNode(), MetaPointer(), PropertyValue(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var input = new PropertyChanged(TargetNode(), MetaPointer(), PropertyValue(), PropertyValue(), Origin(),
            Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region Children

    [TestMethod]
    public void ChildAdded()
    {
        var input = new ChildAdded(TargetNode(), Chunk(), MetaPointer(), Index(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildDeleted()
    {
        var input = new ChildDeleted(TargetNode(), Descendants(), TargetNode(), MetaPointer(), Index(), Origin(),
            Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildReplaced()
    {
        var input = new ChildReplaced(Chunk(), TargetNode(), Descendants(), TargetNode(), MetaPointer(), Index(),
            Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment()
    {
        var input = new ChildMovedFromOtherContainment(TargetNode(), MetaPointer(), Index(), TargetNode(), TargetNode(),
            MetaPointer(), Index(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent()
    {
        var input = new ChildMovedFromOtherContainmentInSameParent(MetaPointer(), Index(), TargetNode(), TargetNode(),
            MetaPointer(), Index(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedInSameContainment()
    {
        var input = new ChildMovedInSameContainment(Index(), TargetNode(), TargetNode(), MetaPointer(), Index(),
            Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment()
    {
        var input = new ChildMovedAndReplacedFromOtherContainment(TargetNode(), MetaPointer(), Index(), TargetNode(),
            TargetNode(), MetaPointer(), Index(), TargetNode(), Descendants(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent()
    {
        var input = new ChildMovedAndReplacedFromOtherContainmentInSameParent(MetaPointer(), Index(), TargetNode(),
            TargetNode(), MetaPointer(), Index(), TargetNode(), Descendants(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment()
    {
        var input = new ChildMovedAndReplacedInSameContainment(Index(), TargetNode(), TargetNode(), MetaPointer(),
            Index(), TargetNode(), Descendants(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region Annotations

    [TestMethod]
    public void AnnotationAdded()
    {
        var input = new AnnotationAdded(TargetNode(), Chunk(), Index(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationDeleted()
    {
        var input = new AnnotationDeleted(TargetNode(), Descendants(), TargetNode(), Index(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationReplaced()
    {
        var input = new AnnotationReplaced(Chunk(), TargetNode(), Descendants(), TargetNode(), Index(), Origin(),
            Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedFromOtherParent()
    {
        var input = new AnnotationMovedFromOtherParent(TargetNode(), Index(), TargetNode(), TargetNode(), Index(),
            Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedInSameParent()
    {
        var input = new AnnotationMovedInSameParent(Index(), TargetNode(), TargetNode(), Index(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedFromOtherParent()
    {
        var input = new AnnotationMovedAndReplacedFromOtherParent(TargetNode(), Index(), TargetNode(), TargetNode(),
            Index(), TargetNode(), Descendants(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent()
    {
        var input = new AnnotationMovedAndReplacedInSameParent(Index(), TargetNode(), TargetNode(), Index(),
            TargetNode(), Descendants(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region References

    [TestMethod]
    public void ReferenceAdded()
    {
        var input = new ReferenceAdded(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(),
            Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceDeleted()
    {
        var input = new ReferenceDeleted(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(),
            Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceChanged()
    {
        var input = new ReferenceChanged(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            TargetNode(), ResolveInfo(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedFromOtherReference()
    {
        var input = new EntryMovedFromOtherReference(TargetNode(), MetaPointer(), Index(), TargetNode(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedFromOtherReferenceInSameParent()
    {
        var input = new EntryMovedFromOtherReferenceInSameParent(TargetNode(), MetaPointer(), Index(), MetaPointer(),
            Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedInSameReference()
    {
        var input = new EntryMovedInSameReference(TargetNode(), MetaPointer(), Index(), Index(), TargetNode(),
            ResolveInfo(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedFromOtherReference()
    {
        var input = new EntryMovedAndReplacedFromOtherReference(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedFromOtherReferenceInSameParent()
    {
        var input = new EntryMovedAndReplacedFromOtherReferenceInSameParent(TargetNode(), MetaPointer(), Index(),
            TargetNode(), ResolveInfo(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(),
            ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedInSameReference()
    {
        var input = new EntryMovedAndReplacedInSameReference(TargetNode(), MetaPointer(), Index(), TargetNode(),
            ResolveInfo(), Index(), TargetNode(), ResolveInfo(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoAdded()
    {
        var input = new ReferenceResolveInfoAdded(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(),
            Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoDeleted()
    {
        var input = new ReferenceResolveInfoDeleted(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoChanged()
    {
        var input = new ReferenceResolveInfoChanged(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(),
            ResolveInfo(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetAdded()
    {
        var input = new ReferenceTargetAdded(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetDeleted()
    {
        var input = new ReferenceTargetDeleted(TargetNode(), MetaPointer(), Index(), ResolveInfo(), TargetNode(),
            Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetChanged()
    {
        var input = new ReferenceTargetChanged(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(),
            TargetNode(), Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion

    #region Miscellaneous

    [TestMethod]
    public void CompositeEvent()
    {
        var input = new CompositeEvent(
        [
            new PropertyDeleted(TargetNode(), MetaPointer(), PropertyValue(), Origin(), 0, ProtocolMessages()),
            new ChildDeleted(TargetNode(), Descendants(), TargetNode(), MetaPointer(), Index(), Origin(), 0,
                ProtocolMessages()),
            new AnnotationDeleted(TargetNode(), Descendants(), TargetNode(), Index(), Origin(), 0, ProtocolMessages()),
            new ReferenceDeleted(TargetNode(), MetaPointer(), Index(), TargetNode(), ResolveInfo(), Origin(), 0,
                ProtocolMessages())
        ], Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void NoOpEvent()
    {
        var input = new NoOpEvent(Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    [TestMethod]
    public void Error()
    {
        var input = new Error("myError", "very nice message", Origin(), Sequence(), ProtocolMessages());
        AssertSerialization(input);
    }

    #endregion
}