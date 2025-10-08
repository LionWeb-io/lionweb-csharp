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
public class GenericAddInsertRemoveApiTests : DynamicNodeTestsBase
{
    #region dynamic language add/insert/remove generic api

    #region entities

    [TestMethod]
    public void AddEntities()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        var lang = new DynamicLanguage("myDynLang", lionWebVersion);

        List<LanguageEntity> entities = [new DynamicConcept("myDynConcept1", lionWebVersion, lang)];

        lang.Add(lionWebVersion.LionCore.Language_entities, entities);

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

        lang.Insert(lionWebVersion.LionCore.Language_entities, 0, entities);

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

        lang.Insert(lionWebVersion.LionCore.Language_entities, 0, entities);
        lang.Remove(lionWebVersion.LionCore.Language_entities, entities);

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

        lang.Add(lionWebVersion.LionCore.Language_dependsOn, languages);

        Assert.AreEqual(2, lang.DependsOn.Count);
    }

    [TestMethod]
    public void InsertDependsOn()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        var lang = new DynamicLanguage("myDynLang", lionWebVersion);
        List<Language> languages = [ShapesLanguage.Instance, MultiLanguage.Instance];

        lang.Insert(lionWebVersion.LionCore.Language_dependsOn, 0, languages);
        lang.Insert(lionWebVersion.LionCore.Language_dependsOn, 2, [ALangLanguage.Instance]);

        Assert.AreEqual(3, lang.DependsOn.Count);
    }


    [TestMethod]
    public void RemoveDependsOn()
    {
        var lionWebVersion = LionWebVersions.v2024_1;
        var lang = new DynamicLanguage("myDynLang", lionWebVersion);
        List<Language> languages = [ShapesLanguage.Instance, MultiLanguage.Instance];

        lang.Insert(lionWebVersion.LionCore.Language_dependsOn, 0, languages);
        lang.Remove(lionWebVersion.LionCore.Language_dependsOn, languages);

        Assert.AreEqual(0, lang.DependsOn.Count);
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
        parent.Add(null, [bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }
    
    #region Insert

    [TestMethod]
    public void Single_Insert_Empty()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 0, [bom]);
        Assert.AreSame(parent, bom.GetParent());
        Assert.IsTrue(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_Empty_UnderBounds()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(null, -1, [bom]));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_Empty_OverBounds()
    {
        var parent = newLine("g");
        var bom = newBillOfMaterials("myId");
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => parent.Insert(null,1, [bom]));
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Insert_One_Before()
    {
        var doc = newDocumentation("cId");
        var parent = newLine("g");
        parent.Add(null, [doc]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 0, [bom]);
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
        parent.Add(null, [doc]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 1, [bom]);
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
        parent.Add(null, [docA, docB]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 0, [bom]);
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
        parent.Add(null, [docA, docB]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 1, [bom]);
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
        parent.Add(null, [docA, docB]);
        var bom = newBillOfMaterials("myId");
        parent.Insert(null, 2, [bom]);
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
        parent.Remove(null, [bom]);
        Assert.IsNull(bom.GetParent());
        Assert.IsFalse(parent.GetAnnotations().Contains(bom));
    }

    [TestMethod]
    public void Single_Remove_NotContained()
    {
        var doc = newDocumentation("myC");
        var parent = newLine("cs");
        parent.Add(null, [doc]);
        var bom = newBillOfMaterials("myId");
        parent.Remove(null, [bom]);
        Assert.AreSame(parent, doc.GetParent());
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { doc }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_Only()
    {
        var bom = newBillOfMaterials("myId");
        var parent = newLine("g");
        parent.Add(null, [bom]);
        parent.Remove(null, [bom]);
        Assert.IsNull(bom.GetParent());
        CollectionAssert.AreEqual(new List<INode> { }, parent.GetAnnotations().ToList());
    }

    [TestMethod]
    public void Single_Remove_First()
    {
        var doc = newDocumentation("cId");
        var bom = newBillOfMaterials("myId");
        var parent = newLine("g");
        parent.Add(null, [bom, doc]);
        parent.Remove(null, [bom]);
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
        parent.Add(null, [doc, bom]);
        parent.Remove(null, [bom]);
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
        parent.Add(null, [docA, bom, docB]);
        parent.Remove(null, [bom]);
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