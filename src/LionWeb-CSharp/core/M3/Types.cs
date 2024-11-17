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

// The types here implement the LionCore M3.

/// <summary>
/// Something with a name that has a key.
/// </summary>
public interface IKeyed : INamed
{
    /// <summary>
    /// A Key must be a valid <see cref="IReadableNode.GetId">identifier</see>.
    /// It must be unique within its language.
    /// </summary>
    public string Key { get; }
}

/// <summary>
/// A Feature represents a characteristic or some form of data associated with a particular <see cref="Classifier"/>.
/// </summary>
public interface Feature : IKeyed
{
    /// <summary>
    /// An <i>optional</i> feature can be <c>null</c> (or empty for <see cref="Link.Multiple">multiple links</see>).
    /// A non-optional, i.e. <i>required</i> feature can NOT be <c>null</c> or empty.  
    /// </summary>
    public bool Optional { get; }
}

/// <summary>
/// This indicates a simple value associated to a <see cref="Classifier"/>.
/// </summary>
public interface Property : Feature
{
    /// <summary>
    /// LionWeb type of this property.
    /// </summary>
    public Datatype Type { get; }
}

/// <summary>
/// Represent a connection to a <see cref="Classifier"/>.
/// </summary>
public interface Link : Feature
{
    /// <summary>
    /// A <i>multiple</i> link can have several values.
    /// A non-multiple, i.e. <i>single</i> link can have only one value.
    /// </summary>
    public bool Multiple { get; }

    /// <summary>
    /// LionWeb type of this link.
    /// </summary>
    public Classifier Type { get; }
}

/// <summary>
/// Represents a relation between a containing <see cref="Classifier"/> and a contained <see cref="Classifier"/>.
/// </summary>
public interface Containment : Link;

/// <summary>
/// Represents a relation between a referring <see cref="Classifier"/> and referred <see cref="Classifier"/>.
/// </summary>
public interface Reference : Link;

/// <summary>
/// A LanguageEntity is an entity with an identity directly contained in a <see cref="Language"/>.
/// </summary>
public interface LanguageEntity : IKeyed;

/// <summary>
/// Something which can own <see cref="Feature">Features</see>.
/// </summary>
public interface Classifier : LanguageEntity
{
    /// <summary>
    /// <see cref="Feature">Features</see> owned by this classifier.
    /// </summary>
    public IReadOnlyList<Feature> Features { get; }
}

/// <summary>
/// A Concept represents a category of entities sharing the same structure.
/// </summary>
public interface Concept : Classifier
{
    /// <summary>
    /// An <i>abstract</i> concept cannot be instantiated.
    /// A non-abstract, i.e. <i>concrete</i> concept can be instantiated.
    /// </summary>
    public bool Abstract { get; }

    /// <summary>
    /// A <i>partition</i> concept MUST NOT have a <see cref="IReadableNode.GetParent">parent</see>.
    /// It is the root of a node tree.
    /// A non-partition, i.e. <i>regular</i> concept MUST have a <see cref="IReadableNode.GetParent">parent</see>.
    /// </summary>
    public bool Partition { get; }

    /// <summary>
    /// A concept can extend zero or one other concepts, the same way a C# class can extend another class.
    /// </summary>
    public Concept? Extends { get; }

    /// <summary>
    /// A concept can implement zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# class can implement C# interfaces.
    /// </summary>
    public IReadOnlyList<Interface> Implements { get; }
}

/// <summary>
/// An Annotation is an additional piece of information attached to potentially any node, sharing the node’s lifecycle.
/// </summary>
public interface Annotation : Classifier
{
    /// <summary>
    /// An annotation can only be attached to a specific <see cref="Classifier"/> (and all its specializations, aka subtypes).
    /// </summary>
    public Classifier Annotates { get; }

    /// <summary>
    /// An annotation can extend zero or one other annotations, the same way a C# class can extend another class.
    /// </summary>
    public Annotation? Extends { get; }

    /// <summary>
    /// An annotation can implement zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# class can implement C# interfaces.
    /// </summary>
    public IReadOnlyList<Interface> Implements { get; }
}

/// <summary>
/// An Interface represents a category of entities sharing some similar characteristics.
/// </summary>
public interface Interface : Classifier
{
    /// <summary>
    /// A LionWeb interface can extend zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# interface can extend other C# interfaces.
    /// </summary>
    public IReadOnlyList<Interface> Extends { get; }
}

/// <summary>
/// A type of value which has no relevant identity in the context of a model.
/// </summary>
/// <remarks>
/// In official LionWeb, the correct name is <tt>DataType</tt> (uppercase T).
/// We keep the lowercase version for backwards compatibility.
/// </remarks>
public interface Datatype : LanguageEntity;

/// <summary>
/// This represents an arbitrary primitive value.
/// </summary>
public interface PrimitiveType : Datatype;

/// <summary>
/// A primitive value with finite, pre-defined, known set of possible values.
/// </summary>
public interface Enumeration : Datatype
{
    /// <summary>
    /// All possible values of this enumeration.
    /// </summary>
    public IReadOnlyList<EnumerationLiteral> Literals { get; }
}

/// <summary>
/// One of the possible values of an <see cref="Enumeration"/>.
/// </summary>
public interface EnumerationLiteral : IKeyed;

public interface StructuredDataType : Datatype
{
    public IReadOnlyList<Field> Fields { get; }
}

public interface Field : IKeyed
{
    /// <summary>
    /// LionWeb type of this field.
    /// </summary>
    public Datatype Type { get; }
}

/// <summary>
/// A Language will provide the Concepts necessary to describe ideas
/// in a particular domain together with supporting elements necessary for the definition of those Concepts.
/// </summary>
public interface Language : IKeyed
{
    /// <summary>
    /// The version of this language. Can be any non-empty string.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// All <see cref="LanguageEntity">LanguageEntities</see> defined by this language.
    /// </summary>
    public IReadOnlyList<LanguageEntity> Entities { get; }

    /// <summary>
    /// Other languages that define <see cref="LanguageEntity">LanguageEntities</see> that this language depends on.
    /// </summary>
    IReadOnlyList<Language> DependsOn { get; }

    /// <summary>
    /// Returns a node factory, capable of creating instances of this language's <see cref="Classifier">Classifiers</see>.
    /// We need a factory to establish the relation between the Language and generated C# types.  
    /// </summary>
    /// <returns>A node factory, capable of creating instances of this language's <see cref="Classifier">Classifiers</see>.</returns>
    public INodeFactory GetFactory();
}

/// <inheritdoc/>
public interface Language<out T> : Language where T : INodeFactory
{
    /// <inheritdoc/>
    INodeFactory Language.GetFactory() => GetFactory();

    /// <inheritdoc cref="Language.GetFactory"/>
    public new T GetFactory();
}