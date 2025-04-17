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

namespace LionWeb.Core.Test;

using Languages;
using M2;
using M3;

[TestClass]
public class M2ReflectionTests
{
    #region Helpers

    private static readonly LionWebVersions _lionWebVersion = LionWebVersions.v2024_1;

    private static readonly ILionCoreLanguageWithStructuredDataType _m3 =
        (ILionCoreLanguageWithStructuredDataType)_lionWebVersion.LionCore;

    private static readonly IBuiltInsLanguage _builtIns = _lionWebVersion.BuiltIns;

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

    private static Feature Fields = _m3.StructuredDataType_fields;

    private static Feature FieldType = _m3.Field_type;

    DynamicLanguage shapesLang;
    DynamicLanguage enumLang;
    DynamicLanguage sdtLang;

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

    DynamicStructuredDataType sdt
    {
        get => enumLang.Entities.OfType<StructuredDataType>().First() as DynamicStructuredDataType;
    }

    DynamicField fld { get => sdt.Fields.First() as DynamicField; }


    [TestInitialize]
    public void InitLanguages()
    {
        shapesLang = ShapesDynamic.GetLanguage(_lionWebVersion);
        enumLang = LanguagesUtils
            .LoadLanguages("LionWeb.Core.Test", "LionWeb.Core.Test.Languages.defChunks.with-enum.json").First();
        enumLang = LanguagesUtils
            .LoadLanguages("LionWeb.Core.Test", "LionWeb.Core.Test.Languages.defChunks.sdtLang.json").First();
    }

    #endregion

    #region Language

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
    public void Language_TryGet_Name()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Language_TryGet_Key()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Language_TryGet_Version()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(node.TryGetVersion(out _));

