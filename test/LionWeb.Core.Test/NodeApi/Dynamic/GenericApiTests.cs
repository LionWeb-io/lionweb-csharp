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
public class GenericApiTests: DynamicNodeTestsBase
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
        
        List<LanguageEntity> entities = [
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
        
        List<LanguageEntity> entities = [
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

    #region containments add/insert/remove generic api
    
    #region add

    [TestMethod]
    public void Single_Add_generic_api()
    {
        var parent = new Geometry("g");
        var line = new Line("line");
        
        parent.Add(ShapesLanguage.Instance.Geometry_shapes, [line]);
        
        Assert.AreSame(parent, line.GetParent());
        Assert.IsTrue(parent.Shapes.Contains(line));
    }
    
    [TestMethod]
    public void Multiple_Add_generic_api()
    {
        var parent = new Geometry("g");
        var line1 = new Line("line1");
        var line2 = new Line("line2");
        
        parent.Add(ShapesLanguage.Instance.Geometry_shapes, [line1, line2]);
        
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

        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line1, line2]);

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
        
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line]);
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line]);
        
        Assert.IsNull(line.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line));
        
    }
    
    [TestMethod]
    public void Multiple_Remove_generic_api()
    {
        var parent = new Geometry("g");
        var line1 = new Line("line1");
        var line2 = new Line("line2");
        
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line1]);
        parent.Insert(ShapesLanguage.Instance.Geometry_shapes, 0, [line2]);
        parent.Remove(ShapesLanguage.Instance.Geometry_shapes, [line1, line2]);
        
        Assert.IsEmpty(parent.Shapes);
        Assert.IsNull(line1.GetParent());
        Assert.IsNull(line2.GetParent());
        Assert.IsFalse(parent.Shapes.Contains(line1));
        Assert.IsFalse(parent.Shapes.Contains(line2));
    }
    
    #endregion

    #endregion
    
    #endregion
    
}