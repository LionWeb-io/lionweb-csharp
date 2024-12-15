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
using LionWeb.Core;
using LionWeb.Core.M3;

// ReSharper disable SuggestVarOrType_SimpleTypes

public class TestLanguagesDefinitions
{
    private static readonly LionWebVersions _lionWebVersion = LionWebVersions.Current;
    
    public Language ALang { get; private set; }
    public Language BLang {get; private set;}
    public Language TinyRefLang {get; private set;}
    public Language SdtLang {get; private set;}
    
    public List<Language> MixedLangs { get; private set; }

    public TestLanguagesDefinitions()
    {
        CreateALangBLang();
        CreateTinyRefLang();
        CreateSdtLang();
        CreateMixedLangs();
    }

    private void CreateALangBLang()
    {
        var aLang = new DynamicLanguage("id-ALang", _lionWebVersion) { Key = "key-ALang", Name = "ALang", Version = "1" };
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
    }

    private void CreateTinyRefLang()
    {
        var tinyRefLang = new DynamicLanguage("id-TinyRefLang", _lionWebVersion) { Key = "key-tinyRefLang", Name = "TinyRefLang", Version = "0" };
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

    private void CreateSdtLang()
    {
        var sdtLang = new DynamicLanguage("id-SDTLang", _lionWebVersion) { Key = "key-SDTLang", Name = "SDTLang", Version = "0"};
        var sdtConcept = sdtLang.Concept("id-SDTConcept", "key-SDTConcept", "SDTConcept");
        var currency = sdtLang.Enumeration("id-SDTCurrency", "key-SDTCurrency", "Currency");
        var amount = sdtLang.StructuredDataType("id-SDTAmount", "key-SDTAmount", "Amount");
        var dec = sdtLang.StructuredDataType("id-SDTDecimal", "key-SDTDecimal", "Decimal");
        var complex = sdtLang.StructuredDataType("id-SDTComplexNumber", "key-SDTComplexNumber", "ComplexNumber");
        var fqn = sdtLang.StructuredDataType("id-SDTFullyQualifiedName", "key-SDTFullyQualifiedName", "FullyQualifiedName");
        
        sdtConcept.Property("id-SDTamountField", "key-SDTamountField", "amount").OfType(amount);
        sdtConcept.Property("id-SDTDecimalField", "key-SDTDecimalField", "decimal").OfType(dec);
        sdtConcept.Property("id-SDTComplexField", "key-SDTComplexField", "complex").OfType(complex);
        sdtConcept.Property("id-SDTFqnField", "key-SDTFqnField", "fqn").OfType(fqn);
        
        currency.EnumerationLiteral("id-SDT-eur", "key-SDTEur", "EUR");
        currency.EnumerationLiteral("id-SDT-gbp", "key-SDTGbp", "GBP");

        amount.Field("id-SDTValue", "key-SDTValue", "value").OfType(dec);
        amount.Field("id-SDTCurrency", "key-SDTCurrency", "currency").OfType(currency);
        amount.Field("id-SDTDigital", "key-SDTDigital", "digital").OfType(_lionWebVersion.BuiltIns.Boolean);
        
        dec.Field("id-SDTInt", "key-SDTInt", "int").OfType(_lionWebVersion.BuiltIns.Integer);
        dec.Field("id-SDTFrac", "key-SDTFrac", "frac").OfType(_lionWebVersion.BuiltIns.Integer);

        complex.Field("id-SDTReal", "key-SDTReal", "real").OfType(dec);
        complex.Field("id-SDTImaginary", "key-SDTImaginary", "imaginary").OfType(dec);

        fqn.Field("id-SDT-FQN-name", "key-SDTFqnName", "name").OfType(_lionWebVersion.BuiltIns.String);
        fqn.Field("id-SDT-FQN-nested", "key-SDTFqnNested", "nested").OfType(fqn);
        
        // Shape:
        //     A
        //    / \
        //   B   C
        //    \ / \
        //     D   E -- B
        //          \
        //           F -- C

        var a = sdtLang.StructuredDataType("id-SDTA", "key-SDTA", "A");
        var b = sdtLang.StructuredDataType("id-SDTB", "key-SDTB", "B");
        var c = sdtLang.StructuredDataType("id-SDCT", "key-SDCT", "C");
        var d = sdtLang.StructuredDataType("id-SDD", "key-SDD", "D");
        var e = sdtLang.StructuredDataType("id-SDE", "key-SDE", "E");
        var f = sdtLang.StructuredDataType("id-SDF", "key-SDF", "F");
        
        sdtConcept.Property("id-SDTAField", "key-SDTAField", "A").OfType(a);
        
        a.Field("id-SDTaName", "key-SDTaName", "name").OfType(_lionWebVersion.BuiltIns.String);
        b.Field("id-SDTbName", "key-SDTbName", "name").OfType(_lionWebVersion.BuiltIns.String);
        c.Field("id-SDTcName", "key-SDTcName", "name").OfType(_lionWebVersion.BuiltIns.String);
        d.Field("id-SDTdName", "key-SDTdName", "name").OfType(_lionWebVersion.BuiltIns.String);
        e.Field("id-SDTeName", "key-SDTeName", "name").OfType(_lionWebVersion.BuiltIns.String);
        f.Field("id-SDTfName", "key-SDTfName", "name").OfType(_lionWebVersion.BuiltIns.String);
        
        a.Field("id-SDTa2b", "key-SDTa2b", "a2b").OfType(b);
        a.Field("id-SDTa2c", "key-SDTa2c", "a2c").OfType(c);
        b.Field("id-SDTb2d", "key-SDTb2d", "b2d").OfType(d);
        c.Field("id-SDTc2d", "key-SDTc2d", "c2d").OfType(d);
        c.Field("id-SDTc2e", "key-SDTc2e", "c2e").OfType(e);
        e.Field("id-SDTe2f", "key-SDTf", "e2f").OfType(f);
        e.Field("id-SDTe2b", "key-SDTb", "e2b").OfType(b);
        f.Field("id-SDTf2c", "key-SDTf2c", "f2c").OfType(c);
        
        SdtLang = sdtLang;
    }

    private void CreateMixedLangs()
    {
        var mixedBasePropertyLang = new DynamicLanguage("id-mixedBasePropertyLang", _lionWebVersion) { Key = "key-mixedBasePropertyLang", Name = "MixedBasePropertyLang", Version = "1" };
        var mixedBaseContainmentLang = new DynamicLanguage("id-mixedBaseContainmentLang", _lionWebVersion) { Key = "key-mixedBaseContainmentLang", Name = "MixedBaseContainmentLang", Version = "1" };
        var mixedBaseReferenceLang = new DynamicLanguage("id-mixedBaseReferenceLang", _lionWebVersion) { Key = "key-mixedBaseReferenceLang", Name = "MixedBaseReferenceLang", Version = "1" };
        var mixedBaseConceptLang = new DynamicLanguage("id-mixedBaseConceptLang", _lionWebVersion) { Key = "key-mixedBaseConceptLang", Name = "MixedBaseConceptLang", Version = "1" };
        var mixedConceptLang = new DynamicLanguage("id-mixedConceptLang", _lionWebVersion) { Key = "key-mixedConceptLang", Name = "MixedConceptLang", Version = "1" };
        var mixedDirectEnumLang = new DynamicLanguage("id-mixedDirectEnumLang", _lionWebVersion) { Key = "key-mixedDirectEnumLang", Name = "MixedDirectEnumLang", Version = "1" };
        var mixedNestedEnumLang = new DynamicLanguage("id-mixedNestedEnumLang", _lionWebVersion) { Key = "key-mixedNestedEnumLang", Name = "MixedNestedEnumLang", Version = "1" };
        var mixedDirectSdtLang = new DynamicLanguage("id-mixedDirectSdtLang", _lionWebVersion) { Key = "key-mixedDirectSdtLang", Name = "MixedDirectSdtLang", Version = "1" };
        var mixedNestedSdtLang = new DynamicLanguage("id-mixedNestedSdtLang", _lionWebVersion) { Key = "key-mixedNestedSdtLang", Name = "MixedNestedSdtLang", Version = "1" };
        
        var nestedSdt = mixedNestedSdtLang.StructuredDataType("id-nestedSdt", "key-nestedSdt", "NestedSdt");
        nestedSdt.Field("id-nestedSdtField", "key-nestedSdtField", "nestedSdtField").OfType(_lionWebVersion.BuiltIns.String);
        
        var nestedEnum = mixedNestedEnumLang.Enumeration("id-nestedEnum", "key-nestedEnum", "NestedEnum");
        nestedEnum.EnumerationLiteral("id-nestedLiteralA", "key-nestedLiteralA", "nestedLiteralA");
        
        var directSdt = mixedDirectSdtLang.StructuredDataType("id-directSdt", "key-directSdt", "DirectSdt");
        directSdt.Field("id-directSdtEnum", "key-directSdtEnum", "directSdtEnum").OfType(nestedEnum);
        directSdt.Field("id-directSdtSdt", "key-directSdtSdt", "directSdtSdt").OfType(nestedSdt);
        
        mixedDirectSdtLang.AddDependsOn([mixedNestedEnumLang, mixedNestedSdtLang]);
        
        var directEnum = mixedDirectEnumLang.Enumeration("id-directEnum", "key-directEnum", "DirectEnum");
        directEnum.EnumerationLiteral("id-directEnumA", "key-directEnumA", "directEnumA");
        
        var basePropertyIface = mixedBasePropertyLang.Interface("id-basePropertyIface", "key-basePropertyIface","BasePropertyIface");
        basePropertyIface.Property("id-basePropertyIface-prop", "key-basePropertyIface-prop", "Prop").OfType(_lionWebVersion.BuiltIns.String);
        
        var baseContainmentIface = mixedBaseContainmentLang.Interface("id-baseContainmentIface", "key-baseContainmentIface", "BaseContainmentIface");
        baseContainmentIface.Containment("id-baseContainemntIface-cont", "key-baseContainmentIface-cont", "Cont").OfType(_lionWebVersion.BuiltIns.Node);
        
        var baseReferenceIface = mixedBaseReferenceLang.Interface("id-baseReferenceIface", "key-baseReferenceIface", "BaseReferenceIface");
        baseReferenceIface.Reference("id-baseReferenceIface-ref", "key-baseReferenceIface-ref", "Ref").OfType(_lionWebVersion.BuiltIns.Node);
        
        var baseConcept = mixedBaseConceptLang.Concept("id-baseConcept", "key-baseConcept", "BaseConcept").IsAbstract().Implementing(basePropertyIface, baseContainmentIface, baseReferenceIface);
        baseConcept.Property("id-enumProp", "key-enumProp", "enumProp").OfType(directEnum);
        baseConcept.Property("id-sdtProp", "key-sdtProp", "sdtProp").OfType(directSdt);
        
        mixedBaseConceptLang.AddDependsOn([mixedBasePropertyLang, mixedBaseContainmentLang, mixedBaseReferenceLang, mixedDirectEnumLang, mixedDirectSdtLang]);
        
        mixedConceptLang.Concept("id-mixedConcept", "key-mixedConcept", "MixedConcept").Extending(baseConcept);
        mixedConceptLang.AddDependsOn([mixedBaseConceptLang]);
        
        MixedLangs = [mixedBasePropertyLang, mixedBaseContainmentLang, mixedBaseReferenceLang, mixedBaseConceptLang, mixedConceptLang, mixedDirectEnumLang, mixedNestedEnumLang, mixedDirectSdtLang, mixedNestedSdtLang];
    }
}