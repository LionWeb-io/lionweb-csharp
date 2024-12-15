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
public partial class Names : INames
{
    private readonly HashSet<Type> _usedTypes = [];

    /// <inheritdoc />
    public IReadOnlySet<Type> UsedTypes => _usedTypes;

    private readonly Language _language;
    private readonly string _namespaceName;
    protected readonly ILionCoreLanguage _m3;
    protected readonly IBuiltInsLanguage _builtIns;
    
    private readonly Dictionary<Language, string> _namespaceMappings = new();

    /// <summary>
    /// Central handling of all naming.
    /// This helps
    /// - to automatically generate `using` statements.
    /// - to mangle names in a uniform manner. 
    /// </summary>
    public Names(Language language, string namespaceName)
    {
        _language = language;
        _namespaceName = namespaceName;
        
        _m3 = language.LionWebVersion.LionCore;
        _builtIns = language.LionWebVersion.BuiltIns;
    }

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
    public NameSyntax LanguageType => AsType(_language);

    /// <inheritdoc />
    public string FactoryInterfaceName => $"I{FactoryName}";

    /// <inheritdoc />
    public string FactoryName => $"{LanguageBaseName(_language)}Factory";
    /// <inheritdoc />
    public IdentifierNameSyntax FactoryInterfaceType => IdentifierName(FactoryInterfaceName);
    /// <inheritdoc />
    public IdentifierNameSyntax FactoryType => IdentifierName(FactoryName);
    /// <inheritdoc />
    public Language Language => _language;
    /// <inheritdoc />
    public string NamespaceName => _namespaceName;

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
        if (_builtIns.Node.EqualsIdentity(classifier))
            return AsType(typeof(NodeBase));
        if (_builtIns.INamed.EqualsIdentity(classifier))
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
        var result = VersionSpecifics.AsType(datatype);
        if (result != null)
            return result;

        // TODO externalize
        if (datatype is Enumeration or StructuredDataType && _namespaceMappings.TryGetValue(datatype.GetLanguage(), out var ns))
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
    
    /// <inheritdoc cref="IGeneratorVersionSpecifics"/>
    internal IGeneratorVersionSpecifics VersionSpecifics =>
        new Lazy<IGeneratorVersionSpecifics>(() => IGeneratorVersionSpecifics.Create(_language.LionWebVersion)).Value;
    

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
    public IdentifierNameSyntax AsProperty(Field field) =>
        IdentifierName($"{field.GetStructuredDataType().Name}_{field.Name}");

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

    /// <inheritdoc />
    public IdentifierNameSyntax FieldField(Field field) =>
        IdentifierName($"_{field.Name.ToFirstLower()}");

    /// <inheritdoc />
    public IdentifierNameSyntax FieldProperty(Field field) =>
        IdentifierName(field.Name.ToFirstUpper());

    [GeneratedRegex("`.*$")]
    private static partial Regex AfterIncludingBacktick();
}