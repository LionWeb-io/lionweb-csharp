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
using LionWeb.Build.Languages;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Generator.Impl;
using System.Reflection;

// ReSharper disable SuggestVarOrType_SimpleTypes

public class TestLanguagesDefinitions
{
    private readonly LionWebVersions _lionWebVersion;

    public Language ALang { get; private set; }
    public Language BLang { get; private set; }
    public Language TinyRefLang { get; private set; }
    public Language? SdtLang { get; private set; }
    public Language? KeywordLang { get; private set; }
    public Language MultiInheritLang { get; private set; }
    public Language NamedLang { get; private set; }
    public Language GeneralNodeLang { get; private set; }
    public Language? LowerCaseLang { get; private set; }
    public Language? UpperCaseLang { get; private set; }
    public Language NamespaceStartsWithLionWebLang { get; private set; }
    public Language NamespaceContainsLionWebLang { get; private set; }
    public Language LanguageWithLionWebNamedConcepts { get; private set; }
    public Language FeatureSameNameAsContainingConcept { get; private set; }
    public Language? CustomPrimitiveTypeLang { get; private set; }
    public Dictionary<PrimitiveType, Type> CustomPrimitiveTypeMapping { get; } = [];

    public List<Language> MixedLangs { get; private set; } = [];

    public TestLanguagesDefinitions(LionWebVersions lionWebVersion)
    {
        _lionWebVersion = lionWebVersion;

        CreateALangBLang();
        CreateTinyRefLang();
        CreateMultiInheritLang();
        CreateNamedLang();
        CreateGeneralNodeLang();
        CreateNamespaceStartsWithLionWebLang();
        CreateNamespaceContainsLionWebLang();
        CreateLanguageWithLionWebNamedConcepts();
        CreateFeatureSameNameAsContainingConcept();

        if (lionWebVersion.LionCore is ILionCoreLanguageWithStructuredDataType)
        {
            CreateSdtLang();
            CreateMixedLangs();
            CreateKeywordLang();
            CreateLowerCaseLang();
            CreateUpperCaseLang();
            CreateCustomPrimitiveTypeLang();
        }
    }

    private void CreateALangBLang()
    {
        var aLang = new DynamicLanguage("id-ALang", _lionWebVersion)
        {
            Key = "key-ALang", Name = "ALang", Version = "1"
        };
        var aDatatype = aLang.PrimitiveType("id-AType", "key-AType", "CustomPrimitiveType");
        var aConcept = aLang.Concept("id-AConcept", "key-AConcept", "AConcept");
        aConcept.AddAnnotations([
            CreateConceptDescription("aaa-desc", "AConcept Alias", """
                                                                   This is my
                                                                     Des
                                                                   crip
                                                                      tion
                                                                   """, "https://example.com/abc?u=i#x"),
            CreateKeyedDescription("aaa-keyedDesc", "keyed description", [aLang])
        ]);
        var aEnum = aLang.Enumeration("id-aEnum", "key-AEnum", "AEnum");
        aEnum.AddAnnotations([
            CreateKeyedDescription("aaa-enumDesc", "enum description", [aLang, aConcept])
        ]);

        var bLang = new DynamicLanguage("id-BLang", _lionWebVersion)
        {
            Key = "key-BLang", Name = "BLang", Version = "2"
        };
        bLang.AddAnnotations([
            CreateKeyedDescription("aaa-bLangDesc", "bLang desc", [aLang])
        ]);
        var bDatatype = bLang.PrimitiveType("id-BType", "key-BType", "CustomPrimitiveType");
        var bConcept = bLang.Concept("id-BConcept", "key-BConcept", "BConcept");
        bConcept.AddAnnotations([CreateConceptDescription("xxx", null, "Some enum", null)]);

        aLang.AddDependsOn([bLang]);
        bLang.AddDependsOn([aLang]);

        var literalLeft = aEnum.EnumerationLiteral("id-left", "key-left", "left");
        literalLeft.AddAnnotations([
            CreateKeyedDescription("aaa-leftDesc", "left desc", [aEnum])
        ]);
        aEnum.EnumerationLiteral("id-right", "key-right", "right");

        var bRef = aConcept.Reference("id-AConcept-BRef", "key-BRef", "BRef").IsOptional().OfType(bConcept);
        bRef.AddAnnotations([
            CreateKeyedDescription("aaa-bRefDesc", "bRef desc", [literalLeft, bRef])
        ]);
        bConcept.Reference("id-BConcept-ARef", "key-ARef", "ARef").IsOptional().OfType(aConcept);
        var aEnumProp = bConcept.Property("id-BConcept-AEnumProp", "key-AEnumProp", "AEnumProp").OfType(aEnum);
        aEnumProp.AddAnnotations([
            CreateKeyedDescription("aaa-enumPropDesc", "enum desc", [aLang, bRef])
        ]);

        aConcept.Property("AProp").OfType(aDatatype);
        aConcept.Property("BProp").OfType(bDatatype);
        
        bConcept.Property("AProp").OfType(aDatatype);
        bConcept.Property("BProp").OfType(bDatatype);

        ALang = aLang;
        BLang = bLang;
    }

