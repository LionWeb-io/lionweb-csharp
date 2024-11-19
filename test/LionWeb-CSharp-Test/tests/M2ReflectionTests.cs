// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License");
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

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.M3.Test;

using M2;

[TestClass]
public class M2ReflectionTests
{
    private static readonly ILionCoreLanguage _m3 = LionCoreLanguage_2023_1.Instance;
    private static readonly BuiltInsLanguage_2023_1 _builtIns = BuiltInsLanguage_2023_1.Instance;

    private static Classifier Concept = _m3.Concept;

    private static Feature INamedName = _builtIns.INamed_name;
    private static Feature IKeyedKey = _m3.IKeyed_key;

    private static Feature LanguageVersion = _m3.Language_version;

    private static Feature LanguageEntities = _m3.Language_entities;

    private static Feature LanguageDependsOn = _m3.Language_dependsOn;

    private static Feature ClassifierFeatures = _m3.Classifier_features;

    private static Feature AnnotationAnnotates = _m3.Annotation_annotates;

    private static Feature AnnotationExtends = _m3.Annotation_extends;

    private static Feature AnnotationImplements = _m3.Annotation_implements;

    private static Feature ConceptAbstract = _m3.Concept_abstract;

    private static Feature ConceptPartition = _m3.Concept_partition;

    private static Feature ConceptExtends = _m3.Concept_extends;

    private static Feature ConceptImplements = _m3.Concept_implements;

    private static Feature FeatureOptional = _m3.Feature_optional;

    private static Feature LinkMultiple = _m3.Link_multiple;

    private static Feature LinkType = _m3.Link_type;

    private static Feature EnumerationLiterals = _m3.Enumeration_literals;

    private static Feature InterfaceExtends = _m3.Interface_extends;

    DynamicLanguage shapesLang;
    DynamicLanguage enumLang;
    private readonly LionWebVersions _lionWebVersion = LionWebVersions.v2023_1;

    DynamicLanguage lang { get => shapesLang; }
    DynamicAnnotation ann { get => shapesLang.ClassifierByKey("key-Documentation") as DynamicAnnotation; }
    DynamicConcept conc { get => shapesLang.ClassifierByKey("key-CompositeShape") as DynamicConcept; }
    DynamicContainment cont { get => conc.FeatureByKey("key-parts") as DynamicContainment; }

    DynamicReference refe
    {
        get => shapesLang.ClassifierByKey("key-OffsetDuplicate").FeatureByKey("key-source") as DynamicReference;
    }

    DynamicEnumeration enm { get => enumLang.Enumerations().First() as DynamicEnumeration; }
    DynamicEnumerationLiteral enLit { get => enm.Literals.Last() as DynamicEnumerationLiteral; }

    private DynamicPrimitiveType prim
    {
        get => shapesLang.Entities.First(e => e.Key == "key-Time") as DynamicPrimitiveType;
    }

    DynamicInterface iface { get => shapesLang.ClassifierByKey("key-IShape") as DynamicInterface; }

    [TestInitialize]
    public void InitLanguages()
    {
        shapesLang = LanguagesUtils
            .LoadLanguages("LionWeb-CSharp-Test", "LionWeb_CSharp_Test.languages.defChunks.shapes.json").First();
        enumLang = LanguagesUtils
            .LoadLanguages("LionWeb-CSharp-Test", "LionWeb_CSharp_Test.languages.defChunks.with-enum.json").First();
    }

