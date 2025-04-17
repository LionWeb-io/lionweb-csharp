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

namespace LionWeb.Core.M3;

using M2;
using System.Diagnostics.CodeAnalysis;

/// <inheritdoc cref="Language"/>
public abstract class LanguageBase<TNodeFactory>(string id, LionWebVersions lionWebVersion)
    : ReadableNodeBase<IReadableNode>(id, null), Language<TNodeFactory>
    where TNodeFactory : INodeFactory
{
    /// This language's current factory.
    protected TNodeFactory _factory;

    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get; } = lionWebVersion;

    /// <inheritdoc />
    protected override IBuiltInsLanguage _builtIns => new Lazy<IBuiltInsLanguage>(() => LionWebVersion.BuiltIns).Value;

    /// <inheritdoc />
    protected override ILionCoreLanguage _m3 => new Lazy<ILionCoreLanguage>(() => LionWebVersion.LionCore).Value;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
    [
        _builtIns.INamed_name,
        _m3.IKeyed_key,
        _m3.Language_version,
        _m3.Language_entities,
        _m3.Language_dependsOn
    ];

    /// <inheritdoc cref="IConceptInstance.GetClassifier()" />
    public override Classifier GetClassifier() => GetConcept();

    /// <inheritdoc />
    public Concept GetConcept() => _m3.Language;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        if (feature == _builtIns.INamed_name)
            return Name;
        if (feature == _m3.IKeyed_key)
            return Key;
        if (feature == _m3.Language_version)
            return Version;
        if (feature == _m3.Language_entities)
            return Entities;
        if (feature == _m3.Language_dependsOn)
            return DependsOn;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return name != null;
    }

    /// <inheritdoc />
    public abstract string Key { get; }

    /// <inheritdoc />
    public bool TryGetKey([NotNullWhen(true)] out string? key)
    {
        key = Key;
        return key != null;
    }

    /// <inheritdoc />
    public abstract string Version { get; }

    /// <inheritdoc />
    public bool TryGetVersion([NotNullWhen(true)] out string? version)
    {
        version = Version;
        return version != null;
    }

    /// <inheritdoc />
    public abstract IReadOnlyList<LanguageEntity> Entities { get; }

    /// <inheritdoc />
    public abstract IReadOnlyList<Language> DependsOn { get; }

    /// <inheritdoc />
    public virtual TNodeFactory GetFactory() => _factory;

    /// <inheritdoc />
    public virtual void SetFactory(TNodeFactory factory) => _factory = factory;
}

/// <inheritdoc cref="IKeyed"/>
public abstract class IKeyedBase<TLanguage> : ReadableNodeBase<IReadableNode>, IKeyed where TLanguage : Language
{
    protected readonly TLanguage _language;

    /// <inheritdoc />
    protected override IBuiltInsLanguage _builtIns =>
        new Lazy<IBuiltInsLanguage>(() => _language.LionWebVersion.BuiltIns).Value;

    /// <inheritdoc />
    protected override ILionCoreLanguage _m3 =>
        new Lazy<ILionCoreLanguage>(() => _language.LionWebVersion.LionCore).Value;

    /// <inheritdoc />
    protected IKeyedBase(string id, TLanguage language) : this(id, language, language) { }

    /// <inheritdoc />
    protected IKeyedBase(string id, IKeyed parent, TLanguage language) : base(id, parent)
    {
        _language = language;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
    [
        _builtIns.INamed_name,
        _m3.IKeyed_key
    ];

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        if (feature == _builtIns.INamed_name)
            return Name;
        if (feature == _m3.IKeyed_key)
            return Key;

        return null;
    }

    /// <inheritdoc />
    public required string Name { get; init; }

    /// <inheritdoc />
    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return name != null;
    }

    /// <inheritdoc />
    public required string Key { get; init; }

    /// <inheritdoc />
    public bool TryGetKey([NotNullWhen(true)] out string? key)
    {
        key = Key;
        return key != null;
    }

    /// <inheritdoc cref="M2Extensions.GetLanguage"/>
    protected TLanguage GetLanguage() => _language;

    /// <inheritdoc />
    public override string? ToString() => Name ?? base.ToString();
}

