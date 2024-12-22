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

/// Something with a name that has a key.
public interface IKeyed : INamed
{
    /// A Key must be a valid <see cref="IReadableNode.GetId">identifier</see>.
    /// It must be unique within its language.
    public string Key { get; }
}

/// A Feature represents a characteristic or some form of data associated with a particular <see cref="Classifier"/>.
public interface Feature : IKeyed
{
    /// An <i>optional</i> feature can be <c>null</c> (or empty for <see cref="Link.Multiple">multiple links</see>).
    /// A non-optional, i.e. <i>required</i> feature can NOT be <c>null</c> or empty.  
    public bool Optional { get; }
}

/// This indicates a simple value associated to a <see cref="Classifier"/>.
public interface Property : Feature
{
    /// LionWeb type of this property.
    public Datatype Type { get; }
}

/// Represent a connection to a <see cref="Classifier"/>.
public interface Link : Feature
{
    /// A <i>multiple</i> link can have several values.
    /// A non-multiple, i.e. <i>single</i> link can have only one value.
    public bool Multiple { get; }

    /// LionWeb type of this link.
    public Classifier Type { get; }
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
}

/// A Concept represents a category of entities sharing the same structure.
public interface Concept : Classifier
{
    /// An <i>abstract</i> concept cannot be instantiated.
    /// A non-abstract, i.e. <i>concrete</i> concept can be instantiated.
    public bool Abstract { get; }

    /// A <i>partition</i> concept MUST NOT have a <see cref="IReadableNode.GetParent">parent</see>.
    /// It is the root of a node tree.
    /// A non-partition, i.e. <i>regular</i> concept MUST have a <see cref="IReadableNode.GetParent">parent</see>.
    public bool Partition { get; }

    /// A concept can extend zero or one other concepts, the same way a C# class can extend another class.
    public Concept? Extends { get; }

    /// A concept can implement zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# class can implement C# interfaces.
    public IReadOnlyList<Interface> Implements { get; }
}

/// An Annotation is an additional piece of information attached to potentially any node, sharing the node’s lifecycle.
public interface Annotation : Classifier
{
    /// An annotation can only be attached to a specific <see cref="Classifier"/> (and all its specializations, aka subtypes).
    public Classifier Annotates { get; }

    /// An annotation can extend zero or one other annotations, the same way a C# class can extend another class.
    public Annotation? Extends { get; }

    /// An annotation can implement zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# class can implement C# interfaces.
    public IReadOnlyList<Interface> Implements { get; }
}

/// An Interface represents a category of entities sharing some similar characteristics.
public interface Interface : Classifier
{
    /// A LionWeb interface can extend zero or more <see cref="Interface">LionWeb interfaces</see>,
    /// the same way a C# interface can extend other C# interfaces.
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

/// This represents an arbitrary primitive value.
public interface PrimitiveType : Datatype;

/// A primitive value with finite, pre-defined, known set of possible values.
public interface Enumeration : Datatype
{
    /// All possible values of this enumeration.
    public IReadOnlyList<EnumerationLiteral> Literals { get; }
}

/// One of the possible values of an <see cref="Enumeration"/>.
public interface EnumerationLiteral : IKeyed;

/// Represents a collection of named instances of Data Types.
/// Meant to support a small composite of values that semantically form a unit. 
public interface StructuredDataType : Datatype
{
    /// All fields of this structured datatype. 
    public IReadOnlyList<Field> Fields { get; }
}

/// Represents one part of a <see cref="StructuredDataType"/>.
public interface Field : IKeyed
{
    /// LionWeb type of this field.
    public Datatype Type { get; }
}

/// A Language will provide the Concepts necessary to describe ideas
/// in a particular domain together with supporting elements necessary for the definition of those Concepts.
public interface Language : IKeyed
{
    /// The version of this language. Can be any non-empty string.
    public string Version { get; }

    /// All <see cref="LanguageEntity">LanguageEntities</see> defined by this language.
    public IReadOnlyList<LanguageEntity> Entities { get; }

    /// Other languages that define <see cref="LanguageEntity">LanguageEntities</see> that this language depends on.
    public IReadOnlyList<Language> DependsOn { get; }

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
    void Language.SetFactory(INodeFactory factory) => SetFactory((T) factory);

    /// <inheritdoc cref="Language.SetFactory"/>
    public void SetFactory(T factory);
}