    [TestMethod]
    public void Language_Meta()
    {
        Assert.AreEqual(_m3.Language, lang.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                LanguageVersion,
                LanguageEntities,
                LanguageDependsOn
            },
            lang.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Language_Get()
    {
        Assert.AreEqual(lang.Name, lang.Get(INamedName));
        Assert.AreEqual(lang.Key, lang.Get(IKeyedKey));
        Assert.AreEqual(lang.Version, lang.Get(LanguageVersion));
        CollectionAssert.AreEqual(lang.Entities.ToList(),
            (lang.Get(LanguageEntities) as IReadOnlyList<LanguageEntity>).ToList());
        CollectionAssert.AreEqual(lang.DependsOn.ToList(),
            (lang.Get(LanguageDependsOn) as IReadOnlyList<Language>).ToList());
        Assert.ThrowsException<UnknownFeatureException>(() => lang.Get(AnnotationAnnotates));
    }

    [TestMethod]
    public void Language_Set_Name()
    {
        lang.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", lang.Name);
        Assert.ThrowsException<InvalidValueException>(() => lang.Set(INamedName, null));
        Assert.AreEqual("Hello", lang.Name);
        Assert.ThrowsException<InvalidValueException>(() => lang.Set(INamedName, 123));
    }

    [TestMethod]
    public void Language_Set_Key()
    {
        var language = lang;
        language.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", language.Key);
        Assert.ThrowsException<InvalidValueException>(() => language.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", language.Key);
        Assert.ThrowsException<InvalidValueException>(() => language.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void Language_Set_Version()
    {
        lang.Set(LanguageVersion, "453");
        Assert.AreEqual("453", lang.Version);
        Assert.ThrowsException<InvalidValueException>(() => lang.Set(LanguageVersion, null));
        Assert.AreEqual("453", lang.Version);
        Assert.ThrowsException<InvalidValueException>(() => lang.Set(LanguageVersion, 123));
    }

    [TestMethod]
    public void Language_Set_Entities()
    {
        Concept conceptA = new DynamicConcept("my-id", lang) { Key = "my-key", Name = "SomeName" };
        Enumeration enumA = new DynamicEnumeration("enum-id", lang) { Key = "enum-key", Name = "SomeEnum" };
        lang.Set(LanguageEntities, new List<LanguageEntity> { conceptA, enumA });
        CollectionAssert.AreEqual(new List<object> { conceptA, enumA }, lang.Entities.ToList());
        lang.Set(LanguageEntities, Enumerable.Empty<LanguageEntity>());
        CollectionAssert.AreEqual(Enumerable.Empty<LanguageEntity>().ToList(), lang.Entities.ToList());
        Assert.ThrowsException<InvalidValueException>(() => lang.Set(LanguageEntities, null));
    }

    [TestMethod]
    public void Language_Set_DependsOn()
    {
        Language langA =
            new DynamicLanguage("langAId", _lionWebVersion) { Version = "123", Key = "langAKey", Name = "LangA" };
        Language langB =
            new DynamicLanguage("langBId", _lionWebVersion) { Version = "23", Key = "langBKey", Name = "LangB" };
        lang.Set(LanguageDependsOn, new List<Language> { langA, langB });
        CollectionAssert.AreEqual(new List<Language> { langA, langB }, lang.DependsOn.ToList());
        lang.Set(LanguageDependsOn, Enumerable.Empty<Language>());
        CollectionAssert.AreEqual(Enumerable.Empty<Language>().ToList(), lang.DependsOn.ToList());
        Assert.ThrowsException<InvalidValueException>(() => lang.Set(LanguageDependsOn, null));
    }

    [TestMethod]
    public void Language_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => lang.Set(AnnotationExtends, null));
    }

    [TestMethod]
    public void Annotation_Meta()
    {
        Assert.AreEqual(_m3.Annotation, ann.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                ClassifierFeatures,
                AnnotationAnnotates,
                AnnotationExtends,
                AnnotationImplements
            },
            ann.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Annotation_Get()
    {
        Assert.AreEqual(ann.Name, ann.Get(INamedName));
        Assert.AreEqual(ann.Key, ann.Get(IKeyedKey));
        CollectionAssert.AreEqual(ann.Features.ToList(),
            (ann.Get(ClassifierFeatures) as IReadOnlyList<Feature>).ToList());
        Assert.AreEqual(ann.Annotates, ann.Get(AnnotationAnnotates));
        Assert.AreEqual(ann.Extends, ann.Get(AnnotationExtends));
        CollectionAssert.AreEqual(ann.Implements.ToList(),
            (ann.Get(AnnotationImplements) as IReadOnlyList<Interface>).ToList());
        Assert.ThrowsException<UnknownFeatureException>(() => ann.Get(LanguageVersion));
    }

    [TestMethod]
    public void Annotation_Set_Name()
    {
        ann.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", ann.Name);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(INamedName, null));
        Assert.AreEqual("Hello", ann.Name);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(INamedName, 123));
        Assert.AreEqual("Hello", ann.Name);
    }

    [TestMethod]
    public void Annotation_Set_Key()
    {
        var annotation = ann;
        annotation.Set(IKeyedKey, "asdf");
        Assert.AreEqual("asdf", annotation.Key);
        Assert.ThrowsException<InvalidValueException>(() => annotation.Set(IKeyedKey, null));
        Assert.AreEqual("asdf", annotation.Key);
        Assert.ThrowsException<InvalidValueException>(() => annotation.Set(IKeyedKey, 123));
        Assert.AreEqual("asdf", annotation.Key);
    }

    [TestMethod]
    public void Annotation_Set_Features()
    {
        Property propA = new DynamicProperty("my-id", ann) { Key = "my-key", Name = "SomeName" };
        Reference refB = new DynamicReference("ref-id", ann) { Key = "ref-key", Name = "SomeRef" };
        ann.Set(ClassifierFeatures, new List<Feature> { propA, refB });
        CollectionAssert.AreEqual(new List<object> { propA, refB }, ann.Features.ToList());
        ann.Set(ClassifierFeatures, Enumerable.Empty<Feature>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), ann.Features.ToList());
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(ClassifierFeatures, null));
    }

