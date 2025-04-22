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

// The types here implement the LionCore M3.

/// Something with a name that has a key.
public interface IKeyed : INamed
{
    /// A Key must be a valid <see cref="IReadableNode.GetId">identifier</see>.
    /// It must be unique within its language.
    public string Key { get; }

    /// <summary>
    /// Gets the <see cref="Key"/>.
    /// </summary>
    /// <param name="key">Value of <see cref="Key"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Key"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetKey([NotNullWhen(true)] out string? key);
}

/// A Feature represents a characteristic or some form of data associated with a particular <see cref="Classifier"/>.
public interface Feature : IKeyed
{
    /// An <i>optional</i> feature can be <c>null</c> (or empty for <see cref="Link.Multiple">multiple links</see>).
    /// A non-optional, i.e. <i>required</i> feature can NOT be <c>null</c> or empty.  
    public bool Optional { get; }

    /// <summary>
    /// Gets the <see cref="Optional"/>.
    /// </summary>
    /// <param name="optional">Value of <see cref="Optional"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Optional"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetOptional([NotNullWhen(true)] out bool? optional)
    {
        optional = Optional;
        return optional != null;
    }
}

/// This indicates a simple value associated to a <see cref="Classifier"/>.
public interface Property : Feature
{
    /// LionWeb type of this property.
    public Datatype Type { get; }

    /// <summary>
    /// Gets the <see cref="Type"/>.
    /// </summary>
    /// <param name="type">Value of <see cref="Type"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Type"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetType([NotNullWhen(true)] out Datatype? type);
}

/// Represent a connection to a <see cref="Classifier"/>.
public interface Link : Feature
{
    /// A <i>multiple</i> link can have several values.
    /// A non-multiple, i.e. <i>single</i> link can have only one value.
    public bool Multiple { get; }

    /// <summary>
    /// Gets the <see cref="Multiple"/>.
    /// </summary>
    /// <param name="multiple">Value of <see cref="Multiple"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Multiple"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetMultiple([NotNullWhen(true)] out bool? multiple)
    {
        multiple = Multiple;
        return multiple != null;
    }

    /// LionWeb type of this link.
    public Classifier Type { get; }

    /// <summary>
    /// Gets the <see cref="Type"/>.
    /// </summary>
    /// <param name="type">Value of <see cref="Type"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Type"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetType([NotNullWhen(true)] out Classifier? type);
}

/// Represents a relation between a containing <see cref="Classifier"/> and a contained <see cref="Classifier"/>.
public interface Containment : Link;

/// Represents a relation between a referring <see cref="Classifier"/> and referred <see cref="Classifier"/>.
public interface Reference : Link;

/// A LanguageEntity is an entity with an identity directly contained in a <see cref="Language"/>.
public interface LanguageEntity : IKeyed;

/// Something which can own <see cref="Feature">Features</see>.
public interface Classifier : LanguageEntity
{
    /// <see cref="Feature">Features</see> owned by this classifier.
    public IReadOnlyList<Feature> Features { get; }

    /// <summary>
    /// Gets the <see cref="Features"/>.
    /// </summary>
    /// <param name="features">Value of <see cref="Features"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Features"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetFeatures([NotNullWhen(true)] out IReadOnlyList<Feature>? features)
    {
        features = Features;
        return features is { Count: > 0 };
    }
}

/// A Concept represents a category of entities sharing the same structure.
public interface Concept : Classifier
{
    /// An <i>abstract</i> concept cannot be instantiated.
    /// A non-abstract, i.e. <i>concrete</i> concept can be instantiated.
    public bool Abstract { get; }

    /// <summary>
    /// Gets the <see cref="Abstract"/>.
    /// </summary>
    /// <param name="abstract">Value of <see cref="Abstract"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Abstract"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetAbstract([NotNullWhen(true)] out bool? @abstract)
    {
        @abstract = Abstract;
        return @abstract != null;
    }

    /// A <i>partition</i> concept MUST NOT have a <see cref="IReadableNode.GetParent">parent</see>.
    /// It is the root of a node tree.
    /// A non-partition, i.e. <i>regular</i> concept MUST have a <see cref="IReadableNode.GetParent">parent</see>.
    public bool Partition { get; }

