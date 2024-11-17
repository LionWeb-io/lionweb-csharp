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

namespace LionWeb.Core.M2;

using M3;

/// <summary>
/// The definition of the LionCore language containing the built-ins: primitive types, <see cref="INamed"/> and <see cref="Node"/>.
/// </summary>
public sealed class BuiltInsLanguage : LanguageBase<BuiltInsFactory>
{
    /// <summary>
    /// The definition of the LionCore built-ins language.
    /// </summary>
    public static readonly BuiltInsLanguage Instance = new Lazy<BuiltInsLanguage>(() => new()).Value;

    private const string _name = "LionCore_builtins";

    internal BuiltInsLanguage() : base(LionCoreBuiltInsIdAndKey)
    {
        _boolean = new(() => new PrimitiveTypeBase<BuiltInsLanguage>($"{LionCoreBuiltInsIdAndKey}-Boolean", this)
        {
            Key = $"{LionCoreBuiltInsIdAndKey}-Boolean", Name = "Boolean"
        });

        _integer = new(() => new PrimitiveTypeBase<BuiltInsLanguage>($"{LionCoreBuiltInsIdAndKey}-Integer", this)
        {
            Key = $"{LionCoreBuiltInsIdAndKey}-Integer", Name = "Integer"
        });

        _string = new(() => new PrimitiveTypeBase<BuiltInsLanguage>($"{LionCoreBuiltInsIdAndKey}-String", this)
        {
            Key = $"{LionCoreBuiltInsIdAndKey}-String", Name = "String"
        });

        _iNamed = new(() =>
            new InterfaceBase<BuiltInsLanguage>($"{LionCoreBuiltInsIdAndKey}-INamed", this)
            {
                Key = $"{LionCoreBuiltInsIdAndKey}-INamed", Name = "INamed", FeaturesLazy = new(() => [INamed_name])
            });
        _iNamed_name = new(() =>
            new PropertyBase<BuiltInsLanguage>($"{LionCoreBuiltInsIdAndKey}-INamed-name", INamed, this)
            {
                Key = $"{LionCoreBuiltInsIdAndKey}-INamed-name", Name = "name", Optional = false, Type = String
            });

        _node = new(() => new ConceptBase<BuiltInsLanguage>($"{LionCoreBuiltInsIdAndKey}-Node", this)
        {
            Key = $"{LionCoreBuiltInsIdAndKey}-Node", Name = "Node", Abstract = true, Partition = false
        });
    }

    /// <inheritdoc />
    public override string Name => _name;

    /// <inheritdoc />
    public override string Key => LionCoreBuiltInsIdAndKey;

    /// <inheritdoc />
    public override string Version => ReleaseVersion.Current;

    /// <inheritdoc />
    public override IReadOnlyList<LanguageEntity> Entities
    {
        get =>
        [
            Boolean,
            Integer,
            String,
            INamed,
            Node
        ];
    }

    /// <inheritdoc />
    public override IReadOnlyList<Language> DependsOn { get => []; }

    /// <inheritdoc />
    public override BuiltInsFactory GetFactory() => new(this);

    private readonly Lazy<PrimitiveType> _boolean;

    /// <summary>
    /// The built-in primitive type Boolean.
    /// </summary>
    public PrimitiveType Boolean => _boolean.Value;

    private readonly Lazy<PrimitiveType> _integer;

    /// <summary>
    /// The built-in primitive type Integer.
    /// </summary>
    public PrimitiveType Integer => _integer.Value;

    private readonly Lazy<PrimitiveType> _string;

    /// <summary>
    /// The built-in primitive type String.
    /// </summary>
    public PrimitiveType String => _string.Value;

    private readonly Lazy<Interface> _iNamed;

    /// <summary>
    /// Generic concept for "named things".
    /// </summary>
    public Interface INamed => _iNamed.Value;

    private readonly Lazy<Property> _iNamed_name;

    /// <summary>
    /// The "name" <see cref="Property"/> of <c>INamed</c>.
    /// </summary>
    public Property INamed_name => _iNamed_name.Value;

    private readonly Lazy<Concept> _node;

    /// <summary>
    /// Generic concept that corresponds to a model node, i.e. <see cref="INode"/>.
    /// </summary>
    public Concept Node => _node.Value;

    /// <summary>
    /// The ID and key of 
    /// </summary>
    public const string LionCoreBuiltInsIdAndKey = "LionCore-builtins";
}

/// Factory for <see cref="BuiltInsLanguage"/>.
public sealed class BuiltInsFactory : INodeFactory
{
    private readonly BuiltInsLanguage _language;

    internal BuiltInsFactory(BuiltInsLanguage language)
    {
        _language = language;
    }

    /// <inheritdoc />
    public INode CreateNode(string id, Classifier classifier) =>
        throw new UnsupportedClassifierException(classifier);

    /// <inheritdoc />
    public Enum GetEnumerationLiteral(EnumerationLiteral literal) =>
        throw new UnsupportedEnumerationLiteralException(literal);

    /// <inheritdoc />
    public IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType,
        IFieldValues fieldValues) =>
        throw new UnsupportedStructuredDataTypeException(structuredDataType);
}

/// <summary>
/// An interface for "named things".
/// It corresponds nominally to the <c>INamed</c> Concept defined above.
/// </summary>
public interface INamed : IReadableNode
{
    /// <summary>
    /// The human-readable name.
    /// </summary>
    public string Name { get; }
}

/// Writable variant of <see cref="INamed"/>.
public interface INamedWritable : INamed, IWritableNode
{
    /// <inheritdoc cref="INamed.Name"/>
    public new string Name { get; set; }

    /// <inheritdoc cref="INamed.Name"/>
    public INamedWritable SetName(string value);
}