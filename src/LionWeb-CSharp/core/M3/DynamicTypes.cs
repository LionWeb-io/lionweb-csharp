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

// The types here implement the LionCore M3.

/// <inheritdoc cref="IKeyed"/>
public abstract class DynamicIKeyed(string id) : NodeBase(id), IKeyed
{
    private string? _key;
    private string? _name;

    /// <inheritdoc />
    public string Key
    {
        get => _key ?? throw new UnsetFeatureException(M3Language.Instance.IKeyed_key);
        set => _key = value;
    }

    /// <inheritdoc />
    public string Name
    {
        get => _name ?? throw new UnsetFeatureException(BuiltInsLanguage.Instance.INamed_name);
        set => _name = value;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
    [
        BuiltInsLanguage.Instance.INamed_name,
        M3Language.Instance.IKeyed_key
    ];

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (BuiltInsLanguage.Instance.INamed_name == feature)
        {
            result = Name;
            return true;
        }

        if (M3Language.Instance.IKeyed_key == feature)
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
        if (BuiltInsLanguage.Instance.INamed_name == feature)
        {
            Name = value switch
            {
                string str => str,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (M3Language.Instance.IKeyed_key == feature)
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
            M3Language.Instance.Feature_optional
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (M3Language.Instance.Feature_optional == feature)
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

        if (M3Language.Instance.Feature_optional == feature)
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
        get => _type ?? throw new UnsetFeatureException(M3Language.Instance.Property_type);
        set => _type = value;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => M3Language.Instance.Property;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            M3Language.Instance.Property_type
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (M3Language.Instance.Property_type == feature)
        {
            result = Type;
            return false;
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

        if (M3Language.Instance.Property_type == feature)
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
        get => _type ?? throw new UnsetFeatureException(M3Language.Instance.Link_type);
        set => _type = value;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            M3Language.Instance.Link_multiple,
            M3Language.Instance.Link_type
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (M3Language.Instance.Link_multiple == feature)
        {
            result = Multiple;
            return true;
        }

        if (M3Language.Instance.Link_type == feature)
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

        if (M3Language.Instance.Link_multiple == feature)
        {
            Multiple = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (M3Language.Instance.Link_type == feature)
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
    public override Classifier GetClassifier() => M3Language.Instance.Containment;
}

/// <inheritdoc cref="Reference"/>
public class DynamicReference(string id, DynamicClassifier? classifier) : DynamicLink(id, classifier), Reference
{
    /// <inheritdoc />
    public override Classifier GetClassifier() => M3Language.Instance.Reference;
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
        _features.AddRange(SetSelfParent(features?.ToList(), M3Language.Instance.Classifier_features));

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var c = GetContainmentOf(child);
        if (c == M3Language.Instance.Classifier_features)
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
            return M3Language.Instance.Classifier_features;

        return null;
    }

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            M3Language.Instance.Classifier_features
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (M3Language.Instance.Classifier_features == feature)
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

        if (M3Language.Instance.Classifier_features == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_features?.ToList(), _features, M3Language.Instance.Classifier_features);
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
    public override Classifier GetClassifier() => M3Language.Instance.Concept;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            M3Language.Instance.Concept_abstract,
            M3Language.Instance.Concept_partition,
            M3Language.Instance.Concept_extends,
            M3Language.Instance.Concept_implements
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (M3Language.Instance.Concept_abstract == feature)
        {
            result = Abstract;
            return true;
        }

        if (M3Language.Instance.Concept_partition == feature)
        {
            result = Partition;
            return true;
        }

        if (M3Language.Instance.Concept_extends == feature)
        {
            result = Extends;
            return true;
        }

        if (M3Language.Instance.Concept_implements == feature)
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

        if (M3Language.Instance.Concept_abstract == feature)
        {
            Abstract = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (M3Language.Instance.Concept_partition == feature)
        {
            Partition = value switch
            {
                bool bol => bol,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (M3Language.Instance.Concept_extends == feature)
        {
            Extends = value switch
            {
                Concept ct => ct,
                null => null,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (M3Language.Instance.Concept_implements == feature)
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
        get => _annotates ?? BuiltInsLanguage.Instance.Node;
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
    public override Classifier GetClassifier() => M3Language.Instance.Annotation;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            M3Language.Instance.Annotation_annotates,
            M3Language.Instance.Annotation_extends,
            M3Language.Instance.Annotation_implements
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (M3Language.Instance.Annotation_annotates == feature)
        {
            result = Annotates;
            return true;
        }

        if (M3Language.Instance.Annotation_extends == feature)
        {
            result = Extends;
            return true;
        }

        if (M3Language.Instance.Annotation_implements == feature)
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

        if (M3Language.Instance.Annotation_annotates == feature)
        {
            Annotates = value switch
            {
                Classifier cl => cl,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (M3Language.Instance.Annotation_extends == feature)
        {
            Extends = value switch
            {
                Annotation at => at,
                null => null,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (M3Language.Instance.Annotation_implements == feature)
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
    public override Classifier GetClassifier() => M3Language.Instance.Interface;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            M3Language.Instance.Interface_extends
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (M3Language.Instance.Interface_extends == feature)
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

        if (M3Language.Instance.Interface_extends == feature)
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
    public override Classifier GetClassifier() => M3Language.Instance.PrimitiveType;
}

/// <inheritdoc cref="Enumeration"/>
public class DynamicEnumeration(string id, DynamicLanguage? language) : DynamicDatatype(id, language), Enumeration
{
    private readonly List<EnumerationLiteral> _literals = [];

    /// <inheritdoc />
    public IReadOnlyList<EnumerationLiteral> Literals => _literals.AsReadOnly();

    /// <inheritdoc cref="Literals"/>
    public void AddLiterals(IEnumerable<EnumerationLiteral> literals) =>
        _literals.AddRange(SetSelfParent(literals?.ToList(), M3Language.Instance.Enumeration_literals));

    /// <inheritdoc />
    protected override bool DetachChild(INode child)
    {
        if (base.DetachChild(child))
        {
            return true;
        }

        var c = GetContainmentOf(child);
        if (c == M3Language.Instance.Enumeration_literals)
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
            return M3Language.Instance.Enumeration_literals;

        return null;
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => M3Language.Instance.Enumeration;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            M3Language.Instance.Enumeration_literals
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (M3Language.Instance.Enumeration_literals == feature)
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

        if (M3Language.Instance.Enumeration_literals == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_literals?.ToList(), _literals, M3Language.Instance.Enumeration_literals);
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
    public override Classifier GetClassifier() => M3Language.Instance.EnumerationLiteral;
}

/// <inheritdoc cref="Language"/>
public class DynamicLanguage(string id) : DynamicIKeyed(id), Language
{
    /// <inheritdoc />
    public string Version
    {
        get => _version ?? throw new UnsetFeatureException(M3Language.Instance.Language_version);
        set => _version = value;
    }

    private readonly List<LanguageEntity> _entities = [];

    /// <inheritdoc />
    public IReadOnlyList<LanguageEntity> Entities => _entities.AsReadOnly();

    /// <inheritdoc cref="Entities"/>
    public void AddEntities(IEnumerable<LanguageEntity> entities) =>
        _entities.AddRange(SetSelfParent(entities?.ToList(), M3Language.Instance.Language_entities));

    private readonly List<Language> _dependsOn = [];
    private string? _version;

    /// <inheritdoc />
    public IReadOnlyList<Language> DependsOn => _dependsOn.AsReadOnly();

    /// <inheritdoc cref="DependsOn"/>
    public void AddDependsOn(IEnumerable<Language> languages)
    {
        AssureNotNull(languages, M3Language.Instance.Language_dependsOn);
        var safeNodes = languages.ToList();
        AssureNotNullMembers(safeNodes, M3Language.Instance.Language_dependsOn);
        _dependsOn.AddRange(safeNodes);
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => M3Language.Instance.Language;

    /// <inheritdoc />
    public override IEnumerable<Feature> CollectAllSetFeatures() =>
        base.CollectAllSetFeatures().Concat([
            M3Language.Instance.Language_version,
            M3Language.Instance.Language_entities,
            M3Language.Instance.Language_dependsOn
        ]);

    /// <inheritdoc />
    protected override bool GetInternal(Feature? feature, out object? result)
    {
        if (base.GetInternal(feature, out result))
            return true;

        if (M3Language.Instance.Language_version == feature)
        {
            result = Version;
            return true;
        }

        if (M3Language.Instance.Language_entities == feature)
        {
            result = Entities;
            return true;
        }

        if (M3Language.Instance.Language_dependsOn == feature)
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

        if (M3Language.Instance.Language_version == feature)
        {
            Version = value switch
            {
                string str => str,
                _ => throw new InvalidValueException(feature, value)
            };
            return true;
        }

        if (M3Language.Instance.Language_entities == feature)
        {
            switch (value)
            {
                case IEnumerable e:
                    RemoveSelfParent(_entities?.ToList(), _entities, M3Language.Instance.Language_entities);
                    AddEntities(e.OfType<LanguageEntity>().ToArray());
                    return true;
                default:
                    throw new InvalidValueException(feature, value);
            }
        }

        if (M3Language.Instance.Language_dependsOn == feature)
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
}