    /// <summary>
    /// Gets the <see cref="Partition"/>.
    /// </summary>
    /// <param name="partition">Value of <see cref="Partition"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Partition"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetPartition([NotNullWhen(true)] out bool? partition)
    {
        partition = Partition;
        return partition != null;
    }

    /// A concept can extend zero or one other concepts, the same way a C# class can extend another class.
    public Concept? Extends { get; }

    /// <summary>
    /// Gets the <see cref="Extends"/>.
    /// </summary>
    /// <param name="extends">Value of <see cref="Extends"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Extends"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetExtends([NotNullWhen(true)] out Concept? extends)
    {
        extends = Extends;
        return extends != null;
    }

    /// A concept can implement zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# class can implement C# interfaces.
    public IReadOnlyList<Interface> Implements { get; }

    /// <summary>
    /// Gets the <see cref="Implements"/>.
    /// </summary>
    /// <param name="implements">Value of <see cref="Implements"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Implements"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetImplements([NotNullWhen(true)] out IReadOnlyList<Interface>? implements)
    {
        implements = Implements;
        return implements is { Count: > 0 };
    }
}

/// An Annotation is an additional piece of information attached to potentially any node, sharing the node’s lifecycle.
public interface Annotation : Classifier
{
    /// An annotation can only be attached to a specific <see cref="Classifier"/> (and all its specializations, aka subtypes).
    public Classifier Annotates { get; }

    /// <summary>
    /// Gets the <see cref="Annotates"/>.
    /// </summary>
    /// <param name="annotates">Value of <see cref="Annotates"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Annotates"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetAnnotates([NotNullWhen(true)] out Classifier? annotates)
    {
        annotates = Annotates;
        return annotates != null;
    }

    /// An annotation can extend zero or one other annotations, the same way a C# class can extend another class.
    public Annotation? Extends { get; }

    /// <summary>
    /// Gets the <see cref="Extends"/>.
    /// </summary>
    /// <param name="extends">Value of <see cref="Extends"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Extends"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetExtends([NotNullWhen(true)] out Annotation? extends)
    {
        extends = Extends;
        return extends != null;
    }

    /// An annotation can implement zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# class can implement C# interfaces.
    public IReadOnlyList<Interface> Implements { get; }

    /// <summary>
    /// Gets the <see cref="Implements"/>.
    /// </summary>
    /// <param name="implements">Value of <see cref="Implements"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Implements"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetImplements([NotNullWhen(true)] out IReadOnlyList<Interface>? implements)
    {
        implements = Implements;
        return implements is { Count: > 0 };
    }
}

/// An Interface represents a category of entities sharing some similar characteristics.
public interface Interface : Classifier
{
    /// A LionWeb interface can extend zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# interface can extend other C# interfaces.
    public IReadOnlyList<Interface> Extends { get; }

    /// <summary>
    /// Gets the <see cref="Extends"/>.
    /// </summary>
    /// <param name="extends">Value of <see cref="Extends"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Extends"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetExtends([NotNullWhen(true)] out IReadOnlyList<Interface>? extends)
    {
        extends = Extends;
        return extends is { Count: > 0 };
    }
}

/// <summary>
/// A type of value which has no relevant identity in the context of a model.
/// </summary>
/// <remarks>
/// In official LionWeb, the correct name is <tt>DataType</tt> (uppercase T).
/// We keep the lowercase version for backwards compatibility.
/// </remarks>
public interface Datatype : LanguageEntity;

/// This represents an arbitrary primitive value.
public interface PrimitiveType : Datatype;

/// A primitive value with finite, pre-defined, known set of possible values.
public interface Enumeration : Datatype
{
    /// All possible values of this enumeration.
    public IReadOnlyList<EnumerationLiteral> Literals { get; }

    /// <summary>
    /// Gets the <see cref="Literals"/>.
    /// </summary>
    /// <param name="literals">Value of <see cref="Literals"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Literals"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetLiterals([NotNullWhen(true)] out IReadOnlyList<EnumerationLiteral>? literals)
    {
        literals = Literals;
        return literals is { Count: > 0 };
    }
}

/// One of the possible values of an <see cref="Enumeration"/>.
public interface EnumerationLiteral : IKeyed;