    [TestMethod]
    public void Annotation_Set_Annotates()
    {
        Annotation tgt = new DynamicAnnotation("my-id", lang) { Key = "my-key", Name = "SomeName" };
        ann.Set(AnnotationAnnotates, tgt);
        Assert.AreEqual(tgt, ann.Annotates);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(AnnotationAnnotates, null));
        Assert.AreEqual(tgt, ann.Annotates);
    }

    [TestMethod]
    public void Annotation_Set_Extends()
    {
        Annotation sup = new DynamicAnnotation("my-id", lang) { Key = "my-key", Name = "SomeName" };
        ann.Set(AnnotationExtends, sup);
        Assert.AreEqual(sup, ann.Extends);
        ann.Set(AnnotationExtends, null);
        Assert.AreEqual(null, ann.Extends);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(AnnotationExtends, lang));
    }

    [TestMethod]
    public void Annotation_Set_Implements()
    {
        Interface ifaceA = new DynamicInterface("my-id", lang) { Key = "my-key", Name = "SomeName" };
        Interface ifaceB = new DynamicInterface("ref-id", lang) { Key = "ref-key", Name = "SomeRef" };
        ann.Set(AnnotationImplements, new List<Interface> { ifaceA, ifaceB });
        CollectionAssert.AreEqual(new List<object> { ifaceA, ifaceB }, ann.Implements.ToList());
        ann.Set(AnnotationImplements, Enumerable.Empty<Interface>());
        CollectionAssert.AreEqual(Enumerable.Empty<Interface>().ToList(), ann.Implements.ToList());
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(AnnotationImplements, null));
    }

    [TestMethod]
    public void Annotation_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => ann.Set(LanguageVersion, "asdf"));
    }

    [TestMethod]
    public void Concept_Meta()
    {
        Assert.AreEqual(Concept, conc.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                ClassifierFeatures,
                ConceptAbstract,
                ConceptPartition,
                ConceptExtends,
                ConceptImplements
            },
            conc.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Concept_Get()
    {
        Assert.AreEqual(conc.Name, conc.Get(INamedName));
        Assert.AreEqual(conc.Key, conc.Get(IKeyedKey));
        CollectionAssert.AreEqual(conc.Features.ToList(),
            (conc.Get(ClassifierFeatures) as IReadOnlyList<Feature>).ToList());
        Assert.AreEqual(conc.Abstract, conc.Get(ConceptAbstract));
        Assert.AreEqual(conc.Partition, conc.Get(ConceptPartition));
        Assert.AreEqual(conc.Extends, conc.Get(ConceptExtends));
        CollectionAssert.AreEqual(conc.Implements.ToList(),
            (conc.Get(ConceptImplements) as IReadOnlyList<Interface>).ToList());
        Assert.ThrowsException<UnknownFeatureException>(() => conc.Get(LanguageVersion));
    }

    [TestMethod]
    public void Concept_Set_Name()
    {
        conc.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", conc.Name);
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(INamedName, null));
        Assert.AreEqual("Hello", conc.Name);
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(INamedName, 123));
    }

    [TestMethod]
    public void Concept_Set_Key()
    {
        var concept = conc;
        concept.Set(IKeyedKey, "myKey");
        Assert.AreEqual("myKey", concept.Key);
        concept.Set(IKeyedKey, "otherKey");
        Assert.AreEqual("otherKey", concept.Key);
        Assert.ThrowsException<InvalidValueException>(() => concept.Set(IKeyedKey, null));
    }

    [TestMethod]
    public void Concept_Set_Features()
    {
        Property propA = new DynamicProperty("my-id", ann) { Key = "my-key", Name = "SomeName" };
        Reference refB = new DynamicReference("ref-id", ann) { Key = "ref-key", Name = "SomeRef" };
        conc.Set(ClassifierFeatures, new List<Feature> { propA, refB });
        CollectionAssert.AreEqual(new List<object> { propA, refB }, conc.Features.ToList());
        conc.Set(ClassifierFeatures, Enumerable.Empty<Feature>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), conc.Features.ToList());
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(ClassifierFeatures, null));
    }

    [TestMethod]
    public void Concept_Set_Abstract()
    {
        conc.Set(ConceptAbstract, true);
        Assert.AreEqual(true, conc.Abstract);
        conc.Set(ConceptAbstract, false);
        Assert.AreEqual(false, conc.Abstract);
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(ConceptAbstract, null));
    }

    [TestMethod]
    public void Concept_Set_Partition()
    {
        conc.Set(ConceptPartition, true);
        Assert.AreEqual(true, conc.Partition);
        conc.Set(ConceptPartition, false);
        Assert.AreEqual(false, conc.Partition);
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(ConceptPartition, null));
    }

    [TestMethod]
    public void Concept_Set_Extends()
    {
        Concept sup = new DynamicConcept("my-id", lang) { Key = "my-key", Name = "SomeName" };
        conc.Set(ConceptExtends, sup);
        Assert.AreEqual(sup, conc.Extends);
        conc.Set(ConceptExtends, null);
        Assert.AreEqual(null, conc.Extends);
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(ConceptExtends, lang));
    }

    [TestMethod]
    public void Concept_Set_Implements()
    {
        Interface ifaceA = new DynamicInterface("my-id", lang) { Key = "my-key", Name = "SomeName" };
        Interface ifaceB = new DynamicInterface("ref-id", lang) { Key = "ref-key", Name = "SomeRef" };
        conc.Set(ConceptImplements, new List<Interface> { ifaceA, ifaceB });
        CollectionAssert.AreEqual(new List<object> { ifaceA, ifaceB }, conc.Implements.ToList());
        conc.Set(ConceptImplements, Enumerable.Empty<Interface>());
        CollectionAssert.AreEqual(Enumerable.Empty<Interface>().ToList(), conc.Implements.ToList());
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(ConceptImplements, null));
    }

    [TestMethod]
    public void Concept_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => conc.Set(LanguageVersion, "asdf"));
    }

    [TestMethod]
    public void Containment_Meta()
    {
        Assert.AreEqual(_m3.Containment, cont.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                FeatureOptional,
                LinkMultiple,
                LinkType
            },
            cont.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Containment_Get()
    {
        Assert.AreEqual(cont.Name, cont.Get(INamedName));
        Assert.AreEqual(cont.Key, cont.Get(IKeyedKey));
        Assert.AreEqual(cont.Optional, cont.Get(FeatureOptional));
        Assert.AreEqual(cont.Multiple, cont.Get(LinkMultiple));
        Assert.AreEqual(cont.Type, cont.Get(LinkType));
        Assert.ThrowsException<UnknownFeatureException>(() => cont.Get(LanguageVersion));
    }

    [TestMethod]
    public void Containment_Set_Name()
    {
        cont.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", cont.Name);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(INamedName, null));
        Assert.AreEqual("Hello", cont.Name);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(INamedName, 123));
    }

    [TestMethod]
    public void Containment_Set_Key()
    {
        var containment = cont;
        containment.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", containment.Key);
        Assert.ThrowsException<InvalidValueException>(() => containment.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", containment.Key);
        Assert.ThrowsException<InvalidValueException>(() => containment.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void Containment_Set_Optional()
    {
        cont.Set(FeatureOptional, true);
        Assert.AreEqual(true, cont.Optional);
        cont.Set(FeatureOptional, false);
        Assert.AreEqual(false, cont.Optional);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(FeatureOptional, null));
    }

    [TestMethod]
    public void Containment_Set_Multiple()
    {
        cont.Set(LinkMultiple, true);
        Assert.AreEqual(true, cont.Multiple);
        cont.Set(LinkMultiple, false);
        Assert.AreEqual(false, cont.Multiple);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(LinkMultiple, null));
    }

    [TestMethod]
    public void Containment_Set_Type()
    {
        cont.Set(LinkType, ann);
        Assert.AreEqual(ann, cont.Type);
        Assert.ThrowsException<InvalidValueException>(() => cont.Set(LinkType, lang));
        Assert.AreEqual(ann, cont.Type);
    }

    [TestMethod]
    public void Containment_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => cont.Set(LanguageVersion, "asdf"));
    }

    [TestMethod]
    public void Reference_Meta()
    {
        Assert.AreEqual(_m3.Reference, refe.GetClassifier());

        var collection = refe.CollectAllSetFeatures().ToList();
        CollectionAssert.AreEqual(new List<Feature>
            {
                INamedName,
                IKeyedKey,
                FeatureOptional,
                LinkMultiple,
                LinkType
            },
            collection);
    }

    [TestMethod]
    public void Reference_Get()
    {
        Assert.AreEqual(refe.Name, refe.Get(INamedName));
        Assert.AreEqual(refe.Key, refe.Get(IKeyedKey));
        Assert.AreEqual(refe.Optional, refe.Get(FeatureOptional));
        Assert.AreEqual(refe.Multiple, refe.Get(LinkMultiple));
        Assert.AreEqual(refe.Type, refe.Get(LinkType));
        Assert.ThrowsException<UnknownFeatureException>(() => refe.Get(LanguageVersion));
    }

    [TestMethod]
    public void Reference_Set_Name()
    {
        refe.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", refe.Name);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(INamedName, null));
        Assert.AreEqual("Hello", refe.Name);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(INamedName, 123));
    }

    [TestMethod]
    public void Reference_Set_Key()
    {
        var reference = refe;
        reference.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", reference.Key);
        Assert.ThrowsException<InvalidValueException>(() => reference.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", reference.Key);
        Assert.ThrowsException<InvalidValueException>(() => reference.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void Reference_Set_Optional()
    {
        refe.Set(FeatureOptional, true);
        Assert.AreEqual(true, refe.Optional);
        refe.Set(FeatureOptional, false);
        Assert.AreEqual(false, refe.Optional);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(FeatureOptional, null));
    }

    [TestMethod]
    public void Reference_Set_Multiple()
    {
        refe.Set(LinkMultiple, true);
        Assert.AreEqual(true, refe.Multiple);
        refe.Set(LinkMultiple, false);
        Assert.AreEqual(false, refe.Multiple);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(LinkMultiple, null));
    }

    [TestMethod]
    public void Reference_Set_Type()
    {
        refe.Set(LinkType, ann);
        Assert.AreEqual(ann, refe.Type);
        Assert.ThrowsException<InvalidValueException>(() => refe.Set(LinkType, lang));
        Assert.AreEqual(ann, refe.Type);
    }

    [TestMethod]
    public void Reference_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => refe.Set(LanguageVersion, "asdf"));
    }

    [TestMethod]
    public void Enumeration_Meta()
    {
        Assert.AreEqual(_m3.Enumeration, enm.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey, EnumerationLiterals },
            enm.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Enumeration_Get()
    {
        Assert.AreEqual(enm.Name, enm.Get(INamedName));
        Assert.AreEqual(enm.Key, enm.Get(IKeyedKey));
        CollectionAssert.AreEqual(enm.Literals.ToList(),
            (enm.Get(EnumerationLiterals) as IReadOnlyList<EnumerationLiteral>).ToList());
        Assert.ThrowsException<UnknownFeatureException>(() => enm.Get(LanguageVersion));
    }

    [TestMethod]
    public void Enumeration_Set_Name()
    {
        enm.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", enm.Name);
        Assert.ThrowsException<InvalidValueException>(() => enm.Set(INamedName, null));
        Assert.AreEqual("Hello", enm.Name);
        Assert.ThrowsException<InvalidValueException>(() => enm.Set(INamedName, 123));
    }

    [TestMethod]
    public void Enumeration_Set_Key()
    {
        var enumeration = enm;
        enumeration.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", enumeration.Key);
        Assert.ThrowsException<InvalidValueException>(() => enumeration.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", enumeration.Key);
        Assert.ThrowsException<InvalidValueException>(() => enumeration.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void Enumeration_Set_Literals()
    {
        EnumerationLiteral litA = new DynamicEnumerationLiteral("my-id", enm) { Key = "my-key", Name = "SomeName" };
        EnumerationLiteral litB = new DynamicEnumerationLiteral("ref-id", enm) { Key = "ref-key", Name = "SomeRef" };
        enm.Set(EnumerationLiterals, new List<EnumerationLiteral> { litA, litB });
        CollectionAssert.AreEqual(new List<object> { litA, litB }, enm.Literals.ToList());
        enm.Set(EnumerationLiterals, Enumerable.Empty<EnumerationLiteral>());
        CollectionAssert.AreEqual(Enumerable.Empty<EnumerationLiteral>().ToList(), enm.Literals.ToList());
        Assert.ThrowsException<InvalidValueException>(() => enm.Set(EnumerationLiterals, null));
    }

    [TestMethod]
    public void Enumeration_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => enm.Set(LanguageVersion, "asdf"));
    }

    [TestMethod]
    public void EnumerationLiteral_Meta()
    {
        Assert.AreEqual(_m3.EnumerationLiteral, enLit.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey },
            enLit.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void EnumerationLiteral_Get()
    {
        Assert.AreEqual(enLit.Name, enLit.Get(INamedName));
        Assert.AreEqual(enLit.Key, enLit.Get(IKeyedKey));
        Assert.ThrowsException<UnknownFeatureException>(() => enLit.Get(LanguageVersion));
    }

    [TestMethod]
    public void EnumerationLiteral_Set_Name()
    {
        enLit.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", enLit.Name);
        Assert.ThrowsException<InvalidValueException>(() => enLit.Set(INamedName, null));
        Assert.AreEqual("Hello", enLit.Name);
        Assert.ThrowsException<InvalidValueException>(() => enLit.Set(INamedName, 123));
    }

    [TestMethod]
    public void EnumerationLiteral_Set_Key()
    {
        var literal = enLit;
        literal.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", literal.Key);
        Assert.ThrowsException<InvalidValueException>(() => literal.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", literal.Key);
        Assert.ThrowsException<InvalidValueException>(() => literal.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void EnumerationLiteral_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => enLit.Set(LanguageVersion, "asdf"));
    }

    [TestMethod]
    public void PrimitiveType_Meta()
    {
        Assert.AreEqual(_m3.PrimitiveType, prim.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey },
            prim.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void PrimitiveType_Get()
    {
        Assert.AreEqual(prim.Name, prim.Get(INamedName));
        Assert.AreEqual(prim.Key, prim.Get(IKeyedKey));
        Assert.ThrowsException<UnknownFeatureException>(() => prim.Get(LanguageVersion));
    }

    [TestMethod]
    public void PrimitiveType_Set_Name()
    {
        prim.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", prim.Name);
        Assert.ThrowsException<InvalidValueException>(() => prim.Set(INamedName, null));
        Assert.AreEqual("Hello", prim.Name);
        Assert.ThrowsException<InvalidValueException>(() => prim.Set(INamedName, 123));
    }

    [TestMethod]
    public void PrimitiveType_Set_Key()
    {
        var primitive = prim;
        primitive.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", primitive.Key);
        Assert.ThrowsException<InvalidValueException>(() => primitive.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", primitive.Key);
        Assert.ThrowsException<InvalidValueException>(() => primitive.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void PrimitiveType_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => prim.Set(LanguageVersion, "asdf"));
    }

    [TestMethod]
    public void Interface_Meta()
    {
        Assert.AreEqual(_m3.Interface, iface.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey, ClassifierFeatures, InterfaceExtends },
            iface.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Interface_Get()
    {
        Assert.AreEqual(iface.Name, iface.Get(INamedName));
        Assert.AreEqual(iface.Key, iface.Get(IKeyedKey));
        CollectionAssert.AreEqual(iface.Features.ToList(),
            (iface.Get(ClassifierFeatures) as IReadOnlyList<Feature>).ToList());
        CollectionAssert.AreEqual(iface.Extends.ToList(),
            (iface.Get(InterfaceExtends) as IReadOnlyList<Interface>).ToList());
        Assert.ThrowsException<UnknownFeatureException>(() => iface.Get(LanguageVersion));
    }

    [TestMethod]
    public void Interface_Set_Name()
    {
        iface.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", iface.Name);
        Assert.ThrowsException<InvalidValueException>(() => iface.Set(INamedName, null));
        Assert.AreEqual("Hello", iface.Name);
        Assert.ThrowsException<InvalidValueException>(() => iface.Set(INamedName, 123));
    }

    [TestMethod]
    public void Interface_Set_Key()
    {
        var ifac = iface;
        ifac.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", ifac.Key);
        Assert.ThrowsException<InvalidValueException>(() => ifac.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", ifac.Key);
        Assert.ThrowsException<InvalidValueException>(() => ifac.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void Interface_Set_Features()
    {
        Property propA = new DynamicProperty("my-id", ann) { Key = "my-key", Name = "SomeName" };
        Reference refB = new DynamicReference("ref-id", ann) { Key = "ref-key", Name = "SomeRef" };
        iface.Set(ClassifierFeatures, new List<Feature> { propA, refB });
        CollectionAssert.AreEqual(new List<object> { propA, refB }, iface.Features.ToList());
        iface.Set(ClassifierFeatures, Enumerable.Empty<Feature>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), iface.Features.ToList());
        Assert.ThrowsException<InvalidValueException>(() => iface.Set(ClassifierFeatures, null));
    }

    [TestMethod]
    public void Interface_Set_Extends()
    {
        Interface ifaceA = new DynamicInterface("my-id", lang) { Key = "my-key", Name = "SomeName" };
        Interface ifaceB = new DynamicInterface("ref-id", lang) { Key = "ref-key", Name = "SomeRef" };
        iface.Set(InterfaceExtends, new List<Interface> { ifaceA, ifaceB });
        CollectionAssert.AreEqual(new List<object> { ifaceA, ifaceB }, iface.Extends.ToList());
        iface.Set(InterfaceExtends, Enumerable.Empty<Interface>());
        CollectionAssert.AreEqual(Enumerable.Empty<Interface>().ToList(), iface.Extends.ToList());
        Assert.ThrowsException<InvalidValueException>(() => iface.Set(InterfaceExtends, null));
    }

    [TestMethod]
    public void Interface_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => iface.Set(LanguageVersion, "asdf"));
    }
}