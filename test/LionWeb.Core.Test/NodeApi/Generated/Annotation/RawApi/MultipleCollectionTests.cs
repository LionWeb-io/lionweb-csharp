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

using Languages.Generated.V2024_1.Shapes.M2;
using Languages.Generated.V2024_1.TestLanguage;

[TestClass]
public class MultipleCollectionTests
{
    #region Insert

    [TestMethod]
    public void Multiple_Insert_ListMatchingType()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.InsertAnnotationsRaw(0, values[0]);
        parent.InsertAnnotationsRaw(1, values[1]);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Empty()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.InsertAnnotationsRaw(0, values[0]);
        parent.InsertAnnotationsRaw(1, values[1]);
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_Before()
    {
        var ann = new TestAnnotation("cId");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(ann);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.InsertAnnotationsRaw(0, values[0]);
        parent.InsertAnnotationsRaw(1, values[1]);
        Assert.AreSame(parent, ann.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { valueA, valueB, ann }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_One_After()
    {
        var ann = new TestAnnotation("cId");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(ann);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.InsertAnnotationsRaw(1, values[0]);
        parent.InsertAnnotationsRaw(2, values[1]);
        Assert.AreSame(parent, ann.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { ann, valueA, valueB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Before()
    {
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(docA);
        parent.AddAnnotationsRaw(docB);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.InsertAnnotationsRaw(0, values[0]);
        parent.InsertAnnotationsRaw(1, values[1]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { valueA, valueB, docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_Between()
    {
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(docA);
        parent.AddAnnotationsRaw(docB);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.InsertAnnotationsRaw(1, values[0]);
        parent.InsertAnnotationsRaw(2, values[1]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { docA, valueA, valueB, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Insert_Two_After()
    {
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(docA);
        parent.AddAnnotationsRaw(docB);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.InsertAnnotationsRaw(2, values[0]);
        parent.InsertAnnotationsRaw(3, values[1]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, valueA.GetParent());
        Assert.AreSame(parent, valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { docA, docB, valueA, valueB }, parent.GetAnnotations().ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Multiple_Remove_Set()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance>() { valueA, valueB };
        parent.RemoveAnnotationsRaw(values[0]);
        parent.RemoveAnnotationsRaw(values[1]);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_Empty()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.RemoveAnnotationsRaw(values[0]);
        parent.RemoveAnnotationsRaw(values[1]);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(valueA));
        Assert.IsFalse(parent.GetAnnotations().Contains(valueB));
    }

    [TestMethod]
    public void Multiple_Remove_NonContained()
    {
        var docA = new TestAnnotation("cA");
        var docB = new TestAnnotation("cB");
        var parent = new LinkTestConcept("cs");
        parent.AddAnnotationsRaw(docA);
        parent.AddAnnotationsRaw(docB);
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.RemoveAnnotationsRaw(values[0]);
        parent.RemoveAnnotationsRaw(values[1]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_HalfContained()
    {
        var docA = new TestAnnotation("cA");
        var docB = new TestAnnotation("cB");
        var parent = new LinkTestConcept("cs");
        parent.AddAnnotationsRaw(docA);
        parent.AddAnnotationsRaw(docB);
        var valueA = new TestAnnotation("sA");
        var values = new List<IAnnotationInstance> { valueA, docA };
        parent.RemoveAnnotationsRaw(values[0]);
        parent.RemoveAnnotationsRaw(values[1]);
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(docA.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Only()
    {
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(valueA);
        parent.AddAnnotationsRaw(valueB);
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.RemoveAnnotationsRaw(values[0]);
        parent.RemoveAnnotationsRaw(values[1]);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_First()
    {
        var doc = new TestAnnotation("cId");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(valueA);
        parent.AddAnnotationsRaw(valueB);
        parent.AddAnnotationsRaw(doc);
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.RemoveAnnotationsRaw(values[0]);
        parent.RemoveAnnotationsRaw(values[1]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Last()
    {
        var doc = new TestAnnotation("cId");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(doc);
        parent.AddAnnotationsRaw(valueA);
        parent.AddAnnotationsRaw(valueB);
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.RemoveAnnotationsRaw(values[0]);
        parent.RemoveAnnotationsRaw(values[1]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Between()
    {
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(docA);
        parent.AddAnnotationsRaw(valueA);
        parent.AddAnnotationsRaw(valueB);
        parent.AddAnnotationsRaw(docB);
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.RemoveAnnotationsRaw(values[0]);
        parent.RemoveAnnotationsRaw(values[1]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Multiple_Remove_Mixed()
    {
        var docA = new TestAnnotation("cIdA");
        var docB = new TestAnnotation("cIdB");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var parent = new LinkTestConcept("g");
        parent.AddAnnotationsRaw(valueA);
        parent.AddAnnotationsRaw(docA);
        parent.AddAnnotationsRaw(valueB);
        parent.AddAnnotationsRaw(docB);
        var values = new List<IAnnotationInstance> { valueA, valueB };
        parent.RemoveAnnotationsRaw(values[0]);
        parent.RemoveAnnotationsRaw(values[1]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
        CollectionAssert.AreEqual(new List<IAnnotationInstance> { docA, docB }, parent.GetAnnotations().ToList());
    }

    #endregion

    [TestMethod]
    public void MultipleListMatchingType()
    {
        var parent = new LinkTestConcept("g");
        var valueA = new TestAnnotation("sA");
        var valueB = new TestAnnotation("sB");
        var values = new List<IAnnotationInstance>() { valueA, valueB };
        Assert.IsTrue(parent.AddAnnotationsRaw(values[0]));
        Assert.IsTrue(parent.AddAnnotationsRaw(values[1]));
        Assert.AreSame(parent, valueA.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueA));
        Assert.AreSame(parent, valueB.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(valueB));
    }


    [TestMethod]
    public void SingleList_NotAnnotating()
    {
        var parent = new LinkTestConcept("g");
        var value = new Documentation("sA");
        var values = new List<IAnnotationInstance>() { value };
        Assert.IsFalse(parent.AddAnnotationsRaw(values[0]));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Insert()
    {
        var parent = new LinkTestConcept("g");
        var value = new Documentation("sA");
        var values = new List<IAnnotationInstance>() { value };
        Assert.IsFalse(parent.InsertAnnotationsRaw(0, values[0]));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }

    [TestMethod]
    public void SingleList_NotAnnotating_Remove()
    {
        var parent = new LinkTestConcept("g");
        var value = new Documentation("sA");
        var values = new List<IAnnotationInstance>() { value };
        Assert.IsFalse(parent.RemoveAnnotationsRaw(values[0]));
        Assert.IsNull(value.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(value));
    }
}