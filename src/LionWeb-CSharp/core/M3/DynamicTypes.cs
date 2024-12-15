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

namespace LionWeb.Core.M3;

using M2;
using System.Collections;
using VersionSpecific.V2024_1;

// The types here implement the LionCore M3.

/// <inheritdoc cref="IKeyed"/>
public abstract class DynamicIKeyed(string id) : NodeBase(id), IKeyed
{
    private string? _key;
    private string? _name;

    /// <inheritdoc />
    protected override IBuiltInsLanguage _builtIns => new Lazy<IBuiltInsLanguage>(() => this.GetLanguage().LionWebVersion.BuiltIns).Value;

    /// <inheritdoc />
    protected override ILionCoreLanguage _m3 => new Lazy<ILionCoreLanguage>(() => this.GetLanguage().LionWebVersion.LionCore).Value;

    /// <inheritdoc />
    public string Key
    {
        get => _key ?? throw new UnsetFeatureException(_m3.IKeyed_key);
        set => _key = value;
    }

    /// <inheritdoc />
    public string Name
    {
        get => _name ?? throw new UnsetFeatureException(_builtIns.INamed_name);
        set => _name = value;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
    [
        _builtIns.INamed_name,
        _m3.IKeyed_key
    ];

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_builtIns.INamed_name == feature)
        {
            result = Name;
            return true;
        }

        if (_m3.IKeyed_key == feature)
        {
            result = Key;
            return true;
        }

