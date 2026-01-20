// Copyright 2026 TRUMPF Laser SE and other contributors
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

using Build.Languages;
using Core;
using Core.M3;
using Names;

[TestClass]
public class CustomPrimitiveTypeTests
{
    [TestMethod]
    public void MissingMapping()
    {
        var lang = new DynamicLanguage("lang", LionWebVersions.Current);
        lang.PrimitiveType("id-primitive", "key-primitive", "name-primitive");

        var generator = new GeneratorFacade { Names = new Names(lang, "ns") };

        var exception = Assert.ThrowsExactly<ArgumentException>(() => generator.Generate());
        Assert.Contains("Missing", exception.Message);
        Assert.Contains("key-primitive", exception.Message);
    }

    [TestMethod]
    public void InvalidRefStructMapping()
    {
        var lang = new DynamicLanguage("lang", LionWebVersions.Current);
        var primitive = lang.PrimitiveType("id-primitive", "key-primitive", "name-primitive");

        var generator = new GeneratorFacade
        {
            Names = new Names(lang, "ns") { PrimitiveTypeMappings = { { primitive, typeof(CustomRefStruct) } } }
        };

        var exception = Assert.ThrowsExactly<ArgumentException>(() => generator.Generate());
        Assert.Contains("Cannot use ref types", exception.Message);
        Assert.Contains(nameof(CustomRefStruct), exception.Message);
    }

    [TestMethod]
    public void FindsAllDatatypes()
    {
        var propertyType = new DynamicPrimitiveType("id-propertyType", LionWebVersions.Current, null)
        {
            Key = "key-propertyType"
        };
        var fieldType = new DynamicPrimitiveType("fieldType", LionWebVersions.Current, null) { Key = "key-fieldType" };

        var lang = new DynamicLanguage("lang", LionWebVersions.Current);
        lang.PrimitiveType("id-primitive", "key-primitive", "name-primitive");
        lang.Concept("concept", "concept", "concept").Property("prop", "prop", "prop").OfType(propertyType);
        lang.StructuredDataType("sdt", "sdt", "sdt").Field("field", "field", "field").OfType(fieldType);

        var generator = new GeneratorFacade { Names = new Names(lang, "ns") };

        var exception = Assert.ThrowsExactly<ArgumentException>(() => generator.Generate());
        Assert.Contains("Missing", exception.Message);
        Assert.Contains("key-primitive", exception.Message);
        Assert.Contains("key-propertyType", exception.Message);
        Assert.Contains("key-fieldType", exception.Message);
    }
}