        node.Version = "Hello";
        Assert.IsTrue(node.TryGetVersion(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Language_Set_Entities()
    {
        Concept conceptA = new DynamicConcept("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        Enumeration enumA = new DynamicEnumeration("enum-id", _lionWebVersion, lang) { Key = "enum-key", Name = "SomeEnum" };
        lang.Set(LanguageEntities, new List<LanguageEntity> { conceptA, enumA });
        CollectionAssert.AreEqual(new List<object> { conceptA, enumA }, lang.Entities.ToList());
        lang.Set(LanguageEntities, Enumerable.Empty<LanguageEntity>());
        CollectionAssert.AreEqual(Enumerable.Empty<LanguageEntity>().ToList(), lang.Entities.ToList());
        Assert.ThrowsException<InvalidValueException>(() => lang.Set(LanguageEntities, null));
    }

    [TestMethod]
    public void Language_TryGet_Entities()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(((Language)node).TryGetEntities(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddEntities([new DynamicConcept("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Language)node).TryGetEntities(out var value));
        Assert.IsFalse(value.Count == 0);
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
    public void Language_TryGet_DependsOn()
    {
        var node = new DynamicLanguage("a", _lionWebVersion);
        Assert.IsFalse(((Language)node).TryGetDependsOn(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddDependsOn([node]);
        Assert.IsTrue(((Language)node).TryGetDependsOn(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Language_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => lang.Set(AnnotationExtends, null));
    }

    #endregion

    #region Annotation

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
    public void Annotation_TryGet_Name()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Annotation_TryGet_Key()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Annotation_Set_Features()
    {
        Property propA = new DynamicProperty("my-id", _lionWebVersion, ann) { Key = "my-key", Name = "SomeName" };
        Reference refB = new DynamicReference("ref-id", _lionWebVersion, ann) { Key = "ref-key", Name = "SomeRef" };
        ann.Set(ClassifierFeatures, new List<Feature> { propA, refB });
        CollectionAssert.AreEqual(new List<object> { propA, refB }, ann.Features.ToList());
        ann.Set(ClassifierFeatures, Enumerable.Empty<Feature>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), ann.Features.ToList());
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(ClassifierFeatures, null));
    }

    [TestMethod]
    public void Annotation_TryGet_Features()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, lang);
        Assert.IsFalse(((Annotation)node).TryGetFeatures(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddFeatures([new DynamicProperty("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Annotation)node).TryGetFeatures(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Annotation_Set_Annotates()
    {
        Annotation tgt = new DynamicAnnotation("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        ann.Set(AnnotationAnnotates, tgt);
        Assert.AreEqual(tgt, ann.Annotates);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(AnnotationAnnotates, null));
        Assert.AreEqual(tgt, ann.Annotates);
    }

    [TestMethod]
    public void Annotation_TryGet_Annotates()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, lang);
        Assert.IsTrue(((Annotation)node).TryGetAnnotates(out var empty));
        Assert.AreEqual(_builtIns.Node, empty);

        node.Annotates = _builtIns.INamed;
        Assert.IsTrue(((Annotation)node).TryGetAnnotates(out var value));
        Assert.AreEqual(_builtIns.INamed, value);
    }

    [TestMethod]
    public void Annotation_Set_Extends()
    {
        Annotation sup = new DynamicAnnotation("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        ann.Set(AnnotationExtends, sup);
        Assert.AreEqual(sup, ann.Extends);
        ann.Set(AnnotationExtends, null);
        Assert.AreEqual(null, ann.Extends);
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(AnnotationExtends, lang));
    }

    [TestMethod]
    public void Annotation_TryGet_Extends()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, lang);
        Assert.IsFalse(((Annotation)node).TryGetExtends(out var empty));
        Assert.IsNull(empty);

        node.Extends = node;
        Assert.IsTrue(((Annotation)node).TryGetExtends(out var value));
        Assert.AreEqual(node, value);
    }

    [TestMethod]
    public void Annotation_Set_Implements()
    {
        Interface ifaceA = new DynamicInterface("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        Interface ifaceB = new DynamicInterface("ref-id", _lionWebVersion, lang) { Key = "ref-key", Name = "SomeRef" };
        ann.Set(AnnotationImplements, new List<Interface> { ifaceA, ifaceB });
        CollectionAssert.AreEqual(new List<object> { ifaceA, ifaceB }, ann.Implements.ToList());
        ann.Set(AnnotationImplements, Enumerable.Empty<Interface>());
        CollectionAssert.AreEqual(Enumerable.Empty<Interface>().ToList(), ann.Implements.ToList());
        Assert.ThrowsException<InvalidValueException>(() => ann.Set(AnnotationImplements, null));
    }

    [TestMethod]
    public void Annotation_TryGet_Implements()
    {
        var node = new DynamicAnnotation("a", _lionWebVersion, lang);
        Assert.IsFalse(((Annotation)node).TryGetImplements(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddImplements([new DynamicInterface("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Annotation)node).TryGetImplements(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Annotation_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => ann.Set(LanguageVersion, "asdf"));
    }

    #endregion

    #region Concept

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
    public void Concept_TryGet_Name()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Concept_TryGet_Key()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Concept_Set_Features()
    {
        Property propA = new DynamicProperty("my-id", _lionWebVersion, ann) { Key = "my-key", Name = "SomeName" };
        Reference refB = new DynamicReference("ref-id", _lionWebVersion, ann) { Key = "ref-key", Name = "SomeRef" };
        conc.Set(ClassifierFeatures, new List<Feature> { propA, refB });
        CollectionAssert.AreEqual(new List<object> { propA, refB }, conc.Features.ToList());
        conc.Set(ClassifierFeatures, Enumerable.Empty<Feature>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), conc.Features.ToList());
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(ClassifierFeatures, null));
    }

    [TestMethod]
    public void Concept_TryGet_Features()
    {
        var node = new DynamicConcept("a", _lionWebVersion, lang);
        Assert.IsFalse(((Concept)node).TryGetFeatures(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddFeatures([new DynamicProperty("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Concept)node).TryGetFeatures(out var value));
        Assert.IsFalse(value.Count == 0);
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
    public void Concept_TryGet_Abstract()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
        Assert.IsTrue(((Concept)node).TryGetAbstract(out var empty));
        Assert.AreEqual(false, empty);

        node.Abstract = true;
        Assert.IsTrue(((Concept)node).TryGetAbstract(out var value));
        Assert.AreEqual(true, value);
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
    public void Concept_TryGet_Partition()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
        Assert.IsTrue(((Concept)node).TryGetPartition(out var empty));
        Assert.AreEqual(false, empty);

        node.Partition = true;
        Assert.IsTrue(((Concept)node).TryGetPartition(out var value));
        Assert.AreEqual(true, value);
    }

    [TestMethod]
    public void Concept_Set_Extends()
    {
        Concept sup = new DynamicConcept("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        conc.Set(ConceptExtends, sup);
        Assert.AreEqual(sup, conc.Extends);
        conc.Set(ConceptExtends, null);
        Assert.AreEqual(null, conc.Extends);
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(ConceptExtends, lang));
    }

    [TestMethod]
    public void Concept_TryGet_Extends()
    {
        var node = new DynamicConcept("a", _lionWebVersion, null);
        Assert.IsFalse(((Concept)node).TryGetExtends(out var empty));
        Assert.AreEqual(null, empty);

        node.Extends = node;
        Assert.IsTrue(((Concept)node).TryGetExtends(out var value));
        Assert.AreEqual(node, value);
    }

    [TestMethod]
    public void Concept_Set_Implements()
    {
        Interface ifaceA = new DynamicInterface("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        Interface ifaceB = new DynamicInterface("ref-id", _lionWebVersion, lang) { Key = "ref-key", Name = "SomeRef" };
        conc.Set(ConceptImplements, new List<Interface> { ifaceA, ifaceB });
        CollectionAssert.AreEqual(new List<object> { ifaceA, ifaceB }, conc.Implements.ToList());
        conc.Set(ConceptImplements, Enumerable.Empty<Interface>());
        CollectionAssert.AreEqual(Enumerable.Empty<Interface>().ToList(), conc.Implements.ToList());
        Assert.ThrowsException<InvalidValueException>(() => conc.Set(ConceptImplements, null));
    }

    [TestMethod]
    public void Concept_TryGet_Implements()
    {
        var node = new DynamicConcept("a", _lionWebVersion, lang);
        Assert.IsFalse(((Concept)node).TryGetImplements(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddImplements([new DynamicInterface("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Concept)node).TryGetImplements(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Concept_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => conc.Set(LanguageVersion, "asdf"));
    }

    #endregion

    #region Containment

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
    public void Containment_TryGet_Name()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Containment_TryGet_Key()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Containment_TryGet_Optional()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsTrue(((Containment)node).TryGetOptional(out var empty));
        Assert.AreEqual(false, empty);

        node.Optional = true;
        Assert.IsTrue(((Containment)node).TryGetOptional(out var value));
        Assert.AreEqual(true, value);
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
    public void Containment_TryGet_Multiple()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsTrue(((Containment)node).TryGetMultiple(out var empty));
        Assert.AreEqual(false, empty);

        node.Multiple = true;
        Assert.IsTrue(((Containment)node).TryGetMultiple(out var value));
        Assert.AreEqual(true, value);
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
    public void Containment_TryGet_Type()
    {
        var node = new DynamicContainment("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetType(out _));

        var type = new DynamicConcept("b", _lionWebVersion, null);
        node.Type = type;
        Assert.IsTrue(node.TryGetType(out var value));
        Assert.AreEqual(type, value);
    }

    [TestMethod]
    public void Containment_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => cont.Set(LanguageVersion, "asdf"));
    }

    #endregion

    #region Reference

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
    public void Reference_TryGet_Name()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Reference_TryGet_Key()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Reference_TryGet_Optional()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsTrue(((Reference)node).TryGetOptional(out var empty));
        Assert.AreEqual(false, empty);

        node.Optional = true;
        Assert.IsTrue(((Reference)node).TryGetOptional(out var value));
        Assert.AreEqual(true, value);
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
    public void Reference_TryGet_Multiple()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsTrue(((Reference)node).TryGetMultiple(out var empty));
        Assert.AreEqual(false, empty);

        node.Multiple = true;
        Assert.IsTrue(((Reference)node).TryGetMultiple(out var value));
        Assert.AreEqual(true, value);
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
    public void Reference_TryGet_Type()
    {
        var node = new DynamicReference("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetType(out _));

        var type = new DynamicConcept("b", _lionWebVersion, null);
        node.Type = type;
        Assert.IsTrue(node.TryGetType(out var value));
        Assert.AreEqual(type, value);
    }

    [TestMethod]
    public void Reference_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => refe.Set(LanguageVersion, "asdf"));
    }

    #endregion

    #region Enumeration

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
    public void Enumeration_TryGet_Name()
    {
        var node = new DynamicEnumeration("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Enumeration_TryGet_Key()
    {
        var node = new DynamicEnumeration("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Enumeration_Set_Literals()
    {
        EnumerationLiteral litA = new DynamicEnumerationLiteral("my-id", _lionWebVersion, enm) { Key = "my-key", Name = "SomeName" };
        EnumerationLiteral litB = new DynamicEnumerationLiteral("ref-id", _lionWebVersion, enm) { Key = "ref-key", Name = "SomeRef" };
        enm.Set(EnumerationLiterals, new List<EnumerationLiteral> { litA, litB });
        CollectionAssert.AreEqual(new List<object> { litA, litB }, enm.Literals.ToList());
        enm.Set(EnumerationLiterals, Enumerable.Empty<EnumerationLiteral>());
        CollectionAssert.AreEqual(Enumerable.Empty<EnumerationLiteral>().ToList(), enm.Literals.ToList());
        Assert.ThrowsException<InvalidValueException>(() => enm.Set(EnumerationLiterals, null));
    }

    [TestMethod]
    public void Enumeration_TryGet_Literals()
    {
        var node = new DynamicEnumeration("a", _lionWebVersion, lang);
        Assert.IsFalse(((Enumeration)node).TryGetLiterals(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddLiterals([new DynamicEnumerationLiteral("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Enumeration)node).TryGetLiterals(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Enumeration_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => enm.Set(LanguageVersion, "asdf"));
    }

    #endregion

    #region EnumerationLiteral

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
    public void EnumerationLiteral_TryGet_Name()
    {
        var node = new DynamicEnumerationLiteral("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
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
    public void EnumerationLiteral_TryGet_Key()
    {
        var node = new DynamicEnumerationLiteral("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void EnumerationLiteral_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => enLit.Set(LanguageVersion, "asdf"));
    }

    #endregion

    #region PrimitiveType

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
    public void PrimitiveType_TryGet_Name()
    {
        var node = new DynamicPrimitiveType("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
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
    public void PrimitiveType_TryGet_Key()
    {
        var node = new DynamicPrimitiveType("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void PrimitiveType_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => prim.Set(LanguageVersion, "asdf"));
    }

    #endregion

    #region Interface

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
    public void Interface_TryGet_Name()
    {
        var node = new DynamicInterface("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
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
    public void Interface_TryGet_Key()
    {
        var node = new DynamicInterface("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Interface_Set_Features()
    {
        Property propA = new DynamicProperty("my-id", _lionWebVersion, ann) { Key = "my-key", Name = "SomeName" };
        Reference refB = new DynamicReference("ref-id", _lionWebVersion, ann) { Key = "ref-key", Name = "SomeRef" };
        iface.Set(ClassifierFeatures, new List<Feature> { propA, refB });
        CollectionAssert.AreEqual(new List<object> { propA, refB }, iface.Features.ToList());
        iface.Set(ClassifierFeatures, Enumerable.Empty<Feature>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), iface.Features.ToList());
        Assert.ThrowsException<InvalidValueException>(() => iface.Set(ClassifierFeatures, null));
    }

    [TestMethod]
    public void Interface_TryGet_Features()
    {
        var node = new DynamicInterface("a", _lionWebVersion, lang);
        Assert.IsFalse(((Interface)node).TryGetFeatures(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddFeatures([new DynamicProperty("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Interface)node).TryGetFeatures(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Interface_Set_Extends()
    {
        Interface ifaceA = new DynamicInterface("my-id", _lionWebVersion, lang) { Key = "my-key", Name = "SomeName" };
        Interface ifaceB = new DynamicInterface("ref-id", _lionWebVersion, lang) { Key = "ref-key", Name = "SomeRef" };
        iface.Set(InterfaceExtends, new List<Interface> { ifaceA, ifaceB });
        CollectionAssert.AreEqual(new List<object> { ifaceA, ifaceB }, iface.Extends.ToList());
        iface.Set(InterfaceExtends, Enumerable.Empty<Interface>());
        CollectionAssert.AreEqual(Enumerable.Empty<Interface>().ToList(), iface.Extends.ToList());
        Assert.ThrowsException<InvalidValueException>(() => iface.Set(InterfaceExtends, null));
    }

    [TestMethod]
    public void Interface_TryGet_Extends()
    {
        var node = new DynamicInterface("a", _lionWebVersion, lang);
        Assert.IsFalse(((Interface)node).TryGetExtends(out var empty));
        Assert.IsTrue(empty?.Count == 0);

        node.AddExtends([new DynamicInterface("b", _lionWebVersion, null)]);
        Assert.IsTrue(((Interface)node).TryGetExtends(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void Interface_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => iface.Set(LanguageVersion, "asdf"));
    }

    #endregion

    #region StructuredDataType

    [TestMethod]
    public void StructuredDataType_Meta()
    {
        Assert.AreEqual(_m3.StructuredDataType, sdt.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey, Fields },
            sdt.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void StructuredDataType_Get()
    {
        Assert.AreEqual(sdt.Name, sdt.Get(INamedName));
        Assert.AreEqual(sdt.Key, sdt.Get(IKeyedKey));
        CollectionAssert.AreEqual(sdt.Fields.ToList(),
            (sdt.Get(Fields) as IReadOnlyList<Field>).ToList());
        Assert.ThrowsException<UnknownFeatureException>(() => sdt.Get(LanguageVersion));
    }

    [TestMethod]
    public void StructuredDataType_Set_Name()
    {
        sdt.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", sdt.Name);
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(INamedName, null));
        Assert.AreEqual("Hello", sdt.Name);
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(INamedName, 123));
    }

    [TestMethod]
    public void StructuredDataType_TryGet_Name()
    {
        var node = new DynamicStructuredDataType("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void StructuredDataType_Set_Key()
    {
        sdt.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", sdt.Key);
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", sdt.Key);
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void StructuredDataType_TryGet_Key()
    {
        var node = new DynamicStructuredDataType("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void StructuredDataType_Set_Fields()
    {
        Field litA = new DynamicField("my-id", _lionWebVersion, sdt) { Key = "my-key", Name = "SomeName" };
        Field litB = new DynamicField("ref-id", _lionWebVersion, sdt) { Key = "ref-key", Name = "SomeRef" };
        sdt.Set(Fields, new List<Field> { litA, litB });
        CollectionAssert.AreEqual(new List<object> { litA, litB }, sdt.Fields.ToList());
        sdt.Set(Fields, Enumerable.Empty<Field>());
        CollectionAssert.AreEqual(Enumerable.Empty<Feature>().ToList(), sdt.Fields.ToList());
        Assert.ThrowsException<InvalidValueException>(() => sdt.Set(Fields, null));
    }

    [TestMethod]
    public void StructuredDataType_TryGet_Fields()
    {
        var node = new DynamicStructuredDataType("a", _lionWebVersion, lang);
        Assert.IsFalse(((StructuredDataType)node).TryGetFields(out var empty));
        Assert.IsTrue(empty.Count == 0);

        node.AddFields([new DynamicField("b", _lionWebVersion, null)]);
        Assert.IsTrue(((StructuredDataType)node).TryGetFields(out var value));
        Assert.IsFalse(value.Count == 0);
    }

    [TestMethod]
    public void StructuredDataType_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => sdt.Set(LanguageVersion, "asdf"));
    }

    #endregion

    #region Field

    [TestMethod]
    public void Field_Meta()
    {
        Assert.AreEqual(_m3.Field, fld.GetClassifier());

        CollectionAssert.AreEqual(new List<Feature> { INamedName, IKeyedKey, FieldType },
            fld.CollectAllSetFeatures().ToList());
    }

    [TestMethod]
    public void Field_Get()
    {
        Assert.AreEqual(fld.Name, fld.Get(INamedName));
        Assert.AreEqual(fld.Key, fld.Get(IKeyedKey));
        Assert.ThrowsException<UnknownFeatureException>(() => fld.Get(LanguageVersion));
    }

    [TestMethod]
    public void Field_Set_Name()
    {
        fld.Set(INamedName, "Hello");
        Assert.AreEqual("Hello", fld.Name);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(INamedName, null));
        Assert.AreEqual("Hello", fld.Name);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(INamedName, 123));
    }

    [TestMethod]
    public void Field_TryGet_Name()
    {
        var node = new DynamicField("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetName(out _));

        node.Name = "Hello";
        Assert.IsTrue(node.TryGetName(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Field_Set_Key()
    {
        fld.Set(IKeyedKey, "Hello");
        Assert.AreEqual("Hello", fld.Key);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(IKeyedKey, null));
        Assert.AreEqual("Hello", fld.Key);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(IKeyedKey, 123));
    }

    [TestMethod]
    public void Field_TryGet_Key()
    {
        var node = new DynamicField("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetKey(out _));

        node.Key = "Hello";
        Assert.IsTrue(node.TryGetKey(out var value));
        Assert.AreEqual("Hello", value);
    }

    [TestMethod]
    public void Field_Set_Invalid()
    {
        Assert.ThrowsException<UnknownFeatureException>(() => fld.Set(LanguageVersion, "asdf"));
    }

    [TestMethod]
    public void Field_Set_Type()
    {
        fld.Set(FieldType, prim);
        Assert.AreEqual(prim, fld.Type);
        Assert.ThrowsException<InvalidValueException>(() => fld.Set(FieldType, lang));
        Assert.AreEqual(prim, fld.Type);
    }

    [TestMethod]
    public void Field_TryGet_Type()
    {
        var node = new DynamicField("a", _lionWebVersion, null);
        Assert.IsFalse(node.TryGetType(out _));

        var type = new DynamicPrimitiveType("b", _lionWebVersion, null);
        node.Type = type;
        Assert.IsTrue(node.TryGetType(out var value));
        Assert.AreEqual(type, value);
    }

    #endregion
}