    private INode CreateConceptDescription(string id, string? alias, string? description, string? helpUrl)
    {
        ISpecificLanguage specificLanguage = ISpecificLanguage.Get(_lionWebVersion);
        INode conceptDescrption = specificLanguage.GetFactory().CreateNode(id, specificLanguage.ConceptDescription);

        if (alias != null)
            conceptDescrption.Set(specificLanguage.ConceptDescription_conceptAlias, alias);

        if (description != null)
            conceptDescrption.Set(specificLanguage.ConceptDescription_conceptShortDescription, description);

        if (helpUrl != null)
            conceptDescrption.Set(specificLanguage.ConceptDescription_helpUrl, helpUrl);

        return conceptDescrption;
    }

    private INode CreateKeyedDescription(string id, string? documentation, IEnumerable<IReadableNode> seeAlso)
    {
        ISpecificLanguage specificLanguage = ISpecificLanguage.Get(_lionWebVersion);
        INode keyedDescription = specificLanguage.GetFactory().CreateNode(id, specificLanguage.KeyedDescription);

        if (documentation != null)
            keyedDescription.Set(specificLanguage.KeyedDescription_documentation, documentation);

        keyedDescription.Set(specificLanguage.KeyedDescription_seeAlso, seeAlso);

        return keyedDescription;
    }

    private void CreateTinyRefLang()
    {
        var tinyRefLang =
            new DynamicLanguage("id-TinyRefLang", _lionWebVersion)
            {
                Key = "key-tinyRefLang", Name = "TinyRefLang", Version = "0"
            };

        var myConcept = tinyRefLang.Concept("id-Concept", "key-MyConcept", "MyConcept")
            .Implementing(_lionWebVersion.BuiltIns.INamed);

        myConcept.Reference("id-MyConcept-singularRef", "key-MyConcept-singularRef", "singularRef")
            .OfType(_lionWebVersion.BuiltIns.INamed);

        myConcept.Reference("id-Concept-multivaluedRef", "key-MyConcept-multivaluedRef", "multivaluedRef")
            .OfType(_lionWebVersion.BuiltIns.INamed).IsMultiple();

        TinyRefLang = tinyRefLang;
    }

    private void CreateMultiInheritLang()
    {
        var lang = new DynamicLanguage("id-MultiInheritLang", _lionWebVersion)
        {
            Key = "key-MultiInheritLang", Name = "MultiInheritLang", Version = "1"
        };

        var baseIface = lang.Interface("id-BaseIface", "key-BaseIface", "BaseIface");

        var abstractConcept = lang.Concept("id-AbstractConcept", "key-AbstractConcept", "AbstractConcept")
            .Implementing(baseIface)
            .IsAbstract();

        var intermediateConcept = lang
            .Concept("id-IntermediateConcept", "key-IntermediateConcept", "IntermediateConcept")
            .Extending(abstractConcept);

        var combinedConcept = lang.Concept("id-CombinedConcept", "key-CombinedConcept", "CombinedConcept")
            .Extending(intermediateConcept)
            .Implementing(baseIface);

        baseIface.Containment("id-ifaceContainment", "key-ifaceContainment", "ifaceContainment")
            .OfType(_lionWebVersion.BuiltIns.Node);

        MultiInheritLang = lang;
    }

