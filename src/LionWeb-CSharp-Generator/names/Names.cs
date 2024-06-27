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

namespace LionWeb.CSharp.Generator.Names;

using Core;
using Core.M2;
using Core.M3;
using Core.Utilities;
using Impl;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

/// <summary>
/// Central handling of all naming.
/// This helps
/// - to automatically generate `using` statements.
/// - to mangle names in a uniform manner. 
/// </summary>
public partial class Names(Language language, string namespaceName) : INames
{
    private readonly HashSet<Type> _usedTypes = [];

    /// <inheritdoc />
    public IReadOnlySet<Type> UsedTypes => _usedTypes;

    private readonly Dictionary<Language, string> _namespaceMappings = new();

    /// <inheritdoc />
    public IDictionary<Language, string> NamespaceMappings
    {
        get => _namespaceMappings;
        init
        {
            foreach ((Language? lang, var ns) in value)
            {
                _namespaceMappings[lang] = ns;
            }
        }
    }

    /// <inheritdoc />
    public void AddNamespaceMapping(Language lang, string nsName) =>
        _namespaceMappings[lang] = nsName;

    /// <inheritdoc />
    public string LanguageName(Language lang) => LanguageBaseName(lang) + "Language";

    private string LanguageBaseName(Language lang) => lang.Name.Split('.').Last().ToFirstUpper();

    /// <inheritdoc />
    public NameSyntax LanguageType => AsType(language);
    /// <inheritdoc />
    public string FactoryName => $"{LanguageBaseName(language)}Factory";
    /// <inheritdoc />
    public IdentifierNameSyntax FactoryType => IdentifierName(FactoryName);
    /// <inheritdoc />
    public Language Language => language;
    /// <inheritdoc />
    public string NamespaceName => namespaceName;

    /// <inheritdoc />
    public TypeSyntax AsType(Type type, params TypeSyntax?[] generics)
    {
        TypeSyntax? result = null;

        if (type == typeof(string))
            result = PredefinedType(Token(SyntaxKind.StringKeyword));
        else if (type == typeof(int))
            result = PredefinedType(Token(SyntaxKind.IntKeyword));
        else if (type == typeof(bool))
            result = PredefinedType(Token(SyntaxKind.BoolKeyword));

        else if (generics == null || generics.Length == 0)
        {
            if (type.GenericTypeArguments is { Length: > 0 })
            {
                generics = type.GenericTypeArguments.Select(g => AsType(g)).ToArray();
            } else
            {
                result = IdentifierName(Use(type));
            }
        }

        if (result == null)
            result = GenericName(Use(type))
                .WithTypeArgumentList(TypeArgumentList(SeparatedList(generics)));

        return result;
    }

    /// <inheritdoc />
    public TypeSyntax AsType(Classifier classifier, bool disambiguate = false)
    {
        if (BuiltInsLanguage.Instance.Node.EqualsIdentity(classifier))
            return AsType(typeof(NodeBase));
        if (BuiltInsLanguage.Instance.INamed.EqualsIdentity(classifier))
            return AsType(typeof(INamedWritable));

        if (_namespaceMappings.TryGetValue(classifier.GetLanguage(), out var ns))
        {
            return QualifiedName(ParseName(ns), IdentifierName(classifier.Name));
        }

        var type = IdentifierName(classifier.Name);
        if (!disambiguate)
            return type;
        return QualifiedName(ParseName(NamespaceName), type);
    }

    /// <inheritdoc />
    public TypeSyntax AsType(Datatype datatype, bool disambiguate = false)
    {
        if (BuiltInsLanguage.Instance.Boolean.EqualsIdentity(datatype))
            return PredefinedType(Token(SyntaxKind.BoolKeyword));
        if (BuiltInsLanguage.Instance.Integer.EqualsIdentity(datatype))
            return PredefinedType(Token(SyntaxKind.IntKeyword));
        if (BuiltInsLanguage.Instance.String.EqualsIdentity(datatype))
            return PredefinedType(Token(SyntaxKind.StringKeyword));
        if (BuiltInsLanguage.Instance.Json.EqualsIdentity(datatype))
            return PredefinedType(Token(SyntaxKind.StringKeyword));
        if (datatype is Enumeration && _namespaceMappings.TryGetValue(datatype.GetLanguage(), out var ns))
        {
            return
                QualifiedName(
                    ParseName(ns),
                    IdentifierName(datatype.Name)
                );
        }

        var type = IdentifierName(datatype.Name);

        if (!disambiguate || datatype is PrimitiveType)
            return type;

        return QualifiedName(ParseName(NamespaceName), type);
    }

    /// <inheritdoc />
    public NameSyntax AsType(Language lang)
    {
        var typeName = IdentifierName(LanguageBaseName(lang) + "Language");
        
        if (_namespaceMappings.TryGetValue(lang, out var ns))
        {
            return QualifiedName(
                ParseName(ns),
                typeName
            );
        }

        return typeName;
    }

    /// <inheritdoc />
    public string Use(Type type)
    {
        _usedTypes.Add(type);
        return AfterIncludingBacktick().Replace(type.Name, "");
    }

    /// <inheritdoc />
    public IdentifierNameSyntax AsProperty(LanguageEntity classifier) =>
        IdentifierName(classifier.Name);

    /// <inheritdoc />
    public IdentifierNameSyntax AsProperty(Feature feature) =>
        IdentifierName($"{feature.GetFeatureClassifier().Name}_{feature.Name}");

    /// <inheritdoc />
    public IdentifierNameSyntax AsProperty(EnumerationLiteral literal) =>
        IdentifierName($"{literal.GetEnumeration().Name}_{literal.Name}");

    /// <inheritdoc />
    public NameSyntax MetaProperty(Language lang) =>
        QualifiedName(
            AsType(lang),
            IdentifierName("Instance")
        );

    /// <inheritdoc />
    public IdentifierNameSyntax FeatureField(Feature feature) =>
        IdentifierName($"_{feature.Name.ToFirstLower()}");

    /// <inheritdoc />
    public IdentifierNameSyntax FeatureProperty(Feature feature) =>
        IdentifierName(feature.Name.ToFirstUpper());

    [GeneratedRegex("`.*$")]
    private static partial Regex AfterIncludingBacktick();
}