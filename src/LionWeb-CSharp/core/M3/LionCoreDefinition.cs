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

// Generated by C# language instantiator generator - modify at your own risk!

// ReSharper disable InconsistentNaming

namespace LionWeb.Core.M3;

using M2;

/// <summary>
/// Contains the self-definition of the LionCore M3.
/// </summary>
public sealed class M3Language : LanguageBase<M3Factory>
{
    /// <summary>
    /// Self-definition of the LionCore M3.
    /// </summary>
    public static readonly M3Language Instance = new Lazy<M3Language>(() => new(
        "-id-LionCore-M3"
    )).Value;

    private const string _name = "LionCore_M3";
    private const string _key = "LionCore-M3";

    internal M3Language(string id) : base(id)
    {
        _annotation = new(() => new M3Concept("-id-Annotation", this)
        {
            Key = "Annotation",
            Name = "Annotation",
            Abstract = false,
            Partition = false,
            ExtendsLazy = new(() => Classifier),
            FeaturesLazy = new(() =>
            [
                Annotation_annotates,
                Annotation_extends,
                Annotation_implements
            ])
        });
        _annotation_annotates = new(() =>
            new M3Reference("-id-Annotation-annotates", Annotation, this)
            {
                Key = "Annotation-annotates",
                Name = "annotates",
                Multiple = false,
                Optional = true,
                Type = Classifier
            });
        _annotation_extends = new(() => new M3Reference("-id-Annotation-extends", Annotation, this)
        {
            Key = "Annotation-extends",
            Name = "extends",
            Multiple = false,
            Optional = true,
            Type = Annotation
        });
        _annotation_implements = new(() => new M3Reference("-id-Annotation-implements", Annotation, this)
        {
            Key = "Annotation-implements",
            Name = "implements",
            Multiple = true,
            Optional = true,
            Type = Interface
        });

        _classifier = new(() => new M3Concept("-id-Classifier", this)
        {
            Key = "Classifier",
            Name = "Classifier",
            Abstract = true,
            Partition = false,
            ExtendsLazy = new(() => LanguageEntity),
            FeaturesLazy = new(() => [Classifier_features])
        });
        _classifier_features = new(() => new M3Containment("-id-Classifier-features", Classifier, this)
        {
            Key = "Classifier-features",
            Name = "features",
            Multiple = true,
            Optional = true,
            Type = Feature
        });

        _concept = new(() => new M3Concept("-id-Concept", this)
        {
            Key = "Concept",
            Name = "Concept",
            Abstract = false,
            Partition = false,
            ExtendsLazy = new(() => Classifier),
            FeaturesLazy = new(() =>
            [
                Concept_abstract,
                Concept_partition,
                Concept_extends,
                Concept_implements
            ])
        });
        _concept_abstract = new(() => new M3Property("-id-Concept-abstract", Concept, this)
        {
            Key = "Concept-abstract", Name = "abstract", Optional = false, Type = BuiltInsLanguage.Instance.Boolean
        });
        _concept_partition = new(() => new M3Property("-id-Concept-partition", Concept, this)
        {
            Key = "Concept-partition",
            Name = "partition",
            Optional = false,
            Type = BuiltInsLanguage.Instance.Boolean
        });
        _concept_extends = new(() =>
            new M3Reference("-id-Concept-extends", Concept, this)
            {
                Key = "Concept-extends",
                Name = "extends",
                Multiple = false,
                Optional = true,
                Type = Concept
            });
        _concept_implements = new(() =>
            new M3Reference("-id-Concept-implements", Concept, this)
            {
                Key = "Concept-implements",
                Name = "implements",
                Multiple = true,
                Optional = true,
                Type = Interface
            });

        _containment = new(() => new M3Concept("-id-Containment", this)
        {
            Key = "Containment",
            Name = "Containment",
            Abstract = false,
            Partition = false,
            ExtendsLazy = new(() => Link)
        });

        _dataType = new(() => new M3Concept("-id-DataType", this)
        {
            Key = "DataType",
            Name = "DataType",
            Abstract = true,
            Partition = false,
            ExtendsLazy = new(() => LanguageEntity)
        });

        _enumeration = new(() => new M3Concept("-id-Enumeration", this)
        {
            Key = "Enumeration",
            Name = "Enumeration",
            Abstract = false,
            Partition = false,
            ExtendsLazy = new(() => DataType),
            FeaturesLazy = new(() => [Enumeration_literals])
        });
        _enumeration_literals = new(() => new M3Containment("-id-Enumeration-literals", Enumeration, this)
        {
            Key = "Enumeration-literals",
            Name = "literals",
            Multiple = true,
            Optional = true,
            Type = EnumerationLiteral
        });

        _enumerationLiteral = new(() => new M3Concept("-id-EnumerationLiteral", this)
        {
            Key = "EnumerationLiteral",
            Name = "EnumerationLiteral",
            Abstract = false,
            Partition = false,
            ImplementsLazy = new(() => [IKeyed])
        });

        _feature = new(() => new M3Concept("-id-Feature", this)
        {
            Key = "Feature",
            Name = "Feature",
            Abstract = true,
            Partition = false,
            ImplementsLazy = new(() => [IKeyed]),
            FeaturesLazy = new(() => [Feature_optional])
        });
        _feature_optional = new(() => new M3Property("-id-Feature-optional", Feature, this)
        {
            Key = "Feature-optional", Name = "optional", Optional = false, Type = BuiltInsLanguage.Instance.Boolean
        });

        _field = new(() => new M3Concept("-id-Field", this)
        {
            Key = "Field",
            Name = "Field",
            Abstract = false,
            Partition = false,
            ImplementsLazy = new(() => [IKeyed]),
            FeaturesLazy = new(() => [Field_type])
        });
        _field_type = new(() =>
            new M3Reference("-id-Field-type", Field, this)
            {
                Key = "Field-type",
                Name = "type",
                Multiple = false,
                Optional = false,
                Type = DataType
            });

        _iKeyed = new(() =>
            new M3Interface("-id-IKeyed", this)
            {
                Key = "IKeyed",
                Name = "IKeyed",
                ExtendsLazy = new(() => [BuiltInsLanguage.Instance.INamed]),
                FeaturesLazy = new(() => [IKeyed_key])
            });
        _iKeyed_key = new(() => new M3Property("-id-IKeyed-key", IKeyed, this)
        {
            Key = "IKeyed-key", Name = "key", Optional = false, Type = BuiltInsLanguage.Instance.String
        });

        _interface = new(() => new M3Concept("-id-Interface", this)
        {
            Key = "Interface",
            Name = "Interface",
            Abstract = false,
            Partition = false,
            ExtendsLazy = new(() => Classifier),
            FeaturesLazy = new(() => [Interface_extends])
        });
        _interface_extends = new(() =>
            new M3Reference("-id-Interface-extends", Interface, this)
            {
                Key = "Interface-extends",
                Name = "extends",
                Multiple = true,
                Optional = true,
                Type = Interface
            });

        _language = new(() => new M3Concept("-id-Language", this)
        {
            Key = "Language",
            Name = "Language",
            Abstract = false,
            Partition = true,
            ImplementsLazy = new(() => [IKeyed]),
            FeaturesLazy = new(() =>
            [
                Language_version,
                Language_entities,
                Language_dependsOn
            ])
        });
        _language_version = new(() => new M3Property("-id-Language-version", Language, this)
        {
            Key = "Language-version", Name = "version", Optional = false, Type = BuiltInsLanguage.Instance.String
        });
        _language_entities = new(() => new M3Containment("-id-Language-entities", Language, this)
        {
            Key = "Language-entities",
            Name = "entities",
            Multiple = true,
            Optional = true,
            Type = LanguageEntity
        });
        _language_dependsOn = new(() =>
            new M3Reference("-id-Language-dependsOn", Language, this)
            {
                Key = "Language-dependsOn",
                Name = "dependsOn",
                Multiple = true,
                Optional = true,
                Type = Language
            });

        _languageEntity = new(() => new M3Concept("-id-LanguageEntity", this)
        {
            Key = "LanguageEntity",
            Name = "LanguageEntity",
            Abstract = true,
            Partition = false,
            ImplementsLazy = new(() => [IKeyed])
        });

        _link = new(() => new M3Concept("-id-Link", this)
        {
            Key = "Link",
            Name = "Link",
            Abstract = true,
            Partition = false,
            ExtendsLazy = new(() => Feature),
            FeaturesLazy = new(() =>
            [
                Link_multiple,
                Link_type
            ])
        });
        _link_multiple = new(() => new M3Property("-id-Link-multiple", Link, this)
        {
            Key = "Link-multiple", Name = "multiple", Optional = false, Type = BuiltInsLanguage.Instance.Boolean
        });
        _link_type = new(() => new M3Reference("-id-Link-type", Link, this)
        {
            Key = "Link-type",
            Name = "type",
            Multiple = false,
            Optional = false,
            Type = Classifier
        });

        _primitiveType = new(() => new M3Concept("-id-PrimitiveType", this)
        {
            Key = "PrimitiveType",
            Name = "PrimitiveType",
            Abstract = false,
            Partition = false,
            ExtendsLazy = new(() => DataType)
        });

        _property = new(() => new M3Concept("-id-Property", this)
        {
            Key = "Property",
            Name = "Property",
            Abstract = false,
            Partition = false,
            ExtendsLazy = new(() => Feature),
            FeaturesLazy = new(() => [Property_type])
        });
        _property_type = new(() =>
            new M3Reference("-id-Property-type", Property, this)
            {
                Key = "Property-type",
                Name = "type",
                Multiple = false,
                Optional = false,
                Type = DataType
            });

        _reference = new(() => new M3Concept("-id-Reference", this)
        {
            Key = "Reference",
            Name = "Reference",
            Abstract = false,
            Partition = false,
            ExtendsLazy = new(() => Link)
        });

        _structuredDataType = new(() => new M3Concept("-id-StructuredDataType", this)
        {
            Key = "StructuredDataType",
            Name = "StructuredDataType",
            Abstract = false,
            Partition = false,
            ExtendsLazy = new(() => DataType),
            FeaturesLazy = new(() => [StructuredDataType_fields])
        });
        _structuredDataType_fields = new(() => new M3Containment("-id-StructuredDataType-fields", Enumeration, this)
        {
            Key = "StructuredDataType-fields",
            Name = "fields",
            Multiple = true,
            Optional = true,
            Type = Field
        });
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => Concept;

    /// <inheritdoc />
    public override string Name => _name;

    /// <inheritdoc />
    public override string Key => _key;

    /// <inheritdoc />
    public override string Version => ReleaseVersion.Current;

    /// <inheritdoc />
    public override IReadOnlyList<LanguageEntity> Entities
    {
        get =>
        [
            Annotation,
            Classifier,
            Concept,
            Containment,
            DataType,
            Enumeration,
            EnumerationLiteral,
            Feature,
            Field,
            IKeyed,
            Interface,
            Language,
            LanguageEntity,
            Link,
            PrimitiveType,
            Property,
            Reference,
            StructuredDataType
        ];
    }

    /// <inheritdoc />
    public override IReadOnlyList<Language> DependsOn { get; } = [BuiltInsLanguage.Instance];

    /// <inheritdoc />
    public override M3Factory GetFactory() => new(this);

    private readonly Lazy<Concept> _annotation;

    /// <inheritdoc cref="M3.Annotation"/>
    public Concept Annotation => _annotation.Value;

    private readonly Lazy<Reference> _annotation_annotates;

    /// <inheritdoc cref="M3.Annotation.Annotates"/>
    public Reference Annotation_annotates => _annotation_annotates.Value;

    private readonly Lazy<Reference> _annotation_extends;

    /// <inheritdoc cref="M3.Annotation.Extends"/>
    public Reference Annotation_extends => _annotation_extends.Value;

    private readonly Lazy<Reference> _annotation_implements;

    /// <inheritdoc cref="M3.Annotation.Implements"/>
    public Reference Annotation_implements => _annotation_implements.Value;

    private readonly Lazy<Concept> _classifier;

    /// <inheritdoc cref="M3.Classifier"/>
    public Concept Classifier => _classifier.Value;

    private readonly Lazy<Containment> _classifier_features;

    /// <inheritdoc cref="M3.Classifier.Features"/>
    public Containment Classifier_features => _classifier_features.Value;

    private readonly Lazy<Concept> _concept;

    /// <inheritdoc cref="M3.Concept"/>
    public Concept Concept => _concept.Value;

    private readonly Lazy<Property> _concept_abstract;

    /// <inheritdoc cref="M3.Concept.Abstract"/>
    public Property Concept_abstract => _concept_abstract.Value;

    private readonly Lazy<Property> _concept_partition;

    /// <inheritdoc cref="M3.Concept.Partition"/>
    public Property Concept_partition => _concept_partition.Value;

    private readonly Lazy<Reference> _concept_extends;

    /// <inheritdoc cref="M3.Concept.Extends"/>
    public Reference Concept_extends => _concept_extends.Value;

    private readonly Lazy<Reference> _concept_implements;

    /// <inheritdoc cref="M3.Concept.Implements"/>
    public Reference Concept_implements => _concept_implements.Value;

    private readonly Lazy<Concept> _containment;

    /// <inheritdoc cref="M3.Containment"/>
    public Concept Containment => _containment.Value;

    private readonly Lazy<Concept> _dataType;

    /// <inheritdoc cref="M3.Datatype"/>
    public Concept DataType => _dataType.Value;

    private readonly Lazy<Concept> _enumeration;

    /// <inheritdoc cref="M3.Enumeration"/>
    public Concept Enumeration => _enumeration.Value;

    private readonly Lazy<Containment> _enumeration_literals;

    /// <inheritdoc cref="M3.Enumeration.Literals"/>
    public Containment Enumeration_literals => _enumeration_literals.Value;

    private readonly Lazy<Concept> _enumerationLiteral;

    /// <inheritdoc cref="M3.EnumerationLiteral"/>
    public Concept EnumerationLiteral => _enumerationLiteral.Value;

    private readonly Lazy<Concept> _feature;

    /// <inheritdoc cref="M3.Feature"/>
    public Concept Feature => _feature.Value;

    private readonly Lazy<Property> _feature_optional;

    /// <inheritdoc cref="M3.Feature.Optional"/>
    public Property Feature_optional => _feature_optional.Value;

    private readonly Lazy<Concept> _field;

    /// <inheritdoc cref="M3.Field"/>
    public Concept Field => _field.Value;

    private readonly Lazy<Reference> _field_type;

    /// <inheritdoc cref="M3.Field.Type"/>
    public Reference Field_type => _field_type.Value;

    private readonly Lazy<Interface> _iKeyed;

    /// <inheritdoc cref="M3.IKeyed"/>
    public Interface IKeyed => _iKeyed.Value;

    private readonly Lazy<Property> _iKeyed_key;

    /// <inheritdoc cref="M3.IKeyed.Key"/>
    public Property IKeyed_key => _iKeyed_key.Value;

    private readonly Lazy<Concept> _interface;

    /// <inheritdoc cref="M3.Interface"/>
    public Concept Interface => _interface.Value;

    private readonly Lazy<Reference> _interface_extends;

    /// <inheritdoc cref="M3.Interface.Extends"/>
    public Reference Interface_extends => _interface_extends.Value;

    private readonly Lazy<Concept> _language;

    /// <inheritdoc cref="M3.Language"/>
    public Concept Language => _language.Value;

    private readonly Lazy<Property> _language_version;

    /// <inheritdoc cref="M3.Language.Version"/>
    public Property Language_version => _language_version.Value;

    private readonly Lazy<Containment> _language_entities;

    /// <inheritdoc cref="M3.Language.Entities"/>
    public Containment Language_entities => _language_entities.Value;

    private readonly Lazy<Reference> _language_dependsOn;

    /// <inheritdoc cref="M3.Language.DependsOn"/>
    public Reference Language_dependsOn => _language_dependsOn.Value;

    private readonly Lazy<Concept> _languageEntity;

    /// <inheritdoc cref="M3.LanguageEntity"/>
    public Concept LanguageEntity => _languageEntity.Value;

    private readonly Lazy<Concept> _link;

    /// <inheritdoc cref="M3.Link"/>
    public Concept Link => _link.Value;

    private readonly Lazy<Property> _link_multiple;

    /// <inheritdoc cref="M3.Link.Multiple"/>
    public Property Link_multiple => _link_multiple.Value;

    private readonly Lazy<Reference> _link_type;

    /// <inheritdoc cref="M3.Link.Type"/>
    public Reference Link_type => _link_type.Value;

    private readonly Lazy<Concept> _primitiveType;

    /// <inheritdoc cref="M3.PrimitiveType"/>
    public Concept PrimitiveType => _primitiveType.Value;

    private readonly Lazy<Concept> _property;

    /// <inheritdoc cref="M3.Property"/>
    public Concept Property => _property.Value;

    private readonly Lazy<Reference> _property_type;

    /// <inheritdoc cref="M3.Property.Type"/>
    public Reference Property_type => _property_type.Value;

    private readonly Lazy<Concept> _reference;

    /// <inheritdoc cref="M3.Reference"/>
    public Concept Reference => _reference.Value;

    private readonly Lazy<Concept> _structuredDataType;

    /// <inheritdoc cref="M3.StructuredDataType"/>
    public Concept StructuredDataType => _structuredDataType.Value;

    private readonly Lazy<Containment> _structuredDataType_fields;

    /// <inheritdoc cref="M3.StructuredDataType"/>
    public Containment StructuredDataType_fields => _structuredDataType_fields.Value;
}

/// <inheritdoc />
public sealed class M3Concept : ConceptBase<M3Language>
{
    internal M3Concept(string id, M3Language parent) : base(id, parent)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        if (feature == BuiltInsLanguage.Instance.INamed_name)
            return Name;
        if (feature == GetLanguage().IKeyed_key)
            return Key;
        if (feature == GetLanguage().Classifier_features)
            return Features;
        if (feature == GetLanguage().Concept_abstract)
            return Abstract;
        if (feature == GetLanguage().Concept_partition)
            return Partition;
        if (feature == GetLanguage().Concept_extends)
            return Extends;
        if (feature == GetLanguage().Concept_implements)
            return Implements;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }
}