/// <inheritdoc cref="Classifier"/>
public abstract class ClassifierBase<TLanguage>(string id, TLanguage parent)
    : IKeyedBase<TLanguage>(id, parent), Classifier
    where TLanguage : Language
{
    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Classifier_features
        ]);

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;

        if (feature == _m3.Classifier_features)
            return Features;

        return null;
    }

    /// <inheritdoc />
    public IReadOnlyList<Feature> Features => FeaturesLazy.Value;

    /// <inheritdoc cref="Features"/>
    public Lazy<IReadOnlyList<Feature>> FeaturesLazy { protected get; init; } = new([]);
}

/// <inheritdoc cref="Annotation"/>
public class AnnotationBase<TLanguage>(string id, TLanguage parent) : ClassifierBase<TLanguage>(id, parent), Annotation
    where TLanguage : Language
{
    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Annotation_annotates,
            _m3.Annotation_extends,
            _m3.Annotation_implements
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Annotation;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Annotation_annotates)
            return Annotates;
        if (feature == _m3.Annotation_extends)
            return Extends;
        if (feature == _m3.Annotation_implements)
            return Implements;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public Classifier Annotates => AnnotatesLazy.Value;

    /// <inheritdoc cref="Annotates"/>
    public required Lazy<Classifier> AnnotatesLazy { protected get; init; }

    /// <inheritdoc />
    public Annotation? Extends => ExtendsLazy.Value;

    /// <inheritdoc cref="Extends"/>
    public Lazy<Annotation?> ExtendsLazy { protected get; init; } = new((Annotation?)null);

    /// <inheritdoc />
    public IReadOnlyList<Interface> Implements => ImplementsLazy.Value;

    /// <inheritdoc cref="Implements"/>
    public Lazy<IReadOnlyList<Interface>> ImplementsLazy { protected get; init; } = new([]);
}

/// <inheritdoc cref="Concept"/>
public class ConceptBase<TLanguage>(string id, TLanguage parent) : ClassifierBase<TLanguage>(id, parent), Concept
    where TLanguage : Language
{
    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Concept_abstract,
            _m3.Concept_partition,
            _m3.Concept_extends,
            _m3.Concept_implements
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Concept;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Concept_abstract)
            return Abstract;
        if (feature == _m3.Concept_partition)
            return Partition;
        if (feature == _m3.Concept_extends)
            return Extends;
        if (feature == _m3.Concept_implements)
            return Implements;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public bool Abstract { get; init; } = false;

    /// <inheritdoc />
    public bool Partition { get; init; } = false;

    /// <inheritdoc />
    public Concept? Extends => ExtendsLazy.Value;

    /// <inheritdoc cref="Extends"/>
    public Lazy<Concept?> ExtendsLazy { protected get; init; } = new((Concept?)null);

    /// <inheritdoc />
    public IReadOnlyList<Interface> Implements => ImplementsLazy.Value;

    /// <inheritdoc cref="Implements"/>
    public Lazy<IReadOnlyList<Interface>> ImplementsLazy { protected get; init; } = new([]);
}

/// <inheritdoc cref="Interface"/>
public class InterfaceBase<TLanguage>(string id, TLanguage parent) : ClassifierBase<TLanguage>(id, parent), Interface
    where TLanguage : Language
{
    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Interface_extends
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Interface;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;

        if (feature == _m3.Interface_extends)
            return Extends;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public IReadOnlyList<Interface> Extends => ExtendsLazy.Value;

    /// <inheritdoc cref="Extends"/>
    public Lazy<IReadOnlyList<Interface>> ExtendsLazy { protected get; init; } = new([]);
}

/// <inheritdoc cref="Feature"/>
public abstract class FeatureBase<TLanguage> : IKeyedBase<TLanguage>, Feature where TLanguage : Language
{
    internal FeatureBase(string id, Classifier parent, TLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Feature_optional
        ]);

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Feature_optional)
            return Optional;

        return null;
    }

    /// <inheritdoc />
    public required bool Optional { get; init; }
}

/// <inheritdoc cref="Link"/>
public abstract class LinkBase<TLanguage> : FeatureBase<TLanguage>, Link where TLanguage : Language
{
    internal LinkBase(string id, Classifier parent, TLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Link_multiple,
            _m3.Link_type
        ]);

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Link_multiple)
            return Multiple;
        if (feature == _m3.Link_type)
            return Type;

        return null;
    }

    /// <inheritdoc />
    public required bool Multiple { get; init; }

    /// <inheritdoc />
    public required Classifier Type { get; init; }

    /// <inheritdoc />
    public bool TryGetType(out Classifier? type)
    {
        type = Type;
        return type != null;
    }
}

