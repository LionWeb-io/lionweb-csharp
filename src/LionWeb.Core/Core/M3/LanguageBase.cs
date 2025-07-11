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
using Utilities;

/// <inheritdoc cref="Language"/>
public abstract class LanguageBase<TNodeFactory>(NodeId id, LionWebVersions lionWebVersion)
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (_builtIns.INamed_name.EqualsIdentity(feature) && ((Language)this).TryGetName(out var name))
        {
            value = name;
            return true;
        }

        if (_m3.IKeyed_key.EqualsIdentity(feature) && ((Language)this).TryGetKey(out var key))
        {
            value = key;
            return true;
        }

        if (_m3.Language_version.EqualsIdentity(feature) && ((Language)this).TryGetVersion(out var version))
        {
            value = version;
            return true;
        }

        if (_m3.Language_entities.EqualsIdentity(feature) && ((Language)this).TryGetEntities(out var entities))
        {
            value = entities;
            return true;
        }

        if (_m3.Language_dependsOn.EqualsIdentity(feature) && ((Language)this).TryGetDependsOn(out var dependsOn))
        {
            value = dependsOn;
            return true;
        }

        value = null;
        return false;
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
    public abstract MetaPointerKey Key { get; }

    /// <inheritdoc />
    public bool TryGetKey([NotNullWhen(true)] out MetaPointerKey? key)
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
    protected IKeyedBase(NodeId id, TLanguage language) : this(id, language, language) { }

    /// <inheritdoc />
    protected IKeyedBase(NodeId id, IKeyed parent, TLanguage language) : base(id, parent)
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (_builtIns.INamed_name.EqualsIdentity(feature) && TryGetName(out var name))
        {
            value = name;
            return true;
        }

        if (_m3.IKeyed_key.EqualsIdentity(feature) && TryGetKey(out var key))
        {
            value = key;
            return true;
        }

        value = null;
        return false;
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
    public required MetaPointerKey Key { get; init; }

    /// <inheritdoc />
    public bool TryGetKey([NotNullWhen(true)] out MetaPointerKey? key)
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
public abstract class ClassifierBase<TLanguage>(NodeId id, TLanguage parent)
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Classifier_features.EqualsIdentity(feature) && ((Classifier)this).TryGetFeatures(out var features))
        {
            value = features;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    public IReadOnlyList<Feature> Features => FeaturesLazy.Value;

    /// <inheritdoc cref="Features"/>
    public Lazy<IReadOnlyList<Feature>> FeaturesLazy { protected get; init; } = new([]);
}

/// <inheritdoc cref="Annotation"/>
public class AnnotationBase<TLanguage>(NodeId id, TLanguage parent) : ClassifierBase<TLanguage>(id, parent), Annotation
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Annotation_annotates.EqualsIdentity(feature) && ((Annotation)this).TryGetAnnotates(out var annotates))
        {
            value = annotates;
            return true;
        }

        if (feature == _m3.Annotation_extends && ((Annotation)this).TryGetExtends(out var extends))
        {
            value = extends;
            return true;
        }

        if (feature == _m3.Annotation_implements && ((Annotation)this).TryGetImplements(out var implements))
        {
            value = implements;
            return true;
        }

        value = null;
        return false;
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
public class ConceptBase<TLanguage>(NodeId id, TLanguage parent) : ClassifierBase<TLanguage>(id, parent), Concept
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Concept_abstract.EqualsIdentity(feature) && ((Concept)this).TryGetAbstract(out var @abstract))
        {
            value = @abstract;
            return true;
        }

        if (_m3.Concept_partition.EqualsIdentity(feature) && ((Concept)this).TryGetPartition(out var partition))
        {
            value = partition;
            return true;
        }

        if (_m3.Concept_extends.EqualsIdentity(feature) && ((Concept)this).TryGetExtends(out var extends))
        {
            value = extends;
            return true;
        }

        if (_m3.Concept_implements.EqualsIdentity(feature) && ((Concept)this).TryGetImplements(out var implements))
        {
            value = implements;
            return true;
        }

        value = null;
        return false;
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
public class InterfaceBase<TLanguage>(NodeId id, TLanguage parent) : ClassifierBase<TLanguage>(id, parent), Interface
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Interface_extends.EqualsIdentity(feature) && ((Interface)this).TryGetExtends(out var extends))
        {
            value = extends;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    public IReadOnlyList<Interface> Extends => ExtendsLazy.Value;

    /// <inheritdoc cref="Extends"/>
    public Lazy<IReadOnlyList<Interface>> ExtendsLazy { protected get; init; } = new([]);
}

