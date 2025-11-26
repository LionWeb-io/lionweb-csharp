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

namespace LionWeb.Core.Test.M2Reflection;

using Languages;
using M2;
using M3;

public abstract class M2ReflectionTestsBase
{
    protected static readonly LionWebVersions _lionWebVersion = LionWebVersions.v2024_1;

    protected static readonly ILionCoreLanguageWithStructuredDataType _m3 =
        (ILionCoreLanguageWithStructuredDataType)_lionWebVersion.LionCore;

    protected static readonly IBuiltInsLanguage _builtIns = _lionWebVersion.BuiltIns;

    protected static Classifier Concept = _m3.Concept;

    protected static Feature INamedName = _builtIns.INamed_name;
    protected static Feature IKeyedKey = _m3.IKeyed_key;

    protected static Feature LanguageVersion = _m3.Language_version;

    protected static Feature LanguageEntities = _m3.Language_entities;

    protected static Feature LanguageDependsOn = _m3.Language_dependsOn;

    protected static Feature ClassifierFeatures = _m3.Classifier_features;

    protected static Feature AnnotationAnnotates = _m3.Annotation_annotates;

    protected static Feature AnnotationExtends = _m3.Annotation_extends;

    protected static Feature AnnotationImplements = _m3.Annotation_implements;

    protected static Feature ConceptAbstract = _m3.Concept_abstract;

    protected static Feature ConceptPartition = _m3.Concept_partition;

    protected static Feature ConceptExtends = _m3.Concept_extends;

    protected static Feature ConceptImplements = _m3.Concept_implements;

    protected static Feature FeatureOptional = _m3.Feature_optional;

    protected static Feature LinkMultiple = _m3.Link_multiple;

    protected static Feature LinkType = _m3.Link_type;

    protected static Feature EnumerationLiterals = _m3.Enumeration_literals;

    protected static Feature InterfaceExtends = _m3.Interface_extends;

    protected static Feature Fields = _m3.StructuredDataType_fields;

    protected static Feature FieldType = _m3.Field_type;

    protected DynamicLanguage shapesLang;
    protected DynamicLanguage enumLang;
    protected DynamicLanguage sdtLang;

    protected DynamicLanguage lang { get => shapesLang; }
    protected DynamicAnnotation ann { get => shapesLang.ClassifierByKey("key-Documentation") as DynamicAnnotation; }
    protected DynamicConcept conc { get => shapesLang.ClassifierByKey("key-CompositeShape") as DynamicConcept; }
    protected DynamicContainment cont { get => conc.FeatureByKey("key-parts") as DynamicContainment; }

    protected DynamicReference refe
    {
        get => shapesLang.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-source") as DynamicReference;
    }

    protected DynamicEnumeration enm { get => enumLang.Enumerations().First() as DynamicEnumeration; }
    protected DynamicEnumerationLiteral enLit { get => enm.Literals.Last() as DynamicEnumerationLiteral; }

    protected DynamicPrimitiveType prim
    {
        get => shapesLang.Entities.First(e => e.Key == "key-Time") as DynamicPrimitiveType;
    }

    protected DynamicInterface iface { get => shapesLang.ClassifierByKey("key-IShape") as DynamicInterface; }

    protected DynamicStructuredDataType sdt
    {
        get => enumLang.Entities.OfType<StructuredDataType>().First() as DynamicStructuredDataType;
    }

    protected DynamicField fld { get => sdt.Fields.First() as DynamicField; }


    [TestInitialize]
    public void InitLanguages()
    {
        shapesLang = ShapesDynamic.GetLanguage(_lionWebVersion);
        enumLang = LanguagesUtils
            .LoadLanguages("LionWeb.Core.Test", "LionWeb.Core.Test.Languages.defChunks.with-enum.json").First();
        enumLang = LanguagesUtils
            .LoadLanguages("LionWeb.Core.Test", "LionWeb.Core.Test.Languages.defChunks.sdtLang.json").First();
    }
}