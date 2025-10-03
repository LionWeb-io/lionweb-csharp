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

namespace LionWeb.Core.Test.NodeApi.Dynamic;

using Languages.Generated.V2024_1.Multi.M2;
using Languages.Generated.V2024_1.Shapes.M2;
using Languages.Generated.V2025_1.Circular.A;
using M3;

[TestClass]
public class GenericApiTests : DynamicNodeTestsBase
{
    #region dynamic language add/insert/remove generic api

    #region entities

    [TestMethod]
    public void AddEntities()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        var lang = new DynamicLanguage("myDynLang", lionWebVersion);

        List<LanguageEntity> entities = [new DynamicConcept("myDynConcept1", lionWebVersion, lang)];

        lang.Add(entities, lionWebVersion.LionCore.Language_entities);

        Assert.AreEqual(1, lang.Entities.Count);
    }


    [TestMethod]
    public void InsertEntities()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        var lang = new DynamicLanguage("myDynLang", lionWebVersion);

        List<LanguageEntity> entities =
        [
            new DynamicConcept("myDynConcept1", lionWebVersion, lang),
            new DynamicConcept("myDynConcept2", lionWebVersion, lang),
        ];

        lang.Insert(entities, 0, lionWebVersion.LionCore.Language_entities);

        Assert.AreEqual(2, lang.Entities.Count);
    }

    [TestMethod]
    public void RemoveEntities()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        var lang = new DynamicLanguage("myDynLang", lionWebVersion);

        List<LanguageEntity> entities =
        [
            new DynamicConcept("myDynConcept1", lionWebVersion, lang),
            new DynamicConcept("myDynConcept2", lionWebVersion, lang),
        ];

        lang.Insert(entities, 0, lionWebVersion.LionCore.Language_entities);
        lang.Remove(entities, lionWebVersion.LionCore.Language_entities);

        Assert.AreEqual(0, lang.Entities.Count);
    }

    #endregion

    #region dependsOn

    [TestMethod]
    public void AddDependsOn()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        var lang = new DynamicLanguage("myDynLang", lionWebVersion);
        List<Language> languages = [ShapesLanguage.Instance, MultiLanguage.Instance];

        lang.Add(languages, lionWebVersion.LionCore.Language_dependsOn);

        Assert.AreEqual(2, lang.DependsOn.Count);
    }

    [TestMethod]
    public void InsertDependsOn()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        var lang = new DynamicLanguage("myDynLang", lionWebVersion);
        List<Language> languages = [ShapesLanguage.Instance, MultiLanguage.Instance];

        lang.Insert(languages, 0, lionWebVersion.LionCore.Language_dependsOn);
        lang.Insert([ALangLanguage.Instance], 2, lionWebVersion.LionCore.Language_dependsOn);

        Assert.AreEqual(3, lang.DependsOn.Count);
    }


    [TestMethod]
    public void RemoveDependsOn()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        var lang = new DynamicLanguage("myDynLang", lionWebVersion);
        List<Language> languages = [ShapesLanguage.Instance, MultiLanguage.Instance];

        lang.Insert(languages, 0, lionWebVersion.LionCore.Language_dependsOn);
        lang.Remove(languages, lionWebVersion.LionCore.Language_dependsOn);

        Assert.AreEqual(0, lang.DependsOn.Count);
    }

    #endregion

    #endregion
    
    #region containments add/insert/remove generic api

    #region add

    [TestMethod]
    public void Single_Add_generic_api()
    {
        var parent = new Geometry("g");
        var line = new Line("line");

        parent.Add([line], ShapesLanguage.Instance.Geometry_shapes);

        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Multiple_Add_generic_api()
    {
        var parent = new Geometry("g");
        var line1 = new Line("line1");
        var line2 = new Line("line2");

        parent.Add([line1, line2], ShapesLanguage.Instance.Geometry_shapes);

        Assert.AreEqual(2, parent.Shapes.Count);
        Assert.AreSame(parent, line1.GetParent());
        Assert.AreSame(parent, line2.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line1));
        Assert.IsTrue(parent.Shapes.Contains(line2));
    }

    #endregion

    #region insert

    [TestMethod]
    public void Multiple_Insert_generic_api()
    {
        var parent = new Geometry("g");
        var line1 = new Line("line1");
        var line2 = new Line("line2");

        parent.Insert([line1, line2], 0, ShapesLanguage.Instance.Geometry_shapes);

        Assert.AreEqual(2, parent.Shapes.Count);
        Assert.AreSame(parent, line1.GetParent());
        Assert.AreSame(parent, line2.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line1));
        Assert.IsTrue(parent.Shapes.Contains(line2));
    }

    #endregion

    #region remove

    [TestMethod]
    public void Single_Remove_generic_api()
    {
        var parent = new Geometry("g");
        var line = new Line("myId");

        parent.Insert([line], 0, ShapesLanguage.Instance.Geometry_shapes);
        parent.Remove([line], ShapesLanguage.Instance.Geometry_shapes);

        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
    }

    [TestMethod]
    public void Multiple_Remove_generic_api()
    {
        var parent = new Geometry("g");
        var line1 = new Line("line1");
        var line2 = new Line("line2");

        parent.Insert([line1], 0, ShapesLanguage.Instance.Geometry_shapes);
        parent.Insert([line2], 0, ShapesLanguage.Instance.Geometry_shapes);
        parent.Remove([line1, line2], ShapesLanguage.Instance.Geometry_shapes);

        Assert.IsEmpty(parent.Shapes);
        Assert.IsNull(line1.GetParent());
        Assert.IsNull(line2.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line1));
        Assert.IsFalse(parent.Shapes.Contains(line2));
    }

    #endregion

    #endregion

    #region annotations add/insert/remove generic api

    #region Single

    [TestMethod]
    public void Single_Add()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        parent.Add([bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }
    
    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        parent.Insert([bom], 0);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_Empty_UnderBounds()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert([bom], -1));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_Empty_OverBounds()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert([bom], 1));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var doc = newDocumentation("cId");
        var parent = newLine("g");
        parent.Add([doc]);
        var bom = newBillOfMaterials("myId");
        parent.Insert([bom], 0);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { bom, doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_One_After()
    {
        var doc = newDocumentation("cId");
        var parent = newLine("g");
        parent.Add([doc]);
        var bom = newBillOfMaterials("myId");
        parent.Insert([bom], 1);
        Assert.AreSame(parent, doc.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { doc, bom }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Before()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var parent = newLine("g");
        parent.Add([docA, docB]);
        var bom = newBillOfMaterials("myId");
        parent.Insert([bom], 0);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { bom, docA, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_Between()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var parent = newLine("g");
        parent.Add([docA, docB]);
        var bom = newBillOfMaterials("myId");
        parent.Insert([bom], 1);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { docA, bom, docB }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Insert_Two_After()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var parent = newLine("g");
        parent.Add([docA, docB]);
        var bom = newBillOfMaterials("myId");
        parent.Insert([bom], 2);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
        CollectionAssert.AreEqual(new List<INode> { docA, docB, bom }, parent.GetAnnotations().ToList());
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Single_Remove_Empty()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        parent.Remove([bom]);
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var doc = newDocumentation("myC");
        var parent = newLine("cs");
        parent.Add([doc]);
        var bom = newBillOfMaterials("myId");
        parent.Remove([bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var bom = newBillOfMaterials("myId");
        var parent = newLine("g");
        parent.Add([bom]);
        parent.Remove([bom]);
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var doc = newDocumentation("cId");
        var bom = newBillOfMaterials("myId");
        var parent = newLine("g");
        parent.Add([bom, doc]);
        parent.Remove([bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Last()
    {
        var doc = newDocumentation("cId");
        var bom = newBillOfMaterials("myId");
        var parent = newLine("g");
        parent.Add([doc, bom]);
        parent.Remove([bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Between()
    {
        var docA = newDocumentation("cIdA");
        var docB = newDocumentation("cIdB");
        var bom = newBillOfMaterials("myId");
        var parent = newLine("g");
        parent.Add([docA, bom, docB]);
        parent.Remove([bom]);
        Assert.AreSame(parent, docA.GetParent());
        Assert.AreSame(parent, docB.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { docA, docB }, parent.GetAnnotations().ToList());
    }

    #endregion

    #endregion

    #region Null

    // TODO: copy the rest of the tests from ContainmentTests_Annotation and replace api calls with generic calls 

    #endregion

    #endregion
}