/// <inheritdoc cref="Reference"/>
public class ReferenceBase<TLanguage> : LinkBase<TLanguage>, Reference where TLanguage : Language
{
    /// <inheritdoc />
    public ReferenceBase(string id, Classifier parent, TLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Reference;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }
}

/// <inheritdoc cref="Containment"/>
public class ContainmentBase<TLanguage> : LinkBase<TLanguage>, Containment where TLanguage : Language
{
    /// <inheritdoc />
    public ContainmentBase(string id, Classifier parent, TLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Containment;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }
}

/// <inheritdoc cref="Property"/>
public class PropertyBase<TLanguage> : FeatureBase<TLanguage>, Property where TLanguage : Language
{
    /// <inheritdoc />
    public PropertyBase(string id, Classifier parent, TLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Property_type
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Property;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Property_type)
            return Type;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public required Datatype Type { get; init; }

    /// <inheritdoc />
    public bool TryGetType(out Datatype? type)
    {
        type = Type;
        return type != null;
    }
}

/// <inheritdoc cref="Datatype"/>
public abstract class DatatypeBase<TLanguage> : IKeyedBase<TLanguage>, Datatype where TLanguage : Language
{
    /// <inheritdoc />
    protected DatatypeBase(string id, TLanguage parent) : base(id, parent)
    {
    }
}

/// <inheritdoc cref="PrimitiveType"/>
public class PrimitiveTypeBase<TLanguage> : DatatypeBase<TLanguage>, PrimitiveType where TLanguage : Language
{
    /// <inheritdoc />
    public PrimitiveTypeBase(string id, TLanguage parent) : base(id, parent)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.PrimitiveType;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }
}

/// <inheritdoc cref="Enumeration"/>
public class EnumerationBase<TLanguage> : DatatypeBase<TLanguage>, Enumeration where TLanguage : Language
{
    /// <inheritdoc />
    public EnumerationBase(string id, TLanguage parent) : base(id, parent)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Enumeration_literals
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Enumeration;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Enumeration_literals)
            return Literals;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public IReadOnlyList<EnumerationLiteral> Literals => LiteralsLazy.Value;

    /// <inheritdoc cref="Literals"/>
    public required Lazy<IReadOnlyList<EnumerationLiteral>> LiteralsLazy { protected get; init; }
}

/// <inheritdoc cref="EnumerationLiteral"/>
public class EnumerationLiteralBase<TLanguage> : IKeyedBase<TLanguage>, EnumerationLiteral where TLanguage : Language
{
    /// <inheritdoc />
    public EnumerationLiteralBase(string id, Enumeration parent, TLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.EnumerationLiteral;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }
}

/// <inheritdoc cref="StructuredDataType"/>
public class StructuredDataTypeBase<TLanguage> : DatatypeBase<TLanguage>, StructuredDataType where TLanguage : Language
{
    /// <inheritdoc />
    protected override ILionCoreLanguageWithStructuredDataType _m3 =>
        new Lazy<ILionCoreLanguageWithStructuredDataType>(() =>
            (ILionCoreLanguageWithStructuredDataType)_language.LionWebVersion.LionCore).Value;

    /// <inheritdoc />
    public StructuredDataTypeBase(string id, TLanguage parent) : base(id, parent)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.StructuredDataType_fields
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.StructuredDataType;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.StructuredDataType_fields)
            return Fields;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public IReadOnlyList<Field> Fields => FieldsLazy.Value;

    /// <inheritdoc cref="Fields"/>
    public required Lazy<IReadOnlyList<Field>> FieldsLazy { protected get; init; }
}

/// <inheritdoc cref="Field"/>
public class FieldBase<TLanguage> : IKeyedBase<TLanguage>, Field where TLanguage : Language
{
    /// <inheritdoc />
    protected override ILionCoreLanguageWithStructuredDataType _m3 =>
        new Lazy<ILionCoreLanguageWithStructuredDataType>(() =>
            (ILionCoreLanguageWithStructuredDataType)_language.LionWebVersion.LionCore).Value;

    /// <inheritdoc />
    public FieldBase(string id, StructuredDataType parent, TLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Field_type
        ]);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Field;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        var result = base.Get(feature);
        if (result != null)
            return result;
        if (feature == _m3.Field_type)
            return Type;

        throw new UnknownFeatureException(GetClassifier(), feature);
    }

    /// <inheritdoc />
    public required Datatype Type { get; init; }

    /// <inheritdoc />
    public bool TryGetType(out Datatype? type)
    {
        type = Type;
        return type != null;
    }
}