// Copyright 2024 TRUMPF Laser SE and other contributors
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Io.Lionweb.Mps.Specific;
using LionWeb.Core.M2;
using LionWeb.Core.M3;

public class TestLanguagesDefinitions
{
    private static readonly LionWebVersions _lionWebVersion = LionWebVersionsExtensions.GetCurrent();

    public readonly Language ALang;
    public readonly Language BLang;
    public readonly Language TinyRefLang;

    public TestLanguagesDefinitions()
    {
        var aLang = new DynamicLanguage("id-ALang", _lionWebVersion)
        {
            Key = "key-ALang", Name = "ALang", Version = "1"
        };
        var aConcept = aLang.Concept("id-AConcept", "key-AConcept", "AConcept");
        aConcept.AddAnnotations([
            new ConceptDescription("aaa-desc")
            {
                ConceptAlias = "AConcept Alias",
                ConceptShortDescription = """
                                          This is my
                                            Des
                                          crip
                                             tion
                                          """
            }
        ]);
        var aEnum = aLang.Enumeration("id-aEnum", "key-AEnum", "AEnum");

        var bLang = new DynamicLanguage("id-BLang", _lionWebVersion)
        {
            Key = "key-BLang", Name = "BLang", Version = "2"
        };
        var bConcept = bLang.Concept("id-BConcept", "key-BConcept", "BConcept");
        bConcept.AddAnnotations([new ConceptDescription("xxx") { ConceptShortDescription = "Some enum" }]);

        aLang.AddDependsOn([bLang]);
        bLang.AddDependsOn([aLang]);

        aEnum.EnumerationLiteral("id-left", "key-left", "left");
        aEnum.EnumerationLiteral("id-right", "key-right", "right");

        aConcept.Reference("id-AConcept-BRef", "key-BRef", "BRef").IsOptional().OfType(bConcept);
        bConcept.Reference("id-BConcept-ARef", "key-ARef", "ARef").IsOptional().OfType(aConcept);
        bConcept.Property("id-BConcept-AEnumProp", "key-AEnumProp", "AEnumProp").OfType(aEnum);

        ALang = aLang;
        BLang = bLang;


        var tinyRefLang =
            new DynamicLanguage("id-TinyRefLang", _lionWebVersion)
            {
                Key = "key-tinyRefLang", Name = "TinyRefLang", Version = "0"
            };
        var myConcept = new DynamicConcept("id-Concept", tinyRefLang) { Key = "key-MyConcept", Name = "MyConcept" };
        tinyRefLang.AddEntities([myConcept]);

        var singularRef =
            new DynamicReference("id-MyConcept-singularRef", myConcept)
            {
                Key = "key-MyConcept-singularRef", Name = "singularRef", Type = myConcept
            };
        var multivalueRef = new DynamicReference("id-Concept-multivaluedRef", myConcept)
        {
            Key = "key-MyConcept-multivaluedRef", Name = "multivaluedRef", Type = myConcept, Multiple = true
        };
        myConcept.AddFeatures([singularRef, multivalueRef]);

        TinyRefLang = tinyRefLang;
    }
}