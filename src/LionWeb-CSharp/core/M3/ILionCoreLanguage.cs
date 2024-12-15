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

/// The self-definition of the LionCore M3.
public interface ILionCoreLanguage : Language
{
    /// Key of all LionWeb LionCore M3 language implementations.
    public const string LanguageKey = "LionCore-M3";

    /// Name of all LionWeb LionCore M3 language implementations.
    protected const string LanguageName = "LionCore_M3";

    internal const string ResolveInfoPrefix = "LionWeb.LionCore_M3.";

    /// <inheritdoc cref="M3.Annotation"/>
    Concept Annotation { get; }

    /// <inheritdoc cref="M3.Annotation.Annotates"/>
    Reference Annotation_annotates { get; }

    /// <inheritdoc cref="M3.Annotation.Extends"/>
    Reference Annotation_extends { get; }

    /// <inheritdoc cref="M3.Annotation.Implements"/>
    Reference Annotation_implements { get; }

    /// <inheritdoc cref="M3.Classifier"/>
    Concept Classifier { get; }

    /// <inheritdoc cref="M3.Classifier.Features"/>
    Containment Classifier_features { get; }

    /// <inheritdoc cref="M3.Concept"/>
    Concept Concept { get; }

    /// <inheritdoc cref="M3.Concept.Abstract"/>
    Property Concept_abstract { get; }

    /// <inheritdoc cref="M3.Concept.Partition"/>
    Property Concept_partition { get; }

    /// <inheritdoc cref="M3.Concept.Extends"/>
    Reference Concept_extends { get; }

    /// <inheritdoc cref="M3.Concept.Implements"/>
    Reference Concept_implements { get; }

    /// <inheritdoc cref="M3.Containment"/>
    Concept Containment { get; }

    /// <inheritdoc cref="M3.Datatype"/>
    Concept DataType { get; }

    /// <inheritdoc cref="M3.Enumeration"/>
    Concept Enumeration { get; }

    /// <inheritdoc cref="M3.Enumeration.Literals"/>
    Containment Enumeration_literals { get; }

    /// <inheritdoc cref="M3.EnumerationLiteral"/>
    Concept EnumerationLiteral { get; }

    /// <inheritdoc cref="M3.Feature"/>
    Concept Feature { get; }

    /// <inheritdoc cref="M3.Feature.Optional"/>
    Property Feature_optional { get; }

    /// <inheritdoc cref="M3.IKeyed"/>
    Interface IKeyed { get; }

    /// <inheritdoc cref="M3.IKeyed.Key"/>
    Property IKeyed_key { get; }

    /// <inheritdoc cref="M3.Interface"/>
    Concept Interface { get; }

    /// <inheritdoc cref="M3.Interface.Extends"/>
    Reference Interface_extends { get; }

    /// <inheritdoc cref="M3.Language"/>
    Concept Language { get; }

    /// <inheritdoc cref="M3.Language.Version"/>
    Property Language_version { get; }

    /// <inheritdoc cref="M3.Language.Entities"/>
    Containment Language_entities { get; }

    /// <inheritdoc cref="M3.Language.DependsOn"/>
    Reference Language_dependsOn { get; }

    /// <inheritdoc cref="M3.LanguageEntity"/>
    Concept LanguageEntity { get; }

    /// <inheritdoc cref="M3.Link"/>
    Concept Link { get; }

    /// <inheritdoc cref="M3.Link.Multiple"/>
    Property Link_multiple { get; }

    /// <inheritdoc cref="M3.Link.Type"/>
    Reference Link_type { get; }

    /// <inheritdoc cref="M3.PrimitiveType"/>
    Concept PrimitiveType { get; }

    /// <inheritdoc cref="M3.Property"/>
    Concept Property { get; }

    /// <inheritdoc cref="M3.Property.Type"/>
    Reference Property_type { get; }

    /// <inheritdoc cref="M3.Reference"/>
    Concept Reference { get; }
}

/// <inheritdoc cref="ILionCoreLanguage"/>
[Obsolete("Use ILionCoreLanguage instead")]
public sealed class M3Language
{
    [Obsolete("Use ILionCoreLanguage instead")]
    public static readonly ILionCoreLanguage Instance = LionWebVersions.Current.LionCore;
}

/// <inheritdoc />
public sealed class M3Concept : ConceptBase<ILionCoreLanguage>
{
    internal M3Concept(string id, ILionCoreLanguage parent) : base(id, parent)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object? Get(Feature feature)
    {
        if (feature == GetLanguage().LionWebVersion.BuiltIns.INamed_name)
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
public sealed class M3Interface : InterfaceBase<ILionCoreLanguage>
{
    internal M3Interface(string id, ILionCoreLanguage parent) : base(id, parent)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        if (feature == GetLanguage().LionWebVersion.BuiltIns.INamed_name)
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
public sealed class M3Reference : ReferenceBase<ILionCoreLanguage>
{
    internal M3Reference(string id, Classifier parent, ILionCoreLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        if (feature == GetLanguage().LionWebVersion.BuiltIns.INamed_name)
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
public sealed class M3Containment : ContainmentBase<ILionCoreLanguage>
{
    internal M3Containment(string id, Classifier parent, ILionCoreLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        if (feature == GetLanguage().LionWebVersion.BuiltIns.INamed_name)
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
public sealed class M3Property : PropertyBase<ILionCoreLanguage>
{
    internal M3Property(string id, Classifier parent, ILionCoreLanguage language) : base(id, parent, language)
    {
    }

    /// <inheritdoc />
    public override Classifier GetClassifier() => GetLanguage().Concept;

    /// <inheritdoc />
    public override object Get(Feature feature)
    {
        if (feature == GetLanguage().LionWebVersion.BuiltIns.INamed_name)
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