    private void CreateSdtLang()
    {
        var sdtLang =
            new DynamicLanguage("id-SDTLang", _lionWebVersion) { Key = "key-SDTLang", Name = "SDTLang", Version = "0" };
        var sdtConcept = sdtLang.Concept("id-SDTConcept", "key-SDTConcept", "SDTConcept");
        var currency = sdtLang.Enumeration("id-SDTCurrency", "key-SDTCurrency", "Currency");
        var amount = sdtLang.StructuredDataType("id-SDTAmount", "key-SDTAmount", "Amount");
        var dec = sdtLang.StructuredDataType("id-SDTDecimal", "key-SDTDecimal", "Decimal");
        var complex = sdtLang.StructuredDataType("id-SDTComplexNumber", "key-SDTComplexNumber", "ComplexNumber");

        sdtConcept.Property("id-SDTamountField", "key-SDTamountField", "amount").OfType(amount);
        sdtConcept.Property("id-SDTDecimalField", "key-SDTDecimalField", "decimal").OfType(dec).IsOptional();
        sdtConcept.Property("id-SDTComplexField", "key-SDTComplexField", "complex").OfType(complex);

        currency.EnumerationLiteral("id-SDT-eur", "key-SDTEur", "EUR");
        currency.EnumerationLiteral("id-SDT-gbp", "key-SDTGbp", "GBP");

        amount.Field("id-SDTValue", "key-SDTValue", "value").OfType(dec);
        amount.Field("id-SDTCurr", "key-SDTCurr", "currency").OfType(currency);
        amount.Field("id-SDTDigital", "key-SDTDigital", "digital").OfType(_lionWebVersion.BuiltIns.Boolean);

        dec.Field("id-SDTInt", "key-SDTInt", "int").OfType(_lionWebVersion.BuiltIns.Integer);
        dec.Field("id-SDTFrac", "key-SDTFrac", "frac").OfType(_lionWebVersion.BuiltIns.Integer);

        complex.Field("id-SDTReal", "key-SDTReal", "real").OfType(dec);
        complex.Field("id-SDTImaginary", "key-SDTImaginary", "imaginary").OfType(dec);

        // Shape:
        //     A
        //    / \
        //   B   C
        //    \ / \
        //     D   E
        //          \
        //           F

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

        SdtLang = sdtLang;
    }

    private void CreateMixedLangs()
    {
        var mixedBasePropertyLang = new DynamicLanguage("id-mixedBasePropertyLang", _lionWebVersion)
        {
            Key = "key-mixedBasePropertyLang", Name = "MixedBasePropertyLang", Version = "1"
        };
        var mixedBaseContainmentLang = new DynamicLanguage("id-mixedBaseContainmentLang", _lionWebVersion)
        {
            Key = "key-mixedBaseContainmentLang", Name = "MixedBaseContainmentLang", Version = "1"
        };
        var mixedBaseReferenceLang = new DynamicLanguage("id-mixedBaseReferenceLang", _lionWebVersion)
        {
            Key = "key-mixedBaseReferenceLang", Name = "MixedBaseReferenceLang", Version = "1"
        };
        var mixedBaseConceptLang = new DynamicLanguage("id-mixedBaseConceptLang", _lionWebVersion)
        {
            Key = "key-mixedBaseConceptLang", Name = "MixedBaseConceptLang", Version = "1"
        };
        var mixedConceptLang = new DynamicLanguage("id-mixedConceptLang", _lionWebVersion)
        {
            Key = "key-mixedConceptLang", Name = "MixedConceptLang", Version = "1"
        };
        var mixedDirectEnumLang = new DynamicLanguage("id-mixedDirectEnumLang", _lionWebVersion)
        {
            Key = "key-mixedDirectEnumLang", Name = "MixedDirectEnumLang", Version = "1"
        };
        var mixedNestedEnumLang = new DynamicLanguage("id-mixedNestedEnumLang", _lionWebVersion)
        {
            Key = "key-mixedNestedEnumLang", Name = "MixedNestedEnumLang", Version = "1"
        };
        var mixedDirectSdtLang = new DynamicLanguage("id-mixedDirectSdtLang", _lionWebVersion)
        {
            Key = "key-mixedDirectSdtLang", Name = "MixedDirectSdtLang", Version = "1"
        };
        var mixedNestedSdtLang = new DynamicLanguage("id-mixedNestedSdtLang", _lionWebVersion)
        {
            Key = "key-mixedNestedSdtLang", Name = "MixedNestedSdtLang", Version = "1"
        };

        var nestedSdt = mixedNestedSdtLang.StructuredDataType("id-nestedSdt", "key-nestedSdt", "NestedSdt");
        nestedSdt.Field("id-nestedSdtField", "key-nestedSdtField", "nestedSdtField")
            .OfType(_lionWebVersion.BuiltIns.String);

        var nestedEnum = mixedNestedEnumLang.Enumeration("id-nestedEnum", "key-nestedEnum", "NestedEnum");
        nestedEnum.EnumerationLiteral("id-nestedLiteralA", "key-nestedLiteralA", "nestedLiteralA");

        var directSdt = mixedDirectSdtLang.StructuredDataType("id-directSdt", "key-directSdt", "DirectSdt");
        directSdt.Field("id-directSdtEnum", "key-directSdtEnum", "directSdtEnum").OfType(nestedEnum);
        directSdt.Field("id-directSdtSdt", "key-directSdtSdt", "directSdtSdt").OfType(nestedSdt);

        mixedDirectSdtLang.AddDependsOn([mixedNestedEnumLang, mixedNestedSdtLang]);

        var directEnum = mixedDirectEnumLang.Enumeration("id-directEnum", "key-directEnum", "DirectEnum");
        directEnum.EnumerationLiteral("id-directEnumA", "key-directEnumA", "directEnumA");

        var basePropertyIface =
            mixedBasePropertyLang.Interface("id-basePropertyIface", "key-basePropertyIface", "BasePropertyIface");
        basePropertyIface.Property("id-basePropertyIface-prop", "key-basePropertyIface-prop", "Prop")
            .OfType(_lionWebVersion.BuiltIns.String);

        var baseContainmentIface = mixedBaseContainmentLang.Interface("id-baseContainmentIface",
            "key-baseContainmentIface", "BaseContainmentIface");
        baseContainmentIface.Containment("id-baseContainemntIface-cont", "key-baseContainmentIface-cont", "Cont")
            .OfType(_lionWebVersion.BuiltIns.Node);

        var baseReferenceIface =
            mixedBaseReferenceLang.Interface("id-baseReferenceIface", "key-baseReferenceIface", "BaseReferenceIface");
        baseReferenceIface.Reference("id-baseReferenceIface-ref", "key-baseReferenceIface-ref", "Ref")
            .OfType(_lionWebVersion.BuiltIns.Node);

        var baseConcept = mixedBaseConceptLang.Concept("id-baseConcept", "key-baseConcept", "BaseConcept").IsAbstract()
            .Implementing(basePropertyIface, baseContainmentIface, baseReferenceIface);
        baseConcept.Property("id-enumProp", "key-enumProp", "enumProp").OfType(directEnum);
        baseConcept.Property("id-sdtProp", "key-sdtProp", "sdtProp").OfType(directSdt);

        mixedBaseConceptLang.AddDependsOn([
            mixedBasePropertyLang, mixedBaseContainmentLang, mixedBaseReferenceLang, mixedDirectEnumLang,
            mixedDirectSdtLang
        ]);

        mixedConceptLang.Concept("id-mixedConcept", "key-mixedConcept", "MixedConcept").Extending(baseConcept);
        mixedConceptLang.AddDependsOn([mixedBaseConceptLang]);

        MixedLangs =
        [
            mixedBasePropertyLang, mixedBaseContainmentLang, mixedBaseReferenceLang, mixedBaseConceptLang,
            mixedConceptLang, mixedDirectEnumLang, mixedNestedEnumLang, mixedDirectSdtLang, mixedNestedSdtLang
        ];
    }

