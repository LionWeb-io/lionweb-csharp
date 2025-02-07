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

namespace LionWeb.Core.Test.NodeApi.Generated;

using Languages.Generated.V2024_1.Shapes.M2;
using System.Collections;

[TestClass]
public class ContainmentTests_Single_Optional
{
    #region Single

    [TestMethod]
    public void Single()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.Documentation = doc;
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Single_Setter()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.SetDocumentation(doc);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Single_Reflective()
    {
        var parent = new Geometry("g");
        var doc = new Documentation("myId");
        parent.Set(ShapesLanguage.Instance.Geometry_documentation, doc);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Single_Constructor()
    {
        var doc = new Documentation("myId");
        var parent = new Geometry("g") { Documentation = doc };
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Result_Reflective()
    {
        var doc = new Documentation("myId");
        var parent = new Geometry("g") { Documentation = doc };
        Assert.AreSame(doc, parent.Get(ShapesLanguage.Instance.Geometry_documentation));
    }

    [TestMethod]
    public void Single_TryGet()
    {
        var doc = new Documentation("myId");
        var parent = new Geometry("g") { Documentation = doc };
        Assert.IsTrue(parent.TryGetDocumentation(out var o));
        Assert.AreSame(doc, o);
    }

    #region existing

    [TestMethod]
    public void Existing()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };
        var doc = new Documentation("myId");
        parent.Documentation = doc;
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Existing_Setter()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };
        var doc = new Documentation("myId");
        parent.SetDocumentation(doc);
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    [TestMethod]
    public void Existing_Reflective()
    {
        var oldDoc = new Documentation("old");
        var parent = new Geometry("g") { Documentation = oldDoc };
        var doc = new Documentation("myId");
        parent.Set(ShapesLanguage.Instance.Geometry_documentation, doc);
        Assert.IsNull(oldDoc.GetParent());
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(doc, parent.Documentation);
    }

    #endregion

    #endregion

    #region Null

    [TestMethod]
    public void Null()
    {
        var parent = new Geometry("g");
        parent.Documentation = null;
        Assert.IsNull(parent.Documentation);
    }

    [TestMethod]
    public void Null_Setter()
    {
        var parent = new Geometry("g");
        parent.SetDocumentation(null);
        Assert.IsNull(parent.Documentation);
    }

    [TestMethod]
    public void Null_Reflective()
    {
        var parent = new Geometry("g");
        parent.Set(ShapesLanguage.Instance.Geometry_documentation, null);
        Assert.IsNull(parent.Documentation);
    }

    [TestMethod]
    public void Null_Constructor()
    {
        var parent = new Geometry("g") { Documentation = null };
        Assert.IsNull(parent.Documentation);
    }

    [TestMethod]
    public void Null_TryGet()
    {
        var parent = new Geometry("g");
        Assert.IsFalse(parent.TryGetDocumentation(out var o));
        Assert.IsNull(o);
    }

    #endregion

    #region EmptyCollection

    [TestMethod]
    public void EmptyArray_Reflective()
    {
        var parent = new Geometry("g");
        var values = new Documentation[0];
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void EmptyUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var values = new ArrayList();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void EmptyListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Documentation>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void EmptySet_Reflective()
    {
        var parent = new Geometry("g");
        var values = new HashSet<Documentation>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void EmptyListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Coord>();
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    #endregion

    #region NullCollection

    [TestMethod]
    public void NullArray_Reflective()
    {
        var parent = new Geometry("g");
        var values = new Documentation[] { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void NullUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var values = new ArrayList() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void NullListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Documentation>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void NullListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var values = new List<Coord>() { null };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void NullSet_Reflective()
    {
        var parent = new Geometry("g");
        var values = new HashSet<Documentation>() { null };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    #endregion

    #region SingleCollection

    [TestMethod]
    public void SingleArray_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Documentation("s");
        var values = new Documentation[] { value };

        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedArray_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Documentation("s");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Documentation("s");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Documentation("s");
        var values = new List<Documentation>() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleSet_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Documentation("s");
        var values = new HashSet<Documentation>() { value };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(value.GetParent());
    }

    [TestMethod]
    public void SingleListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Coord("c");
        var values = new List<Coord>() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void SingleUntypedListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Coord("c");
        var values = new ArrayList() { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    [TestMethod]
    public void SingleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var value = new Coord("c");
        var values = new object[] { value };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
    }

    #endregion

    #region MultipleCollection

    [TestMethod]
    public void MultipleArray_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Documentation("sA");
        var valueB = new Documentation("sB");
        var values = new Documentation[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArray_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Documentation("sA");
        var valueB = new Documentation("sB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedList_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Documentation("sA");
        var valueB = new Documentation("sB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Documentation("sA");
        var valueB = new Documentation("sB");
        var values = new List<Documentation>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleSet_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Documentation("sA");
        var valueB = new Documentation("sB");
        var values = new HashSet<Documentation>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(() =>
            parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new List<Coord>() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedListNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new ArrayList() { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    [TestMethod]
    public void MultipleUntypedArrayNonMatchingType_Reflective()
    {
        var parent = new Geometry("g");
        var valueA = new Coord("cA");
        var valueB = new Coord("cB");
        var values = new object[] { valueA, valueB };
        Assert.ThrowsException<InvalidValueException>(
            () => parent.Set(ShapesLanguage.Instance.Geometry_documentation, values));
        Assert.IsNull(parent.Documentation);
        Assert.IsNull(valueA.GetParent());
        Assert.IsNull(valueB.GetParent());
    }

    #endregion
}