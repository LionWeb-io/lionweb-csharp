// Copyright 2024 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
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

namespace LionWeb.Core.VersionSpecific.V2025_1;

using M2;
using M3;

/// The definition of the LionCore <see cref="IVersion2025_1"/> language containing the built-ins:
/// primitive types, <see cref="IBuiltInsLanguage.INamed"/> and <see cref="IBuiltInsLanguage.Node"/>.
public interface IBuiltInsLanguage_2025_1 : IBuiltInsLanguage;

/// <inheritdoc cref="IBuiltInsLanguage_2025_1" />
public sealed class BuiltInsLanguage_2025_1 : LanguageBase<BuiltInsFactory_2025_1>, IBuiltInsLanguage_2025_1
{
    /// The definition of the LionCore <see cref="IVersion2025_1"/> built-ins language.
    public static readonly BuiltInsLanguage_2025_1 Instance = new Lazy<BuiltInsLanguage_2025_1>(() => new()).Value;

    private const string _idPrefix = "LionCore-builtins-2025_1";
    private const string _keyPrefix = IBuiltInsLanguage.LanguageKey;

    /// <inheritdoc />
    protected override IBuiltInsLanguage_2025_1 _builtIns => this;

    /// <inheritdoc />
    protected override ILionCoreLanguage_2025_1 _m3 => LionCoreLanguage_2025_1.Instance;

    internal BuiltInsLanguage_2025_1() : base(_idPrefix, LionWebVersions.v2025_1)
    {
        _boolean = new(() => new PrimitiveTypeBase<BuiltInsLanguage_2025_1>($"{_idPrefix}-Boolean", this)
        {
            Key = $"{_keyPrefix}-Boolean", Name = "Boolean"
        });

        _integer = new(() => new PrimitiveTypeBase<BuiltInsLanguage_2025_1>($"{_idPrefix}-Integer", this)
        {
            Key = $"{_keyPrefix}-Integer", Name = "Integer"
        });

        _string = new(() => new PrimitiveTypeBase<BuiltInsLanguage_2025_1>($"{_idPrefix}-String", this)
        {
            Key = $"{_keyPrefix}-String", Name = "String"
        });

        _iNamed = new(() =>
            new InterfaceBase<BuiltInsLanguage_2025_1>($"{_idPrefix}-INamed", this)
            {
                Key = $"{_keyPrefix}-INamed", Name = "INamed", FeaturesLazy = new(() => [INamed_name])
            });
        _iNamed_name = new(() =>
            new PropertyBase<BuiltInsLanguage_2025_1>($"{_idPrefix}-INamed-name", INamed, this)
            {
                Key = $"{_keyPrefix}-INamed-name", Name = "name", Optional = false, Type = String
            });

        _node = new(() => new ConceptBase<BuiltInsLanguage_2025_1>($"{_idPrefix}-Node", this)
        {
            Key = $"{_keyPrefix}-Node", Name = "Node", Abstract = true, Partition = false
        });
    }

    /// <inheritdoc />
    public override string Name => IBuiltInsLanguage.LanguageName;

    /// <inheritdoc />
    public override MetaPointerKey Key => IBuiltInsLanguage.LanguageKey;

    /// <inheritdoc />
    public override string Version => LionWebVersion.VersionString;

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
    public override BuiltInsFactory_2025_1 GetFactory() => new(this);

    private readonly Lazy<PrimitiveType> _boolean;

    /// <inheritdoc />
    public PrimitiveType Boolean => _boolean.Value;

    private readonly Lazy<PrimitiveType> _integer;

    /// <inheritdoc />
    public PrimitiveType Integer => _integer.Value;

    private readonly Lazy<PrimitiveType> _string;

    /// <inheritdoc />
    public PrimitiveType String => _string.Value;

    private readonly Lazy<Interface> _iNamed;

    /// <inheritdoc />
    public Interface INamed => _iNamed.Value;

    private readonly Lazy<Property> _iNamed_name;

    /// <inheritdoc />
    public Property INamed_name => _iNamed_name.Value;

    private readonly Lazy<Concept> _node;

    /// <inheritdoc />
    public Concept Node => _node.Value;
}

/// Factory for <see cref="BuiltInsLanguage_2025_1"/>.
public sealed class BuiltInsFactory_2025_1 : INodeFactory
{
    private readonly BuiltInsLanguage_2025_1 _language;

    internal BuiltInsFactory_2025_1(BuiltInsLanguage_2025_1 language)
    {
        _language = language;
    }

    /// <inheritdoc />
    public INode CreateNode(NodeId id, Classifier classifier) =>
        throw new UnsupportedClassifierException(classifier);

    /// <inheritdoc />
    public Enum GetEnumerationLiteral(EnumerationLiteral literal) =>
        throw new UnsupportedEnumerationLiteralException(literal);

    /// <inheritdoc />
    public IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType,
        IFieldValues fieldValues) =>
        throw new UnsupportedStructuredDataTypeException(structuredDataType);
}