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

namespace LionWeb.Generator.Test;

using Core;
using Core.M3;
using Core.Test.Languages.Generated.V2023_1.Circular.A;
using Core.Test.Languages.Generated.V2023_1.Circular.B;
using GeneratorExtensions;
using Names;

[TestClass]
public class GeneratorExtensionsTests
{
    [TestMethod]
    public void SplitClassesInFiles()
    {
        var lionWebVersion = LionWebVersions.v2023_1;

        List<Language> languages = [ALangLanguage.Instance, BLangLanguage.Instance];

        var namespaceMappings = languages
            .ToDictionary(l => l, l => $"SingleFileTest.{l.Name}");

        var path = Directory.CreateTempSubdirectory().FullName;
        try
        {
            foreach (var language in languages)
            {
                var languageNamespace = namespaceMappings[language];
                var basePath = languageNamespace;

                var generator = new GeneratorFacade
                {
                    Names = new Names(language, basePath)
                    {
                        NamespaceMappings = namespaceMappings
                    },
                    LionWebVersion = lionWebVersion
                };

                generator.PersistFilePerType(path);
            }

            var allFiles = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Order()
                .ToList();

            var expected = new[]{
                    Path.Combine("ALang", "AConcept.g.cs"),
                    Path.Combine("ALang", "AEnum.g.cs"),
                    Path.Combine("ALang", "ALangFactory.g.cs"),
                    Path.Combine("ALang", "ALangLanguage.g.cs"),
                    Path.Combine("ALang", "IALangFactory.g.cs"),
                    Path.Combine("BLang", "BConcept.g.cs"),
                    Path.Combine("BLang", "BLangFactory.g.cs"),
                    Path.Combine("BLang", "BLangLanguage.g.cs"),
                    Path.Combine("BLang", "IBLangFactory.g.cs")
                }
                .Select(s => Path.Combine(path, "SingleFileTest", s))
                .ToList();
                
            CollectionAssert.AreEqual(expected, allFiles);
        } finally
        {
            if (Path.Exists(path))
                Directory.Delete(path, true);
        }
    }
}