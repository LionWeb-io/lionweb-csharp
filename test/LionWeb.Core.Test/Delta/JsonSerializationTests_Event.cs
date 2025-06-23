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

[TestClass]
public class JsonSerializationTests_Event : JsonSerializationTestsEventBase
{
    #region Partitions

    [TestMethod]
    public void PartitionAdded()
    {
        var input = CreatePartitionAdded();
        AssertSerialization(input);
    }

    [TestMethod]
    public void PartitionDeleted()
    {
        var input = CreatePartitionDeleted();
        AssertSerialization(input);
    }

    #endregion

    #region Nodes

    [TestMethod]
    public void ClassifierChanged()
    {
        var input = CreateClassifierChanged();
        AssertSerialization(input);
    }

    #endregion

    #region Properties

    [TestMethod]
    public void PropertyAdded()
    {
        var input = CreatePropertyAdded();
        AssertSerialization(input);
    }

    [TestMethod]
    public void PropertyDeleted()
    {
        var input = CreatePropertyDeleted();
        AssertSerialization(input);
    }

    [TestMethod]
    public void PropertyChanged()
    {
        var input = CreatePropertyChanged();
        AssertSerialization(input);
    }

    #endregion

    #region Children

    [TestMethod]
    public void ChildAdded()
    {
        var input = CreateChildAdded();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildDeleted()
    {
        var input = CreateChildDeleted();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildReplaced()
    {
        var input = CreateChildReplaced();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainment()
    {
        var input = CreateChildMovedFromOtherContainment();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedFromOtherContainmentInSameParent()
    {
        var input = CreateChildMovedFromOtherContainmentInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedInSameContainment()
    {
        var input = CreateChildMovedInSameContainment();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainment()
    {
        var input = CreateChildMovedAndReplacedFromOtherContainment();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedFromOtherContainmentInSameParent()
    {
        var input = CreateChildMovedAndReplacedFromOtherContainmentInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChildMovedAndReplacedInSameContainment()
    {
        var input = CreateChildMovedAndReplacedInSameContainment();
        AssertSerialization(input);
    }

    #endregion

    #region Annotations

    [TestMethod]
    public void AnnotationAdded()
    {
        var input = CreateAnnotationAdded();
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationDeleted()
    {
        var input = CreateAnnotationDeleted();
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationReplaced()
    {
        var input = CreateAnnotationReplaced();
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedFromOtherParent()
    {
        var input = CreateAnnotationMovedFromOtherParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedInSameParent()
    {
        var input = CreateAnnotationMovedInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedFromOtherParent()
    {
        var input = CreateAnnotationMovedAndReplacedFromOtherParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void AnnotationMovedAndReplacedInSameParent()
    {
        var input = CreateAnnotationMovedAndReplacedInSameParent();
        AssertSerialization(input);
    }

    #endregion

    #region References

    [TestMethod]
    public void ReferenceAdded()
    {
        var input = CreateReferenceAdded();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceDeleted()
    {
        var input = CreateReferenceDeleted();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceChanged()
    {
        var input = CreateReferenceChanged();
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedFromOtherReference()
    {
        var input = CreateEntryMovedFromOtherReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedFromOtherReferenceInSameParent()
    {
        var input = CreateEntryMovedFromOtherReferenceInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedInSameReference()
    {
        var input = CreateEntryMovedInSameReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedFromOtherReference()
    {
        var input = CreateEntryMovedAndReplacedFromOtherReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedFromOtherReferenceInSameParent()
    {
        var input = CreateEntryMovedAndReplacedFromOtherReferenceInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void EntryMovedAndReplacedInSameReference()
    {
        var input = CreateEntryMovedAndReplacedInSameReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoAdded()
    {
        var input = CreateReferenceResolveInfoAdded();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoDeleted()
    {
        var input = CreateReferenceResolveInfoDeleted();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceResolveInfoChanged()
    {
        var input = CreateReferenceResolveInfoChanged();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetAdded()
    {
        var input = CreateReferenceTargetAdded();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetDeleted()
    {
        var input = CreateReferenceTargetDeleted();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReferenceTargetChanged()
    {
        var input = CreateReferenceTargetChanged();
        AssertSerialization(input);
    }

    #endregion

    #region Miscellaneous

    [TestMethod]
    public void CompositeEvent()
    {
        var input = CreateCompositeEvent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void NoOpEvent()
    {
        var input = CreateNoOpEvent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void Error()
    {
        var input = CreateError();
        AssertSerialization(input);
    }

    #endregion
}