    private void CreateKeywordLang()
    {
        var keywordLang = new DynamicLanguage("id-keyword-lang", _lionWebVersion)
        {
            Name = "class", Key = "class", Version = "struct"
        };

        var iface = keywordLang.Interface("id-keyword-iface", "key-keyword-iface", "interface");
        var iface2 = keywordLang.Interface("id-keyword-iface2", "key-keyword-iface2", "partial")
            .Extending(iface);

        var concept = keywordLang.Concept("id-keyword-concept", "key-keyword-concept", "struct")
            .Implementing(iface);
        var concept2 = keywordLang.Concept("id-keyword-concept2", "key-keyword-concept2", "out")
            .Extending(concept);

        var ann = keywordLang.Annotation("id-keyword-ann", "key-keyword-ann", "record")
            .Implementing(iface);
        var ann2 = keywordLang.Annotation("id-keyword-ann2", "key-keyword-ann2", "var")
            .Extending(ann);

        var enm = keywordLang.Enumeration("id-keyword-enm", "key-keyword-enm", "enum");
        var lit = enm.EnumerationLiteral("id-keyword-enmA", "key-keyword-enmA", "internal");

        var prim = keywordLang.PrimitiveType("id-keyword-prim", "key-keyword-prim", "base");

        var sdt = keywordLang.StructuredDataType("id-keyword-sdt", "key-keyword-sdt", "if");
        var field = sdt.Field("id-keyword-field", "key-keyword-field", "namespace")
            .OfType(_lionWebVersion.BuiltIns.String);

        var reference = concept.Reference("id-keyword-reference", "key-keyword-reference", "ref")
            .OfType(ann);

        var cont = ann.Containment("id-keyword-cont", "key-keyword-cont", "double")
            .OfType(iface);

        var prop = iface.Property("id-keyword-prop", "key-keyword-prop", "string")
            .OfType(enm);

        var prop2 = concept2.Property("id-keyword-prop2", "key-keyword-prop2", "default")
            .OfType(sdt);

        KeywordLang = keywordLang;
    }

