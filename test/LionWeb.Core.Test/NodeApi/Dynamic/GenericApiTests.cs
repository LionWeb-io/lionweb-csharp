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
using M3;

[TestClass]
public class GenericApiTests: DynamicNodeTestsBase
{
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
        
        Assert.AreEqual(2, lang.DependsOn.Count);
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


}