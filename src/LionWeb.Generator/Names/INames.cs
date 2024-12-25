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

namespace LionWeb.Generator.Names;

using Core.M3;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Central handling of all naming.
/// This helps
/// - to automatically generate `using` statements.
/// - to mangle names in a uniform manner.
/// </summary>
public interface INames
{
    /// Currently generated language. 
    Language Language { get; }

    /// Target namespace <see cref="Language"/>. 
    string NamespaceName { get; }

    /// All types used during current generation.
    IReadOnlySet<Type> UsedTypes { get; }

    /// Maps external languages to their namespaces.
    IDictionary<Language, string> NamespaceMappings { get; }

    /// Register a new <see cref="NamespaceMappings">NamespaceMapping</see>.
    void AddNamespaceMapping(Language lang, string nsName);

    /// Name of class generated for <see cref="Language"/>.
    /// <returns><c>MyLangNameLanguage</c></returns>
    string LanguageName(Language lang);

    /// Roslyn type of class generated for <see cref="Language"/>.
    /// <returns><c>MyLangNameLanguage</c></returns>
    NameSyntax LanguageType { get; }

    /// Name of interface generated for <see cref="Language">Language's</see> Factory.
    /// <returns><c>IMyLangFactory</c></returns>
    string FactoryInterfaceName { get; }

    /// Name of class generated for <see cref="Language">Language's</see> Factory.
    /// <returns><c>MyLangFactory</c></returns>
    string FactoryName { get; }

    /// Roslyn type of class generated for <see cref="Language">Language's</see> Factory.
    /// <returns><c>MyLangFactory</c></returns>
    IdentifierNameSyntax FactoryType { get; }

    /// Roslyn type of interface generated for <see cref="Language">Language's</see> Factory.
    /// <returns><c>IMyLangFactory</c></returns>
    IdentifierNameSyntax FactoryInterfaceType { get; }

    /// Converts <paramref name="type"/> to Roslyn type and registers it with <see cref="UsedTypes"/>.
    /// If <paramref name="type"/> includes generic type parameters and <paramref name="generics"/> is unset,
    /// <paramref name="type">type's</paramref> generic type parameters are used.
    /// If <paramref name="generics"/> is set, it overrides any generic type paramerters of <paramref name="type"/>. 
    TypeSyntax AsType(Type type, params TypeSyntax?[] generics);

    /// Roslyn type of <paramref name="classifier"/> (also registered with <see cref="UsedTypes"/>).
    /// Returns FQN if <paramref name="disambiguate"/> is <c>true</c>.
    TypeSyntax AsType(Classifier classifier, bool disambiguate = false);

    /// Roslyn type of <paramref name="datatype"/> (also registered with <see cref="UsedTypes"/>).
    /// Returns FQN if <paramref name="disambiguate"/> is <c>true</c>.
    TypeSyntax AsType(Datatype datatype, bool disambiguate = false);

    /// Roslyn type of <paramref name="lang"/> (also registered with <see cref="UsedTypes"/>).
    /// Returns FQN if <paramref name="lang"/> is mentioned in <see cref="NamespaceMappings"/>.
    /// <returns><c>MyLangNameLanguage</c></returns>
    NameSyntax AsType(Language lang);

    /// Registers <paramref name="type"/> with <see cref="UsedTypes"/>.
    string Use(Type type);

    /// <returns><c>MyClassifier</c></returns>
    IdentifierNameSyntax AsProperty(LanguageEntity classifier);

    /// <returns><c>MyClassifier_MyFeature</c></returns>
    IdentifierNameSyntax AsProperty(Feature feature);

    /// <returns><c>MyEnum_MyLiteral</c></returns>
    IdentifierNameSyntax AsProperty(EnumerationLiteral literal);

    /// <returns><c>MyStructuredDataType_MyField</c></returns>
    IdentifierNameSyntax AsProperty(Field field);

    /// <returns><c>MyLanguage.Instance</c></returns>
    NameSyntax MetaProperty(Language lang);

    /// <returns><c>_myFeature</c></returns>
    IdentifierNameSyntax FeatureField(Feature feature);

    /// <returns><c>MyFeature</c></returns>
    IdentifierNameSyntax FeatureProperty(Feature feature);

    /// <returns><c>_myField</c></returns>
    IdentifierNameSyntax FieldField(Field field);

    /// <returns><c>MyField</c></returns>
    IdentifierNameSyntax FieldProperty(Field field);
    
    /// <returns><c>myField</c></returns>
    string ParamField(Field field);
}