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
namespace LionWeb.Core.VersionSpecific.V2023_1;

using M2;
using M3;

/// The definition of the LionCore <see cref="IVersion2023_1"/> language containing the built-ins:
/// primitive types, <see cref="IBuiltInsLanguage.INamed"/> and <see cref="IBuiltInsLanguage.Node"/>.
public interface IBuiltInsLanguage_2023_1 : IBuiltInsLanguage
{
    /// The built-in primitive type Json.
    PrimitiveType Json { get; }
}

/// <inheritdoc cref="IBuiltInsLanguage_2023_1" />
public sealed class BuiltInsLanguage_2023_1 : LanguageBase<BuiltInsFactory_2023_1>, IBuiltInsLanguage_2023_1 
{
    /// The definition of the LionCore <see cref="IVersion2023_1"/> built-ins language.
    public static readonly BuiltInsLanguage_2023_1 Instance = new Lazy<BuiltInsLanguage_2023_1>(() => new()).Value;

    private const string _name = "LionCore_builtins";

    /// <inheritdoc />
    protected override IBuiltInsLanguage_2023_1 _builtIns => this;

    /// <inheritdoc />
    protected override ILionCoreLanguage_2023_1 _m3 => LionCoreLanguage_2023_1.Instance;

    internal BuiltInsLanguage_2023_1() : base(LionCoreBuiltInsIdAndKey, LionWebVersions.v2023_1)
    {
        _boolean = new(() => new PrimitiveTypeBase<BuiltInsLanguage_2023_1>($"{LionCoreBuiltInsIdAndKey}-Boolean", this)
        {
            Key = $"{LionCoreBuiltInsIdAndKey}-Boolean", Name = "Boolean"
        });

        _integer = new(() => new PrimitiveTypeBase<BuiltInsLanguage_2023_1>($"{LionCoreBuiltInsIdAndKey}-Integer", this)
        {
            Key = $"{LionCoreBuiltInsIdAndKey}-Integer", Name = "Integer"
        });

        _json = new(() => new PrimitiveTypeBase<BuiltInsLanguage_2023_1>($"{LionCoreBuiltInsIdAndKey}-JSON", this)
        {
            Key = $"{LionCoreBuiltInsIdAndKey}-JSON", Name = "JSON"
        });

        _string = new(() => new PrimitiveTypeBase<BuiltInsLanguage_2023_1>($"{LionCoreBuiltInsIdAndKey}-String", this)
        {
            Key = $"{LionCoreBuiltInsIdAndKey}-String", Name = "String"
        });

        _iNamed = new(() =>
            new InterfaceBase<BuiltInsLanguage_2023_1>($"{LionCoreBuiltInsIdAndKey}-INamed", this)
            {
                Key = $"{LionCoreBuiltInsIdAndKey}-INamed", Name = "INamed", FeaturesLazy = new(() => [INamed_name])
            });
        _iNamed_name = new(() =>
            new PropertyBase<BuiltInsLanguage_2023_1>($"{LionCoreBuiltInsIdAndKey}-INamed-name", INamed, this)
            {
                Key = $"{LionCoreBuiltInsIdAndKey}-INamed-name", Name = "name", Optional = false, Type = String
            });

        _node = new(() => new ConceptBase<BuiltInsLanguage_2023_1>($"{LionCoreBuiltInsIdAndKey}-Node", this)
        {
            Key = $"{LionCoreBuiltInsIdAndKey}-Node", Name = "Node", Abstract = true, Partition = false
        });
    }

    /// <inheritdoc />
    public override string Name => _name;

    /// <inheritdoc />
    public override string Key => LionCoreBuiltInsIdAndKey;

    /// <inheritdoc />
    public override string Version => LionWebVersion.VersionString;

    /// <inheritdoc />
    public override IReadOnlyList<LanguageEntity> Entities
    {
        get =>
        [
            Boolean,
            Integer,
            Json,
            String,
            INamed,
            Node
        ];
    }

    /// <inheritdoc />
    public override IReadOnlyList<Language> DependsOn { get => []; }

    /// <inheritdoc />
    public override BuiltInsFactory_2023_1 GetFactory() => new(this);

    private readonly Lazy<PrimitiveType> _boolean;

    /// <inheritdoc />
    public PrimitiveType Boolean => _boolean.Value;

    private readonly Lazy<PrimitiveType> _integer;

    /// <inheritdoc />
    public PrimitiveType Integer => _integer.Value;

    private readonly Lazy<PrimitiveType> _json;

    /// <inheritdoc />
    public PrimitiveType Json => _json.Value;

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

    /// <summary>
    /// The ID and key of 
    /// </summary>
    public const string LionCoreBuiltInsIdAndKey = "LionCore-builtins";
}

/// Factory for <see cref="BuiltInsLanguage_2023_1"/>.
public sealed class BuiltInsFactory_2023_1 : INodeFactory
{
    private readonly BuiltInsLanguage_2023_1 _language;

    internal BuiltInsFactory_2023_1(BuiltInsLanguage_2023_1 language)
    {
        _language = language;
    }

    /// <inheritdoc />
    public INode CreateNode(string id, Classifier classifier) =>
        throw new UnsupportedClassifierException(classifier);

    /// <inheritdoc />
    public Enum GetEnumerationLiteral(EnumerationLiteral literal) =>
        throw new UnsupportedEnumerationLiteralException(literal);
}