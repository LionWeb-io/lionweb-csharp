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
public class JsonSerializationTests_Command : JsonSerializationTestsCommandBase
{
    [TestMethod]
    public void CommandResponse()
    {
        var input = CreateCommandResponse();
        AssertSerialization(input);
    }

    #region Partitions

    [TestMethod]
    public void AddPartition()
    {
        var input = CreateAddPartition();
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeletePartition()
    {
        var input = CreateDeletePartition();
        AssertSerialization(input);
    }

    #endregion

    #region Nodes

    [TestMethod]
    public void ChangeClassifier()
    {
        var input = CreateChangeClassifier();
        AssertSerialization(input);
    }

    #endregion

    #region Properties

    [TestMethod]
    public void AddProperty()
    {
        var input = CreateAddProperty();
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteProperty()
    {
        var input = CreateDeleteProperty();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeProperty()
    {
        var input = CreateChangeProperty();
        AssertSerialization(input);
    }

    #endregion

    #region Children

    [TestMethod]
    public void AddChild()
    {
        var input = CreateAddChild();
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteChild()
    {
        var input = CreateDeleteChild();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReplaceChild()
    {
        var input = CreateReplaceChild();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveChildFromOtherContainment()
    {
        var input = CreateMoveChildFromOtherContainment();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveChildFromOtherContainmentInSameParent()
    {
        var input = CreateMoveChildFromOtherContainmentInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveChildInSameContainment()
    {
        var input = CreateMoveChildInSameContainment();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceChildFromOtherContainment()
    {
        var input = CreateMoveAndReplaceChildFromOtherContainment();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceChildFromOtherContainmentInSameParent()
    {
        var input = CreateMoveAndReplaceChildFromOtherContainmentInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceChildInSameContainment()
    {
        var input = CreateMoveAndReplaceChildInSameContainment();
        AssertSerialization(input);
    }

    #endregion

    #region Annotations

    [TestMethod]
    public void AddAnnotation()
    {
        var input = CreateAddAnnotation();
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteAnnotation()
    {
        var input = CreateDeleteAnnotation();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ReplaceAnnotation()
    {
        var input = CreateReplaceAnnotation();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAnnotationFromOtherParent()
    {
        var input = CreateMoveAnnotationFromOtherParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAnnotationInSameParent()
    {
        var input = CreateMoveAnnotationInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceAnnotationFromOtherParent()
    {
        var input = CreateMoveAndReplaceAnnotationFromOtherParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceAnnotationInSameParent()
    {
        var input = CreateMoveAndReplaceAnnotationInSameParent();
        AssertSerialization(input);
    }

    #endregion

    #region References

    [TestMethod]
    public void AddReference()
    {
        var input = CreateAddReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteReference()
    {
        var input = CreateDeleteReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeReference()
    {
        var input = CreateChangeReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveEntryFromOtherReference()
    {
        var input = CreateMoveEntryFromOtherReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveEntryFromOtherReferenceInSameParent()
    {
        var input = CreateMoveEntryFromOtherReferenceInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveEntryInSameReference()
    {
        var input = CreateMoveEntryInSameReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceEntryFromOtherReference()
    {
        var input = CreateMoveAndReplaceEntryFromOtherReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceEntryFromOtherReferenceInSameParent()
    {
        var input = CreateMoveAndReplaceEntryFromOtherReferenceInSameParent();
        AssertSerialization(input);
    }

    [TestMethod]
    public void MoveAndReplaceEntryInSameReference()
    {
        var input = CreateMoveAndReplaceEntryInSameReference();
        AssertSerialization(input);
    }

    [TestMethod]
    public void AddReferenceResolveInfo()
    {
        var input = CreateAddReferenceResolveInfo();
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteReferenceResolveInfo()
    {
        var input = CreateDeleteReferenceResolveInfo();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeReferenceResolveInfo()
    {
        var input = CreateChangeReferenceResolveInfo();
        AssertSerialization(input);
    }

    [TestMethod]
    public void AddReferenceTarget()
    {
        var input = CreateAddReferenceTarget();
        AssertSerialization(input);
    }

    [TestMethod]
    public void DeleteReferenceTarget()
    {
        var input = CreateDeleteReferenceTarget();
        AssertSerialization(input);
    }

    [TestMethod]
    public void ChangeReferenceTarget()
    {
        var input = CreateChangeReferenceTarget();
        AssertSerialization(input);
    }

    #endregion

    [TestMethod]
    public void CompositeCommand()
    {
        var input = CreateCompositeCommand();
        AssertSerialization(input);
    }
}