/// Represents a collection of named instances of Data Types.
/// Meant to support a small composite of values that semantically form a unit. 
public interface StructuredDataType : Datatype
{
    /// All fields of this structured datatype. 
    public IReadOnlyList<Field> Fields { get; }

    /// <summary>
    /// Gets the <see cref="Fields"/>.
    /// </summary>
    /// <param name="fields">Value of <see cref="Fields"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Fields"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetFields([NotNullWhen(true)] out IReadOnlyList<Field>? fields)
    {
        fields = Fields;
        return fields is { Count: > 0 };
    }
}

/// Represents one part of a <see cref="StructuredDataType"/>.
public interface Field : IKeyed
{
    /// LionWeb type of this field.
    public Datatype Type { get; }

    /// <summary>
    /// Gets the <see cref="Type"/>.
    /// </summary>
    /// <param name="type">Value of <see cref="Type"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Type"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetType([NotNullWhen(true)] out Datatype? type);
}

/// A Language will provide the Concepts necessary to describe ideas
/// in a particular domain together with supporting elements necessary for the definition of those Concepts.
public interface Language : IKeyed, IPartitionInstance
{
    /// The version of this language. Can be any non-empty string.
    public string Version { get; }

    /// <summary>
    /// Gets the <see cref="Version"/>.
    /// </summary>
    /// <param name="version">Value of <see cref="Version"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Version"/> is set; <c>false</c> otherwise.</returns>
    bool TryGetVersion([NotNullWhen(true)] out string? version);

    /// All <see cref="LanguageEntity">LanguageEntities</see> defined by this language.
    public IReadOnlyList<LanguageEntity> Entities { get; }

    /// <summary>
    /// Gets the <see cref="Entities"/>.
    /// </summary>
    /// <param name="entities">Value of <see cref="Entities"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="Entities"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetEntities([NotNullWhen(true)] out IReadOnlyList<LanguageEntity>? entities)
    {
        entities = Entities;
        return entities is { Count: > 0 };
    }

    /// Other languages that define <see cref="LanguageEntity">LanguageEntities</see> that this language depends on.
    public IReadOnlyList<Language> DependsOn { get; }

    /// <summary>
    /// Gets the <see cref="DependsOn"/>.
    /// </summary>
    /// <param name="dependsOn">Value of <see cref="DependsOn"/> if set, or <c>null</c>.</param>
    /// <returns><c>true</c> if <see cref="DependsOn"/> is set and not empty; <c>false</c> otherwise.</returns>
    bool TryGetDependsOn([NotNullWhen(true)] out IReadOnlyList<Language>? dependsOn)
    {
        dependsOn = DependsOn;
        return dependsOn is { Count: > 0 };
    }

    /// <summary>
    /// Returns a node factory, capable of creating instances of this language's <see cref="Classifier">Classifiers</see>
    /// and <see cref="StructuredDataType">StructuredDataTypes</see>.
    /// We need a factory to establish the relation between the Language and generated C# types.  
    /// </summary>
    /// <returns>A node factory, capable of creating instances of this language's
    /// <see cref="Classifier">Classifiers</see> and <see cref="StructuredDataType">StructuredDataTypes</see>.</returns>
    public INodeFactory GetFactory();

    /// <summary>
    /// Sets a custom factory for this language.
    /// </summary>
    /// <param name="factory">New factory capable of creating instances of this language's
    /// <see cref="Classifier">Classifiers</see> and <see cref="StructuredDataType">StructuredDataTypes</see>.</param>
    public void SetFactory(INodeFactory factory);

    /// Version of LionWeb standard this language is based on.
    public LionWebVersions LionWebVersion { get; }
}

/// <inheritdoc/>
/// <typeparam name="T">Type of this language's factory.</typeparam>
public interface Language<T> : Language where T : INodeFactory
{
    /// <inheritdoc/>
    INodeFactory Language.GetFactory() => GetFactory();

    /// <inheritdoc cref="Language.GetFactory"/>
    public new T GetFactory();

    /// <inheritdoc />
    void Language.SetFactory(INodeFactory factory) => SetFactory((T)factory);

    /// <inheritdoc cref="Language.SetFactory"/>
    public void SetFactory(T factory);
}