    private void CreateNamedLang()
    {
        var namedLang = new DynamicLanguage("id-NamedLang", _lionWebVersion)
        {
            Name = "NamedLang", Key = "key-NamedLang", Version = "1"
        };

        var iface = namedLang.Interface("id-Iface", "key-Iface", "Iface");
        var namedIface = namedLang.Interface("id-NamedIface", "key-NamedIface", "NamedIface")
            .Extending(_lionWebVersion.BuiltIns.INamed);
        var namedAnn = namedLang.Annotation("id-NamedAnn", "key-NamedAnn", "NamedAnn")
            .Implementing(_lionWebVersion.BuiltIns.INamed);
        var namedAbstractConcept = namedLang
            .Concept("id-NamedAbstractConcept", "key-NamedAbstractConcept", "NamedAbstractConcept")
            .IsAbstract()
            .Implementing(_lionWebVersion.BuiltIns.INamed);
        var namedConcept = namedLang.Concept("id-NamedConcept", "key-NamedConcept", "NamedConcept")
            .Implementing(_lionWebVersion.BuiltIns.INamed);

        var ifaceConcept = namedLang.Concept("id-IfaceConcept", "key-IfaceConcept", "IfaceConcept")
            .Implementing(iface);
        var namedIfaceAnn = namedLang.Annotation("id-NamedIfaceAnn", "key-NamedIfaceAnn", "NamedIfaceAnn")
            .Implementing(namedIface);
        var namedIfaceConcept = namedLang.Concept("id-NamedIfaceConcept", "key-NamedIfaceConcept", "NamedIfaceConcept")
            .Implementing(namedIface);
        var namedConcreteConcept = namedLang
            .Concept("id-NamedConcreteConcept", "key-NamedConcreteConcept", "NamedConcreteConcept")
            .Extending(namedAbstractConcept);
        var namedSubConcept = namedLang.Concept("id-NamedSubConcept", "key-NamedSubConcept", "NamedSubConcept")
            .Extending(namedConcept)
            .Implementing(_lionWebVersion.BuiltIns.INamed);

        NamedLang = namedLang;
    }

    /// <summary>
    /// This language is used to test the generation of a language concept features of builtin <see cref="IBuiltInsLanguage.Node"/> type.
    /// </summary>
    private void CreateGeneralNodeLang()
    {
        var generalNodeLang = new DynamicLanguage("id-GeneralNodeLang", _lionWebVersion)
        {
            Name = "GeneralNodeLang", Key = "key-GeneralNodeLang", Version = "1"
        };

        var concept = generalNodeLang.Concept("id-GeneralNodeConcept", "key-GeneralNodeConcept", "GeneralNodeConcept");
        concept.Reference("id-singleRef", "key-singleRef", "singleRef").OfType(_lionWebVersion.BuiltIns.Node);
        concept.Reference("id-multipleRef", "key-multipleRef", "multipleRef").OfType(_lionWebVersion.BuiltIns.Node)
            .IsMultiple();

        concept.Reference("id-singleOptionalRef", "key-singleOptionalRef", "singleOptionalRef")
            .OfType(_lionWebVersion.BuiltIns.Node).IsOptional();
        concept.Reference("id-multipleOptionalRef", "key-multipleOptionalRef", "multipleOptionalRef")
            .OfType(_lionWebVersion.BuiltIns.Node).IsMultiple().IsOptional();

        concept.Containment("id-singleContainment", "key-singleContainment", "singleContainment")
            .OfType(_lionWebVersion.BuiltIns.Node);
        concept.Containment("id-multipleContainment", "key-multipleContainment", "multipleContainment")
            .OfType(_lionWebVersion.BuiltIns.Node).IsMultiple();

        concept.Containment("id-singleOptionalContainment", "key-singleOptionalContainment",
            "singleOptionalContainment").OfType(_lionWebVersion.BuiltIns.Node).IsOptional();
        concept.Containment("id-multipleOptionalContainment", "key-multipleOptionalContainment",
            "multipleOptionalContainment").OfType(_lionWebVersion.BuiltIns.Node).IsMultiple().IsOptional();

        GeneralNodeLang = generalNodeLang;
    }