/// <inheritdoc />
public sealed class M3Interface : InterfaceBase<M3Language>
{
    internal M3Interface(string id, M3Language parent) : base(id, parent)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        if (feature == BuiltInsLanguage.Instance.INamed_name)
            return Name;
        if (feature == GetLanguage().IKeyed_key)
            return Key;
        if (feature == GetLanguage().Classifier_features)
            return Features;
        if (feature == GetLanguage().Interface_extends)
            return Extends;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }
}

/// <inheritdoc />
public sealed class M3Reference : ReferenceBase<M3Language>
{
    internal M3Reference(string id, Classifier parent, M3Language language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        if (feature == BuiltInsLanguage.Instance.INamed_name)
            return Name;
        if (feature == GetLanguage().IKeyed_key)
            return Key;
        if (feature == GetLanguage().Feature_optional)
            return Optional;
        if (feature == GetLanguage().Link_multiple)
            return Multiple;
        if (feature == GetLanguage().Link_type)
            return Type;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }
}

/// <inheritdoc />
public sealed class M3Containment : ContainmentBase<M3Language>
{
    internal M3Containment(string id, Classifier parent, M3Language language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        if (feature == BuiltInsLanguage.Instance.INamed_name)
            return Name;
        if (feature == GetLanguage().IKeyed_key)
            return Key;
        if (feature == GetLanguage().Feature_optional)
            return Optional;
        if (feature == GetLanguage().Link_multiple)
            return Multiple;
        if (feature == GetLanguage().Link_type)
            return Type;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }
}