/// <inheritdoc cref="Feature"/>
public abstract class FeatureBase<TLanguage> : IKeyedBase<TLanguage>, Feature where TLanguage : Language
{
    internal FeatureBase(NodeId id, Classifier parent, TLanguage language) : base(id, parent, language)
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Feature_optional.EqualsIdentity(feature) && ((Feature)this).TryGetOptional(out var optional))
        {
            value = optional;
            return true;
        }

        value = null;
        return false;
    }

    /// <inheritdoc />
    public required bool Optional { get; init; }
}

/// <inheritdoc cref="Link"/>
public abstract class LinkBase<TLanguage> : FeatureBase<TLanguage>, Link where TLanguage : Language
{
    internal LinkBase(NodeId id, Classifier parent, TLanguage language) : base(id, parent, language)
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Link_multiple.EqualsIdentity(feature) && ((Link)this).TryGetMultiple(out var multiple))
        {
            value = multiple;
            return true;
        }

        if (_m3.Link_type.EqualsIdentity(feature) && ((Link)this).TryGetType(out var type))
        {
            value = type;
            return true;
        }

        value = null;
        return false;
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
    public ReferenceBase(NodeId id, Classifier parent, TLanguage language) : base(id, parent, language)
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
    public ContainmentBase(NodeId id, Classifier parent, TLanguage language) : base(id, parent, language)
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
    public PropertyBase(NodeId id, Classifier parent, TLanguage language) : base(id, parent, language)
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Property_type.EqualsIdentity(feature) && ((Property)this).TryGetType(out var type))
        {
            value = type;
            return true;
        }

        value = null;
        return false;
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
    protected DatatypeBase(NodeId id, TLanguage parent) : base(id, parent)
    {
    }
}

/// <inheritdoc cref="PrimitiveType"/>
public class PrimitiveTypeBase<TLanguage> : DatatypeBase<TLanguage>, PrimitiveType where TLanguage : Language
{
    /// <inheritdoc />
    public PrimitiveTypeBase(NodeId id, TLanguage parent) : base(id, parent)
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
    public EnumerationBase(NodeId id, TLanguage parent) : base(id, parent)
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Enumeration_literals.EqualsIdentity(feature) && ((Enumeration)this).TryGetLiterals(out var literals))
        {
            value = literals;
            return true;
        }

        value = null;
        return false;
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
    public EnumerationLiteralBase(NodeId id, Enumeration parent, TLanguage language) : base(id, parent, language)
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
    public StructuredDataTypeBase(NodeId id, TLanguage parent) : base(id, parent)
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.StructuredDataType_fields.EqualsIdentity(feature) && ((StructuredDataType)this).TryGetFields(out var fields))
        {
            value = fields;
            return true;
        }

        value = null;
        return false;
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
    public FieldBase(NodeId id, StructuredDataType parent, TLanguage language) : base(id, parent, language)
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
    public override bool TryGet(Feature feature, [NotNullWhen(true)] out object? value)
    {
        if (base.TryGet(feature, out value))
            return true;

        if (_m3.Field_type.EqualsIdentity(feature) && ((Field)this).TryGetType(out var type))
        {
            value = type;
            return true;
        }

        value = null;
        return false;
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