    private void CreateLowerCaseLang()
    {
        var casingLang = new DynamicLanguage("id-lowerCase-lang", _lionWebVersion)
        {
            Name = "myLowerCaseLang", Key = "key-LowerCaseLang", Version = "1"
        };

        var concept = casingLang.Concept("id-concept", "key-concept", "myConcept");

        var prop = concept.Property("id-property", "key-property", "myProperty")
            .OfType(_lionWebVersion.BuiltIns.String);
        var refer = concept.Reference("id-reference", "key-reference", "myReference")
            .OfType(_lionWebVersion.BuiltIns.Node);
        var cont = concept.Containment("id-containment", "key-containment", "myContainment")
            .OfType(_lionWebVersion.BuiltIns.Node);

        var enm = casingLang.Enumeration("id-enumeration", "key-enumeration", "myEnum");
        var lit = enm.EnumerationLiteral("id-literal", "key-literal", "myLiteral");

        var sdt = casingLang.StructuredDataType("id-sdt", "key-sdt", "mySdt");
        var field = sdt.Field("id-field", "key-field", "myField").OfType(_lionWebVersion.BuiltIns.String);

        var ann = casingLang.Annotation("id-annotation", "key-annotation", "myAnnotation");

        ann.AddAnnotations([
            CreateKeyedDescription("aaa-keyedDesc", null,
                [casingLang, concept, prop, refer, cont, enm, lit, sdt, field, ann])
        ]);

        LowerCaseLang = casingLang;
    }

    private void CreateUpperCaseLang()
    {
        var casingLang = new DynamicLanguage("id-UpperCase-lang", _lionWebVersion)
        {
            Name = "MYUpperCaseLang", Key = "key-UpperCaseLang", Version = "1"
        };

        var concept = casingLang.Concept("id-concept", "key-concept", "MYConcept");

        var prop = concept.Property("id-property", "key-property", "MYProperty")
            .OfType(_lionWebVersion.BuiltIns.String);
        var refer = concept.Reference("id-reference", "key-reference", "MYReference")
            .OfType(_lionWebVersion.BuiltIns.Node);
        var cont = concept.Containment("id-containment", "key-containment", "MYContainment")
            .OfType(_lionWebVersion.BuiltIns.Node);

        var enm = casingLang.Enumeration("id-enumeration", "key-enumeration", "MYEnum");
        var lit = enm.EnumerationLiteral("id-literal", "key-literal", "MYLiteral");

        var sdt = casingLang.StructuredDataType("id-sdt", "key-sdt", "MYSdt");
        var field = sdt.Field("id-field", "key-field", "MYField").OfType(_lionWebVersion.BuiltIns.String);

        var ann = casingLang.Annotation("id-annotation", "key-annotation", "MYAnnotation");

        ann.AddAnnotations([
            CreateKeyedDescription("aaa-keyedDesc", null,
                [casingLang, concept, prop, refer, cont, enm, lit, sdt, field, ann])
        ]);

        UpperCaseLang = casingLang;
    }

    private void CreateNamespaceStartsWithLionWebLang()
    {
        var lwLang = new DynamicLanguage("id-NamespaceStartsWithLionWebLang-lang", _lionWebVersion)
        {
            Name = "NamespaceStartsWithLionWebLang", Key = "key-NamespaceStartsWithLionWebLang", Version = "1"
        };
        
        lwLang.Concept("id-concept", "key-concept", "MYConcept");

        NamespaceStartsWithLionWebLang = lwLang;
    }

    private void CreateNamespaceContainsLionWebLang()
    {
        var lwLang = new DynamicLanguage("id-NamespaceContainsLionWebLang-lang", _lionWebVersion)
        {
            Name = "NamespaceContainsLionWebLang", Key = "key-NamespaceContainsLionWebLang", Version = "1"
        };
        
        lwLang.Concept("id-concept", "key-concept", "MYConcept");

        NamespaceContainsLionWebLang = lwLang;
    }
    