        result = null;
        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        if (_builtIns.INamed_name == feature)
        {
            Name = value switch
            {
                string str => str,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.IKeyed_key == feature)
        {
            Key = value switch
            {
                string str => str,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        return false;
    }
}

/// <inheritdoc cref="Feature"/>
public abstract class DynamicFeature : DynamicIKeyed, Feature
{
    /// <inheritdoc />
    public bool Optional { get; set; }

    /// <inheritdoc />
    protected DynamicFeature(string id, DynamicClassifier? parent) : base(id)
    {
        parent?.AddFeatures([this]);
        _parent = parent;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Feature_optional
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Feature_optional == feature)
        {
            result = Optional;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Feature_optional == feature)
        {
            Optional = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        return false;
    }
}

/// <inheritdoc cref="Property"/>
public class DynamicProperty(string id, DynamicClassifier? classifier) : DynamicFeature(id, classifier), Property
{
    private Datatype? _type;

    /// <inheritdoc />
    public Datatype Type
    {
        get => _type ?? throw new UnsetFeatureException(_m3.Property_type);
        set => _type = value;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Property;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Property_type
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Property_type == feature)
        {
            result = Type;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Property_type == feature)
        {
            Type = value switch
            {
                Datatype dt => dt,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        return false;
    }
}

/// <inheritdoc cref="Link"/>
public abstract class DynamicLink(string id, DynamicClassifier? classifier) : DynamicFeature(id, classifier), Link
{
    private Classifier? _type;

    /// <inheritdoc />
    public bool Multiple { get; set; }

    /// <inheritdoc />
    public Classifier Type
    {
        get => _type ?? throw new UnsetFeatureException(_m3.Link_type);
        set => _type = value;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Link_multiple,
            _m3.Link_type
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Link_multiple == feature)
        {
            result = Multiple;
            return true;
        }

        if (_m3.Link_type == feature)
        {
            result = Type;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Link_multiple == feature)
        {
            Multiple = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Link_type == feature)
        {
            Type = value switch
            {
                Classifier cf => cf,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        return false;
    }
}

/// <inheritdoc cref="Containment"/>
public class DynamicContainment(string id, DynamicClassifier? classifier) : DynamicLink(id, classifier), Containment
{
    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Containment;
}

/// <inheritdoc cref="Reference"/>
public class DynamicReference(string id, DynamicClassifier? classifier) : DynamicLink(id, classifier), Reference
{
    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Reference;
}

/// <inheritdoc cref="LanguageEntity"/>
public abstract class DynamicLanguageEntity : DynamicIKeyed, LanguageEntity
{
    /// <inheritdoc />
    protected DynamicLanguageEntity(string id, DynamicLanguage? language)
        : base(id)
    {
        _parent = language;
    }
}

/// <inheritdoc cref="Classifier"/>
public abstract class DynamicClassifier(string id, DynamicLanguage? language)
    : DynamicLanguageEntity(id, language), Classifier
{
    private readonly List<Feature> _features = [];

    /// <inheritdoc />
    public IReadOnlyList<Feature> Features => _features.AsReadOnly();

    /// <inheritdoc cref="Features"/>
    public void AddFeatures(IEnumerable<Feature> features) =>
        _features.AddRange(SetSelfParent(features?.ToList(), _m3.Classifier_features));

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var c = GetContainmentOf(child);
        if (c == _m3.Classifier_features)
            return _features.Remove((Feature)child);

        return false;
    }

    /// <inheritdoc />
    public override Containment? GetContainmentOf(INode child)
    {
        var result = base.GetContainmentOf(child);
        if (result != null)
            return result;

        if (child is Feature s && _features.Contains(s))
            return _m3.Classifier_features;

        return null;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Classifier_features
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Classifier_features == feature)
        {
            result = Features;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Classifier_features == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_features?.ToList(), _features, _m3.Classifier_features);
                    AddFeatures(e.OfType<Feature>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}

/// <inheritdoc cref="Concept"/>
public class DynamicConcept(string id, DynamicLanguage? language) : DynamicClassifier(id, language), Concept
{
    /// <inheritdoc />
    public bool Abstract { get; set; }

    /// <inheritdoc />
    public bool Partition { get; set; }

    /// <inheritdoc />
    public Concept? Extends { get; set; }

    private readonly List<Interface> _implements = [];

    /// <inheritdoc />
    public IReadOnlyList<Interface> Implements => _implements.AsReadOnly();

    /// <inheritdoc cref="Implements"/>
    public void AddImplements(IEnumerable<Interface> interfaces) => _implements.AddRange(interfaces);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Concept;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Concept_abstract,
            _m3.Concept_partition,
            _m3.Concept_extends,
            _m3.Concept_implements
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Concept_abstract == feature)
        {
            result = Abstract;
            return true;
        }

        if (_m3.Concept_partition == feature)
        {
            result = Partition;
            return true;
        }

        if (_m3.Concept_extends == feature)
        {
            result = Extends;
            return true;
        }

        if (_m3.Concept_implements == feature)
        {
            result = Implements;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Concept_abstract == feature)
        {
            Abstract = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Concept_partition == feature)
        {
            Partition = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Concept_extends == feature)
        {
            Extends = value switch
            {
                Concept ct => ct,
                null => null,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Concept_implements == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    _implements.Clear();
                    _implements.AddRange(e.OfType<Interface>());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}

/// <inheritdoc cref="Annotation"/>
public class DynamicAnnotation(string id, DynamicLanguage? language) : DynamicClassifier(id, language), Annotation
{
    /// <inheritdoc />
    public Classifier Annotates
    {
        get => _annotates ?? _builtIns.Node;
        set => _annotates = value;
    }

    /// <inheritdoc />
    public Annotation? Extends { get; set; }

    private readonly List<Interface> _implements = [];
    private Classifier? _annotates;

    /// <inheritdoc />
    public IReadOnlyList<Interface> Implements => _implements.AsReadOnly();

    /// <inheritdoc cref="Implements"/>
    public void AddImplements(IEnumerable<Interface> interfaces) => _implements.AddRange(interfaces);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Annotation;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Annotation_annotates,
            _m3.Annotation_extends,
            _m3.Annotation_implements
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Annotation_annotates == feature)
        {
            result = Annotates;
            return true;
        }

        if (_m3.Annotation_extends == feature)
        {
            result = Extends;
            return true;
        }

        if (_m3.Annotation_implements == feature)
        {
            result = Implements;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Annotation_annotates == feature)
        {
            Annotates = value switch
            {
                Classifier cl => cl,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Annotation_extends == feature)
        {
            Extends = value switch
            {
                Annotation at => at,
                null => null,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Annotation_implements == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    _implements.Clear();
                    _implements.AddRange(e.OfType<Interface>());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}

/// <inheritdoc cref="Interface"/>
public class DynamicInterface(string id, DynamicLanguage? language) : DynamicClassifier(id, language), Interface
{
    private readonly List<Interface> _extends = [];

    /// <inheritdoc />
    public IReadOnlyList<Interface> Extends => _extends.AsReadOnly();

    /// <inheritdoc cref="Extends"/>
    public void AddExtends(IEnumerable<Interface> extends) => _extends.AddRange(extends);

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Interface;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Interface_extends
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Interface_extends == feature)
        {
            result = Extends;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Interface_extends == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    _extends.Clear();
                    _extends.AddRange(e.OfType<Interface>());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}

/// <inheritdoc cref="Datatype"/>
public abstract class DynamicDatatype(string id, DynamicLanguage? language)
    : DynamicLanguageEntity(id, language), Datatype;

/// <inheritdoc cref="PrimitiveType"/>
public class DynamicPrimitiveType(string id, DynamicLanguage? language) : DynamicDatatype(id, language), PrimitiveType
{
    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.PrimitiveType;
}

/// <inheritdoc cref="Enumeration"/>
public class DynamicEnumeration(string id, DynamicLanguage? language) : DynamicDatatype(id, language), Enumeration
{
    private readonly List<EnumerationLiteral> _literals = [];

    /// <inheritdoc />
    public IReadOnlyList<EnumerationLiteral> Literals => _literals.AsReadOnly();

    /// <inheritdoc cref="Literals"/>
    public void AddLiterals(IEnumerable<EnumerationLiteral> literals) =>
        _literals.AddRange(SetSelfParent(literals?.ToList(), _m3.Enumeration_literals));

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var c = GetContainmentOf(child);
        if (c == _m3.Enumeration_literals)
            return _literals.Remove((EnumerationLiteral)child);

        return false;
    }

    /// <inheritdoc />
    public override Containment? GetContainmentOf(INode child)
    {
        var result = base.GetContainmentOf(child);
        if (result != null)
            return result;

        if (child is EnumerationLiteral s && _literals.Contains(s))
            return _m3.Enumeration_literals;

        return null;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Enumeration;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Enumeration_literals
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Enumeration_literals == feature)
        {
            result = Literals;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Enumeration_literals == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_literals?.ToList(), _literals, _m3.Enumeration_literals);
                    AddLiterals(e.OfType<EnumerationLiteral>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}

/// <inheritdoc cref="EnumerationLiteral"/>
public class DynamicEnumerationLiteral : DynamicIKeyed, EnumerationLiteral
{
    /// <inheritdoc />
    public DynamicEnumerationLiteral(string id, DynamicEnumeration? enumeration)
        : base(id)
    {
        enumeration?.AddLiterals([this]);
        _parent = enumeration;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.EnumerationLiteral;
}

/// <inheritdoc cref="StructuredDataType"/>
public class DynamicStructuredDataType(string id, DynamicLanguage? language) : DynamicDatatype(id, language), StructuredDataType
{
    /// <inheritdoc />
    protected override IBuiltInsLanguage_2024_1 _builtIns => new Lazy<IBuiltInsLanguage_2024_1>(() => (IBuiltInsLanguage_2024_1)this.GetLanguage().LionWebVersion.BuiltIns).Value;

    /// <inheritdoc />
    protected override ILionCoreLanguage_2024_1 _m3 => new Lazy<ILionCoreLanguage_2024_1>(() => (ILionCoreLanguage_2024_1)this.GetLanguage().LionWebVersion.LionCore).Value;

    private readonly List<Field> _fields = [];

    /// <inheritdoc />
    public IReadOnlyList<Field> Fields => _fields.AsReadOnly();

    /// <inheritdoc cref="Fields"/>
    public void AddFields(IEnumerable<Field> fields) =>
        _fields.AddRange(SetSelfParent(fields?.ToList(), _m3.StructuredDataType_fields));

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var c = GetContainmentOf(child);
        if (c == _m3.StructuredDataType_fields)
            return _fields.Remove((Field)child);

        return false;
    }

    /// <inheritdoc />
    public override Containment? GetContainmentOf(INode child)
    {
        var result = base.GetContainmentOf(child);
        if (result != null)
            return result;

        if (child is Field s && _fields.Contains(s))
            return _m3.StructuredDataType_fields;

        return null;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.StructuredDataType;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.StructuredDataType_fields
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.StructuredDataType_fields == feature)
        {
            result = Fields;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.StructuredDataType_fields == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_fields?.ToList(), _fields, _m3.StructuredDataType_fields);
                    AddFields(e.OfType<Field>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }
}

/// <inheritdoc cref="Field"/>
public class DynamicField(string id, DynamicStructuredDataType? structuredDataType) : DynamicIKeyed(id), Field
{
    /// <inheritdoc />
    protected override IBuiltInsLanguage_2024_1 _builtIns => new Lazy<IBuiltInsLanguage_2024_1>(() => (IBuiltInsLanguage_2024_1)this.GetLanguage().LionWebVersion.BuiltIns).Value;

    /// <inheritdoc />
    protected override ILionCoreLanguage_2024_1 _m3 => new Lazy<ILionCoreLanguage_2024_1>(() => (ILionCoreLanguage_2024_1)this.GetLanguage().LionWebVersion.LionCore).Value;

    private Datatype? _type;

    /// <inheritdoc />
    public Datatype Type
    {
        get => _type ?? throw new UnsetFeatureException(_m3.Field_type);
        set => _type = value;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Field;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Field_type
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Field_type == feature)
        {
            result = Type;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Field_type == feature)
        {
            Type = value switch
            {
                Datatype dt => dt,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        return false;
    }
}

/// <inheritdoc cref="Language"/>
public class DynamicLanguage(string id, LionWebVersions lionWebVersion) : DynamicIKeyed(id), Language
{
    /// <inheritdoc />
    public LionWebVersions LionWebVersion { get; } = lionWebVersion;

    /// <inheritdoc />
    public string Version
    {
        get => _version ?? throw new UnsetFeatureException(_m3.Language_version);
        set => _version = value;
    }

    private readonly List<LanguageEntity> _entities = [];

    /// <inheritdoc />
    public IReadOnlyList<LanguageEntity> Entities => _entities.AsReadOnly();

    /// <inheritdoc cref="Entities"/>
    public void AddEntities(IEnumerable<LanguageEntity> entities) =>
        _entities.AddRange(SetSelfParent(entities?.ToList(), _m3.Language_entities));

    private readonly List<Language> _dependsOn = [];
    private string? _version;

    /// <inheritdoc />
    public IReadOnlyList<Language> DependsOn => _dependsOn.AsReadOnly();

    /// <inheritdoc cref="DependsOn"/>
    public void AddDependsOn(IEnumerable<Language> languages)
    {
        AssureNotNull(languages, _m3.Language_dependsOn);
        var safeNodes = languages.ToList();
        AssureNotNullMembers(safeNodes, _m3.Language_dependsOn);
        _dependsOn.AddRange(safeNodes);
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => _m3.Language;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            _m3.Language_version,
            _m3.Language_entities,
            _m3.Language_dependsOn
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (_m3.Language_version == feature)
        {
            result = Version;
            return true;
        }

        if (_m3.Language_entities == feature)
        {
            result = Entities;
            return true;
        }

        if (_m3.Language_dependsOn == feature)
        {
            result = DependsOn;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    protected override bool SetInternal(Feature? feature, object? value)
    {
        var result = base.SetInternal(feature, value);
        if (result)
        {
            return result;
        }

        if (_m3.Language_version == feature)
        {
            Version = value switch
            {
                string str => str,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (_m3.Language_entities == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_entities?.ToList(), _entities, _m3.Language_entities);
                    AddEntities(e.OfType<LanguageEntity>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        if (_m3.Language_dependsOn == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    _dependsOn.Clear();
                    _dependsOn.AddRange(e.OfType<Language>());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        return false;
    }

    /// <inheritdoc cref="GetFactory"/>
    public INodeFactory? NodeFactory { private get; set; }

    /// <inheritdoc />
    public INodeFactory GetFactory() => NodeFactory ??= new ReflectiveBaseNodeFactory(this);

    /// <inheritdoc />
    public void SetFactory(INodeFactory factory) => NodeFactory = factory;
}