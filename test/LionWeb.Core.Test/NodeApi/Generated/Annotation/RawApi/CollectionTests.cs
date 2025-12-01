// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace LionWeb.Core.Test.NodeApi.Generated.Annotation.RawApi;

using LionWeb.Core.Test.Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class CollectionTests
{
    #region EmptyCollection

    [TestMethod]
    public void EmptyList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<IAnnotationInstance>();
        Assert.IsFalse(parent.AddAnnotationsRaw(values));
        Assert.IsEmpty(parent.GetAnnotations());
    }


    [TestMethod]
    public void Insert_EmptyList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<IAnnotationInstance>();
        Assert.IsFalse(parent.InsertAnnotationsRaw(0, values));
        Assert.IsEmpty(parent.GetAnnotations());
    }

    [TestMethod]
    public void Remove_EmptyList()
    {
        var parent = new LinkTestConcept("g");
        var values = new HashSet<IAnnotationInstance>();
        Assert.IsFalse(parent.RemoveAnnotationsRaw(values));
        Assert.IsEmpty(parent.GetAnnotations());
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<IAnnotationInstance> { null };
        Assert.IsFalse(parent.AddAnnotationsRaw(values));
        Assert.IsEmpty(parent.GetAnnotations());
    }


    [TestMethod]
    public void Insert_NullList()
    {
        var parent = new LinkTestConcept("g");
        var values = new List<IAnnotationInstance> { null };
        Assert.IsFalse(parent.InsertAnnotationsRaw(0, values));
        Assert.IsEmpty(parent.GetAnnotations());
    }

    [TestMethod]
    public void Remove_NullList()
    {
        var parent = new LinkTestConcept("g");
        var values = new HashSet<IAnnotationInstance> { null };
        Assert.IsFalse(parent.RemoveAnnotationsRaw(values));
        Assert.IsEmpty(parent.GetAnnotations());
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleList()
    {
        var parent = new LinkTestConcept("g");
        var value = new TestAnnotation("s");
        var values = new List<IAnnotationInstance> { value };
        Assert.IsTrue(parent.AddAnnotationsRaw(values));
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void Insert_SingleList()
    {
        var parent = new LinkTestConcept("g");
        var value = new TestAnnotation("s");
        var values = new List<IAnnotationInstance> { value };
        Assert.IsTrue(parent.InsertAnnotationsRaw(0, values));
        Assert.AreSame(parent, value.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(value));
    }

    #region Remove

    [TestMethod]
    public void SingleList_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var annotation = new TestAnnotation("myId");
        var values = new HashSet<IAnnotationInstance> { annotation };
        Assert.IsFalse(parent.RemoveAnnotationsRaw(values));
        Assert.IsNull(annotation.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(annotation));
    }

    [TestMethod]
    public void SingleList_Remove_Only()
    {
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw([annotation]));
        var values = new HashSet<IAnnotationInstance> { annotation };
        Assert.IsTrue(parent.RemoveAnnotationsRaw(values));
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void SingleList_Remove_First()
    {
        var otherAnnotation = new TestAnnotation("cId");
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw([annotation, otherAnnotation]));
        var values = new HashSet<IAnnotationInstance> { annotation };
        Assert.IsTrue(parent.RemoveAnnotationsRaw(values));
        Assert.AreSame(parent, otherAnnotation.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { otherAnnotation }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void SingleList_Remove_Last()
    {
        var otherAnnotation = new TestAnnotation("cId");
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw([otherAnnotation, annotation]));
        var values = new HashSet<IAnnotationInstance> { annotation };
        Assert.IsTrue(parent.RemoveAnnotationsRaw(values));
        Assert.AreSame(parent, otherAnnotation.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { otherAnnotation }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void SingleList_Remove_Between()
    {
        var otherAnnotationA = new TestAnnotation("cIdA");
        var otherAnnotationB = new TestAnnotation("cIdB");
        var annotation = new TestAnnotation("myId");
        var parent = new LinkTestConcept("g");
        Assert.IsTrue(parent.AddAnnotationsRaw([otherAnnotationA, annotation, otherAnnotationB]));
        var values = new HashSet<IAnnotationInstance> { annotation };
        Assert.IsTrue(parent.RemoveAnnotationsRaw(values));
        Assert.AreSame(parent, otherAnnotationA.GetParent());
        Assert.AreSame(parent, otherAnnotationB.GetParent());
        Assert.IsNull(annotation.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { otherAnnotationA, otherAnnotationB },
            parent.GetAnnotations().ToList());
    }

    #endregion

    #endregion
}