/// <inheritdoc />
public sealed class M3Property : PropertyBase<M3Language>
{
    internal M3Property(string id, Classifier parent, M3Language language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        if (feature == BuiltInsLanguage.Instance.INamed_name)
            return Name;
        if (feature == GetLanguage().IKeyed_key)
            return Key;
        if (feature == GetLanguage().Feature_optional)
            return Optional;
        if (feature == GetLanguage().Property_type)
            return Type;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }
}

/// <inheritdoc />
public sealed class M3Factory : INodeFactory
{
    private readonly M3Language _language;

    internal M3Factory(M3Language language)
    {
        _language = language;
    }

    /// <inheritdoc cref="M3.Annotation"/>
    public DynamicAnnotation Annotation(string id) => new(id, null);

    /// <inheritdoc cref="M3.Concept"/>
    public DynamicConcept Concept(string id) => new(id, null);

    /// <inheritdoc cref="M3.Containment"/>
    public DynamicContainment Containment(string id) => new(id, null);

    /// <inheritdoc cref="M3.Enumeration"/>
    public DynamicEnumeration Enumeration(string id) => new(id, null);

    /// <inheritdoc cref="M3.EnumerationLiteral"/>
    public DynamicEnumerationLiteral EnumerationLiteral(string id) => new(id, null);