    private void CreateLanguageWithLionWebNamedConcepts()
    {
        var lang = new DynamicLanguage("id-LanguageWithLionWebNamedConcepts-lang", _lionWebVersion)
        {
            Name = "LanguageWithLionWebNamedConcepts", Key = "key-LanguageWithLionWebNamedConcepts", Version = "1"
        };

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        
        HashSet<string> conceptNames = 
        [
            // M3
            "Language",
            "EnumerationLiteral",
            "Link",
            "Containment",
            "Reference",
            "Property",
            "Field",
            "LanguageEntity",
            "Classifier",
            "Annotation",
            "Concept",
            "Interface",
            "Datatype",
            "Enumeration",
            "PrimitiveType",
            "StructuredDataType",

            // M3 base
            "LanguageBase",
            "EnumerationLiteralBase",
            "ContainmentBase",
            "ReferenceBase",
            "PropertyBase",
            "FieldBase",
            "AnnotationBase",
            "ConceptBase",
            "InterfaceBase",
            "DatatypeBase",
            "EnumerationBase",
            "PrimitiveTypeBase",
            "StructuredDataTypeBase",

            // Framework
            "Abstract",
            "AbstractBaseNodeFactory",
            "AddChildRaw",
            "AddContainmentsRaw",
            "AddInternal",
            "AddOptionalMultipleContainment",
            "AddOptionalMultipleReference",
            "AddReferencesRaw",
            "AddRequiredMultipleContainment",
            "AddRequiredMultipleReference",
            "AnnotatesLazy",
            "AnnotationInstanceBase",
            "ArgumentOutOfRangeException",
            "Bool",
            "Boolean",
            // "boolean",
            "Char",
            "Character",
            "CodeAnalysis",
            "CollectAllSetFeatures",
            "Collections",
            "ConceptInstanceBase",
            "Core",
            "Decimal",
            "DetachChild",
            "Diagnostics",
            "Enum",
            "ExchangeChildRaw",
            "ExchangeChildrenRaw",
            "FeaturesLazy",
            "FieldsLazy",
            "Generic",
            "GetAnnotation",
            "GetConcept",
            "GetContainmentOf",
            "GetInternal",
            "IEnumerable",
            "IFieldValues",
            "INamed",
            "INamedWritable",
            "INode",
            "INodeFactory",
            "IPartitionInstance",
            "IReadOnlyList",
            "IReadableNode",
            "IStructuredDataTypeInstance",
            "IWritableNode",
            "ImplementsLazy",
            "InsertChildRaw",
            "InsertInternal",
            "InsertOptionalMultipleContainment",
            "InsertOptionalMultipleReference",
            "InsertReferencesRaw",
            "InsertRequiredMultipleContainment",
            "InsertRequiredMultipleReference",
            "Instance",
            "Integer",
            // "integer",
            "InvalidValueException",
            "Key",
            "Lazy",
            "LionCoreFeature",
            "LionCoreFeatureKind",
            "LionCoreLanguage",
            "LionCoreMetaPointer",
            "LionWeb",
            "LionWebVersions",
            "List",
            "LiteralsLazy",
            "M2",
            "M3",
            "Multiple",
            "Name",
            // "_name",
            "NotNullWhenAttribute",
            "Notification",
            "Optional",
            "Partition",
            "Pipe",
            "ReferenceEquals",
            "RemoveChildRaw",
            "RemoveInternal",
            "RemoveOptionalMultipleContainment",
            "RemoveOptionalMultipleReference",
            "RemoveReferencesRaw",
            "RemoveRequiredMultipleContainment",
            "RemoveRequiredMultipleReference",
            "RemoveSelfParentRaw",
            "SetContainmentRaw",
            "SetInternal",
            "SetName",
            "SetNameRaw",
            "SetOptionalMultipleContainment",
            "SetOptionalMultipleReference",
            "SetOptionalSingleContainment",
            "SetOptionalSingleReference",
            "SetPropertyRaw",
            "SetReferenceRaw",
            "SetReferencesRaw",
            "SetRequiredMultipleContainment",
            "SetRequiredMultipleReference",
            "SetRequiredReferenceTypeProperty",
            "SetRequiredSingleContainment",
            "SetRequiredSingleReference",
            "SetRequiredValueTypeProperty",
            "String",
            "System",
            "TryGetContainmentRaw",
            "TryGetContainmentsRaw",
            "TryGetName",
            "TryGetPropertyRaw",
            "TryGetReferenceRaw",
            "TryGetReferencesRaw",
            "Type",
            "UnsetFeatureException",
            "UnsetFieldException",
            "UnsupportedClassifierException",
            "UnsupportedEnumerationLiteralException",
            "UnsupportedStructuredDataTypeException",
            "Utilities",
            "V2023_1",
            "V2024_1",
            "V2024_1_Compatible",
            "V2025_1",
            "V2025_1_Compatible",
            "VersionSpecific",
            "_builtIns",
            "result",
            // "v2023_1",
            // "v2024_1",
            // "v2024_1_Compatible",
            // "v2025_1",
            // "v2025_1_Compatible",
            ..typeof(ReadableNodeBase<>).GetMembers(bindingFlags)
                .Concat(typeof(NodeBase).GetMembers(bindingFlags))
                .OfType<MethodInfo>()
                .Select(i => i.Name)
                .Where(n => !n.Contains('.'))
                .Where(IdUtils.IsValid)
        ];
        
        foreach (var conceptName in conceptNames)
        {
            lang.Concept(conceptName);
        }
        
        LanguageWithLionWebNamedConcepts = lang;
    }
    