    /// <inheritdoc cref="M3.Field"/>
    public DynamicField Field(string id) => new(id, null);

    /// <inheritdoc cref="M3.Interface"/>
    public DynamicInterface Interface(string id) => new(id, null);

    /// <inheritdoc cref="M3.Language"/>
    public DynamicLanguage Language(string id) => new(id);

    /// <inheritdoc cref="M3.PrimitiveType"/>
    public DynamicPrimitiveType PrimitiveType(string id) => new(id, null);

    /// <inheritdoc cref="M3.Property"/>
    public DynamicProperty Property(string id) => new(id, null);

    /// <inheritdoc cref="M3.Reference"/>
    public DynamicReference Reference(string id) => new(id, null);

    /// <inheritdoc cref="M3.StructuredDataType"/>
    public DynamicStructuredDataType StructuredDataType(string id) => new(id, null);

    /// <inheritdoc />
    public INode CreateNode(string id, Classifier classifier)
    {
        if (classifier == _language.Annotation)
            return Annotation(id);
        if (classifier == _language.Concept)
            return Concept(id);
        if (classifier == _language.Containment)
            return Containment(id);
        if (classifier == _language.Enumeration)
            return Enumeration(id);
        if (classifier == _language.EnumerationLiteral)
            return EnumerationLiteral(id);
        if (classifier == _language.Field)
            return Field(id);
        if (classifier == _language.Interface)
            return Interface(id);
        if (classifier == _language.Language)
            return Language(id);
        if (classifier == _language.PrimitiveType)
            return PrimitiveType(id);
        if (classifier == _language.Property)
            return Property(id);
        if (classifier == _language.Reference)
            return Reference(id);
        if (classifier == _language.StructuredDataType)
            return StructuredDataType(id);

        throw new UnsupportedClassifierException(classifier);
    }

    /// <inheritdoc />
    public Enum GetEnumerationLiteral(EnumerationLiteral literal) =>
        throw new UnsupportedEnumerationLiteralException(literal);

    /// <inheritdoc />
    public IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType,
        IFieldValues fieldValues) =>
        throw new UnsupportedStructuredDatatypeException(structuredDataType);
}