    private void CreateFeatureSameNameAsContainingConcept()
    {
        var lang = new DynamicLanguage("id-FeatureSameNameAsContainingConcept-lang", _lionWebVersion)
        {
            Name = "FeatureSameNameAsContainingConcept", Key = "key-FeatureSameNameAsContainingConcept", Version = "1"
        };

        var baseConceptA = lang.Concept("BaseConceptA").IsAbstract();
        baseConceptA.Property("BaseConceptA");
        lang.Concept("SubConceptA").Extending(baseConceptA);

        var baseConceptB = lang.Concept("BaseConceptB").IsAbstract();
        baseConceptB.Property("SubConceptB");
        lang.Concept("SubConceptB").Extending(baseConceptB);

        var iface = lang.Interface("Iface");
        iface.Property("IfaceMethod");
        var ifaceImplA = lang.Concept("IfaceImplA").Implementing(iface);
        lang.Concept("IfaceImplB").Implementing(iface);
        var ifaceMethodConcept = lang.Concept("IfaceMethod").Extending(ifaceImplA);
        
        var baseConceptC = lang.Concept("BaseConceptC").IsAbstract();
        baseConceptC.Property("BaseConceptD");
        
        var baseConceptD = lang.Concept("BaseConceptD").IsAbstract();
        baseConceptD.Property("BaseConceptE");
        
        FeatureSameNameAsContainingConcept = lang;
    }
    
    private void CreateCustomPrimitiveTypeLang()
    {
        var lang = new DynamicLanguage("id-CustomPrimitiveTypeLang-lang", _lionWebVersion)
        {
            Name = "CustomPrimitiveTypeLang", Key = "key-CustomPrimitiveTypeLang", Version = "1"
        };

        var concept = lang.Concept("PrimitiveConcept");
        
        CustomType(typeof(CustomType));
        var nested = CustomType(typeof(LionWeb.Build.Languages.SubNamespace.CustomType), "CustomNestedType");
        CustomType(typeof(CustomEqualityType));
        var enm = CustomType(typeof(CustomEnumType));
        CustomType(typeof(CustomReferenceType));
        CustomType(typeof(string), "String");
        CustomType(typeof(int), "Integer");
        CustomType(typeof(int), "Int");
        CustomType(typeof(bool), "Boolean");
        
        var sdt = lang.StructuredDataType("Sdt");
        
        var boolType = lang.PrimitiveType("Bool");
        CustomPrimitiveTypeMapping[boolType] = typeof(bool);
        sdt.Field("boolField").OfType(boolType);
        
        sdt.Field("customField").OfType(nested);
        sdt.Field("enumField").OfType(enm);
        
        CustomPrimitiveTypeLang = lang;
        return;

        PrimitiveType CustomType(Type type, string? name = null)
        {
            var safeName = name ?? type.Name;
            var customType = lang.PrimitiveType(safeName);
            concept.Property(safeName.ToFirstLower() + "Optional").IsOptional(true).OfType(customType);
            concept.Property(safeName.ToFirstLower() + "Required").IsOptional(false).OfType(customType);
            CustomPrimitiveTypeMapping[customType] = type;
            return customType;
        }
    }
}

internal static class LanguageExtensions
{
    internal static DynamicConcept Concept(this DynamicLanguage language, string name) =>
        language.Concept($"id-{name}", $"key-{name}", name);

    internal static DynamicInterface Interface(this DynamicLanguage language, string name) =>
        language.Interface($"id-{name}", $"key-{name}", name);

    internal static DynamicPrimitiveType PrimitiveType(this DynamicLanguage language, string name) =>
        language.PrimitiveType($"id-{name}", $"key-{name}", name);

    internal static DynamicStructuredDataType StructuredDataType(this DynamicLanguage language, string name) =>
        language.StructuredDataType($"id-{name}", $"key-{name}", name);


    internal static DynamicProperty Property(this DynamicClassifier classifier, string name) =>
        classifier.Property($"id-{name}", $"key-{name}", name).OfType(classifier.GetLanguage().LionWebVersion.BuiltIns.String);

    
    internal static DynamicField Field(this DynamicStructuredDataType sdt, string name) =>
        sdt.Field($"id-{name}", $"key-{name}", name).OfType(sdt.GetLanguage().LionWebVersion.BuiltIns.String);
}